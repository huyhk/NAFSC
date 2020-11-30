using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Megatech.NAFSC.WPFApp.Global;
using Megatech.NAFSC.WPFApp.Model;
using FMS.Data;
using System.Net.Http.Headers;
using Megatech.FMS.WebAPI.Models;

namespace Megatech.NAFSC.WPFApp.Helpers
{
    public class ApiHelper
    {
        private HttpClient client = new HttpClient();

        private static string token;

        public static void SetToken(string token)
        {
            ApiHelper.token = token;
        }

        private async Task<string> PostData(string url, HttpContent data)
        {
            var response = string.Empty;
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(AppSetting.API_BASE_URL);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", ApiHelper.token);
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

        internal ICollection<UserViewModel> GetUsers()
        {
            var url = "api/users";

            var t = Task.Run(() => GetData(url));
            t.Wait();
            try
            {
                return JsonConvert.DeserializeObject<List<UserViewModel>>(t.Result);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        internal string PostInvoice(InvoiceViewModel invoice)
        {
            var url = "api/invoices";

            string data = JsonConvert.SerializeObject(invoice);
            HttpContent content = new StringContent(data, Encoding.UTF8, "application/json");
            var t = Task.Run(() => PostData(url, content));
            t.Wait();

            return t.Result;
        }

        internal string PostRefuel(RefuelViewModel model)
        {
            var url = "api/refuels";

            string data = JsonConvert.SerializeObject(model);
            HttpContent content = new StringContent(data, Encoding.UTF8, "application/json");
            var t = Task.Run(() => PostData(url, content));
            t.Wait();

            return t.Result;
        }

        internal ICollection<TruckViewModel> GetTrucks()
        {
            var url = "api/trucks";

            var t = Task.Run(() => GetData(url));
            t.Wait();
            try
            {
                return JsonConvert.DeserializeObject<List<TruckViewModel>>(t.Result);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        internal RefuelViewModel GetRefuel(int id)
        {
            var url = "api/refuels/" + id.ToString();

            var t = Task.Run(() => GetData(url));
            t.Wait();

            try
            {
                return JsonConvert.DeserializeObject<RefuelViewModel>(t.Result);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        internal ICollection<RefuelViewModel> GetRefuels(string truckNo)
        {
            var url = "api/refuels?sDate="+truckNo;

            var t = Task.Run(() => GetData(url));
            t.Wait();

            try
            {
                return JsonConvert.DeserializeObject<List<RefuelViewModel>>(t.Result);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async Task<string> GetData(string url)
        {
            var response = string.Empty;
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(AppSetting.API_BASE_URL);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", ApiHelper.token);
                    HttpResponseMessage result = await client.GetAsync(url);
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

        public ICollection<AirlineViewModel> GetAirlines()
        {
            var url = "api/airlines";

            var t = Task.Run(() => GetData(url));
            t.Wait();
            try
            {
                return JsonConvert.DeserializeObject<List<AirlineViewModel>>(t.Result);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public LoginModel Login(string username, string password)
        {
            var url = "token";

            var loginParam = new Dictionary<string, string>
            {
              {"userName", username },
              {"password", password },
              {"grant_type","password"}
            };
          
            HttpContent content = new FormUrlEncodedContent(loginParam);
            var t = Task.Run(() => PostData(url, content));
            t.Wait();

            if (t.Result != string.Empty)
            {
                var login = JsonConvert.DeserializeObject<LoginModel>(t.Result);
                return login;
            }
            return null;
        }
    }
}
