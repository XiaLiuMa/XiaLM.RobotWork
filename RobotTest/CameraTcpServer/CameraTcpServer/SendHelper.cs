using CameraTcpServer.Model;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XiaLM.Tool450.source.common;

namespace CameraTcpServer
{
    public class SendHelper
    {
        /// <summary>
        /// 功能码：低温报警
        /// </summary>
        private byte[] CODE_ALARM = new byte[4] { 0xA2, 0x00, 0x01, 0x03 };

        public float temperature { get; set; } = (float)10;

        public SendHelper()
        {
            SendAlarm();
        }

        /// <summary>
        /// 发送报警信息
        /// </summary>
        public void SendAlarm()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                    if (temperature <= 5)
                    {
                        BaseAlarmInfo baseAlarmInfo = new BaseAlarmInfo()
                        {
                            xpoint = (ushort)12,
                            ypoint = (ushort)24,
                            temperature = (short)-3
                        };
                        string objStr = SerializeHelper.SerializeObjectToJson(baseAlarmInfo);
                        byte[] body = Encoding.UTF8.GetBytes(objStr);

                        CameraServer.GetInstance().ServerBroadcast(CODE_ALARM, body).Employ();
                    }
                }
            });
        }

    }
}
