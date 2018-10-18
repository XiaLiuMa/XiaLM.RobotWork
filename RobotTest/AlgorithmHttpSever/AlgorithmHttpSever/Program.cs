using System;

namespace AlgorithmHttpSever
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("程序已启动，输入Q/q退出程序！");
            new AlgorithmSever().Start();
            string txt = string.Empty;
            while (!(txt = Console.ReadLine()).ToUpper().Equals("Q"))
            {
                Console.WriteLine(txt);
            }
        }
    }
}
