using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketBasket
{
    class Basket
    {
        public int CustomerId { get; set; }
        public string State { get; set; }
        public string Weekday { get; set; }
        public int ItemNum { get; set; }
        public ICollection<Item> Items { get; set; }
    }
}
