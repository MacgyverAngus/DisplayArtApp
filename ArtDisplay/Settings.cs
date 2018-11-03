using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtDisplay
{
    public class Settings
    {
        public string DisplayFolder { get; set; }
        public string FavoritesFolder { get; set; }
        public bool AutoSleep { get; set; }
        public string TurnOnMonitor { get; set; }
        public string TurnOffMonitor { get; set; }
        public bool SlideShow { get; set; }
        public int SlideShowMinutes { get; set; }


        public int GetHour(string time)
        {
            var result = 0;
            if (string.IsNullOrWhiteSpace(time)) return result;

            int.TryParse(time.Split(':')[0], out result);

            return result;
        }

        public int GetMinutes(string time)
        {
            var result = 0;
            if (string.IsNullOrWhiteSpace(time)) return result;

            int.TryParse(time.Split(':')[1], out result);

            return result;
        }
    }
}
