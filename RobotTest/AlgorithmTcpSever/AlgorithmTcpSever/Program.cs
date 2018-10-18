using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmTcpSever
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("程序已启动，输入Q/q退出程序！");
            ServerControl.GetInstance().ServerInit();
            string txt = string.Empty;
            while (!(txt = Console.ReadLine()).ToUpper().Equals("Q"))
            {
                Console.WriteLine(txt);
            }
        }
    }
}
