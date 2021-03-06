﻿using System;
//using System.Numerics;
using Deveel.Math;

namespace distribucionMultinomial
{
    public class Mate
    {

        public static BigInteger recfact(long start, long n)
        {
            long i;
            if (n <= 16)
            {
                BigInteger r = BigInteger.Parse(start.ToString());
                for (i = start + 1; i < start + n; i++) r *= i;
                return r;
            }
            i = n / 2;
            return recfact(start, i) * recfact(start + i, n - i);
        }

        public static BigInteger factorial(long n) { return recfact(1, n); }





        public static double FractionToDecimal(String input)
        {
            String[] fraction = input.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            if (fraction.Length != 2)
            {
                throw new ArgumentOutOfRangeException();
            }
            long numerator, denominator;
            if (long.TryParse(fraction[0], out numerator) && long.TryParse(fraction[1], out denominator))
            {
                if (denominator == 0)
                {
                    throw new InvalidOperationException("Divide by 0 occurred");
                }
                return (double)numerator / denominator;
            }
            throw new ArgumentException();
        }


    }
}
