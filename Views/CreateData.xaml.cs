using BinarDataGenerator.ViewModels;
using BinarDataGenerator.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BinarDataGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel ViewModel;

        public MainWindow()
        {
            InitializeComponent();
            if (DataContext is MainViewModel vm)
            {
                ViewModel = vm;
                ViewModel.Report.SetTestView(dg);
            }

        }

        /// <summary>
        /// 
        ///     Create that data, Bro
        ///     
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Create_Data_Click(object sender, RoutedEventArgs e)
        {
            ViewModel?.CreateData(lstFrq.SelectedItems, lstDep.SelectedItems, lstPol.SelectedItems, lstStat.SelectedItems);
        }



        /// <summary>
        /// 
        ///     Upon clicking any options, show the user what they will get
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
                return;

            int vectors = lstFrq.SelectedItems.Count * lstDep.SelectedItems.Count * lstPol.SelectedItems.Count;
            int objects = vectors * 360;
            int values = objects * lstStat.SelectedItems.Count;

            ViewModel.VectorMax = vectors;
            ViewModel.ObjectMax = objects;
            ViewModel.ValueMax = values;
        }




    }
}
