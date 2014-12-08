using CardWorkbench.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardWorkbench.Utils
{
    public class WordPropertiesManager
    {
        private static Dictionary<string, InitializeWordProperties> wordPropertiesDictionary = null;
        private static object _lock = new object();

        public static Dictionary<string, InitializeWordProperties> getCurrentWordPropertiesInstance()
        {
            if (wordPropertiesDictionary == null)
            {
                lock (_lock)
                {
                    if (wordPropertiesDictionary == null)
                    {
                        return null;
                    }

                }
            }
            return wordPropertiesDictionary;
        }

        public static void addCurrentWordProperties2Dictionary(InitializeWordProperties initializeWordProperties)
        {
            if (wordPropertiesDictionary == null)
            {
                wordPropertiesDictionary = new Dictionary<string, InitializeWordProperties>();
            }
            if (initializeWordProperties != null && !string.IsNullOrEmpty(initializeWordProperties.deviceID) && !string.IsNullOrEmpty(initializeWordProperties.channelID))
	        {
                string key = initializeWordProperties.deviceID + "-" + initializeWordProperties.channelID;
                wordPropertiesDictionary[key] = initializeWordProperties;
	        }
        }

        public static InitializeWordProperties findCurrentWordProperties(string deviceID, string channnelID) 
        {
            if (wordPropertiesDictionary != null)
            {
                string key = deviceID + "-" + channnelID;
                if (wordPropertiesDictionary.ContainsKey(key))
                {
                    return wordPropertiesDictionary[key];
                }
            }
            return null;
        }

    }
}
