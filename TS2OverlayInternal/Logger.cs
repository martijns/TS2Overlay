using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TS2OverlayCore
{
    public class Logger
    {
        private static string LogFile = "Log.txt";

        public static void Debug(string msg, params object[] parameters)
        {
            lock (LogFile)
            {
                using (StreamWriter sw = GetWriter())
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " - " + msg, parameters);
                    sw.Close();
                }
            }
        }

        private static StreamWriter GetWriter()
        {
            if (File.Exists(LogFile))
                return File.AppendText(LogFile);
            else
                return File.CreateText(LogFile);
        }
    }
}
