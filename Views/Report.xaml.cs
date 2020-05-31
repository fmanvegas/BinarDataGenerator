using BinarDataGenerator.Models;
using BinarDataGenerator.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BinarDataGenerator.Views
{
    /// <summary>
    /// Interaction logic for Report.xaml
    /// </summary>
    public partial class Report : Window
    {
        public Report(Models.BDataObject data)
        {
            InitializeComponent();

            SetGrid(data);
        }

        /// <summary>
        /// Do we rebuild the whole plot?   Change the data only?
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            MainViewModel.Instance.Report.Generate();
        }

        /// <summary>
        /// Note; this only has to be done ONCE ever
        /// </summary>
        /// <param name="data"></param>
        private void SetGrid(BDataObject data)
        {
            dg1.Columns.Clear();

            if (MainViewModel.Instance.Report.Format1.Count < 1)
                return;
            int i = 0;
            foreach (var c in MainViewModel.Instance.Report.Sectors)
            {
                DataGridTextColumn textColumn = new DataGridTextColumn();
                textColumn.Header = c;
                textColumn.Binding = new Binding($"Columns[{i++}].Entry");
                dg1.Columns.Add(textColumn);
            }
        }

        /// <summary>
        /// R. Click on a cell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem m)
                if (m.CommandParameter is ReportRow row)
                {

                }
            MessageBox.Show("HI");
        }

        private void dg1_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGrid g)
            {
                var hit = VisualTreeHelper.HitTest((Visual)sender, e.GetPosition((IInputElement)sender));
                DependencyObject cell = VisualTreeHelper.GetParent(hit.VisualHit);
                while (cell != null && !(cell is System.Windows.Controls.DataGridCell)) cell = VisualTreeHelper.GetParent(cell);
                System.Windows.Controls.DataGridCell targetCell = cell as System.Windows.Controls.DataGridCell;

                if (targetCell is null)
                    return;
                ReportRow row = targetCell.DataContext as ReportRow;
                ReportColumn entry = row.Columns[targetCell.Column.DisplayIndex] as ReportColumn;

                MessageBox.Show($"Creating for: {row.RowHeader} & {entry.ColumnHeader} at {entry.Entry}");
            }

        }

        private void dg1_MouseEnter(object sender, MouseEventArgs e)
        {

        }
    }
}
