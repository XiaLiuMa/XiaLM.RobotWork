using AlgorithmTcpSever.Model;
using Loxi.Core.Tcp.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XiaLM.Tool450.source.common;


namespace AlgorithmTcpSever
{
    public class ServerControl
    {
        private AsyncTcpServer tcpServer;
        private bool isLive = false;    //是否存活
        public List<FaceInfo> faceList; //名单列表
        private static ServerControl instance;
        private readonly static object objLock = new object();
        public static ServerControl GetInstance()
        {
            if (instance == null)
            {
                lock (objLock)
                {
                    if (instance == null)
                    {
                        instance = new ServerControl();
                    }
                }
            }
            return instance;
        }
        public ServerControl()
        {
            faceList = new List<FaceInfo>()
            {
                new FaceInfo()
                {
                    type = "black",filename="101.jpg",name="101",sex="w",serialnumber=101,idnumber="101",imagebytes = Convert.ToBase64String(FileReadWriteHelper.ReadBytesFromFile(@"D:\Test\123.jpg"))
                },
                new FaceInfo()
                {
                    type = "white",filename="102.jpg",name="102",sex="m",serialnumber=102,idnumber="101",imagebytes = Convert.ToBase64String(FileReadWriteHelper.ReadBytesFromFile(@"D:\Test\124.jpg"))
                },
                new FaceInfo()
                {
                    type = "black",filename="103.jpg",name="103",sex="m",serialnumber=103,idnumber="101",imagebytes = Convert.ToBase64String(FileReadWriteHelper.ReadBytesFromFile(@"D:\Test\125.jpg"))
                },
                new FaceInfo()
                {
                    type = "white",filename="104.jpg",name="104",sex="w",serialnumber=104,idnumber="101",imagebytes = Convert.ToBase64String(FileReadWriteHelper.ReadBytesFromFile(@"D:\Test\126.jpg"))
                },
                new FaceInfo()
                {
                    type = "black",filename="105.jpg",name="105",sex="m",serialnumber=105,idnumber="101",imagebytes = Convert.ToBase64String(FileReadWriteHelper.ReadBytesFromFile(@"D:\Test\127.jpg"))
                }
            };
        }

