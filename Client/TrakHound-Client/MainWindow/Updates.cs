﻿// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

using Microsoft.Win32;
using System.Reflection;
using System.Diagnostics;
using System.Windows;
using System.IO;

using TH_Global;
using TH_WPF;

namespace TrakHound_Client
{
    public partial class MainWindow
    {

        private void LaunchUpdater()
        {
            //string appStartPath = AppDomain.CurrentDomain.BaseDirectory + "\\" + "trakhound-client.exe";

            string appStartPath = @"F:\feenux\TrakHound\TrakHound\Client\TrakHound-Client-Updater\bin\Debug\TrakHound-Client-Updater.exe";

            //string appStartPath = AppDomain.CurrentDomain.BaseDirectory + "\\Updater\\" + "AppStart.exe";

            if (File.Exists(appStartPath))
            {
                var p = new Process();

                p.StartInfo.FileName = appStartPath;
                p.StartInfo.Arguments = Process.GetCurrentProcess().ProcessName;

                p.Start();
            }
            else
            {
                Logger.Log("LaunchUpdater() :: Can't find " + appStartPath);
            }
        }



        

        static void CheckUpdateQueue()
        {
            string[] keyNames = GetRegistryKeyNames();
            if (keyNames != null)
            {
                if (keyNames.Length > 0)
                {
                    int updateBehavior = GetUpdateBehavior();
                    if (updateBehavior == 0)
                    {
                        OpenUpdater();
                    }
                    else if (updateBehavior == 1)
                    {
                        var yesClicked = TH_WPF.MessageBox.Show(
                            "Updates are available for TrakHound" + Environment.NewLine + "Click Yes to Install",
                            "TrakHound Updates Available",
                            TH_WPF.MessageBoxButtons.YesNo
                            );

                        if (yesClicked == MessageBoxDialogResult.Yes)
                        {
                            OpenUpdater();
                        }
                    }
                }
            }
        }

        static int GetUpdateBehavior()
        {
            int Result = -1;

            object updateBehavior = GetRegistryKey("Update_Behavior");

            if (updateBehavior != null)
            {
                int val = -1;
                int.TryParse(updateBehavior.ToString(), out val);
                if (val >= 0)
                {
                    Result = val;
                }
            }

            return Result;
        }

        static void OpenClient()
        {
            try
            {
                string clientPath = AppDomain.CurrentDomain.BaseDirectory + "\\" + "trakhound-client.exe";

                //string clientPath = @"F:\feenux\TrakHound\TrakHound\Client\TrakHound-Client\bin\Debug\trakhound-client.exe";

                Process p = new Process();

                p.StartInfo.FileName = clientPath;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.UseShellExecute = false;

                p.Start();

                string stdout = p.StandardOutput.ReadToEnd();
                string stderr = p.StandardError.ReadToEnd();

                p.WaitForExit();

                Console.WriteLine("Client Exited with Code : " + p.ExitCode.ToString());
                Console.WriteLine(stdout);
                Console.WriteLine(stderr);
            }
            catch (Exception ex) { Console.WriteLine("TrakHound-Client-Updater.OpenClient() :: " + ex.Message); }
        }

        static void OpenApp(string processPath)
        {
            if (processPath != null)
            {
                if (File.Exists(processPath))
                {
                    try
                    {
                        Process p = new Process();
                        p.StartInfo.FileName = processPath;
                        p.Start();
                    }
                    catch (Exception ex) { System.Windows.MessageBox.Show("TrakHound-Client-Updater.OpenApp() :: Exception :: " + processPath + " :: " + ex.Message); }
                }
            }
        }

        static void OpenUpdater()
        {
            try
            {
                string clientPath = AppDomain.CurrentDomain.BaseDirectory + "\\Updater\\" + "trakhound-client-updater.exe";

                //string clientPath = @"F:\feenux\TrakHound\TrakHound\Client\TrakHound-Client-Updater\bin\Debug\trakhound-client-updater.exe";

                Process p = new Process();
                p.StartInfo.FileName = clientPath;
                p.Start();
                p.WaitForExit();
            }
            catch (Exception ex) { Console.WriteLine("TrakHound-Client-Updater.OpenUpdater() :: " + ex.Message); }
        }


        public static string GetRegistryKey(string keyName)
        {
            string Result = null;

            try
            {
                // Open CURRENT_USER/Software Key
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);

                // Open CURRENT_USER/Software/TrakHound Key
                RegistryKey rootKey = key.OpenSubKey("TrakHound");

                // Open CURRENT_USER/Software/TrakHound/Updates/[keyName] Key
                RegistryKey updateKey = rootKey.OpenSubKey(keyName);

                // Read value for [keyName] to [keyValue]
                Result = updateKey.GetValue(keyName).ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("AutoUpdater_GetRegistryKey() : " + ex.Message);
            }

            return Result;
        }

        public static string[] GetRegistryKeyNames()
        {
            string[] Result = null;

            try
            {
                // Open CURRENT_USER/Software Key
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);

                // Open CURRENT_USER/Software/TrakHound Key
                RegistryKey rootKey = key.OpenSubKey("TrakHound");

                // Open CURRENT_USER/Software/TrakHound/Updates Key
                RegistryKey updatesKey = rootKey.OpenSubKey("Updates");

                Result = updatesKey.GetSubKeyNames();
            }
            catch (Exception ex)
            {
                Console.WriteLine("AutoUpdater_GetRegistryKeys() : " + ex.Message);
            }

            return Result;
        }

        public static void DeleteRegistryKey(string keyName)
        {
            try
            {
                // Open CURRENT_USER/Software Key
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);

                // Open CURRENT_USER/Software/TrakHound Key
                RegistryKey rootKey = key.OpenSubKey("TrakHound", true);

                // Open CURRENT_USER/Software/TrakHound/Updates Key
                RegistryKey updatesKey = rootKey.OpenSubKey("Update_Behavior", true);

                // Delete CURRENT_USER/Software/TrakHound/Updates/[keyName] Key
                updatesKey.DeleteSubKey(keyName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("AutoUpdater_DeleteRegistryKey() : " + ex.Message);
            }
        }

    }
}