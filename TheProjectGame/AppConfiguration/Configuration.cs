using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea.AppConfiguration
{
    public abstract class ConfigurationGA
    {
        public int KeepAliveInterval { get; set; }


        public ConfigurationGA(Configuration.Configuration conf)
        {
            KeepAliveInterval = (int)conf.KeepAliveInterval;
        }
        public ConfigurationGA(int interval)
        {
            KeepAliveInterval = interval;
        }
    }
}
