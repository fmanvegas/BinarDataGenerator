using BinarDataGenerator.Models;
using BinarDataGenerator.ViewModels;
using System;
using System.Collections;
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
    /// Interaction logic for SearchVector.xaml
    /// </summary>
    public partial class SearchVector : Window
    {
        readonly MainViewModel ViewModel;


        public SearchVector(MainViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            ViewModel = vm;
        }

        BDataVector vector;


        private void POL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstPol.SelectedIndex < 0 || ViewModel.BinnedData.Count == 0)
                return;
            ListBox_SelectionChanged();

            int pol = lstPol.SelectedItem.ToString().GetHashCode();

            ViewModel.HashTableSet = ViewModel.BinnedData[0].VectByPOL[pol] as Hashtable;

        }

        private void DEP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstDep.SelectedIndex < 0 || ViewModel.BinnedData.Count == 0)
                return;
            ListBox_SelectionChanged();

            double pol = double.Parse(lstDep.SelectedItem.ToString());

            ViewModel.HashTableSet = ViewModel.BinnedData[0].VectByDep[pol] as Hashtable;

        }
        private void FRQ_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstFrq.SelectedIndex < 0 || ViewModel.BinnedData.Count == 0)
                return;
            ListBox_SelectionChanged();

            double pol = double.Parse(lstFrq.SelectedItem.ToString());

            ViewModel.HashTableSet = ViewModel.BinnedData[0].VectByFreq[pol] as Hashtable;

        }


        private void ListBox_SelectionChanged()
        {
            if (lstPol.SelectedIndex < 0 || lstDep.SelectedIndex < 0 || lstFrq.SelectedIndex < 0)
                return;

            string pol = lstPol.SelectedItem.ToString();
            string dep = lstDep.SelectedItem.ToString();
            string fre = lstFrq.SelectedItem.ToString();

            int polValue = pol.GetHashCode();

            vector = new BDataVector(double.Parse(dep), double.Parse(fre), pol, polValue);
        }

        private void Search_List(object sender, RoutedEventArgs e)
        {
            startTime();
            BDataVector v = null;

            foreach (var c in ViewModel.BinnedData)
            {

                foreach (var vec in c.Vectors)
                {
                    if (vec.Frequency == vector.Frequency &&
                        vec.Depression == vector.Depression &&
                        vec.POLValue == vector.POLValue)
                    {
                        v = vec;
                        break;
                    }
                }


            }

            ViewModel.SelectedVector = v;
            stopTime();
        }

        private void Search_Hash(object sender, RoutedEventArgs e)
        {
            startTime();
            BDataVector v = null;

            foreach (var c in ViewModel.BinnedData)
            {

                if (c.VectorHash.ContainsKey(vector.Hash))
                {
                    v = c.VectorHash[vector.Hash] as BDataVector;
                    break;
                }



            }

            ViewModel.SelectedVector = v;
            stopTime();
        }
        DateTime start;
        private void startTime()
        {
            start = DateTime.Now;
        }
        private void stopTime()
        {
            var stop = DateTime.Now;

            var diff = start - stop;

            txtTime.Text = diff.TotalSeconds.ToString();
        }
    }
}