        /// <summary>
        /// 服务端初始化
        /// </summary>
        public void ServerInit()
        {
            try
            {
                if (isLive) return;
                IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11368);
                tcpServer = new AsyncTcpServer(iPEndPoint, new ServerMessage(), new AlgorithmBuilder());
                tcpServer.Listener();
                Console.WriteLine("服务端已启动");
                isLive = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"服务端启动失败:{ex.ToString()}");
            }
        }

        /// <summary>
        /// 服务端发送数据
        /// </summary>
        /// <param name="code">功能码</param>
        /// <param name="bytes">正文</param>
        public void ServerSendMsg(byte code, byte[] bytes)
        {
            try
            {
                string base64txt = Convert.ToBase64String(bytes); //bas64加密
                byte[] base64Bytes = Encoding.UTF8.GetBytes(base64txt);
                byte[] allBytes = new byte[base64Bytes.Length + 1];
                allBytes[0] = code;
                Array.Copy(base64Bytes, 0, allBytes, 1, base64Bytes.Length);
                tcpServer?.BroadcastAsync(allBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"服务端发送数据失败:{ex.ToString()}");
            }
        }
    }

    /// <summary>
    /// 服务端接收数据
    /// </summary>
    public class ServerMessage : IAsyncTcpServerSessionMessage
    {
        private bool IsSend = false;    //是否持续发送截图 
        #region     常量
        /// <summary>
        /// code：心跳
        /// </summary>
        private const byte CODE_HEARTBEAT = 0x01;
        /// <summary>
        /// code：跟随
        /// </summary>
        private const byte CODE_FOLLOW = 0x02;
        /// <summary>
        /// code：人脸识别
        /// </summary>
        private const byte CODE_FACEIDENTIFY = 0x03;
        /// <summary>
        /// code：(人脸)名单上传
        /// </summary>
        private const byte CODE_FACEUPLOAD = 0x04;
        /// <summary>
        /// code：(人脸)名单查询
        /// </summary>
        private const byte CODE_FACESELECT = 0x05;
        /// <summary>
        /// code：(人脸)名单删除
        /// </summary>
        private const byte CODE_FACEDELETE = 0x06;
        /// <summary>
        /// code：(人脸)名单清空
        /// </summary>
        private const byte CODE_FACECLEAR = 0x07;
        /// <summary>
        /// code：(人脸)名单报警
        /// </summary>
        private const byte CODE_FACEALARM = 0x08;
        /// <summary>
        /// code：人脸对比
        /// </summary>
        private const byte CODE_FACCONTRAST = 0x40;
        /// <summary>
        /// code：(人脸对比)发送截图
        /// </summary>
        private const byte CODE_SENDIMG = 0x41;
        /// <summary>
        /// code：获取黑体坐标
        /// </summary>
        private const byte CODE_GETPOINT = 0x50;
        /// <summary>
        /// code：返回黑体坐标
        /// </summary>
        private const byte CODE_SETPOINT = 0x51;

        /// <summary>
        /// body：启动
        /// </summary>
        private const byte START = 0x1E;
        /// <summary>
        /// body：关闭
        /// </summary>
        private const byte STOP = 0x3E;
        /// <summary>
        /// body：重启
        /// </summary>
        private const byte RESTART = 0x5E;
        /// <summary>
        /// body：成功(启动成功)
        /// </summary>
        private const byte SUCCESS = 0x1F;
        /// <summary>
        /// body：失败(启动失败)
        /// </summary>
        private const byte FAIL = 0x2F;
        /// <summary>
        /// body：关闭成功
        /// </summary>
        private const byte STOPSUCCESS = 0x3F;
        /// <summary>
        /// body：关闭失败
        /// </summary>
        private const byte STOPFAIL = 0x4F;
        /// <summary>
        /// body：重启成功
        /// </summary>
        private const byte RESTARTSUCCESS = 0x5F;
        /// <summary>
        /// body：重启失败
        /// </summary>
        private const byte RESTARTFAIL = 0x6F;
        /// <summary>
        /// body：人脸白名单
        /// </summary>
        private const byte WHITEFACE = 0x1E;
        /// <summary>
        /// body：人脸黑名单
        /// </summary>
        private const byte BLACKFACE = 0x2E;
        /// <summary>
        /// body：全部人脸名单
        /// </summary>
        private const byte ALLFACE = 0x3E;

        #endregion

        public async Task OnSessionDataReceived(AsyncTcpServerSession session, byte[] data, int offset, int count)
        {
            if (data == null || data.Length <= 0) return;
            byte code = data[0];    //功能码
            byte[] bytes = new byte[data.Length - 1];   //正文数据
            Array.Copy(data, 1, bytes, 0, bytes.Length);
            string unBase64txt = Encoding.UTF8.GetString(bytes);
            byte[] unBase64Bytes = Convert.FromBase64String(unBase64txt);   //bas64解码
            if (!code.Equals(CODE_HEARTBEAT)) Console.WriteLine($"功能码:{code.ToString()},数据{Encoding.UTF8.GetString(unBase64Bytes)}");
            try
            {
                switch (code)
                {
                    case CODE_FACEUPLOAD:   //名单上传
                        {
                            string upLoadFaceStr = Encoding.UTF8.GetString(unBase64Bytes);
                            UpLoadFace upLoadFace = SerializeHelper.SerializeJsonToObject<UpLoadFace>(upLoadFaceStr);
                            byte[] imgBytes = Convert.FromBase64String(upLoadFace.imagebytes);
                            FileReadWriteHelper.WriteBytesToFile($@"D:\Test\{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.jpg", imgBytes);  //保存图片

                            ServerControl.GetInstance().faceList.Add(new FaceInfo()
                            {
                                filename = upLoadFace.filename,
                                idnumber = upLoadFace.idnumber,
                                imagebytes = upLoadFace.imagebytes,
                                name = upLoadFace.name,
                                sex = upLoadFace.sex,
                                serialnumber = upLoadFace.serialnumber,
                                type = upLoadFace.type
                            });
                            ServerControl.GetInstance().ServerSendMsg(CODE_FACEUPLOAD, new byte[] { SUCCESS });
                            Console.WriteLine($"名单上传成功,列表还有白名单{ServerControl.GetInstance().faceList.Where(p => p.type.Equals("white")).Count()}个,和名单{ServerControl.GetInstance().faceList.Where(p => p.type.Equals("black")).Count()}个！");
                            break;
                        }
                    case CODE_FACESELECT:   //名单查询
                        {
                            SelectFace selectFace = new SelectFace();
                            switch (unBase64Bytes[0])
                            {
                                case WHITEFACE:
                                    selectFace.faceList = ServerControl.GetInstance().faceList.Where(p => p.type.Equals("white")).ToList();
                                    break;
                                case BLACKFACE:
                                    selectFace.faceList = ServerControl.GetInstance().faceList.Where(p => p.type.Equals("black")).ToList();
                                    break;
                                case ALLFACE:
                                    selectFace.faceList = ServerControl.GetInstance().faceList;
                                    break;
                            }
                            string jsonStr = SerializeHelper.SerializeObjectToJson(selectFace);
                            byte[] tBytes = Encoding.UTF8.GetBytes(jsonStr);
                            ServerControl.GetInstance().ServerSendMsg(CODE_FACESELECT, tBytes);
                            Console.WriteLine($"名单查询成功,一共查到{selectFace.faceList.Count}个名单！");
                            break;
                        }
                    case CODE_FACEDELETE:   //名单删除
                        {
                            string deleteFaceStr = Encoding.UTF8.GetString(unBase64Bytes);
                            DeleteFace deleteFace = SerializeHelper.SerializeJsonToObject<DeleteFace>(deleteFaceStr);

                            var dFace = ServerControl.GetInstance().faceList.Where(p => p.type.Equals(deleteFace.type) && p.serialnumber.Equals(deleteFace.serialnumber)).First();
                            ServerControl.GetInstance().faceList.Remove(dFace);

                            ServerControl.GetInstance().ServerSendMsg(CODE_FACEDELETE, new byte[] { SUCCESS });
                            Console.WriteLine($"名单删除成功,列表还有白名单{ServerControl.GetInstance().faceList.Where(p => p.type.Equals("white")).Count()}个,和名单{ServerControl.GetInstance().faceList.Where(p => p.type.Equals("black")).Count()}个！");
                            break;
                        }
                    case CODE_FACECLEAR:    //名单清除
                        {
                            List<FaceInfo> clList = new List<FaceInfo>();
                            switch (unBase64Bytes[0])
                            {
                                case WHITEFACE:
                                    clList = ServerControl.GetInstance().faceList.Where(p => p.type.Equals("black")).ToList();
                                    break;
                                case BLACKFACE:
                                    clList = ServerControl.GetInstance().faceList.Where(p => p.type.Equals("black")).ToList();
                                    break;
                                case ALLFACE:
                                    clList = ServerControl.GetInstance().faceList;
                                    break;
                            }
                            foreach (var item in clList)
                            {
                                ServerControl.GetInstance().faceList.Remove(item);
                            }

                            ServerControl.GetInstance().ServerSendMsg(CODE_FACECLEAR, new byte[] { SUCCESS });
                            Console.WriteLine($"名单清除成功,列表还有白名单{ServerControl.GetInstance().faceList.Where(p => p.type.Equals("white")).Count()}个,和名单{ServerControl.GetInstance().faceList.Where(p => p.type.Equals("black")).Count()}个！");
                            break;
                        }
                    case CODE_FACCONTRAST:    //人脸对比
                        {
                            switch (unBase64Bytes[0])
                            {
                                case START:
                                    ServerControl.GetInstance().ServerSendMsg(CODE_FACCONTRAST, new byte[] { SUCCESS });
                                    Console.WriteLine("启动人脸对比成功，抓图中。。。");
                                    SendImg();
                                    break;
                                case STOP:
                                    ServerControl.GetInstance().ServerSendMsg(CODE_FACCONTRAST, new byte[] { SUCCESS });
                                    IsSend = false;
                                    Console.WriteLine("关闭人脸对比成功!");
                                    break;
                            }
                            break;
                        }
                    case CODE_FOLLOW:   //跟随
                        {
                            switch (unBase64Bytes[0])
                            {
                                case START:
                                    ServerControl.GetInstance().ServerSendMsg(CODE_FOLLOW, new byte[] { SUCCESS });
                                    Console.WriteLine("启动跟随成功!");
                                    break;
                                case STOP:
                                    ServerControl.GetInstance().ServerSendMsg(CODE_FOLLOW, new byte[] { SUCCESS });
                                    Console.WriteLine("关闭跟随成功!");
                                    break;
                                case RESTART:
                                    ServerControl.GetInstance().ServerSendMsg(CODE_FOLLOW, new byte[] { SUCCESS });
                                    Console.WriteLine("重新跟随成功!");
                                    break;
                            }
                            break;
                        }
                    case CODE_GETPOINT: //获取黑体坐标
                        {
                            BlackBodyPoint blackBodyPoint = new BlackBodyPoint()
                            {
                                xpoint = 55,
                                ypoint = 105
                            };
                            string jsonStr = SerializeHelper.SerializeObjectToJson(blackBodyPoint);
                            string base64txt = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonStr));   //bas64编码
                            byte[] body = Encoding.UTF8.GetBytes(base64txt);
                            ServerControl.GetInstance().ServerSendMsg(CODE_SETPOINT, body);
                            Console.WriteLine("启动人脸对比成功，抓图中。。。");
                            break;
                        }
                    case CODE_FACEIDENTIFY: //人脸识别
                        break;
                    case CODE_FACEALARM:    //人脸报警
                        break;
                    case CODE_HEARTBEAT:    //心跳
                        break;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
                Console.WriteLine($"解析接收数据异常:{ex.ToString()}");
            }
        }

        private void SendImg()
        {
            IsSend = true;
            Task.Factory.StartNew(() =>
            {
                while (IsSend)
                {
                    byte[] bytes;
                    using (FileStream fs = new FileStream(@"D:\Test\111.jpg", FileMode.Open))
                    {
                        bytes = new byte[fs.Length];
                        fs.Read(bytes, 0, (int)fs.Length);
                    }
                    string base64txt = Convert.ToBase64String(bytes);   //bas64编码

                    AlgorithmRequestParam rparam = new AlgorithmRequestParam()
                    {
                        camNo = "53010102",
                        image = base64txt,
                        imageBody = base64txt
                    };
                    string jsonStr = SerializeHelper.SerializeObjectToJson(rparam);
                    ServerControl.GetInstance().ServerSendMsg(CODE_SENDIMG, Encoding.UTF8.GetBytes(jsonStr));
                    Thread.Sleep(TimeSpan.FromSeconds(3));
                }
            });
        }

        public async Task OnSessionClosed(AsyncTcpServerSession session)
        {
            Console.WriteLine("客户端断开连接！");
        }

        public async Task OnSessionError(string msg, Exception ex)
        {
            Console.WriteLine("客户端连接错误！");
        }

        public async Task OnSessionStarted(AsyncTcpServerSession session)
        {
            Console.WriteLine("客户端建立连接！");
        }
    }
}
