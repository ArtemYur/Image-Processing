using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Processing
{
    class DiscoverFeatures
    {
        public IList<DisplayDevice> DiscoverAvailableDevices()
        {
            var availableDisplayDevices = new List<DisplayDevice>();
            foreach (DisplayIndex index in Enum.GetValues(typeof(DisplayIndex)))
            {
                DisplayDevice device = DisplayDevice.GetDisplay(index);
                if (device == null)
                {
                    continue;
                }

                availableDisplayDevices.Add(device);
            }
            return availableDisplayDevices;
        }
    }
}
