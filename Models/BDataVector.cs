using BinarDataGenerator.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BinarDataGenerator.Models
{
    public class BDataVector : BDataBase
    {
        public double Depression { get; private set; }
        public double Frequency { get; private set; }
        public string POL { get; private set; }
        public List<BDataRecord> Data;




        public BDataVector(double d, double f, string p, int pol = -1)
        {
            Depression = d;
            DepKey = (int)Depression * 10;

            Frequency = f;
            FreqKey = (int)Frequency * 10000;
            POL = p;

            if (POLKey != -1)
            {
                POLKey = pol;
                KeyFreqDepPol = (DepKey) + (FreqKey) * (POLKey);

            }

            Data = new List<BDataRecord>(360);
        }

        public override void FinalizePOLKey(int pol)
        {
            base.FinalizePOLKey(pol);
            if (Data != null)
                Parallel.ForEach(Data, (d) => { d.FinalizePOLKey(pol); });

            KeyFreqDepPol = (DepKey) + (FreqKey) * (POLKey);
        }

        public override int GetHashCode() => KeyFreqDepPol;
        public static bool operator ==(BDataVector one, BDataVector two)
        {
            if (one is null || two is null)
                return false;

            return one.KeyFreqDepPol == two.KeyFreqDepPol;
        }
        public static bool operator !=(BDataVector one, BDataVector two)
        {
            return (!(one == two));
        }
        public override bool Equals(object obj)
        {
            if (obj is BDataVector v)
                return this == v;

            return false;
        }
        public override string ToString()
        {
            return $"{POL} {Frequency} {Depression}";
        }

        internal void Create360DegData(MainViewModel vm, IList stats)
        {
            //For each stat, create 360 degress of data
            Random r = new Random();
            //Create 360 data spots

            for (int i = 0; i < 360; i++)
            {
                BDataRecord record = new BDataRecord(stats);

                //Now for each record, create data for each stat
                foreach (string s in stats)
                {
                    record.Values.Add(r.Next(0, 30));
                    vm.ValueCurrent++;

                }
                vm.ObjectCurrent++;

                Data.Add(record);
            }

        }
    }

}
