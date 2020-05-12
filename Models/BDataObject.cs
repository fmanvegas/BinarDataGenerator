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
using System.Windows;
using System.Windows.Media.Media3D.Converters;

namespace BinarDataGenerator.Models
{
    //          Hash = (int)(Depression * 10) + (int)(Frequency * 10000) * (POLKey);
    public class BDataBase
    {
        public int POLKey = -1;
        public int STATKey = -1;

        public int DepKey = 0;
        public int FreqKey = 0;

        public int KeyFreqDepPol = 0;


        public virtual void FinalizePOLKey(int pol)
        {
            POLKey = pol;
        }

    }


    public class BDataObject
    {
        internal List<BDataObject> Children;

        public int NeutralFileName { get;  }

        public BDataObject(int index)
        {
            NeutralFileName = index;
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

      

        //Found in file(s)
        public List<string> STATs { get; set; } = new List<string>();

        internal Dictionary<int, BDataVector> GetData(object frq, object dep, object pol)
        {
            var vectToReturn = VectorDic;

            //get any freqs
            if (frq is double f)
            {
                var match = VectByFreq[f];
                vectToReturn = vectToReturn.Intersect(match).ToDictionary(x => x.Key, s=>s.Value);
            }

            //get any deps
            if (dep is double d)
            {
                var match = VectByDep[d];
                vectToReturn = vectToReturn.Intersect(match).ToDictionary(x => x.Key, s => s.Value);
            }


            //get any pols
            if (pol is string p)
            {
                var match = VectByPOL[p];
                vectToReturn = vectToReturn.Intersect(match).ToDictionary(x => x.Key, s => s.Value);
            }

            return vectToReturn;
        }

        //Found in file(s)
        public List<string> POLs { get; set; } = new List<string>();

        //Built from above
        public List<BDataVector> Vectors { get; set; } = new List<BDataVector>();

        /// <summary>
        ///  Key:  Vector.Hash (Freq, Dep, Pol)
        /// </summary>
       // public Hashtable VectorHash { get; private set; } = new Hashtable();
        public Dictionary<int, BDataVector> VectorDic { get; private set; }


        /// <summary>
        /// Key: Dep (double)
        /// </summary>
        public Dictionary<double, Dictionary<int, BDataVector>> VectByDep { get; private set; } = new Dictionary<double, Dictionary<int, BDataVector>>();

        /// <summary>
        /// Key: Dep * Freq
        /// </summary>
        public Dictionary<double, Dictionary<int, BDataVector>> VectByDEP_Freq { get; private set; } = new Dictionary<double, Dictionary<int, BDataVector>>();
        /// <summary>
        /// Key: Freq (double)
        /// </summary>
        public Dictionary<double, Dictionary<int, BDataVector>> VectByFreq { get; private set; } = new Dictionary<double, Dictionary<int, BDataVector>>();

        /// <summary>
        /// Key: Freq * Pol
        /// </summary>
        public Dictionary<double, Dictionary<int, BDataVector>> VectByFreq_POL { get; private set; } = new Dictionary<double, Dictionary<int, BDataVector>>();
        /// <summary>
        /// Key Pol: (double==polHash)
        /// </summary>
        public Dictionary<string, Dictionary<int, BDataVector>> VectByPOL { get; private set; } = new Dictionary<string, Dictionary<int, BDataVector>>();
        /// <summary>
        /// Key: Pol * Dep
        /// </summary>
        public Dictionary<double, Dictionary<int, BDataVector>> VectByPOL_DEP { get; private set; } = new Dictionary<double, Dictionary<int, BDataVector>>();


        internal async Task<bool> Create(MainViewModel vm, IList frqs, IList deps, IList pols, IList stats)
        {
            VectorDic = new Dictionary<int, BDataVector>();

            Vectors = new List<BDataVector>();

            await Task.Run(() =>
            {
                //Create a vector
                foreach (string p in pols)
                {
                    int pKey = p.ToPolKey();

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

                            BDataVector vector = new BDataVector(d, f, p, pKey);
                            vector.Create360DegData(vm, stats);
                            vector.FinalizePOLKey(pKey);


                            VectorDic.Add(vector.KeyFreqDepPol, vector);

                            Vectors.Add(vector);
                        }
                    }

                }
            });



            return true;
        }

        internal void Format()
        {
            //if (VectByDep.Count > 0)
            //{
            //    var firstDepVectors = VectByDep[Depressions[0]] as Hashtable;

            //    BDataVector test = new BDataVector(-90, 142, "HH", "HH".GetHashCode());
            //    var find = firstDepVectors[test.Hash];

            //    return;
            //}

            //Hash by Depression
            foreach (double dep in Depressions)
            {
                var vectsOfThisDepression = (from v in Vectors where v.Depression == dep select v).ToDictionary(x => x.KeyFreqDepPol);
                VectByDep.Add(dep, vectsOfThisDepression);
            }

            //Hash by Frequency
            foreach (double frq in Frequencies)
            {
                var vectsOfThisFreq = (from v in Vectors where v.Frequency == frq select v).ToDictionary(x => x.KeyFreqDepPol);
                VectByFreq.Add(frq, vectsOfThisFreq);
            }

            //Hash by Frequency + Pol
            foreach (double frq in Frequencies)
            {
                foreach (string pol in POLs)
                {
                    var polHash = pol.GetHashCode();
                    var key = frq * polHash;

                    var freqPolMatch = (from v in Vectors where v.Frequency == frq && v.POLKey == polHash select v).ToDictionary(x => x.KeyFreqDepPol);
                    VectByFreq_POL.Add(key, freqPolMatch);
                }


            }

            //Hash by Pol
            foreach (string pol in POLs)
            {
                var polHash = pol.GetHashCode();
                var vectsofThisPol = (from v in Vectors where v.POLKey == polHash select v).ToDictionary(x => x.KeyFreqDepPol);
                VectByPOL.Add(pol, vectsofThisPol);
            }
            //Hash by Pol + Dep
           
            foreach (string pol in POLs)
            {
                var polHash = pol.GetHashCode();

                foreach (double dep in Depressions)
                {
                    var key = polHash * dep;
                    if (key == 0)
                        key = 99+polHash;

                    var depPolMatch = (from v in Vectors where v.Depression == dep && v.POLKey == polHash select v).ToDictionary(x => x.KeyFreqDepPol);
                    VectByPOL_DEP.Add(key, depPolMatch);
                }
            }


        }


        //public void FinalizeLoad()
        //{
        //    if (Children != null && Children.Count > 0)
        //    {
        //        FinalizeChildren();
        //    }

        //    CombineChildren();
        //}
        //private void FinalizeChildren()
        //{
        //    if (POLs.Count < 1)
        //        return;

        //    int pol = POLs[0].GetHashCode();
        //    //At this point, I need to send my children their POL value
        //    Parallel.ForEach(Vectors, (d) => { d.FinalizePOLKey(pol); });
        //}
        //private void CombineChildren()
        //{

        //}

        public override string ToString() => $"BinnedFile_{NeutralFileName}";
    }

   

}
