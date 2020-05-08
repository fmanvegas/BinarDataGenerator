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
        public MainViewModel()
        {
            Depressions = new List<double>();
            Frequencies = new List<double>();

            double value = -90;
            do
            {
                Depressions.Add(value);
                value += 1.5;
            } while (value < 90);


            double freq = 142.0;
            do
            {
                Frequencies.Add(freq);
                freq += 353;

            } while (freq < 32000);


        }
        internal async void CreateData(System.Collections.IList frqs, System.Collections.IList deps, System.Collections.IList pols, System.Collections.IList stats)
        {
            ValueCurrent = 0;
            VectorCurrent = 0;
            ObjectCurrent = 0;


            BDataObject binData = new BDataObject();

            await binData.Create(this, frqs, deps, pols, stats);

            BinnedData.Add(binData);

            MessageBox.Show($"Vect:{VectorCurrent}, Obj:{ObjectCurrent}, Val:{ValueCurrent}");
            SetDataToLists();
        }

        public Hashtable HashTableSet { get=>_hashTableSet; set { _hashTableSet = value; OnPropChanged(); } }
        private Hashtable _hashTableSet { get; set; }

        public ObservableCollection<BDataObject> BinnedData { get; } = new ObservableCollection<BDataObject>();

        public List<string> STATS { get; } = new List<string>() { "MIN", "MAX", "MED", "STD", "CNT", "GMN", "AVG" };

        public List<double> Frequencies { get; private set; }

        public List<double> Depressions { get; private set; }

        public List<string> POLS { get; } = new List<string>() { "HH", "VV", "HV", "VH", "CC" };

        internal void SetDataToLists()
        {
            BinnedData[0].Format();
        }

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


        private BDataVector _selectedVector;
        public BDataVector SelectedVector { get => _selectedVector; set { _selectedVector = value; OnPropChanged(); } }
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
