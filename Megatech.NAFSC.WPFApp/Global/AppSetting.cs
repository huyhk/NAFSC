using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Megatech.NAFSC.WPFApp.Model;
using Megatech.NAFSC.WPFApp.Helpers;

namespace Megatech.NAFSC.WPFApp.Global
{
    public class AppSetting
    {
        private static AppSetting _setting;
        public static AppSetting CurrentSetting
        {
            get
            {
                if (_setting == null)
                {
                    _setting = new AppSetting();
                    _setting.Get();
                    //_setting = (AppSetting)GetSettingProperty("NAFSC");
                }
                return _setting;
            }
            
        }

        public static string API_BASE_URL = ConfigurationManager.AppSettings["API_BASE_URL"];

        public static decimal GALLON_TO_LITTRE = decimal.Parse(ConfigurationManager.AppSettings["GALLON_TO_LITTRE"]);

        public AppSetting()
        { }


        public void Save()
        {
            Properties.Settings.Default.UserId = this.UserId;
            Properties.Settings.Default.UserName = this.UserName;
            Properties.Settings.Default.AccessToken = this.AccessToken;
            Properties.Settings.Default.TruckNo = this.TruckNo;
            Properties.Settings.Default.Comport= this.ComPort;
            Properties.Settings.Default.MeterTimer = this.MeterTimer;
            Properties.Settings.Default.DataInterval = this.DataInterval;
            Properties.Settings.Default.Save();
        }
        public void Get()
        {
            this.UserId = Properties.Settings.Default.UserId  ;
            this.UserName = Properties.Settings.Default.UserName  ;
            this.AccessToken = Properties.Settings.Default.AccessToken ;
            this.TruckNo= Properties.Settings.Default.TruckNo  ;
            this.ComPort = Properties.Settings.Default.Comport ;
            this.MeterTimer = Properties.Settings.Default.MeterTimer;
            this.DataInterval = Properties.Settings.Default.DataInterval;

            ApiHelper.SetToken(this.AccessToken);
        }


        public bool IsLoggedIn()
        {
            return !String.IsNullOrEmpty(this.AccessToken);
        }

        public bool IsFirstUse()
        {
            return String.IsNullOrEmpty(this.TruckNo);
        }

        internal static void Logout()
        {
            _setting.AccessToken = null;
            _setting.Save();
        }

        public string UserName { get; set; }

        public int UserId { get; set; }

        public string AccessToken { get; set; }

        public string TruckNo { get; set; }

        public string ComPort { get; set; }

        public int DataInterval { get; set; }

        public bool MeterTimer { get; set; }

    }
}
