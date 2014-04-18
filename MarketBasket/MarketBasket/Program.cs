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

        public static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<Basket> baskets = new List<Basket>();
            List<Item> c1 = new List<Item>();
            for (int i = 0; i < NUM_BASKETS; i++)
            {
                var basket = ReadBasket("../../../../new_data/modified_basket_" + i.ToString("000000") + ".dat"); 
                baskets.Add(basket);
                foreach (var item in basket.Items)
                {
                    var foundItem = c1.Where(x => x.ItemId == item.ItemId).FirstOrDefault();
                    if (foundItem != null)
                    {
                        foundItem.Count++;
                    }
                    else
                    {
                        c1.Add(item);
                        item.Count = 1;
                    }
                }
            }

            var f1 = c1.Where(x => x.Count >= 3).ToList();
            var pairs = new List<ItemPair>();

            for (int i = 0; i < f1.Count; i++)
            {
                for (int j = i+1; j <f1.Count; j++)
                {
                    pairs.Add(new ItemPair
                    {
                        ItemOneId = f1[i].ItemId,
                        ItemTwoId = f1[j].ItemId
                    });
                }
            }
            int numPassed = 0;
            foreach (var basket in baskets)
            {
                foreach (var pair in pairs)
                {
                    if (basket.Items.Where(i => i.ItemId == pair.ItemOneId || i.ItemId == pair.ItemTwoId).Count() <= 2)
                    {
                        pair.Count++;
                    }
                }
                numPassed++;
                if (numPassed % 100 == 0)
                {
                    Console.WriteLine(numPassed);
                }
            }
            
            sw.Stop();

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
