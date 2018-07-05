using System;
using System.Collections.Generic;
using Realms.Sync;
using Realms;
using Amazon.Runtime;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Slink
{
    public sealed class RealmManager
    {
        static volatile RealmManager instance;
        static object syncRoot = new Object();

        public void Trim() { }


        public Realm GetRealm(string realmSyncConfigurationString)
        {
            //if (String.IsNullOrEmpty(realmSyncConfigurationString))
            //if (User.Current == null) return Realm.GetInstance();

            //if (realmSyncConfigurationString.Equals(realm_sync_configuration_mine, StringComparison.OrdinalIgnoreCase))
            //return Realm.GetInstance(new SyncConfiguration(User.Current, new Uri(realm_sync_config_url_prefix + User.Current.Identity)));

            var config = new RealmConfiguration();
            config.SchemaVersion = 2;
            config.MigrationCallback += (migration, oldSchemaVersion) =>
            {

            };
            return Realm.GetInstance(config);
        }






        public static RealmManager SharedInstance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new RealmManager();
                    }
                }

                return instance;
            }
        }
    }
}
