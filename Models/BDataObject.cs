using BinarDataGenerator.ViewModels;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D.Converters;

namespace BinarDataGenerator.Models
{

    public class BDataBase
    {
        public int POLValue = -1;
        public int STATValue = -1;
        public int Hash = 0;

        public virtual void FinalizePOLValue(int pol)
        {
            POLValue = pol;
        }

    }


    public class BDataObject
    {
        internal List<BDataObject> Children;

        public BDataObject()
        {
            Children = new List<BDataObject>();
        }
        public BDataObject(string pol)
        {
            POLs.Add(pol);
        }



        //Found in file(s)
        public List<double> Depressions { get; set; } = new List<double>();
        //Found in file(s)
        public List<double> Frequencies { get; set; } = new List<double>();
        //Found in file(s)
        public List<double> Values { get; set; } = new List<double>();

        internal async Task<bool> Create(MainViewModel vm, IList frqs, IList deps, IList pols, IList stats)
        {
            VectorHash = new Hashtable();
            Vectors = new List<BDataVector>();

            await Task.Run(() =>
            {
                //Create a vector
                foreach (string p in pols)
                {
                    int polValue = p.GetHashCode();
                    if (!POLs.Contains(p))
                        POLs.Add(p);


                    //        BDataObject child = new BDataObject(p);


                    foreach (double d in deps)
                    {
                        if (!Depressions.Contains(d))
                            Depressions.Add(d);

                        foreach (double f in frqs)
                        {
                            if (!Frequencies.Contains(f))
                                Frequencies.Add(f);

                            vm.VectorCurrent++;

                            BDataVector vector = new BDataVector(d, f, p, polValue);
                            vector.CreateStats(vm, stats);
                            vector.FinalizePOLValue(polValue);

                            // child.Vectors.Add(vector);
                            //              child.VectorHash.Add(vector.Hash, vector);
                            //              child.Vectors.Add(vector);
                            VectorHash.Add(vector.Hash, vector);
                            Vectors.Add(vector);
                        }
                    }

                    //    Children.Add(child);
                }
            });



            return true;
        }

        internal void Format()
        {
            if (VectByDep.Count > 0)
            {
                var firstDepVectors = VectByDep[Depressions[0]] as Hashtable ;

                BDataVector test = new BDataVector(-90, 142, "HH", "HH".GetHashCode() );
                var find = firstDepVectors[test.Hash];

                return;
            }


            foreach (double dep in Depressions)
            {
                Hashtable table = new Hashtable();

                var vectsOfThisDepression = (from v in Vectors where v.Depression == dep select v);// VectByDep.Add(dep, v);

                vectsOfThisDepression.ToList().ForEach(x => table.Add(x.Hash, x));

                VectByDep.Add(dep, table);
            }


            foreach (double frq in Frequencies)
            {
                Hashtable table = new Hashtable();

                var vectsOfThisFreq = (from v in Vectors where v.Frequency == frq select v);// VectByDep.Add(dep, v);

                vectsOfThisFreq.ToList().ForEach(x => table.Add(x.Hash, x));

                VectByFreq.Add(frq, table);
            }


            foreach (string pol in POLs)
            {
                Hashtable table = new Hashtable();
                var polHash = pol.GetHashCode();


                var vectsofThisPol = (from v in Vectors where v.POLValue == polHash select v);// VectByDep.Add(dep, v);

                vectsofThisPol.ToList().ForEach(x => table.Add(x.Hash, x));

                VectByPOL.Add(polHash, table);
            }

        }

        //Found in file(s)
        public List<string> STATs { get; set; } = new List<string>();
        //Found in file(s)
        public List<string> POLs { get; set; } = new List<string>();

        //Built from above
        public List<BDataVector> Vectors { get; set; } = new List<BDataVector>();
        //Populated from above
        public Hashtable VectorHash { get; set; } = new Hashtable();
        public Hashtable VectByDep { get; set; } = new Hashtable();
        public Hashtable VectByFreq { get; set; } = new Hashtable();
        public Hashtable VectByPOL { get; set; } = new Hashtable();



        public void FinalizeLoad()
        {
            if (Children != null && Children.Count > 0)
            {
                FinalizeChildren();
            }

            CombineChildren();
        }
        private void FinalizeChildren()
        {
            if (POLs.Count < 1)
                return;

            int pol = POLs[0].GetHashCode();
            //At this point, I need to send my children their POL value
            Parallel.ForEach(Vectors, (d) => { d.FinalizePOLValue(pol); });
        }
        private void CombineChildren()
        {

        }
    }

    public class BDataVector : BDataBase
    {
        public double Depression { get; private set; }
        public double Frequency { get; private set; }
        public string POL { get; private set; }
        public List<BDataRecord> Data;


        public BDataVector(double d, double f, string p, int polValue = -1)
        {
            Depression = d;
            Frequency = f;
            POL = p;
            if (polValue != -1)
            {
                POLValue = polValue;
                Hash = (int)(Depression * 10) + (int)(Frequency * 10000) * (POLValue);

            }

            Data = new List<BDataRecord>(360);
        }

        public override void FinalizePOLValue(int pol)
        {
            base.FinalizePOLValue(pol);
            if (Data != null)
                Parallel.ForEach(Data, (d) => { d.FinalizePOLValue(pol); });

            Hash = (int)(Depression * 10) + (int)(Frequency * 10000) * (POLValue);
        }

        public override int GetHashCode() => Hash;
        public static bool operator ==(BDataVector one, BDataVector two)
        {
            if (one is null || two is null)
                return false;

            return one.Hash == two.Hash;
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

        internal void CreateStats(MainViewModel vm, IList stats)
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
