using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrodLib.MathUtils
{
    
    public class PrimeNumber
    {

        /// <summary>
        /// Checks if a number is a prime number
        /// </summary>
        /// <param name="n"></param>
        /// <returns>True if the value is a prime</returns>
        public static bool IsPrime(long n)
        {
            if (n <= 1) return false;
            if (n == 2) return true;

            int boundary = (int)Math.Floor(Math.Sqrt(n));

            for (int i = 2; i < boundary; i++)
            {
                if (n % i == 0) return false;
            }

            return true;
        }

        /// <summary>
        /// Returns the Nth prime number
        /// </summary>
        /// <param name="nprime"></param>
        /// <returns></returns>
        public static long GetNthPrime(int nprime)
        {
            if (nprime == 1) return 2;

            int limit = ApproximateNthPrime(nprime);
            BitArray bits = SieveOfEratosthenes(limit);
            long nthPrime = 0;
            for (int i = 0, found = 0; found < nprime; i++) //i < limit && 
            {
                if (bits[i])
                {
                    nthPrime = i;
                    found++;
                }
            }

            return nthPrime;
        }

        /// <summary>
        /// Approximates the Nth prime
        /// </summary>
        /// <param name="nn">The position of prime number to get</param>
        /// <returns>The prime number at the Nth pos in a sequance</returns>
        private static int ApproximateNthPrime(int nn)
        {
            double prime;
            if (nn >= 7022)
            {
                prime = nn * Math.Log(nn) + nn * (Math.Log(Math.Log(nn)) - 0.9385);
            }
            else if (nn >= 6)
            {
                prime = nn * Math.Log(nn) + nn * (Math.Log(Math.Log(nn)));
            }
            else if (nn > 0)
            {
                prime = new int[] { 2, 3, 5, 7, 11 }[nn - 1];
            }
            else
            {
                prime = 0;
            }
            return (int)prime;
        }

        /// <summary>
        /// Generates a number of primes using a naive approch
        /// </summary>
        /// <param name="n">The number of primes to generate</param>
        /// <returns>A list of prime numbers</returns>
        public static List<int> GeneratePrimesNaive(int n)
        {
            List<int> primes = new List<int>();
            primes.Add(2);
            int nextPrime = 3;
            while (primes.Count < n)
            {
                int sqrt = (int)Math.Sqrt(nextPrime);
                bool isPrime = true;
                for (int i = 0; (int)primes[i] <= sqrt; i++)
                {
                    if (nextPrime % primes[i] == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }
                if (isPrime)
                {
                    primes.Add(nextPrime);
                }
                nextPrime += 2;
            }
            return primes;
        }

        // Find all primes up to and including the limit
        public static BitArray SieveOfEratosthenes(int limit)
        {
            BitArray bits = new BitArray(limit + 1, true);
            bits[0] = false;
            bits[1] = false;
            for (int i = 0; i * i <= limit; i++)
            {
                if (bits[i])
                {
                    for (int j = i * i; j <= limit; j += i)
                    {
                        bits[j] = false;
                    }
                }
            }
            return bits;
        }

        /// <summary>
        /// Generates a number of primes using sieve of eratosthenes
        /// </summary>
        /// <param name="n">The number of primes to generate</param>
        /// <returns>A list of prime numbers</returns>
        public static List<int> GeneratePrimesSieveOfEratosthenes(int n)
        {
            int limit = ApproximateNthPrime(n);
            BitArray bits = SieveOfEratosthenes(limit);
            List<int> primes = new List<int>();
            for (int i = 0, found = 0; found < n; i++)
            {
                if (bits[i])
                {
                    primes.Add(i);
                    found++;
                }
            }
            return primes;
        }

        public static BitArray SieveOfSundaram(int limit)
        {
            limit /= 2;
            BitArray bits = new BitArray(limit + 1, true);
            for (int i = 1; 3 * i + 1 < limit; i++)
            {
                for (int j = 1; i + j + 2 * i * j <= limit; j++)
                {
                    bits[i + j + 2 * i * j] = false;
                }
            }
            return bits;
        }

        /// <summary>
        /// Generates a number of primes using sieve of sundaram
        /// </summary>
        /// <param name="n">The number of primes to generate</param>
        /// <returns>A list of prime numbers</returns>
        public static List<int> GeneratePrimesSieveOfSundaram(int n)
        {
            int limit = ApproximateNthPrime(n);
            BitArray bits = SieveOfSundaram(limit);
            List<int> primes = new List<int>();
            primes.Add(2);
            for (int i = 1, found = 1; 2 * i + 1 <= limit && found < n; i++)
            {
                if (bits[i])
                {
                    primes.Add(2 * i + 1);
                    found++;
                }
            }
            return primes;
        }
    }
}
