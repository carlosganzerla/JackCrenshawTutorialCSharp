using System;

namespace MyCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            //var cradle = new CradleCompiler();
            //cradle.Init();
            //cradle.Assignment();
            //cradle.Terminator();
            var cradle = new CradleInterpreter();
            cradle.Init();
            cradle.Run();
        }
    }
}
