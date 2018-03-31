using System;
using System.Collections.Generic;
namespace Slink
{
    public class Transporter
    {
        public static string NewOutletAddedTransporterKey = "NewOutletAdded";
        public static string NewOutletAddedValueTransporterKey = "NewOutletAddedValue";

        Dictionary<string, object> Dict = new Dictionary<string, object>();

        public void SetObject(string key, object obj)
        {
            if (String.IsNullOrEmpty(key) || obj == null) return;

            //update instead
            if (Dict.ContainsKey(key))
            {
                Dict[key] = obj;
                return;
            }

            Dict.Add(key, obj);
        }
        public bool ContainsKey(string key)
        {
            if (String.IsNullOrEmpty(key)) return true;

            return Dict.ContainsKey(key);
        }
        public object GetObjectForKey(string key)
        {
            if (String.IsNullOrEmpty(key)) return null;

            return Dict.ContainsKey(key) ? Dict[key] : null;
        }
        public void RemoveObject(string key)
        {
            if (String.IsNullOrEmpty(key)) return;

            if (Dict.ContainsKey(key))
                Dict.Remove(key);
        }
        public void Trim()
        {
            Dict.Clear();
        }

        #region Singleton

        private static volatile Transporter Instance;
        private static object syncRoot = new Object();

        public static Transporter SharedInstance
        {
            get
            {
                if (Instance == null)
                {
                    lock (syncRoot)
                    {
                        if (Instance == null)
                        {
                            Instance = new Transporter();
                        }
                    }
                }
                return Instance;
            }
        }
        #endregion
    }
}
