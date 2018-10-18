using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmTcpSever.Model
{
    public class SelectFace
    {
        public List<FaceInfo> faceList { get; set; }
    }

    public class FaceInfo
    {
        public string type { get; set; }
        public int toal { get; set; }
        public int index { get; set; }
        public string filename { get; set; }
        public string name { get; set; }
        public string sex { get; set; }
        public int serialnumber { get; set; }
        public string idnumber { get; set; }
        public string imagebytes { get; set; }
    }
}
