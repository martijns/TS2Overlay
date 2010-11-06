using System;
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
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using TS2OverlayCore.View;
using TS2OverlayCore.Model;
using TS2OverlayCore.Properties;

namespace TS2OverlayCore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _lastSpeakerCount = -1;

        [Import(typeof(ITeamspeak))]
        public ITeamspeak Teamspeak { get; set; }

        public MainWindow()
        {
            Logger.Debug("[TS2OverlayCore] - MainWindow");

            InitializeComponent();
            DataContext = this;
            //new CompositionContainer().ComposeParts(this);

            Teamspeak = new Teamspeak();

            Loaded += MainWindowLoaded;
            Closing += MainWindowClosing;
            LocationChanged += MainWindowLocationChanged;

            AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;
        }

        void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                Logger.Debug("[TS2OverlayCore] - Unhandled crash during execution\r\n" + ex.GetType().Name + ": " + ex.Message + "\r\n" + ex.StackTrace);
            }
            AppStatus.Instance.Kill();
        }

        void MainWindowLocationChanged(object sender, EventArgs e)
        {
            Settings.Default.Save();
        }

        void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            Logger.Debug("[TS2OverlayCore] - MainWindowLoaded");
            Teamspeak.StartCollectingData();
            AppStatus.Instance.Startup();
        }

        void MainWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Logger.Debug("[TS2OverlayCore] - MainWindowClosing");
            AppStatus.Instance.Shutdown();
            Settings.Default.Save();
            Teamspeak.StopCollectingData();
        }

        private void HandleOptionsClicked(object sender, RoutedEventArgs e)
        {
            Logger.Debug("[TS2OverlayCore] - HandleOptionsClicked");
            MessageBox.Show("In the future, this button will bring you to an options screen :)\r\n\r\nTS2Overlay v0.3 by NetRipper / Veshai (c) 2010");
        }

        private void HandleExitClicked(object sender, RoutedEventArgs e)
        {
            Logger.Debug("[TS2OverlayCore] - HandleExitClicked");
            Close();
        }

        private void HandleLeftMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            Logger.Debug("[TS2OverlayCore] - HandleLeftMouseButtonDown");
            DragMove();
        }

        private void HandleListLayoutUpdated(object sender, EventArgs e)
        {
            if (_lastSpeakerCount != Teamspeak.Speakers.Count)
            {
                _lastSpeakerCount = Teamspeak.Speakers.Count;
                _listbox.Height = 16 * _lastSpeakerCount + 2;
            }
        }
    }
}
