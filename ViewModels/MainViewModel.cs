using BinarDataGenerator.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BinarDataGenerator.ViewModels
{
    public class MainViewModel : PropChanged
    {
        private static MainViewModel instance;

        public static MainViewModel Instance
        {
            get
            {
                if (instance is null)
                    instance = new MainViewModel();

                return instance;
            }
        }

        private MainViewModel()
        {
            Depressions = new List<double>();
            Frequencies = new List<double>();

            double value = -90;
            do
            {
                Depressions.Add(value);
                value += 1.5;
            } while (value < 90);


            double freq = 100.00;
            do
            {
                Frequencies.Add(freq);
                freq += 353;

            } while (freq < 32000);

        }


        #region Properties


        /// <summary>
        /// The list of all our data objects
        /// </summary>
        public ObservableCollection<BDataObject> BinnedData { get; } = new ObservableCollection<BDataObject>();

        public ReportViewModel Report { get; private set; } = new ReportViewModel();

        #region Default selections to create data

        public List<string> STATS { get; } = new List<string>() { "STA1", "STA2", "STA3", "STA4", "STA5", "STA6", "CNT" };

        public List<double> Frequencies { get; private set; }

        public List<double> Depressions { get; private set; }

        public List<string> POLS { get; } = new List<string>() { "P1", "P2", "P3", "P4", "P5" };

        #endregion


        public int VectorMax { get => _VectorMax; set { _VectorMax = value; OnPropChanged(); } }
        private int _VectorMax = 0;

        public int VectorCurrent { get => _vectorCurrent; set { _vectorCurrent = value; OnPropChanged(); } }
        private int _vectorCurrent = 0;

        public int ObjectMax { get => _status; set { _status = value; OnPropChanged(); } }
        private int _status = 0;
        public int ObjectCurrent { get => _objectCurrent; set { _objectCurrent = value; OnPropChanged(); } }
        private int _objectCurrent = 0;

        public int ValueMax { get => _ValueMax; set { _ValueMax = value; OnPropChanged(); } }
        private int _ValueMax = 0;
        public int ValueCurrent { get => _valueCurrent; set { _valueCurrent = value; OnPropChanged(); } }
        private int _valueCurrent = 0;


        /// <summary>
        /// selected vector for display purposes
        /// </summary>
        public BDataVector SelectedVector { get => _selectedVector; set { _selectedVector = value; OnPropChanged(); } }
        private BDataVector _selectedVector;

        public Dictionary<int, BDataVector> Records { get => _records; set { _records = value; OnPropChanged(); } }
        private Dictionary<int, BDataVector> _records;

        public ObservableCollection<BDataVector> ViewableRecords { get; set; } = new ObservableCollection<BDataVector>();

        /// <summary>
        /// The bin object we selected
        /// </summary>
        public BDataObject SelectedBin { get => _selectedBin; set { _selectedBin = value; OnPropChanged(); } }
        private BDataObject _selectedBin;

        #endregion


        /// <summary>
        /// 
        ///     Create a data object based on the selections
        /// 
        /// 
        /// </summary>
        /// <param name="frqs"></param>
        /// <param name="deps"></param>
        /// <param name="pols"></param>
        /// <param name="stats"></param>
        internal async void CreateData(IList frqs, IList deps, IList pols, IList stats)
        {           
            //Create a new, main object on this thread
            BDataObject binData = new BDataObject(BinnedData.Count);

            //Tell it to populate on another thread
            await binData.Create(this, frqs, deps, pols, stats);

            binData.Format();

            //Add teh finished result
            BinnedData.Add(binData);

            //Prompt the user
            //MessageBox.Show($"Vect:{VectorCurrent}, Obj:{ObjectCurrent}, Val:{ValueCurrent}");

            //Reset our counts
            ValueCurrent = 0;
            VectorCurrent = 0;
            ObjectCurrent = 0;
        }


      
    }




    public class PropChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
