﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FrodLib.MathUtils;

namespace TestProject
{
    [TestClass]
    public class PrimeUtilsTests
    {
        [TestInitialize()]
        public void MyTestInitialize()
        {

        }

        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {

        }



        //all prime numbers which are less then 1000
        private readonly IList<int> primeNumbers = new ReadOnlyCollection<int>(new int[]{2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197, 199, 211, 223, 227, 229, 233, 239, 241, 251, 257, 263, 269, 271, 277, 281, 283, 293, 307, 311, 313, 317, 331, 337, 347, 349, 353, 359, 367, 373, 379, 383, 389, 397, 401, 409, 419, 421, 431, 433, 439, 443, 449, 457, 461, 463, 467, 479, 487, 491, 499, 503, 509, 521, 523, 541, 547, 557, 563, 569, 571, 577, 587, 593, 599, 601, 607, 613, 617, 619, 631, 641, 643, 647, 653, 659, 661, 673, 677, 683, 691, 701, 709, 719, 727, 733, 739, 743, 751, 757, 761, 769, 773, 787, 797, 809, 811, 821, 823, 827, 829, 839, 853, 857, 859, 863, 877, 881, 883, 887, 907, 911, 919, 929, 937, 941, 947, 953, 967, 971, 977, 983, 991, 997});

        [TestMethod()]
        [TestCategory("PrimeUtils")]
        [TestCategory("MathUtils")]
        public void GeneratePrimesNaiveTest()
        {
            int numberOfPrimes = primeNumbers.Count;
            List<int> generatedPrimes = PrimeNumber.GeneratePrimesNaive(numberOfPrimes);
            Assert.AreEqual(numberOfPrimes, generatedPrimes.Count);
            CheckPrimeNumbers(generatedPrimes);
        }

        [TestMethod()]
        [TestCategory("PrimeUtils")]
        [TestCategory("MathUtils")]
        public void GeneratePrimesSieveOfEratosthenesTest()
        {
            int numberOfPrimes = primeNumbers.Count;
            List<int> generatedPrimes = PrimeNumber.GeneratePrimesSieveOfEratosthenes(numberOfPrimes);
            Assert.AreEqual(numberOfPrimes, generatedPrimes.Count);
            CheckPrimeNumbers(generatedPrimes);
        }

        [TestMethod()]
        [TestCategory("PrimeUtils")]
        [TestCategory("MathUtils")]
        public void GeneratePrimesSieveOfSundaramTest()
        {
            int numberOfPrimes = primeNumbers.Count;
            List<int> generatedPrimes = PrimeNumber.GeneratePrimesSieveOfSundaram(numberOfPrimes);
            Assert.AreEqual(numberOfPrimes, generatedPrimes.Count);
            CheckPrimeNumbers(generatedPrimes);
        }

        private void CheckPrimeNumbers(IList<int> generatedPrimes)
        {
            for (int i = 0; i < generatedPrimes.Count; i++)
            {
                Assert.AreEqual(primeNumbers[i], generatedPrimes[i]);
            }
        }

        [TestMethod()]
        [TestCategory("PrimeUtils")]
        [TestCategory("MathUtils")]
        public void IsPrimeTest()
        {
            Assert.IsTrue(PrimeNumber.IsPrime(23));
            Assert.IsTrue(PrimeNumber.IsPrime(97));
            Assert.IsTrue(PrimeNumber.IsPrime(563));
            Assert.IsTrue(PrimeNumber.IsPrime(787));
            Assert.IsTrue(PrimeNumber.IsPrime(983));

            Assert.IsFalse(PrimeNumber.IsPrime(21));
            Assert.IsFalse(PrimeNumber.IsPrime(85));
            Assert.IsFalse(PrimeNumber.IsPrime(387));
            Assert.IsFalse(PrimeNumber.IsPrime(861));
            Assert.IsFalse(PrimeNumber.IsPrime(920));
        }

        [TestMethod()]
        [TestCategory("PrimeUtils")]
        [TestCategory("MathUtils")]
        public void GetNthPrimeTest()
        {
            for (int i = 1; i <= primeNumbers.Count; i++)
            {
                long prime = PrimeNumber.GetNthPrime(i);
                Assert.AreEqual(primeNumbers[i - 1], prime);
            }
        }

        //[TestMethod()]
        //[TestCategory("PrimeUtils")]
        //[TestCategory("MathUtils")]
        //public void ApproximateNthPrimeTest()
        //{
        //    for (int i = 1; i <= primeNumbers.Count; i++)
        //    {
        //        long prime = PrimeUtils.ApproximateNthPrime(i);
        //        Assert.AreEqual(primeNumbers[i - 1], prime);
        //    }
        //}
    }
}
