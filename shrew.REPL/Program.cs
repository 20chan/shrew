using System;

namespace shrew.REPL
{
    class Program
    {
        static void Main(string[] args)
        {
            var engine = new Engine();
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
