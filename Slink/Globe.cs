using System;
using System.Collections.Generic;

namespace Slink
{
    public sealed class Globe
    {
        public static volatile Globe instance;
        private static object syncRoot = new Object();

        private Globe() { }

        public void Trim() { }

        public static Globe SharedInstance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new Globe();
                    }
                }

                return instance;
            }
        }
    }
}

