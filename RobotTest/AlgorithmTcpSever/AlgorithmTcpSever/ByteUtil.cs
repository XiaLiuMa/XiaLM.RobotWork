using System;
using System.Collections.Generic;
using System.Linq;


namespace AlgorithmTcpSever
{
    public static class ByteUtil
    {
        /// <summary>
        /// 单字节循环异或校验
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte BccVerifica(this IEnumerable<byte> bytes, int startIndex, int length)
        {
            return Bcc(bytes.Skip(startIndex).Take(length));
        }

        private static T Bcc<T>(IEnumerable<T> source)
            where T : struct
        {
            Int32 res = source.Select(p => Convert.ToInt32(p)).Aggregate((a, b) => a ^ b);
            return (T)Convert.ChangeType(res, typeof(T));
        }

        public static IEnumerable<byte> int2CustomBytes(Int32 num, Int32 div)
        {
            if (num < 100)
            {
                yield return (byte)num;
                yield break;
            }
            yield return (byte)(num / div);

            var list = int2CustomBytes(num % div, div / 100);

            foreach (var item in list)
            {
                yield return item;
            }
        }


        public static Int32 CustomBytes2int(byte[] bytes)
        {
            string str = String.Join("", bytes);

            return Convert.ToInt32(str);
        }

    }
}
