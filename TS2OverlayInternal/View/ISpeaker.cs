using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TS2OverlayCore.View
{
    public interface ISpeaker
    {
        /// <summary>
        /// Name of the speaker
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Indicates the currently speaking status
        /// </summary>
        bool Speaking { get; set; }

        /// <summary>
        /// Indicates when the speaker spoke last
        /// </summary>
        DateTime LastSpoke { get; set; }
    }
}
