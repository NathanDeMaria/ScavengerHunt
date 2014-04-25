using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketBasket
{
    public class Triple
    {
        public int[] Ids { get; set; }
        public double?[] Scores { get; set; }
        private int[] Counts { get; set; }

        public Triple(int item1, int item2, int item3)
        {
            Ids = new int[3];
            Scores = new double?[7];
            Counts = new int[7];
            Ids[0] = item1;
            Ids[1] = item2;
            Ids[2] = item3;
        }

        public void AddSentiment(int score, int dayValue)
        {
            if (Scores[dayValue] == null) Scores[dayValue] = 0;
            Scores[dayValue] = (Scores[dayValue] * Counts[dayValue] + score) / (Counts[dayValue] + 1.0);
            Counts[dayValue]++;
        }
    }
}
