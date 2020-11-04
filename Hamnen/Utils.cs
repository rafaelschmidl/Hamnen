using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HarborSimuation
{
    public static class Utils
    {
        private static readonly Random random = new Random();

        public static int RandomNumberInRange(int a, int b)
        {
            return random.Next(a, b + 1);
        }

        public static string RandomUppercaseString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[RandomNumberInRange(0, s.Length - 1)]).ToArray());
        }

        public static string GenerateID(string idPrefix)
        {
            return idPrefix + "-" + RandomUppercaseString(3);
        }

        public static double KnotsToKmph(double knots)
        {
            return knots * 1.852;
        }
    }
}
