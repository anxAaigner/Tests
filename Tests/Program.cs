using System;
using Tests.ContinueWith;

namespace Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var continueWithTests = new ContinueWithTestStarter(100, 100);
                continueWithTests.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.ReadKey();
        }       
    }
}