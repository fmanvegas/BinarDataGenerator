using BinarDataGenerator.Models;
using BinarDataGenerator.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace BinarDataGenerator.ViewModels
{
    public class ReportViewModel
    {
        BDataObject myData;

        public ReportViewModel()
        {
            Test.Clear();

            Random r = new Random();
            for (int i = 0; i < 100; i++)
            {
                ReportRow row = new ReportRow(r.Next(2,3000) );
                row.Columns = new List<ReportColumn>();

                for (int x = 0; x < 100; x++)
                {
                    row.Columns.Add(new ReportColumn(r.Next(2,2000) ));
                }
                Test.Add(row);
            }

        }

        public RelayCommand ShowPolarCommand { get; } = new RelayCommand(showPolarAction);

        private static void showPolarAction(object obj)
        {

        }

        internal List<string> Sectors { get; } = new List<string>() { "Front", "RF", "Right", "RR", "Rear", "LR", "Left", "LF" };

        public ObservableCollection<ReportRow> Test { get; set; } = new ObservableCollection<ReportRow>();
        public ObservableCollection<ReportRow> Format1 { get; set; } = new ObservableCollection<ReportRow>();
        public ObservableCollection<ReportRow> Format4 { get; set; } = new ObservableCollection<ReportRow>();

        internal void SetView(DataGrid dg)
        {
            dg.Columns.Clear();

            if (Test.Count < 1)
                return;
            int i = 0;
            foreach (var c in Test.First().Columns)
            {
                DataGridTextColumn textColumn = new DataGridTextColumn();
                //make a new source
                Binding myBinding = new Binding($"Columns[{i++}].Entry");
                myBinding.Source = c;
                BindingOperations.SetBinding(textColumn, TextBlock.TextProperty, myBinding);
                textColumn.Header = c.ColumnHeader;
                //textColumn.Binding = new Binding($"Columns[{i++}].Entry");
                dg.Columns.Add(textColumn);
            }
        }

        /// <summary>
        /// 
        ///     Generate the data that will drive the report
        /// 
        /// </summary>
        /// <param name="data"></param>
        internal void Generate(BDataObject data = null)
        {
            if (data != null)
                myData = data;

            if (myData is null)
                return;

            generateFormat1();
            generateFormat4();
        }
        private void generateFormat1()
        {
            Format1.Clear();
            Random r = new Random();

            // Each ROW = Frequency.  Within each row, each COLUMN = Sector
            //  Does this actually matter?
            foreach (var freq in myData.Frequencies)
            {
                var row = new ReportRow(freq);
                //For each sector
                foreach(var sector in Sectors)
                {
                    row.Columns.Add(new ReportColumn(row, sector, r.Next(-3, 10)));
                }
                Format1.Add(row);
            }
        }
        private void generateFormat4()
        {
            //Eeach ROW = Depression & Sector.  Within each row, each column = Frequency
            //  Does this actually matter?
            Format4.Clear();


        }
    }

    /// <summary>
    /// Row contains each FREQ down, and within it each SECTOR in columns
    /// </summary>
    public class ReportRow
    {
        public ReportRow(double i)
        {
            RowHeader = $"{i:N2}MHZ";
        }

        public List<ReportColumn> Columns { get; internal set; } = new List<ReportColumn>();

        public string RowHeader { get; }
    }

    /// <summary>
    /// COLUMN contains each SECTOR VALUE for a specific ROW FREQ
    /// </summary>
    public class ReportColumn
    {
        public string ColumnHeader { get; set; }
        //public List<ReportEntry> Rows { get; internal set; } = new List<ReportEntry>();

        internal ReportRow Row;

        public double Value { get; set; } 

        public ReportEntry Entry { get; }

        public ReportColumn(int i)
        {
            ColumnHeader = $"Header {i}";
            Value = 2 * (i + 1) * 7 / 3;
        }
        public ReportColumn(ReportRow row, string header, double value)
        {
            Row = row;
            ColumnHeader = header;
            Entry = new ReportEntry(row, this, value);
            Value = value;
        }
    }
    /// <summary>
    /// This is the data element that displays its own value
    /// </summary>
    public class ReportEntry : PropChanged
    {
        private ReportRow row;
        private ReportColumn reportColumn;
        private double value;

        public ReportRow Row => row;
        public ReportColumn Column => reportColumn;
        public double Value => value;

        public ReportEntry(ReportRow row, ReportColumn reportColumn, double value)
        {
            this.row = row;
            this.reportColumn = reportColumn;
            this.value = value;
        }

        public override string ToString() => $"{value:N2}";
    }

}
