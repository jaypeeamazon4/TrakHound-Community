﻿using System;
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

namespace Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //levels_LI.TotalLevelCount = 4;
            //levels_LI.ActiveLevelCount = 1;
        }

        private void ListButton_Selected(TH_WPF.ListButton LB)
        {
            TH_WPF.MessageBox.Show("Test Box", "TEST TITLE", TH_WPF.MessageBoxButtons.YesNo);
        }
    }
}
