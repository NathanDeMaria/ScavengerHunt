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
            for (int i = 0; i < NUM_BASKETS; i++)
            {
                var basket = ReadBasket("../../../../new_data/modified_basket_" + i.ToString("000000") + ".dat");
                baskets.Add(basket);
                foreach (var item in basket.Items)
                {
                    c1.AddOrUpdate(item.ItemId, 1, (key, value) => value + 1);
                }
            }
            var f1 = c1.Where(x => x.Value >= s).Select(x => x.Key).ToList();
            var c2 = new ConcurrentDictionary<Tuple<int, int>, int>();

            foreach (var basket in baskets)
            {
                for (int i = 0; i < basket.Items.Count - 1; i++)
                {
                    if (f1.Contains(basket.Items[i].ItemId))
                    {
                        for (int j = i + 1; j < basket.Items.Count; j++)
                        {
                            if (f1.Contains(basket.Items[j].ItemId))
                            {
                                c2.AddOrUpdate(new Tuple<int, int>(basket.Items[i].ItemId,
                                                                    basket.Items[j].ItemId),
                                                    1, (key, value) => value + 1);
                            }
                        }
                    }
                }
            }

            var f2 = c2.Where(x => x.Value >= s).ToList();

            /*
             * The pool of items that could be in a triple with support s consists of
             * items that are in at least two pairs with support s
             */
            var singleItems = new ConcurrentDictionary<int, int>();
            foreach (var pair in f2)
            {
                singleItems.AddOrUpdate(pair.Key.Item1, 1, (key, value) => value + 1);
                singleItems.AddOrUpdate(pair.Key.Item2, 1, (key, value) => value + 1);
            }
            var triplePool = singleItems.Keys.Where(x => x >= 2);

            var c3 = new ConcurrentDictionary<Tuple<int, int, int>, int>();
            foreach (var basket in baskets)
            {
                for (int i = 0; i < basket.Items.Count - 2; i++)
                {
                    if (triplePool.Contains(basket.Items[i].ItemId))
                    {
                        for (int j = i + 1; j < basket.Items.Count - 1; j++)
                        {
                            if (triplePool.Contains(basket.Items[j].ItemId))
                            {
                                for (int k = j + 1; k < basket.Items.Count; k++)
                                {
                                    if (triplePool.Contains(basket.Items[k].ItemId))
                                    {
                                        c3.AddOrUpdate(new Tuple<int, int, int>(basket.Items[i].ItemId,
                                                                                basket.Items[j].ItemId,
                                                                                basket.Items[k].ItemId),
                                                        1, (key, value) => value + 1);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            var f3 = c3.Where(x => x.Value >= s).ToList();

            using (var writer = new StreamWriter("../../../../output.txt"))
            {
                foreach (var itemSet in f3)
                {
                    writer.WriteLine(PrintSet(itemSet));
                }
            }

            // Stage 2
            Dictionary<string, int> sentiments = File.ReadLines("../../../../sentiment.csv")
                .Select(line => line.Split('\t'))
                .ToDictionary(line => line[0], line => Int32.Parse(line[1]));
            var triples = f3.Select(x => new Triple(x.Key.Item1, x.Key.Item2, x.Key.Item3)).ToList();

            foreach (var triple in triples)
            {
                foreach (var basket in baskets)
                {
                    var itemIds = basket.Items.Select(x => x.ItemId).ToList();
                    var day = GetDayOfWeek(basket.Weekday);
                    if(itemIds.Contains(triple.Ids[0]) && itemIds.Contains(triple.Ids[1])
                        && itemIds.Contains(triple.Ids[2]))
                    {
                        foreach (var item in triple.Ids)
                        {
                            var itemReview = basket.Items.Where(i => i.ItemId == item).First().Review;
                            var score = getScore(sentiments, itemReview);
                            triple.AddSentiment(score, day);
                        }
                    }
                } 
            }
            

            sw.Stop();

            if (sw.ElapsedMilliseconds < 1000)
            {
                Console.WriteLine("\nProcessing Time: {0} milliseconds", sw.ElapsedMilliseconds);
            }
            else
            {
                Console.WriteLine("\nProcessing Time: {0:00}:{1:00}.{2:00}:{3:000}", sw.Elapsed.Hours, sw.Elapsed.Minutes, sw.Elapsed.Seconds, sw.Elapsed.Milliseconds);
            }
            Console.WriteLine("\nPress the any key to exit.");
            Console.ReadKey();
        }

        private static int getScore(Dictionary<string, int> sentiments, string itemReview)
        {
            var words = itemReview.Trim().Split(' ').ToList();
            var sum = 0;
            foreach (var word in words)
            {
                sum += sentiments[word];
            }
            return sum / words.Count;
        }

        private static int GetDayOfWeek(string day)
        {
            switch (day)
            {
                case "Monday":
                    return 0;
                case "Tuesday":
                    return 1;
                case "Wednesday":
                    return 2;
                case "Thursday":
                    return 3;
                case "Friday":
                    return 4;
                case "Saturday":
                    return 5;
                case "Sunday":
                    return 6;
            }

            return -1;
        }

        private static string PrintSet(KeyValuePair<Tuple<int, int, int>, int> itemSet)
        {
            StringBuilder result = new StringBuilder();

            result.Append("(");
            result.Append(itemSet.Key.Item1);
            result.Append(", ");
            result.Append(itemSet.Key.Item2);
            result.Append(", ");
            result.Append(itemSet.Key.Item3);
            result.Append(") ");
            result.Append(itemSet.Value);

            return result.ToString();
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
