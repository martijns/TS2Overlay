using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TS2OverlayCore;
using System.Threading;

namespace TS2Overlay
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppStatus status = AppStatus.Instance;

            // As we're starting up, also startup the app if it's not running yet
            if (!status.IsRunning())
            {
                Logger.Debug("[TS2Overlay] - App not running on startup, starting core...");
                status.StartApplication();
            }
            Thread.Sleep(2000);

            // Keep checking if the core is running
            while (true)
            {
                if (!status.IsRunning())
                {
                    if (status.IsGracefullyShutdown())
                    {
                        // Normal shutdown. Also shutdown this app.
                        Logger.Debug("[TS2Overlay] - Core was closed gracefully, shutting down...");
                        break;
                    }
                    else
                    {
                        // App crashed, restart it.
                        Logger.Debug("[TS2Overlay] - Core has crashed, restarting...");
                        status.StartApplication();
                    }
                }
                else if (status.IsAppHanging())
                {
                    // App is hanging, restart it.
                    Logger.Debug("[TS2Overlay] - Core app is not responding, restarting...");
                    status.StopApplication();
                    Thread.Sleep(500);
                    status.StartApplication();
                    Thread.Sleep(2000);
                }
                Thread.Sleep(1000);
            }

            // Log our demise...
            Logger.Debug("[TS2Overlay] - Closing app...");
        }
    }
}
