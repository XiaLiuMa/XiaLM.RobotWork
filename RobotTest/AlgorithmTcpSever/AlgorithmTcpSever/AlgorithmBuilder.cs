using Loxi.Core.Tcp;
using Loxi.Core.Tcp.Codecs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AlgorithmTcpSever
{
    public class AlgorithmBuilder : ICodecBuilder
    {
        private ICodecDecoding decoding = new AlgorithmDecoding();
        private ICodecEncoding encoding = new AlgorithmEncoding();
        public ICodecDecoding DecodingBuilder => decoding;

        public ICodecEncoding EncodingBuilder => encoding;

        public ICodecBuilder Clone()
        {
            return new AlgorithmBuilder();
        }
    }

    public class AlgorithmDecoding : ICodecDecoding
    {
        List<byte> _list = new List<byte>();
        private byte _head = 0xEF;
        private byte _end = 0xFF;
        public void Decoding(TcpBuffer playload, DataAnalysisResults callback)
        {
            _list.AddRange(playload.Datas);
            while (true)
            {
                if (_list.Count <= 0) break;
                if (_list[0] != _head)
                {
                    _list.RemoveAt(0);
                    continue;
                }
                if (_list.Count >= 6)
                {
                    var len = BitConverter.ToInt32(_list.Skip(2).Take(4).Reverse().ToArray(), 0);
                    if (_list.Count >= len)
                    {
                        var data = _list.Take(len).ToList();
                        if (data[len - 2] != 0xFF)
                        {
                            _list.RemoveAt(0);
                            continue;
                        }
                        var crc = data[len - 1];
                        var crc2 = data.Take(len - 1).BccVerifica(0, len - 1);
                        if (crc != crc2)
                        {
                            _list.RemoveAt(0);
                            continue;
                        }
                        var code = data[1];
                        var temp = data.Skip(6).Take(len - 8).ToList();
                        temp.Insert(0, code);
                        var datas = temp.ToArray();
                        _list.RemoveRange(0, len);
                        callback?.Invoke(new TcpBuffer(datas, 0, datas.Length));
                    }
                    else break;

                }
                else break;
            }
        }
    }
    public class AlgorithmEncoding : ICodecEncoding
    {
        public TcpBuffer Encoding(TcpBuffer buffer)
        {
            if (buffer.Count < 2) return null;
            byte code = buffer.Datas[0];
            List<byte> list = new List<byte>();
            list.Add(0xEF);
            list.Add(code);
            list.AddRange(BitConverter.GetBytes(buffer.Count - 1 + 8).Reverse());
            list.AddRange(buffer.Datas.Skip(1));
            list.Add(0xFF);
            list.Add(list.BccVerifica(0, list.Count));
            var data = list.ToArray();
            return new TcpBuffer(data, 0, data.Length);
        }
    }
}
