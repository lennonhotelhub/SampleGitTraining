using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HangfireWebApplication
{
    public class Display : IDisplay
    {
        public void DisplayOut()
        {
            Console.WriteLine("Job");
        }
    }
}
