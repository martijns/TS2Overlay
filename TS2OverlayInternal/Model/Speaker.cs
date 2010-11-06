using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TS2OverlayCore.View;

namespace TS2OverlayCore.Model
{
    public class Speaker : ISpeaker
    {
        public string Name { get; set; }

        public bool Speaking { get; set; }

        public DateTime LastSpoke { get; set; }
    }
}
