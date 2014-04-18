using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketBasket
{
    class Item
    {
        public int ItemId { get; set; }
        public string Review { get; set; }
    }

    class ItemPair
    {
        public int ItemOneId { get; set; }
        public int ItemTwoId { get; set; }
    }

    class ItemTriple
    {
        public int ItemOneId { get; set; }
        public int ItemTwoId { get; set; }
        public int ItemThreeId { get; set; }
    }
}
