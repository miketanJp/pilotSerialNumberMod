using System.Collections.Generic;
using it.miketan.PilotSerial.Models;

namespace it.miketan.PilotSerial.Models
{
    internal class CacheRoot
    {
        public Dictionary<string, PilotEntry> serialCache { get; set; }
    }
}