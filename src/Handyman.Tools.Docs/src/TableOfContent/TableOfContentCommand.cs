using McMaster.Extensions.CommandLineUtils;
using System;

namespace Handyman.Tools.Docs.TableOfContent
{
    [Command("table-of-content")]
    public class TableOfContentCommand
    {
        public void OnExecute()
        {
            Console.WriteLine("hello from toc");
        }
    }
}