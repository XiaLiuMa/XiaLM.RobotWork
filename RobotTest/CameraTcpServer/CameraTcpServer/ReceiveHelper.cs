using System;
using System.Text;
using XiaLM.Tool450.source.common;

namespace CameraTcpServer
{
    public class ReceiveHelper
    {
        SendHelper sendHelper;
        /// <summary>
        /// 功能码：下发黑体参数
        /// </summary>
        private byte[] SEND_BLACKBODY = new byte[4] { 0xA2, 0x00, 0x00, 0x02 };
        /// <summary>
        /// 功能码：下发温度阈值
        /// </summary>
        private byte[] SEND_TEMPERATURE = new byte[4] { 0xA2, 0x00, 0x00, 0x03 };

        public ReceiveHelper()
        {
            sendHelper = new SendHelper();
        }

        /// <summary>
        /// 处理接收的消息
        /// </summary>
        /// <param name="code">功能码</param>
        /// <param name="body">正文</param>
        public void DealReceivedgData(byte[] code, byte[] body)
        {
            try
            {
                if (code.Equals(SEND_TEMPERATURE))  //设置低温阈值
                {
                    sendHelper.temperature = BitConverter.ToInt16(body,0);  //温度
                }

                if (code.Equals(SEND_BLACKBODY))
                {
                    string strBody = Encoding.UTF8.GetString(body);     //Json字符串
                }
                
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
        }
    }
}
