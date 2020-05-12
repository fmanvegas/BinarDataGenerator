using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace BinarDataGenerator.Models
{


    public class BDataRecord : BDataBase
    {
        public BDataRecord(IList stats)
        {
            Values = new List<double>(stats.Count);
        }
        public BDataRecord(double dep, byte stat, double[] vals)
        {
            Depression = dep;
            StatByte = stat;
            foreach (var v in vals)
                Values.Add(v);
        }



        public double Depression { get; private set; }
        public string STAT { get; private set; }

        public byte StatByte { get; set; }

        public string POL { get; private set; }

        public List<double> Values { get; private set; }


        public Dictionary<byte, double> Data = new Dictionary<byte, double>();
    }
}
