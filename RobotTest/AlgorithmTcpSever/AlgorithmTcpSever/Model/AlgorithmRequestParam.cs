using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmTcpSever.Model
{
    public class AlgorithmRequestParam
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public string camNo { get; set; }
        /// <summary>
        /// 抓拍照
        /// </summary>
        public string image { get; set; }
        /// <summary>
        /// 半身照
        /// </summary>
        public string imageBody { get; set; }
    }
}
