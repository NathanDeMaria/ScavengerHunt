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
        static void Main(string[] args)
        {
            Console.WriteLine("Hi :)");
            Console.ReadLine();
        }

        Basket ReadBasket(string path)
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
