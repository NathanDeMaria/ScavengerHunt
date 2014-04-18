using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace MarketBasket
{
    class Program
    {
        const int NUM_BASKETS = 3000;
		const int s = NUM_BASKETS / 1000;

        public static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<Basket> baskets = new List<Basket>();
			ConcurrentDictionary<int, int> c1 = new ConcurrentDictionary<int, int>();
            for(int i = 0; i < NUM_BASKETS; i++)
            {
                var basket = ReadBasket("../../../../new_data/modified_basket_" + i.ToString("000000") + ".dat"); 
                baskets.Add(basket);
                foreach (var item in basket.Items)
                {
                    c1.AddOrUpdate(item.ItemId, 1, (key, value) => value + 1);
                }
            }
            var f1 = c1.Where(x => x.Value >= s).Select(x => x.Key).ToList();
            var c2 = new ConcurrentDictionary<int, int>();

            int computedKey;
            foreach (var basket in baskets)
            {
                foreach (var itemOne in basket.Items)
                {
                    if (f1.Contains(itemOne.ItemId))
                    {
                        foreach (var itemTwo in basket.Items)
                        {
                            if (itemTwo.ItemId < itemOne.ItemId && f1.Contains(itemTwo.ItemId))
                            {
                                computedKey = itemOne.ItemId * 1000 + itemTwo.ItemId;
                                c2.AddOrUpdate(computedKey, 1, (key, value) => value + 1);
                            }
                        }
                    }
                }
            }
            var keys = c2.Where(kvp => kvp.Value >= s).Select(kvp => kvp.Key).ToList();
            
            var triplePool = keys.Select(k => k % 1000).ToList();
            triplePool.AddRange(keys.Select(k => k / 1000).Distinct().ToList());
            var c3 = new ConcurrentDictionary<int, int>();

            foreach (var basket in baskets)
            {
                foreach (var itemOne in basket.Items)
                {
                    if (f1.Contains(itemOne.ItemId))
                    {
                        foreach (var itemTwo in basket.Items)
                        {
                            if (itemTwo.ItemId < itemOne.ItemId && f1.Contains(itemTwo.ItemId))
                            {
                                foreach (var itemThree in basket.Items)
                                {
                                    if (itemThree.ItemId < itemTwo.ItemId && f1.Contains(itemThree.ItemId))
                                    {
                                        computedKey = (itemOne.ItemId * 1000000) + (itemTwo.ItemId * 1000) + itemThree.ItemId;
                                        c3.AddOrUpdate(computedKey, 1, (key, value) => value + 1);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            var answer = c3.Where(kvp => kvp.Value >= s).ToList() ;
            
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
            if (sw.ElapsedMilliseconds < 1000)
            {
                Console.WriteLine("\nProcessing Time: {0} milliseconds", sw.ElapsedMilliseconds);
            }
            else
            {
                Console.WriteLine("\nProcessing Time: {0:00}:{1:00}.{2:00}", sw.Elapsed.Hours, sw.Elapsed.Minutes, sw.Elapsed.Seconds, sw.Elapsed.Milliseconds / 10);
            }

            Console.WriteLine("\nPress the any key to exit.");
            Console.ReadKey();
        }

        public static Basket ReadBasket(string path)
        {
            Basket basket = new Basket();
            using (var reader = new StreamReader(path))
            {
                basket.CustomerId = Int32.Parse(reader.ReadLine().Replace("ID: ", ""));
                basket.State = reader.ReadLine().Replace("State: ", "");
                basket.Weekday = reader.ReadLine().Replace("Date: ", "");
                basket.Items = new List<Item>();

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    basket.Items.Add(new Item
                    {
                        ItemId = Int32.Parse(line.Replace("Item: ", "")),
                        Review = reader.ReadLine().Replace("Review: ", "")                    
                    });
                }
            }

            return basket;
        }

    }



}
