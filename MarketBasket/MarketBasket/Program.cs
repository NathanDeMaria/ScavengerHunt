using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketBasket
{
    class Program
    {
        const int NUM_BASKETS = 3000;

        public static void Main(string[] args)
        {
            Console.WriteLine("Hi :)");
            Console.ReadLine();

            List<Basket> baskets = new List<Basket>();
            for (int i = 0; i < NUM_BASKETS; i++)
            {
                baskets.Add(ReadBasket("../../../../new_data/modified_basket_" + i.ToString("000000") + ".dat"));
            }
        }

        public static Basket ReadBasket(string path)
        {
            Basket basket = new Basket();
            using (var reader = new StreamReader(path))
            {

                basket.CustomerId = Int32.Parse(reader.ReadLine().Replace("CustomerId: ", ""));
                basket.State = reader.ReadLine().Replace("State: ", "");
                basket.Weekday = reader.ReadLine().Replace("Weekday: ", "");
                basket.ItemNum = Int32.Parse(reader.ReadLine().Replace("ItemNum: ", ""));

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    basket.Items.Add(new Item
                    {
                        ItemId = Int32.Parse(line.Replace("ItemId: ", "")),
                        Review = reader.ReadLine().Replace("Review: ", "")                    
                    });
                }
            }

            return basket;
        }

    }



}
