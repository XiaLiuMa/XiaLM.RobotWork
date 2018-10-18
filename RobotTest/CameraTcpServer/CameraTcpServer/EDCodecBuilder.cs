using Loxi.Tool;
using Loxi.Tool.Tcp;
using Loxi.Tool.Tcp.Codecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraTcpServer
{
    public class EDCodecBuilder : ICodecBuilder
    {
        private ICodecDecoding _decoding;
        private ICodecEncoding _encoding;
        public EDCodecBuilder()
        {
            _decoding = new EDDecoding();
            _encoding = new EDEncoding();
        }
        public ICodecDecoding DecodingBuilder => _decoding;

        public ICodecEncoding EncodingBuilder => _encoding;

        public ICodecBuilder Clone()
        {
            return new EDCodecBuilder();
        }
    }
    public class EDDecoding : ICodecDecoding
    {
        private List<byte> _list = new List<byte>();
        public void Decoding(TcpBuffer playload, DataAnalysisResults callback)
        {
            _list.AddRange(playload.Datas);
            while (true)
            {
                if (_list.Count < 1) break;
                if (_list.Count >= 1)
                {
                    if (_list[0] != 0xEA)
                    {
                        _list.RemoveAt(0);
                        continue;
                    }
                }
                if (_list.Count >= 2)
                {
                    if (_list[1] != 0x56)
                    {
                        _list.RemoveAt(0);
                        continue;
                    }
                }
                if (_list.Count >= 10)
                {
                    int pageLen = BitConverter.ToInt32(_list.Skip(6).Take(4).Reverse().ToArray(), 0);//包长度
                    if (pageLen <= 0)
                    {
                        _list.RemoveAt(0);
                        continue;
                    }
                    if (_list.Count < pageLen) break;
                    if (_list.Count >= pageLen)
                    {
                        var arry = _list.Take(pageLen).Skip(pageLen - 3).ToArray();//消息尾有三个字节，因为校验码占2个字节
                        if (arry[0] != 0xFF)
                        {
                            _list.RemoveAt(0);
                            continue;
                        }
                        CRC crc = new CRC();
                        ushort checksum1 = crc.CRC16_MODBUS(_list.Take(pageLen - 2).ToArray());
                        ushort checksum2 = BitConverter.ToUInt16(arry.Skip(1).ToArray(), 0);
                        if (checksum1 != checksum2)
                        {
                            _list.RemoveAt(0);
                            continue;
                        }
                        var dataLen = BitConverter.ToInt32(_list.Skip(10).Take(4).Reverse().ToArray(), 0);
                        if (dataLen > pageLen - (2 + 4 + 4 + 4 + 1 + 2))
                        {
                            _list.RemoveAt(0);
                            continue;
                        }
                        var data = _list.Skip(14).Take(dataLen).ToArray();
                        List<byte> temps = new List<byte>();
                        temps.AddRange(_list.Skip(2).Take(4).Reverse());
                        temps.AddRange(data);
                        callback?.Invoke(new TcpBuffer() { Datas = temps.ToArray(), Count = temps.Count, Offset = 0 });
                        _list.RemoveRange(0, pageLen);
                    }
                }
                else break;
            }
        }
    }

    public class EDEncoding : ICodecEncoding
    {
        public TcpBuffer Encoding(TcpBuffer buffer)
        {
            var codeArr = buffer.Datas.Take(4).Reverse().ToArray();//功能码
            var data = buffer.Datas.Skip(4).ToArray();//数据
            List<byte> list = new List<byte>();
            list.AddRange(new byte[] { 0xEA, 0x56 });//数据头
            list.AddRange(codeArr);//功能码
            list.AddRange(BitConverter.GetBytes(2 + 4 + 4 + 4 + data.Length + 1 + 2).Reverse());//包长度
            list.AddRange(BitConverter.GetBytes(data.Length).Reverse());//数据区长度
            list.AddRange(data);//数据区
            list.Add(0xFF);//消息尾
            list.AddRange(BitConverter.GetBytes(new CRC().CRC16_MODBUS(list.ToArray())));//校验码
            return new TcpBuffer() { Datas = list.ToArray(), Count = list.Count, Offset = 0 };
        }
    }
}
