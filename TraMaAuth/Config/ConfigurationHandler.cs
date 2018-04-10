using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cizeta.TraMaAuth
{
    class ConfigurationHandler
    {
        public string GetConnectionString()
        {
            return GetAppSetting("ConnectionString");
        }

        private string GetAppSetting(string settingName)
        {
            //Open the configuration file using the dll location
            Configuration dllConfig = ConfigurationManager.OpenExeConfiguration(this.GetType().Assembly.Location);
            // Get the appSettings section
            AppSettingsSection dllConfigAppSettings = (AppSettingsSection)dllConfig.GetSection("appSettings");
            // return the desired field 
            return dllConfigAppSettings.Settings[settingName].Value;
        }
    }
}
