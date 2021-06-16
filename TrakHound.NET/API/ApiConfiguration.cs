﻿// Copyright (c) 2017 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using Newtonsoft.Json;
using NLog;
using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace TrakHound.API
{
    public class ApiConfiguration
    {       
        public const string LOCAL_API_HOST = "http://localhost:8472/api/";
        public const string CLOUD_API_HOST = "https://api.jbook.in/";
        public const long DEFAULT_BUFFER_SIZE = 500000; // 5 kB
        public const int DEFAULT_UPDATE_INTERVAL = 5000; // 5 Seconds
        public const int DEFAULT_TIMEOUT = 2000; // 2 Seconds

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static Uri DataApiHost = new Uri(LOCAL_API_HOST);
        public static Uri AuthenticationApiHost = new Uri(CLOUD_API_HOST);

        public static long BufferSize = DEFAULT_BUFFER_SIZE;
        public static int UpdateInterval = DEFAULT_UPDATE_INTERVAL;
        public static int Timeout = DEFAULT_TIMEOUT;

        [JsonProperty("data_host")]
        public string DataHost { get; set; }

        [JsonProperty("authentication_host")]
        public string AuthenticationHost { get; set; }


        private const string CONFIG_FILENAME = "api_config.xml";
        private static string CONFIG_FILEPATH = Path.Combine(FileLocations.TrakHound, CONFIG_FILENAME);

        public static void SetTrakHoundCloud()
        {
            var apiConfig = new ApiConfiguration();
            apiConfig.DataHost = CLOUD_API_HOST;
            apiConfig.AuthenticationHost = CLOUD_API_HOST;
            Set(apiConfig);
            Create(apiConfig);
        }

        public static void SetLocal()
        {
            var apiConfig = new ApiConfiguration();
            apiConfig.DataHost = LOCAL_API_HOST;
            apiConfig.AuthenticationHost = CLOUD_API_HOST;
            Set(apiConfig);
            Create(apiConfig);
        }

        public static void Set(ApiConfiguration apiConfig)
        {
            SetDataHost(apiConfig);
            SetAuthenticationHost(apiConfig);
        }

        public static void SetDataHost(ApiConfiguration apiConfig)
        {
            if (apiConfig != null && !string.IsNullOrEmpty(apiConfig.DataHost))
            {
                try
                {
                    DataApiHost = new Uri(apiConfig.DataHost);
                }
                catch (Exception ex)
                {
                    logger.Error("API Data Host Configuration Error");
                    logger.Error(ex);
                }
            }
            else
            {
                DataApiHost = new Uri(LOCAL_API_HOST);
            }

            logger.Info("TrakHound Data API Host set to " + DataApiHost);
        }

        public static void SetAuthenticationHost(ApiConfiguration apiConfig)
        {
            if (apiConfig != null && !string.IsNullOrEmpty(apiConfig.AuthenticationHost))
            {
                try
                {
                    AuthenticationApiHost = new Uri(apiConfig.AuthenticationHost);
                }
                catch (Exception ex)
                {
                    logger.Error("API Authentication Host Configuration Error");
                    logger.Error(ex);
                }
            }
            else
            {
                AuthenticationApiHost = new Uri(CLOUD_API_HOST);
            }

            logger.Error("TrakHound Authentication API Host set to " + AuthenticationApiHost);
        }

        public static bool Create(ApiConfiguration config)
        {
            bool result = false;

            Remove();

            if (config != null)
            {
                var xml = CreateDocument(config);
                Tools.XML.Files.WriteDocument(xml, CONFIG_FILEPATH);
            }

            return result;
        }

        public static ApiConfiguration Read()
        {
            var result = new ApiConfiguration();

            if (File.Exists(CONFIG_FILEPATH))
            {
                try
                {
                    var xml = new XmlDocument();
                    xml.Load(CONFIG_FILEPATH);

                    foreach (XmlNode node in xml.DocumentElement.ChildNodes)
                    {
                        if (node.NodeType == XmlNodeType.Element)
                        {
                            if (node.InnerText != "")
                            {
                                Type c = typeof(ApiConfiguration);
                                PropertyInfo info = c.GetProperty(node.Name);

                                if (info != null)
                                {
                                    Type t = info.PropertyType;
                                    info.SetValue(result, Convert.ChangeType(node.InnerText, t), null);
                                }
                            }
                        }
                    }

                    if (result != null)
                    {
                        // Update to new Url
                        result.AuthenticationHost = updateUrl(result.AuthenticationHost);
                        result.DataHost = updateUrl(result.DataHost);
                    }
                }
                catch (Exception ex) { logger.Error(ex); }
            }

            return result;
        }

        private static string updateUrl(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                var x = s.ToLower();
                if (x == "http://www.jbook.in/api/" || x == "http://api.jbook.in/" || x == "http://jbook.in/api/" ||
                    x == "https://www.jbook.in/api/" || x == "https://api.jbook.in/" || x == "https://jbook.in/api/")
                {
                    return CLOUD_API_HOST;
                }
            }

            return s;
        }

        public static void Remove()
        {
            if (File.Exists(CONFIG_FILEPATH)) File.Delete(CONFIG_FILEPATH);
        }

        private static XmlDocument CreateDocument(ApiConfiguration config)
        {
            var result = new XmlDocument();

            XmlNode docNode = result.CreateXmlDeclaration("1.0", "UTF-8", null);
            result.AppendChild(docNode);

            XmlNode root = result.CreateElement("ApiConfiguration");
            result.AppendChild(root);

            foreach (var info in typeof(ApiConfiguration).GetProperties())
            {
                XmlNode node = result.CreateElement(info.Name);
                var val = info.GetValue(config, new object[] { });
                if (val != null) node.InnerText = val.ToString();
                root.AppendChild(node);
            }

            return result;
        }


        public class Monitor
        {
            public delegate void ApiConfigurationChanged_Handler(ApiConfiguration config);
            public event ApiConfigurationChanged_Handler ApiConfigurationChanged;

            public Monitor()
            {
                logger.Info("API Configuration File Monitor Started");

                string dir = FileLocations.TrakHound;

                var watcher = new FileSystemWatcher(dir, CONFIG_FILENAME);
                watcher.Changed += File_Changed;
                watcher.Created += File_Changed;
                watcher.Deleted += File_Changed;
                watcher.EnableRaisingEvents = true;
            }

            private System.Timers.Timer delayTimer;

            private void File_Changed(object sender, FileSystemEventArgs e)
            {
                StartDelayTimer();
            }

            private void StartDelayTimer()
            {
                if (delayTimer != null) delayTimer.Stop();
                else
                {
                    delayTimer = new System.Timers.Timer();
                    delayTimer.Interval = 2000;
                    delayTimer.Elapsed += DelayTimer_Elapsed;
                }

                delayTimer.Start();
            }

            private void DelayTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
            {
                logger.Debug("API Configuration Changed");

                var timer = (System.Timers.Timer)sender;
                timer.Stop();

                var apiConfig = Read();
                ApiConfigurationChanged?.Invoke(apiConfig);
            }
        }
    }
}
