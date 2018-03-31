using System;
using System.Collections.Generic;
using System.Linq;

namespace Slink
{
    public class ServiceRunner
    {
        //holds a reference to all the services added
        List<BaseQueue> Services = new List<BaseQueue>();

        public T AddService<T>() where T : BaseQueue
        {
            //check if service of that type already exists
            var existingService = Services.OfType<T>().FirstOrDefault();
            if (existingService != null) return existingService;

            //create one and add it
            var service = Activator.CreateInstance<T>();
            if (service != null)
                Services.Add(service);

            return service;
        }
        public void StartService<T>() where T : BaseQueue
        {
            //retrieve a service of the given type if it exists
            var service = Services.OfType<T>().FirstOrDefault();

            //if doesnt exist, add it
            if (service == null)
                service = AddService<T>();

            //start her up
            service?.Start();
        }

        public T FetchService<T>() where T : BaseQueue
        {
            return Services.OfType<T>().FirstOrDefault();
        }

        public bool ContainsService<T>() where T : BaseQueue
        {
            return FetchService<T>() != null;
        }

        public void Trim()
        {
            foreach (BaseQueue service in Services)
            {
                service?.Dispose();
            }
            Services.Clear();
        }




        #region Singleton
        private static volatile ServiceRunner Instance;
        private static object syncRoot = new Object();

        public static ServiceRunner SharedInstance
        {
            get
            {
                if (Instance == null)
                {
                    lock (syncRoot)
                    {
                        if (Instance == null)
                        {
                            Instance = new ServiceRunner();
                        }
                    }
                }
                return Instance;
            }
        }
        #endregion
    }
}
