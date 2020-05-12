using BinarDataGenerator.Models;
using BinarDataGenerator.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BinarDataGenerator.Views
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        MainViewModel ViewModel;

        public UserControl1()
        {
            InitializeComponent();
            //Since we didn't use a singleton... make sure we're looking at the right thing
            ViewModel = DataContext as MainViewModel;
        }

        private void lstFrq2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        private void List_SelectionChanged(object sender, SelectionChangedEventArgs ea)
        {
            //Sanity check
            if (lstBinData.SelectedItems.Count < 1)
                return;

            BDataObject data = lstBinData.SelectedItems[0] as BDataObject;

            var frq = lstFrq2.SelectedItem;
            var dep = lstDep2.SelectedItem;
            var pol = lstPol2.SelectedItem;

            MainViewModel.Instance.Records = data.GetData(frq, dep, pol);

        //    MainViewModel.Instance.ViewableRecords.Clear();


           // foreach (var r in records)
          //      MainViewModel.Instance.ViewableRecords.Add(r.Value);
        }
    }
}
