using System;

namespace Handyman.Tools.Docs.Utils
{
    public class ConsoleWriter : IConsoleWriter
    {
        public void WriteError(string message)
        {
            var color = Console.ForegroundColor;
            using (new Disposable { Action = () => Console.ForegroundColor = color })
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(message);
            }
        }

        public void WriteInfo(string message)
        {
            Console.WriteLine(message);
        }
    }
}