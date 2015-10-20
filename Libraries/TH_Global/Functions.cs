﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TH_Global
{
    public static class Functions
    {

        /// <summary>
        /// Convert string to UTC DateTime (DateTime.TryParse seems to always convert to local time even with DateTimeStyle.AssumeUniveral)
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static DateTime ConvertStringToUTC(string s)
        {
            DateTime result = DateTime.MinValue;

            try
            {
                string sYear = s.Substring(0, 4);
                string sMonth = s.Substring(5, 2);
                string sDay = s.Substring(8, 2);

                string sHour = s.Substring(11, 2);
                string sMinute = s.Substring(14, 2);
                string sSecond = s.Substring(17, 2);

                int year = Convert.ToInt16(sYear);
                int month = Convert.ToInt16(sMonth);
                int day = Convert.ToInt16(sDay);

                int hour = Convert.ToInt16(sHour);
                int minute = Convert.ToInt16(sMinute);
                int second = Convert.ToInt16(sSecond);

                result = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);

                // Get number of fraction characters
                int start = 20;
                int end = 20;
                int n = 20;

                if (s.Length > 20)
                {
                    char c = s[n];

                    while (Char.IsNumber(c))
                    {
                        n += 1;
                        if (n > s.Length - 1) break;
                        else c = s[n];
                    }

                    end = n;

                    string sFraction = s.Substring(start, end - start);
                    double fraction = Convert.ToDouble("." + sFraction);
                    int millisecond = System.Math.Min(999, Convert.ToInt32(System.Math.Round(fraction, 3) * 1000));
                    result = new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Utc);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("ConvertStringToUTC() : Input = " + s + " : Exception : " + ex.Message);
            }

            return result;
        }

        public static DateTime ConvertDateTimeToUTC(DateTime dt)
        {         
            int year = dt.Year;
            int month = dt.Month;
            int day = dt.Day;

            int hour = dt.Hour;
            int minute = dt.Minute;
            int second = dt.Second;
            int millisecond = dt.Millisecond;

            return new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Utc);
        }

        static Random random = new Random();
        public static string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(System.Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        public static class Math
        {
            #region "GetMedian"

            public static double GetMedian(double[] vals)
            {
                int size = vals.Length;
                int index = size / 2;
                double median = -1;

                if (vals.Length > 1)
                {
                    if (IsOdd(index)) median = vals[index];
                    else median = (double)(vals[index] + vals[index - 1]) / 2;
                }
                else median = vals[0];

                return median;
            }

            #endregion



            public static bool IsOdd(int val)
            {
                return val % 2 != 0;
            }

        }

    }

}
