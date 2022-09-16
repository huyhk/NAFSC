using FMS.Data;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace Megatech.NAFSC.DataExport
{
    public class Exporter
    {
        private string EXPORT_BASE_URL = "http://116.193.76.111:8080/";

        public Exporter()
        {
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .WriteTo.File(@"E:\WEBHOST\nafsc\nafscapi.megatech.com.vn\logs\logfile.log", rollingInterval: RollingInterval.Day)
               .CreateLogger();
        }
        public IEnumerable<ExportResult> Export()
        {
            using (var db = new DataContext())
            {
                var result = new List<ExportResult>();
                var ids = db.Invoices.Where(inv => inv.ExportedResult != 0).Select(inv => inv.Id).ToArray();
                foreach (var item in ids)
                {
                    result.Add (Export(item));
                }
                return result;
            }
            
        }
        public ExportResult Export(int id)
        {
            try
            {
                using (var db = new DataContext())
                {
                    var inv = db.Invoices.Include(i => i.Items).Include(i => i.Flight).Include(i => i.Customer).FirstOrDefault(i => i.Id == id);
                    if (inv != null && inv.Flight != null)
                    {
                        var exportModel = new ExportModel
                        {
                            UniqueId = inv.UniqueId.ToString(),
                            AirlineCode = inv.CustomerCode ?? inv.Customer.Code,
                            AirlineName = inv.CustomerName,
                            Address = inv.Address,
                            TaxCode = inv.TaxCode,
                            FlightCode = inv.Flight.Code,
                            ArrivalTime = inv.Flight.ArrivalTime ?? inv.Flight.ArrivalScheduledTime,
                            DepartureTime = inv.Flight.DepartuteTime ?? inv.Flight.DepartureScheduledTime,
                            RefuelScheduledTime = inv.RefuelTime,
                            AircraftCode = inv.Flight.AircraftCode ?? "N/A",
                            AircraftType = inv.Flight.AircraftType ?? "N/A",
                            RouteName = inv.Flight.RouteName,
                            IsInternational = inv.Flight.FlightType == FlightType.OVERSEA,
                            ReceiptNumber = inv.InvoiceNumber,
                            ReceiptDate = inv.EndTime.Value.Date,
                            TotalGallon = inv.Gallon,
                            TotalKg = inv.Weight,
                            TotalLiter = inv.Volume,
                            Mode = DATA_MODE.INSERT,
                            ImagePath = inv.ImagePath

                        };

                        exportModel.Items = inv.Items.Select(it => new ExportItemModel
                        {
                            RefuelerNo = it.TruckNo,
                            Gallon = it.Gallon,
                            Liter = it.Volume,
                            Kg = it.Weight,
                            Density = it.Density,
                            Temperature = it.Temperature,
                            StartNumber = it.StartNumber,
                            EndNumber = it.EndNumber,
                            CertNo = it.Invoice.QualityNo,
                            StartTime = it.Invoice.StartTime.Value,
                            EndTime = it.Invoice.EndTime.Value


                        }).ToList();

                        _token = Login();
                        if (_token != null)
                        {
                            var result = PostData(exportModel, _token);
                            inv.ExportedResult = (int) result.Result ;
                            inv.ExportError = result.Message?.ToString();
                            inv.DateExported = DateTime.Now;
                            db.SaveChanges();
                            return result;
                        }
                    }
                    else return new ExportResult { Result = EXPORT_RESULT.FAILED, Message = $"Invoice id {id} not exists" };
                }
            }catch (Exception ex)
            {
                return new ExportResult { Result = EXPORT_RESULT.FAILED, Message = ex.StackTrace };

            }
            return new  ExportResult { Result = EXPORT_RESULT.FAILED, Message = "unknown error" };
        }
        private string _token;
        private string Login()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(EXPORT_BASE_URL);
                var values = new List<KeyValuePair<string, string>>();
                values.Add(new KeyValuePair<string, string>("username", "NAFSC"));
                values.Add(new KeyValuePair<string, string>("password", "$kypecN@123"));
                values.Add(new KeyValuePair<string, string>("grant_type", "password"));
                var content = new FormUrlEncodedContent(values); FormUrlEncodedContent data = new FormUrlEncodedContent(values);
                var re = client.PostAsync("token", content).Result;

                if (re.IsSuccessStatusCode)
                {
                    var model = JsonConvert.DeserializeObject<LoginModel>(re.Content.ReadAsStringAsync().Result);
                    return model.Access_Token;
                }


            }
            return null;
        }

        private ExportResult PostData(ExportModel model, string token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(EXPORT_BASE_URL);

                var requestContent = new MultipartFormDataContent();
                var json = JsonConvert.SerializeObject(model);

                var jsonContent = new StringContent(json);
                jsonContent.Headers.Add("Content-Disposition", "form-data; name=\"refuelData\"");

                requestContent.Add(jsonContent, "refuelData");

                var folderPath = AppDomain.CurrentDomain.BaseDirectory + "receipts";
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);
                var file = model.ImagePath;
                if (!File.Exists(file))  
                    file = Path.Combine(folderPath, model.ImagePath);
                if (!File.Exists(file))
                    file = Path.Combine(folderPath, model.ReceiptNumber + ".jpg");
                if (File.Exists(file))
                {
                    var imageContent = new StreamContent(new FileStream(file, FileMode.Open));

                    imageContent.Headers.Add("Content-Type", "image/*");
                    imageContent.Headers.Add("Content-Disposition", "form-data; name=\"ReceiptImage\"; filename=\"" + Path.GetFileName(file) + "\"");

                    requestContent.Add(imageContent, "ReceiptImage", Path.GetFileName(file));
                }
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                var re = client.PostAsync("api/fhs", requestContent).Result;

                if (re.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ExportResult>(re.Content.ReadAsStringAsync().Result);
                    return result;
                }
                else
                {
                    return new ExportResult { Result = EXPORT_RESULT.FAILED, Message = re.Content.ReadAsStringAsync().Result };


                }

            }
            return new ExportResult { Result = EXPORT_RESULT.FAILED, Message = "unknown error" };
        }
    }
}
