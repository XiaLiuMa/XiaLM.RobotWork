using Loxi.Tool.Tcp.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using XiaLM.Tool450.source.common;

namespace CameraTcpServer
{
    public class CameraServer
    {
        AsyncTcpServer server;
        ReceiveHelper receiveHelper;
        public List<AsyncTcpServerSession> clientList;
        //public event Action<AsyncTcpServerSession, int, byte[]> DataReceived;
        private static CameraServer instance;
        private readonly static object objLock = new object();
        public static CameraServer GetInstance()
        {
            if (instance == null)
            {
                lock (objLock)
                {
                    if (instance == null)
                    {
                        instance = new CameraServer();
                    }
                }
            }
            return instance;
        }
        public CameraServer()
        {
            clientList = new List<AsyncTcpServerSession>();
            receiveHelper = new ReceiveHelper();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="port">端口号</param>
        public void Init(int port)
        {
            try
            {
                server = new AsyncTcpServer(new IPEndPoint(IPAddress.Any, port), null, new EDCodecBuilder());
                server.OnSessionDataReceived += Server_OnSessionDataReceived;
                server.OnSessionStarted += Server_OnSessionStarted;
                server.OnSessionClosed += Server_OnSessionClosed;
                server.Listener();
                Console.WriteLine("服务端已启动!");
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
        }

        private async Task Server_OnSessionClosed(AsyncTcpServerSession session)
        {
            if (clientList.Contains(session)) clientList.Add(session);
            Console.WriteLine($"{session.LocalEndPoint}已断开!");
        }

        private async Task Server_OnSessionStarted(AsyncTcpServerSession session)
        {
            if (clientList.Contains(session)) clientList.Remove(session);
            Console.WriteLine($"{session.LocalEndPoint}已连接!");
        }

        private async Task Server_OnSessionDataReceived(AsyncTcpServerSession session, byte[] data, int offset, int count)
        {
            try
            {
                byte[] code = data.Take(4).ToArray();    //功能码
                byte[] body = new byte[data.Length - 4];    //正文
                Array.Copy(data, 4, body, 0, body.Length);
                receiveHelper.DealReceivedgData(code, body);

                //DataReceived?.Invoke(session, code, body);
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
        }

        public async Task ServerSendData(AsyncTcpServerSession session, byte[] code, byte[] body)
        {
            try
            {
                byte[] data = new byte[body.Length + 4];
                Array.Copy(code, 0, data, 0, 4);
                Array.Copy(body, 0, data, 4, body.Length);
                await session.SendAsync(data);
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
        }

        /// <summary>
        /// 广播发送
        /// </summary>
        /// <param name="code"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task ServerBroadcast(byte[] code, byte[] body)
        {
            try
            {
                byte[] data = new byte[body.Length + code.Length];
                Array.Copy(code, 0, data, 0, code.Length);
                Array.Copy(body, 0, data, code.Length, body.Length);

                await server.BroadcastAsync(data);
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
        }
    }
}
