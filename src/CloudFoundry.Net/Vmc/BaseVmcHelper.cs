﻿namespace CloudFoundry.Net.Vmc
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Types;

    internal abstract class BaseVmcHelper
    {
        protected readonly VcapCredentialManager credMgr;

        public BaseVmcHelper(VcapCredentialManager credMgr)
        {
            this.credMgr = credMgr;
        }

        public string GetApplicationJson(string name)
        {
            var r = new VcapRequest(credMgr, Constants.APPS_PATH, name);
            return r.Execute().Content;
        }

        public Application GetApplication(string name)
        {
            string json = GetApplicationJson(name);
            return JsonConvert.DeserializeObject<Application>(json);
        }

        public IEnumerable<Application> GetApplications()
        {
            var r = new VcapRequest(credMgr, Constants.APPS_PATH);
            return r.Execute<Application[]>();
        }

        protected bool AppExists(string name)
        {
            bool rv = true;
            try
            {
                string appJson = GetApplicationJson(name);
            }
            catch (VmcNotFoundException)
            {
                rv = false;
            }
            return rv;
        }
    }
}