using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TS2OverlayCore.View;
using System.ComponentModel.Composition;
using System.Timers;
using System.Diagnostics;

namespace TS2OverlayCore.Model
{
    [Export(typeof(ITeamspeak))]
    public class Teamspeak : ViewModelBase, ITeamspeak
    {
        internal static DateTime SpeakersLastSet = DateTime.Now;

        #region Private fields

        private Timer _collectorTimer;
        private IList<ISpeaker> _speakers;
        private string _currentChannelName;
        private int _speakerClearedAfter;

        #endregion

        #region Constructor

        public Teamspeak()
        {
            Speakers = new List<ISpeaker>();
            SpeakerClearedAfter = 5;
        }

        #endregion

        #region ITeamspeak Members

        public void StartCollectingData()
        {
            Logger.Debug("[TS2OverlayCore] - StartCollectingData");
            lock (this)
            {
                if (_collectorTimer == null)
                {
                    _collectorTimer = new Timer();
                    _collectorTimer.Interval = 250;
                    _collectorTimer.Elapsed += DataCollector;
                    _collectorTimer.Enabled = true;
                }
            }
        }

        public void StopCollectingData()
        {
            Logger.Debug("[TS2OverlayCore] - StopCollectingData");
            lock (this)
            {
                if (_collectorTimer != null)
                {
                    _collectorTimer.Enabled = false;
                    _collectorTimer.Elapsed -= DataCollector;
                    _collectorTimer.Dispose();
                    _collectorTimer = null;
                }
            }
        }

        public IList<ISpeaker> Speakers
        {
            get
            {
                return _speakers;
            }
            set
            {
                _speakers = value;
                RaisePropertyChanged("Speakers");
            }
        }

        public int SpeakerClearedAfter
        {
            get
            {
                return _speakerClearedAfter;
            }
            set
            {
                if (_speakerClearedAfter != value)
                {
                    _speakerClearedAfter = value;
                    RaisePropertyChanged("SpeakerClearedAfter");
                }
            }
        }

        public string CurrentChannelName
        {
            get
            {
                return _currentChannelName;
            }
            set
            {
                if (_currentChannelName != value)
                {
                    _currentChannelName = value;
                    RaisePropertyChanged("CurrentChannelName");
                }
            }
        }

        #endregion

        #region DataCollector

        void DataCollector(object sender, ElapsedEventArgs e)
        {
            try
            {
                // Demo
                //{
                //    List<ISpeaker> demo = new List<ISpeaker>();
                //    demo.Add(new Speaker { Name = "NED - Veshai", Speaking = true, LastSpoke = DateTime.Now });
                //    demo.Add(new Speaker { Name = "NED - groenegod", Speaking = true, LastSpoke = DateTime.Now });
                //    demo.Add(new Speaker { Name = "NED - Sjinta", Speaking = false, LastSpoke = DateTime.Now });
                //    demo.Add(new Speaker { Name = "NED - Mr Fanatic", Speaking = false, LastSpoke = DateTime.Now });
                //    CurrentChannelName = "NED Clan";
                //    Speakers = demo;
                //    return;
                //}

                // Update current channel name
                string channelName = TSRemote.UserInfo.Channel.Name;
                if (channelName.StartsWith("\0\0\0\0\0"))
                {
                    CurrentChannelName = "TS2Overlay - Not Connected";
                    Speakers = new List<ISpeaker>();
                    AppStatus.Instance.KeepAlive();
                    return;
                }
                else
                {
                    CurrentChannelName = channelName;
                }

                // Work in a copy of the list
                List<ISpeaker> speakersList = new List<ISpeaker>(Speakers);

                // Remove old speakers from the list
                List<Speaker> speakersToRemove = new List<Speaker>();
                foreach (Speaker s in speakersList)
                {
                    if ((DateTime.Now - s.LastSpoke).TotalSeconds > 5)
                        speakersToRemove.Add(s);
                }
                foreach (Speaker s in speakersToRemove)
                {
                    speakersList.Remove(s);
                }

                // Get the ids of the current talkers
                List<string> talkers = new List<string>();
                foreach (int speakerId in TSRemote.SpeakerIds)
                {
                    TSRemote.TtsrPlayerInfo player = TSRemote.GetPlayerInfo(speakerId);
                    talkers.Add(player.NickName);
                }

                // See if we have new speakers, and update the existing
                List<Speaker> speakersToMoveToTop = new List<Speaker>();
                foreach (string talker in talkers)
                {
                    Speaker s = (from Speaker speaker in speakersList where speaker.Name.Equals(talker) select speaker).FirstOrDefault();
                    if (s == null)
                    {
                        s = new Speaker { Name = talker, Speaking = true, LastSpoke = DateTime.Now };
                        speakersList.Add(s);
                    }
                    else
                    {
                        s.LastSpoke = DateTime.Now;
                        s.Speaking = true;
                    }
                    speakersToMoveToTop.Add(s);
                }

                // Move the speakers to the top
                foreach (Speaker s in speakersToMoveToTop)
                {
                    speakersList.Remove(s);
                    speakersList.Insert(0, s);
                }

                // Update the non-speakers
                foreach (Speaker s in speakersList)
                {
                    if (!talkers.Contains(s.Name))
                    {
                        s.Speaking = false;
                    }
                }

                // Update the Speakers property, so that an event is fired.
                Speakers = speakersList;

                // Update timestamp
                AppStatus.Instance.KeepAlive();
            }
            catch (Exception ex)
            {
                Logger.Debug("[TS2OverlayCore] - DataCollector crashed during execution\r\n" + ex.GetType().Name + ": " + ex.Message + "\r\n" + ex.StackTrace);
                AppStatus.Instance.Kill();
            }
        }

        #endregion
    }
}
