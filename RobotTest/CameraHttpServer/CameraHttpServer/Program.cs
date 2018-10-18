using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraHttpServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("程序已启动，输入Q/q退出程序！");
            new CameraServer().Start();
            string txt = string.Empty;
            while (!(txt = Console.ReadLine()).ToUpper().Equals("Q"))
            {
                Console.WriteLine(txt);
            }
        }
    }
}
