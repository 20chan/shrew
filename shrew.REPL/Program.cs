using System;

namespace shrew.REPL
{
    class Program
    {
        static void Main(string[] args)
        {
            var builtin = new SymbolTable
            {
                { "print", (Action<object>)((obj) => Console.WriteLine(obj)) },
                { "readline", (Func<string>)Console.ReadLine },
            };
            var engine = new Engine(builtin);
            Console.CancelKeyPress += (s, e) => e.Cancel = true;
            while (true)
            {
                Console.Write(">> ");
                var input = Console.ReadLine();
                if (input == null) return;
                var output = engine.ExecuteOrEvaluate(input);
                Console.ForegroundColor = ConsoleColor.Yellow;
                if (output != null)
                    Console.WriteLine(output);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }
    }
}
