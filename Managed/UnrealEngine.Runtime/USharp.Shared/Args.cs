using System;
using System.Collections.Generic;

namespace USharp 
{
    public class Args
    {
        private Dictionary<string, string> args = new Dictionary<string, string>();

        public Args(string arg)
        {
            if (arg != null)
            {
                string[] splitted = arg.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string str in splitted)
                {
                    int equalsIndex = str.IndexOf('=');
                    if (equalsIndex > 0)
                    {
                        string key = str.Substring(0, equalsIndex).Trim();
                        string value = str.Substring(equalsIndex + 1).Trim();
                        if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                        {
                            args[key] = value;
                        }
                    }
                }
            }
        }

        public string this[string key]
        {
            get { return GetString(key); }
        }

        public bool Contains(string key)
        {
            return args.ContainsKey(key);
        }

        public string GetString(string key)
        {
            string value;
            args.TryGetValue(key, out value);
            return value;
        }

        public bool GetBool(string key)
        {
            string valueStr;
            bool value;
            if (args.TryGetValue(key, out valueStr) && bool.TryParse(valueStr, out value))
            {
                return value;
            }
            return false;
        }

        public int GetInt32(string key)
        {
            string valueStr;
            int value;
            if (args.TryGetValue(key, out valueStr) && int.TryParse(valueStr, out value))
            {
                return value;
            }
            return 0;
        }

        public long GetInt64(string key)
        {
            string valueStr;
            long value;
            if (args.TryGetValue(key, out valueStr) && long.TryParse(valueStr, out value))
            {
                return value;
            }
            return 0;
        }
    }
}
