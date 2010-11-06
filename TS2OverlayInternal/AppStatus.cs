using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Timers;
using System.Threading;
using TS2OverlayCore.Model;

namespace TS2OverlayCore
{
    public class AppStatus
    {
        private static string LockFile = "TS2OverlayCore_is_running.lock";

        //private System.Timers.Timer _watchdogTimer;

        #region Singleton

        private static AppStatus _instance;
        public static AppStatus Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new AppStatus();
                return _instance;
            }
        }

        private AppStatus()
        {
            //_watchdogTimer = new System.Timers.Timer();
            //_watchdogTimer.Interval = 1000;
            //_watchdogTimer.Elapsed += HandleKeepAliveTimerElapsed;
            //_watchdogTimer.Enabled = false;
        }

        #endregion

        #region Internal methods

        internal void Startup()
        {
            KeepAlive();
            //_watchdogTimer.Enabled = true;
        }

        internal void Shutdown()
        {
            //_watchdogTimer.Enabled = false;
            if (File.Exists(LockFile))
            {
                File.Delete(LockFile);
            }
        }

        internal void KeepAlive()
        {
            try
            {
                using (StreamWriter sw = File.CreateText(LockFile))
                {
                    sw.WriteLine("This file indicates the application is running. When it crashes, and this file exists, the app is restarted.");
                    sw.WriteLine(DateTime.Now.ToBinary());
                    sw.Close();
                }
            }
            catch (IOException)
            {
                Thread.Sleep(250);
                KeepAlive();
            }
        }

        #endregion

        #region Public methods

        public bool IsRunning()
        {
            Process[] processes = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(GetExecutable()));
            return processes != null && processes.Count() > 0;
        }

        public void StartApplication()
        {
            string exe = GetExecutable();
            ProcessStartInfo psi = new ProcessStartInfo
            {
                WorkingDirectory = Path.GetDirectoryName(exe),
                FileName = exe
            };
            Process.Start(psi);
        }

        public void StopApplication()
        {
            Process[] processes = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(GetExecutable()));
            foreach (Process p in processes)
            {
                p.Kill();
            }
        }

        public bool IsGracefullyShutdown()
        {
            return !File.Exists(LockFile);
        }

        #endregion

        #region Private helpers

        void HandleKeepAliveTimerElapsed(object sender, ElapsedEventArgs e)
        {
            KeepAlive();
        }

        private string GetExecutable()
        {
            return Assembly.GetExecutingAssembly().Location;
        }

        internal void Kill()
        {
            Process.GetCurrentProcess().Kill();
        }

        #endregion


        #region IsAppHung

        //public bool IsAppHanging()
        //{
        //    TimeSpan diff = DateTime.Now - Teamspeak.SpeakersLastSet;
        //    if (diff.TotalSeconds > 5)
        //        return true;
        //    return false;
        //}

        public bool IsAppHanging()
        {
            try
            {
                if (File.Exists(LockFile))
                {
                    string[] contents = File.ReadAllLines(LockFile);
                    if (contents != null && contents.Length == 2)
                    {
                        DateTime lastWrite = DateTime.FromBinary(long.Parse(contents[1]));
                        TimeSpan diff = DateTime.Now - lastWrite;
                        if (diff.TotalSeconds > 5)
                        {
                            return true;
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                Thread.Sleep(250);
                return IsAppHanging();
            }
            catch
            {
            }
            return false;
        }

        //[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        //private static extern int
        //    SendMessageTimeout(
        //    IntPtr hWnd,
        //    int Msg,
        //    int wParam,
        //    string lParam,
        //    int fuFlags,
        //    int uTimeout,
        //    out int lpdwResult
        //    );

        //private const int HWND_BROADCAST = 0xffff;
        //private const int WM_SETTINGCHANGE = 0x001A;
        //private const int SMTO_NORMAL = 0x0000;
        //private const int SMTO_BLOCK = 0x0001;
        //private const int SMTO_ABORTIFHUNG = 0x0002;
        //private const int SMTO_NOTIMEOUTIFNOTHUNG = 0x0008;

        //public bool IsAppHung()
        //{
        //    Process[] processes = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(GetExecutable()));
        //    foreach (Process p in processes)
        //    {
        //        // METHOD 1
        //        if (!p.Responding)
        //            return true;

        //        // METHOD 2
        //        if (!p.WaitForInputIdle(1000))
        //        {
        //            return true;
        //        }

        //        /*int lpdwResult;
                
        //        p.WaitForInputIdle(
        //        int res = SendMessageTimeout(p.MainWindowHandle, WM_SETTINGCHANGE, 0, "Environment", SMTO_BLOCK | SMTO_ABORTIFHUNG | SMTO_NOTIMEOUTIFNOTHUNG, 5000, out lpdwResult);
        //        if (res == 0)
        //        {
        //            int error = Marshal.GetLastWin32Error();
        //            return true;
        //        }*/
        //    }
        //    return false;
        //}

        #endregion
    }
}
