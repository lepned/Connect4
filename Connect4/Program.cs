using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Connect4
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var game = new Game();
            //await game.HumanVsMachine(12);
            await game.MachineVsMachine(12);
        }
        
    }
}
