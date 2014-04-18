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
            //List<Item> c1 = new List<Item>();
			ConcurrentDictionary<int, int> c1 = new ConcurrentDictionary<int, int>();
            for(int i = 0; i < NUM_BASKETS; i++)
            {
                var basket = ReadBasket("../../../../new_data/modified_basket_" + i.ToString("000000") + ".dat"); 
                baskets.Add(basket);
                foreach (var item in basket.Items)
                {
					if (!c1.TryAdd(item.ItemId, 1))
					{
						c1[item.ItemId]++;
					}

					//var foundItem = c1.Where(x => x.ItemId == item.ItemId).FirstOrDefault();
					//if (foundItem != null)
					//{
					//	foundItem.Count++;
					//}
					//else
					//{
					//	c1.Add(item);
					//	item.Count = 1;
					//}
                }
            }
            var f1 = c1.Where(x => x.Value >= s).Select(x => x.Key).ToList();
            var c2 = new List<ItemPair>();
			var c2Prime = new ConcurrentDictionary<int[], int>();

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
								int[] itemPair;
								if(basket.Items[i].ItemId < basket.Items[j].ItemId)
								{
									itemPair = new int[]{ basket.Items[i].ItemId, basket.Items[j].ItemId };
								}
								else
								{
									itemPair = new int[] { basket.Items[j].ItemId, basket.Items[i].ItemId };
								}					

								if(!c2Prime.TryAdd(itemPair, 1))
								{
									c2Prime[itemPair]++;
								}


								//might be able to take the second part out, but not sure
								var foundItem = c2.Where(x => (x.ItemOneId == basket.Items[i].ItemId && x.ItemTwoId == basket.Items[j].ItemId) ||
																(x.ItemOneId == basket.Items[j].ItemId && x.ItemTwoId == basket.Items[i].ItemId)).FirstOrDefault();
								if (foundItem == null)
								{
									c2.Add(new ItemPair
									{
										ItemOneId = basket.Items[i].ItemId,
										ItemTwoId = basket.Items[j].ItemId,
										Count = 1
									});
								}
								else
								{
									foundItem.Count++;
								}
							}
						}
					}
				}
            }

			var f2 = c2.Where(x => x.Count >= s).ToList();
			var singleItems = new List<Item>();
			foreach(var pair in f2)
			{
				if (singleItems.Select(x => x.ItemId).Contains(pair.ItemOneId))
				{
					singleItems.Where(x => x.ItemId == pair.ItemOneId).FirstOrDefault().Count++;
				}
				else
				{
					singleItems.Add(new Item() { ItemId = pair.ItemOneId, Count = 1 });
				}

				if (singleItems.Select(x => x.ItemId).Contains(pair.ItemTwoId))
				{
					singleItems.Where(x => x.ItemId == pair.ItemTwoId).FirstOrDefault().Count++;
				}
				else
				{
					singleItems.Add(new Item() { ItemId = pair.ItemTwoId, Count = 1 });
				}
			}
			var triplePool = singleItems.Where(x => (x.Count >= 2)).Select(x => x.ItemId).ToList();

			var c3 = new List<ItemTriple>();
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
								for(int k = j + 1; k < basket.Items.Count; k++)
								{
									if (triplePool.Contains(basket.Items[k].ItemId))
									{
										var tripleItems = new List<int>() { basket.Items[i].ItemId, basket.Items[j].ItemId, basket.Items[k].ItemId };

										var foundItem = c3.Where(x => tripleItems.Contains(x.ItemOneId) && tripleItems.Contains(x.ItemTwoId) && tripleItems.Contains(x.ItemThreeId)).FirstOrDefault();
										if (foundItem == null)
										{
											c3.Add(new ItemTriple
											{
												ItemOneId = basket.Items[i].ItemId,
												ItemTwoId = basket.Items[j].ItemId,
												ItemThreeId = basket.Items[k].ItemId,
												Count = 1
											});
										}
										else
										{
											foundItem.Count++;
										}
									}
								}
							}
						}
					}
				}
			}
			var f3 = c3.Where(x => x.Count >= s).ToList();

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
