using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ULTRON_2016
{
    class KoleksiData
    {
        public class Sensor
        {
            public double No { set; get; }
            public float Yaw { set; get; }
            public float Pitch { set; get; }
            public float Roll { set; get; }
            public double Elevasi { set; get; }
            public double Tinggi { set; get; }
            public double Kecepatanx { set; get; }
            public double Kecepatany { set; get; }
            public double Kecepatanz { set; get; }
            public double Suhu { set; get; }
            public double Tekanan { set; get; }
            public string Latitude { set; get; }
            public string Longitude { set; get; }
            public string dataLengkap { set; get; }
        }
    }
}
