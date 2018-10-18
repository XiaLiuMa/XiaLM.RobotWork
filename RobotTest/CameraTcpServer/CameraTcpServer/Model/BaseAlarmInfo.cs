using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraTcpServer.Model
{
    public class BaseAlarmInfo
    {
        /// <summary>
        /// X坐标
        /// </summary>
        public ushort xpoint { get; set; }
        /// <summary>
        /// Y坐标
        /// </summary>
        public ushort ypoint { get; set; }
        /// <summary>
        /// 温度
        /// </summary>
        public short temperature { get; set; }
    }
}
