using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace TS2OverlayCore.View
{
    public interface ITeamspeak : INotifyPropertyChanged
    {
        /// <summary>
        /// Starts the data collector
        /// </summary>
        void StartCollectingData();

        /// <summary>
        /// Stops the data collector
        /// </summary>
        void StopCollectingData();

        /// <summary>
        /// A list of currently (or recently) active speakers
        /// </summary>
        IList<ISpeaker> Speakers { get; }

        /// <summary>
        /// The time in seconds after which speakers are removed from the <see cref="Speakers"/> list
        /// </summary>
        int SpeakerClearedAfter { get; set; }

        /// <summary>
        /// Returns the name of the current channel the user is in
        /// </summary>
        string CurrentChannelName { get; }
    }
}
