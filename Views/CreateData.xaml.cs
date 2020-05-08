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
                ViewModel = vm;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ViewModel?.CreateData(lstFrq.SelectedItems, lstDep.SelectedItems, lstPol.SelectedItems, lstStat.SelectedItems);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int vectors = lstFrq.SelectedItems.Count * lstDep.SelectedItems.Count * lstPol.SelectedItems.Count;
            int objects = vectors * 360;
            int values = objects * lstStat.SelectedItems.Count;

            ViewModel.VectorMax = vectors;
            ViewModel.ObjectMax = objects;
            ViewModel.ValueMax = values;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SearchVector search = new SearchVector(ViewModel);
            search.Show();
        }

       
    }
}
