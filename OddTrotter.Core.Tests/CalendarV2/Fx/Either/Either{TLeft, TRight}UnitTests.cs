namespace Fx.Either
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass]
    public sealed class EitherUnitTests
    {
        [TestMethod]
        public void DoesCatchCausePerfImpact()
        {
            //// TODO TOPIC the catch appears to run slightly faster...

            var iterations = 1000000;

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < iterations; ++i)
            {
                WithoutCatch();
            }

            Console.WriteLine(stopwatch.ElapsedTicks);

            stopwatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < iterations; ++i)
            {
                WithCatch();
            }

            Console.WriteLine(stopwatch.ElapsedTicks);

            stopwatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < iterations; ++i)
            {
                WithoutCatch();
            }

            Console.WriteLine(stopwatch.ElapsedTicks);

            stopwatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < iterations; ++i)
            {
                WithCatch();
            }

            Console.WriteLine(stopwatch.ElapsedTicks);
        }

        private static void WithCatch()
        {
            try
            {
                DoWork();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
                throw;
            }
        }

        private static void WithoutCatch()
        {
            DoWork();
        }

        private static int DoWork()
        {
            var random = new Random();
            return random.Next() + 1;
        }
    }
}
