using Megatech.NAFSC.WPFApp.Global;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Megatech.NAFSC.WPFApp.Helpers
{
    public class LogHelper
    {
        private static string LOG_BASE_URL = "https://logger.viennam.vn";
        public void SendLogFile()
        { }

        public static object SendException(Exception ex)
        {
            var exModel = ExceptionModel.FromException(ex);
            if (AppSetting.CheckInternet())
            {

                string data = JsonConvert.SerializeObject(exModel);
                HttpContent content = new StringContent(data, Encoding.UTF8, "application/json");
                var t = Task.Run(() => PostData("api/logger/exception", content));
                t.Wait();

                return t.Result;
            }
            else SaveException(exModel);

            return null;
        }

        public static void SendExceptionFiles()
        {
            
            if (AppSetting.CheckInternet())
            {
                var logFolder = Path.Combine(Directory.GetCurrentDirectory(), "ExFolder");
                if (!Directory.Exists(logFolder))
                    Directory.CreateDirectory(logFolder);
                var files = Directory.GetFiles(logFolder, "*.json");
                if (files != null && files.Count() > 0)
                {
                    foreach (var item in files)
                    {
                        string data = File.ReadAllText(item);
                        HttpContent content = new StringContent(data, Encoding.UTF8, "application/json");
                        var t = Task.Run(() => PostData("api/logger/exception", content));
                        t.Wait();
                        File.Delete(item);
                    }
                   

                }
            }
           

            
        }

        private static void SaveException(ExceptionModel exModel)
        {
            string data = JsonConvert.SerializeObject(exModel);

            var logFolder = Path.Combine(Directory.GetCurrentDirectory(), "ExFolder");
            if (!Directory.Exists(logFolder))
                Directory.CreateDirectory(logFolder);
            var file = File.CreateText(Path.Combine(logFolder,$"{DateTime.Now:yyyyMMdd-HHmmss}.json"));
            file.WriteLine(data);
            file.Flush();
            file.Close();
        }

        private static async Task<string> PostData(string url, HttpContent data)
        {
            var response = string.Empty;
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(LOG_BASE_URL);
                    client.DefaultRequestHeaders.Add("Device-Id", AppSetting.CurrentSetting.TruckNo);
                    HttpResponseMessage result = await client.PostAsync(url, data);
                    if (result.IsSuccessStatusCode)
                    {
                        response = result.Content.ReadAsStringAsync().Result;
                    }

                }
                catch (Exception ex)
                { }
            }
            return response;


        }
    }

    public class ExceptionModel
    {
        public string Message { get; set; }

        public string StackTrace { get; set; }

        public ExceptionModel InnerException { get; set; }

        public static ExceptionModel FromException(Exception ex)
        {
            var exModel = new ExceptionModel { Message = ex.Message, StackTrace = ex.StackTrace };

            if (ex.InnerException != null)
                exModel.InnerException = FromException(ex.InnerException);

            return exModel;
        }
    }
}
