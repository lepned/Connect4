using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect4
{
    public class RecursionTest
    {
        public RecursionTest()
        {
            Path = "Start";
        }
        public RecursionTest(RecursionTest r)
        {
            Path = r.Path;
        }
        public string Path { get; set; }

        public string Run(int depth)
        {
            string ret = "";
            if (depth == 0)
            {
                //Console.WriteLine(Path);
                ret = Path;
            }
            else
            {

                var rec = new RecursionTest(this);
                rec.Path = $"Level {depth} ";
                Console.WriteLine(rec.Path);
                ret = rec.Run(depth - 1);
            }
            return ret;
        }

    }
}
