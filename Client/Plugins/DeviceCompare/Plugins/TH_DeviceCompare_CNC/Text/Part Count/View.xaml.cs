﻿// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

using TH_Configuration;
using TH_Global.Functions;
using TH_Plugins;

namespace TH_DeviceCompare_CNC.Text.Part_Count
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class Plugin : UserControl
    {
        public Plugin()
        {
            InitializeComponent();
            root.DataContext = this;
        }

        const string link = "Part Count";


        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(Plugin), new PropertyMetadata(null));


        const System.Windows.Threading.DispatcherPriority Priority_Background = System.Windows.Threading.DispatcherPriority.Background;

        const System.Windows.Threading.DispatcherPriority Priority_Context = System.Windows.Threading.DispatcherPriority.ContextIdle;


        void Update(EventData data)
        {
            if (data != null && data.data01 != null && data.data01.GetType() == typeof(Configuration))
            {
                // Snapshot Table Data
                if (data.id.ToLower() == "statusdata_snapshots")
                {
                    this.Dispatcher.BeginInvoke(new Action<object>(UpdateText), Priority_Context, new object[] { data.data02 });
                }
            }
        }


        void UpdateText(object snapshotData)
        {
            DataTable dt = snapshotData as DataTable;
            if (dt != null)
            {
                string value = DataTable_Functions.GetTableValue(dt, "name", link, "value");

                Value = value;
            }
        }

    }
}