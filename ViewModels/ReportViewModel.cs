using BinarDataGenerator.Models;
using BinarDataGenerator.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using static BinarDataGenerator.ViewModels.ReportViewModel;

namespace BinarDataGenerator.ViewModels
{
    public class ReportViewModel : PropChanged
    {
        BDataObject myData;

        public ReportViewModel()
        {
            Test.Clear();

            Random r = new Random();
            for (int i = 0; i < 100; i++)
            {
                ReportRow row = new ReportRow(r.Next(2, 3000));
                row.Columns = new List<ReportColumn>();

                for (int x = 0; x < 100; x++)
                {
                    row.Columns.Add(new ReportColumn(r.Next(2, 2000)));
                }
                Test.Add(row);
            }

        }

        public RelayCommand ShowPolarCommand { get; } = new RelayCommand(showPolarAction);

        private static void showPolarAction(object obj)
        {

        }

        internal List<string> Sectors { get; } = new List<string>() { "Front", "RF", "Right", "RR", "Rear", "LR", "Left", "LF", "Frank", "Gerald", "Alex", "Lacey", "Sean", "Nate", "Jeff", "Nooch" };

        public ObservableCollection<ReportRow> Test { get; set; } = new ObservableCollection<ReportRow>();
        public ObservableCollection<ReportRow> Format1 { get; set; } = new ObservableCollection<ReportRow>();
        public ObservableCollection<ReportRow> Format4 { get; set; } = new ObservableCollection<ReportRow>();

        public bool ShowAsWhite { get => showAsWhite; set { showAsWhite = value; OnPropChanged(); } }
        private bool showAsWhite = false;

        public bool ShowBold { get => showBold; set { showBold = value; OnPropChanged(); } }
        private bool showBold = false;

        public bool ShowBorder { get => showBorder; set { showBorder = value; OnPropChanged();  } }
        private bool showBorder = false;

        public enum ReportTypeEnum
        {
            Actual,
            Delta,
            Color
        }
        public ReportTypeEnum ChangeReportType { get => _reportType; set { _reportType = value; OnPropChanged(); } }
        private ReportTypeEnum _reportType = ReportTypeEnum.Actual;

        public bool IsActual { get => isActual; set { isActual = value; if (value) ChangeReportType = ReportTypeEnum.Actual; OnPropChanged(); } }
        private bool isActual = true;
        public bool IsDelta { get => isDelta; set { isDelta = value; if (value) ChangeReportType = ReportTypeEnum.Delta; OnPropChanged(); } }
        private bool isDelta = false;
        public bool IsColor { get => isColor; set { isColor = value; if (value) ChangeReportType = ReportTypeEnum.Color; OnPropChanged(); } }
        private bool isColor = false;


        internal void SetTestView(DataGrid dg)
        {
            dg.Columns.Clear();

            if (Test.Count < 1)
                return;
            int i = 0;
            foreach (var c in Test.First().Columns)
            {
                DataGridTextColumn textColumn = new DataGridTextColumn();
                //make a new source
                Binding myBinding = new Binding($"Columns[{i}]");
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
                foreach (var sector in Sectors)
                {
                    row.Columns.Add(new ReportColumn(row, sector, r.Next(-3, 15)));
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
        public ReportRow Row { get; }
        public ReportColumn Column { get; }
        public double Value { get; }
        public double Delta { get; }
        public string Color { get; }


        public ReportEntry(ReportRow row, ReportColumn reportColumn, double value)
        {
            Row = row;
            Column = reportColumn;
            Value = value;
            if (value < 3)
            {
                MyColor = Brushes.Green;
                Color = "GRN";
                Delta = 0;
            }
            else if (value < 7)
            {
                MyColor = Brushes.Yellow;
                Color = "YLW";
                Delta = 3;
            }
            else if (value < 10)
            {
                MyColor = Brushes.Pink;
                Color = "PNK";
                Delta = 6;
            }
            else
            {
                MyColor = Brushes.Red;
                Color = "RED";
                Delta = 9;
            }
        }

        public Brush MyColor { get; }
        public override string ToString()
        {
            if (MainViewModel.Instance.Report.ChangeReportType == ReportTypeEnum.Actual)
                return $"{Value:N2}";
            if (MainViewModel.Instance.Report.ChangeReportType == ReportTypeEnum.Delta)
                return $"{Delta:N2}";
            if (MainViewModel.Instance.Report.ChangeReportType == ReportTypeEnum.Color)
                return $"{Color}";

            return "";
        }
    }

    public class ReportMultiConvert : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Count() == 2)// is ReportViewModel vm)
            {
                if (targetType == typeof(Brush))
                {
                    if ((bool)values[1])
                        return Brushes.Black;

                    ReportEntryConverter converter = new ReportEntryConverter();
                    return converter.Convert(values[0], typeof(Brush), null, culture) as Brush;
                }
                else if (targetType == typeof(String))
                {
                    ReportEntryConverter converter = new ReportEntryConverter();
                    return converter.Convert(values[0], targetType, null, culture).ToString();
                }
            }
            return Brushes.White;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            string[] values = null;
            if (value != null)
                return values = value.ToString().Split(' ');
            return values;
        }
    }

    public class BorderConverter : IMultiValueConverter
    {
        private ReportEntry findEntry(object obj)
        {
            if (obj is DataGridCell cell && cell.DataContext is ReportRow dataRow)
                if (cell.Column?.DisplayIndex > -1)
                    return dataRow.Columns[cell.Column.DisplayIndex].Entry as ReportEntry;
            return null;
        }

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Count() == 3)// is ReportViewModel vm)
            {
                if ((bool)values[1])
                {
                    if ((bool)values[2])
                        return Brushes.Black;

                    var entry = findEntry(values[0]);
                    
                    if (entry != null)
                        return entry.MyColor;
                }
            }
            return Brushes.Transparent;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            string[] values = null;
            if (value != null)
                return values = value.ToString().Split(' ');
            return values;
        }
    }

    public class ReportBoldConverter : IValueConverter
    {
        private ReportEntry findEntry(object obj)
        {
            if (obj is DataGridCell cell && cell.DataContext is ReportRow dataRow)
                if (cell.Column?.DisplayIndex > -1)
                    return dataRow.Columns[cell.Column.DisplayIndex].Entry as ReportEntry;
            return null;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType == typeof(System.Windows.FontWeight))
            {
                if ((bool)value)
                    return System.Windows.FontWeights.Bold;
            }

            return System.Windows.FontWeights.Normal;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ReportEntryConverter : IValueConverter
    {
        private ReportEntry findEntry(object obj)
        {
            if (obj is DataGridCell cell && cell.DataContext is ReportRow dataRow)
                if (cell.Column?.DisplayIndex > -1)
                    return dataRow.Columns[cell.Column.DisplayIndex].Entry as ReportEntry;           
            return null;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is DataGridCell cell && cell.DataContext is ReportRow row)
            {
                var idx = cell.Column.DisplayIndex;

                if (targetType == typeof(Brush))
                {
                    return row.Columns[idx].Entry.MyColor;
                }
                else if (targetType == typeof(String))
                {
                    return row.Columns[idx].Entry.ToString();
                }
                else if (targetType == typeof(System.Windows.FontWeights))
                {
                    if ((bool)value)
                        return System.Windows.FontWeights.Bold;
                 
                    return System.Windows.FontWeights.Normal;
                }

            }

            return Brushes.White;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
