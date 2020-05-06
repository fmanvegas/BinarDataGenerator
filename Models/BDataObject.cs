using BinarDataGenerator.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace BinarDataGenerator.Models
{
		
    class BDataObject
    {
		internal List<BDataObject> DataParts;


		public List<double> Depressions { get; set; } = new List<double>();
		public List<double> Frequencies { get; set; } = new List<double>();
		public List<double> Values { get; set; } = new List<double>();
		public List<string> STATs { get; set; } = new List<string>();
		public ObservableCollection<BDataRecord> Data { get; set; } = new ObservableCollection<BDataRecord>();


		public void Write()
		{

		}
	}
		

	public class BDataRecord
	{
		public BDataRecord(double dep, string pol, string stat, double[] vals)
		{
			Depression = dep;
			STAT = stat;
			POL = pol;
			foreach (var v in vals)
				Values.Add(v);
		}

		public void Finish(int stat, int pol)
		{
			STATValue = stat;
			POLValue = pol;
		}

		public double Depression { get; private set; }
		public string STAT { get; private set; }
		public int STATValue;
		public string POL { get; private set; }
		public int POLValue;

		public List<double> Values { get; private set; }
	}
}
