using FMS.Data;
using Megatech.FMS.WebAPI.Models;
using Megatech.NAFSC.WPFApp.Global;
using Megatech.NAFSC.WPFApp.Helpers;
using Megatech.NAFSC.WPFApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace Megatech.NAFSC.WPFApp.Data
{
    public class DataRepository
    {
        public DataRepository()
        {
            EnableSync();
            
        }
        private static DataRepository _repo;
        private LocalDbContext _db = new LocalDbContext();
        private Timer timer;
        public void EnableSync()
        {
            var startTimeSpan = TimeSpan.FromSeconds(30);
            var periodTimeSpan = TimeSpan.FromSeconds(30);
            timer = new System.Threading.Timer(new System.Threading.TimerCallback(Synchronize), null, startTimeSpan, periodTimeSpan);

        }

        internal void GetRemoteAirlines()
        {
            try
            {
                ApiHelper client = new ApiHelper();
                var remoteList = client.GetAirlines();
                if (remoteList != null)
                {
                    foreach (var item in remoteList)
                    {
                        var localItem = _db.Airlines.FirstOrDefault(lu => lu.Id == item.Id);
                        if (localItem == null)
                        {
                            localItem = new LocalAirline { Id = item.Id, Key = Guid.NewGuid(), JsonData = JsonConvert.SerializeObject(item), Synced = true };
                            _db.Airlines.Add(localItem);
                        }

                        else
                            localItem.JsonData = JsonConvert.SerializeObject(item);
                    }
                    _db.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }
        }
        public ICollection<AirlineViewModel> GetAirlines()
        {
            GetRemoteAirlines();

            var localList = _db.Airlines.ToList();
            var list = new List<AirlineViewModel>();
            foreach (var item in localList)
            {
                list.Add(JsonConvert.DeserializeObject<AirlineViewModel>(item.JsonData));
            }


            return list.OrderBy(a=>a.Name).ToList();

        }

        public ICollection<RefuelViewModel> GetRefuelList(string sDate)
        {
            using (var _db = new LocalDbContext())
            {
                if (true || UseRemote())
                {
                    ApiHelper client = new ApiHelper();
                    var remoteList = client.GetRefuels(sDate);
                    if (remoteList != null)
                    {

                        foreach (var item in remoteList.Where(r=>!r.IsDeleted))
                        {
                            var localItem = _db.Refuels.FirstOrDefault(lr => lr.Id == item.Id);
                            if (localItem == null)
                            {
                                localItem = new LocalRefuel { Id = item.Id, JsonData = JsonConvert.SerializeObject(item), Key = Guid.NewGuid(),  Synced = true };
                                if (item.RefuelTime != null)
                                    localItem.Date = item.RefuelTime.Value.ToString("yyyyMMdd");
                                _db.Refuels.Add(localItem);
                            }
                            else if (localItem.Synced)
                            {
                                localItem.JsonData = JsonConvert.SerializeObject(item);
                                if (item.RefuelTime != null)
                                    localItem.Date = item.RefuelTime.Value.ToString("yyyyMMdd");
                            }

                        }
                        //remove deleted items
                        var ids = remoteList.Where(r => r.IsDeleted).Select(r => r.Id).ToList();
                        _db.Refuels.Where(l => ids.Contains(l.Id)).Delete();

                        _db.SaveChanges();
                    }

                }
                var localList = _db.Refuels.Where(lr => lr.Date == sDate).ToList();
                var list = new List<RefuelViewModel>();
                foreach (var localItem in localList)
                {
                    var item = JsonConvert.DeserializeObject<RefuelViewModel>(localItem.JsonData);
                    item.LocalGuid = localItem.Key;
                    list.Add(item);
                }

                //list = localList.Select(lr => JsonConvert.DeserializeObject<RefuelViewModel>(lr.JsonData)).ToList();
                return list.OrderBy(r => r.RefuelTime).ThenBy(r=>r.Id).ToList();
            }

        }
        public RefuelViewModel GetRefuel(Guid guid)
        {
            var localItem = _db.Refuels.FirstOrDefault(lr => lr.Key == guid);
            RefuelViewModel item = null;

            if (localItem != null)
            {
                item = JsonConvert.DeserializeObject<RefuelViewModel>(localItem.JsonData);
                item.InvoiceGuid = localItem.InvoiceGuid;
                item.LocalGuid = localItem.Key;
            }
            return item;
        }
        public RefuelViewModel GetRefuel(int id)
        {
            using (var _db = new LocalDbContext())
            {
                var localItem = _db.Refuels.FirstOrDefault(lr => lr.Id == id);
                if (UseRemote())
                {
                    ApiHelper client = new ApiHelper();
                    var remoteItem = client.GetRefuel(id);

                    if (remoteItem != null)
                    {
                        if (localItem == null)
                        {
                            var key = Guid.NewGuid();
                            remoteItem.LocalGuid = key;
                            localItem = new LocalRefuel { Key = key, Id = remoteItem.Id, Date = remoteItem.RefuelTime.Value.ToString("yyyyMMdd"), JsonData = JsonConvert.SerializeObject(remoteItem), Synced = true };
                            _db.Refuels.Add(localItem);
                        }
                        else if (localItem.Synced)
                        {
                            localItem.JsonData = JsonConvert.SerializeObject(remoteItem);
                        }

                        _db.SaveChanges();
                    }
                }
                RefuelViewModel item = null;


                if (localItem != null)
                {
                    item = JsonConvert.DeserializeObject<RefuelViewModel>(localItem.JsonData);
                    item.InvoiceGuid = localItem.InvoiceGuid;
                    item.LocalGuid = localItem.Key;
                }
                return item;
            }
        }

        internal void GetRemoteUsers()
        {
            try
            {
                ApiHelper client = new ApiHelper();
                var remoteList = client.GetUsers();
                if (remoteList != null)
                {
                    foreach (var item in remoteList)
                    {
                        var localItem = _db.Users.FirstOrDefault(lu => lu.Id == item.Id);
                        if (localItem == null)
                        {
                            localItem = new LocalUser { Id = item.Id, Key = Guid.NewGuid(), JsonData = JsonConvert.SerializeObject(item), Synced = true };
                            _db.Users.Add(localItem);
                        }

                        else
                            localItem.JsonData = JsonConvert.SerializeObject(item);
                    }
                    _db.SaveChanges();
                }
            }
            catch (Exception ex)
            { }
        }
        internal ICollection<UserViewModel> GetUsers()
        {
            GetRemoteUsers();
            var localList = _db.Users.ToList();
            var list = new List<UserViewModel>();
            foreach (var item in localList)
            {
                list.Add(JsonConvert.DeserializeObject<UserViewModel>(item.JsonData));
            }


            return list;
        }


        internal ICollection<AirportModel> GetAirports()
        {
            return GetRemoteAirports();          
       
        }

        private ICollection<AirportModel> GetRemoteAirports()
        {
            ApiHelper client = new ApiHelper();
           return client.GetAirports();
        }

        internal static DataRepository GetInstance()
        {
            if (_repo == null)
                _repo = new DataRepository();
            return _repo;
        }

        public RefuelViewModel PostRefuel(RefuelViewModel model)
        {
            var localItem = new LocalRefuel();

            model.RefuelTime = model.Airline != null && model.Airline.Vendor == Vendor.SKYPEC ? model.EndTime : model.StartTime;
            model.TruckNo = AppSetting.CurrentSetting.TruckNo;

            var canPost = false;
            if (UseRemote())
            {
                ApiHelper client = new ApiHelper();
                var response = client.PostRefuel(model);

                var respItem = JsonConvert.DeserializeObject<RefuelViewModel>(response);
                if (respItem != null)
                {
                    localItem = _db.Refuels.FirstOrDefault(lr => lr.Id == respItem.Id);
                    model.Id = respItem.Id;
                    if (localItem == null)
                    {
                        localItem = new LocalRefuel { Id = respItem.Id, JsonData = JsonConvert.SerializeObject(respItem), Key = Guid.NewGuid(), Date = respItem.RefuelTime.Value.ToString("yyyyMMdd"), Synced = true };
                        _db.Refuels.Add(localItem);
                    }
                    else
                    {
                        localItem.Id = respItem.Id;
                        localItem.JsonData = JsonConvert.SerializeObject(respItem);
                        localItem.Date = respItem.RefuelTime.Value.ToString("yyyyMMdd");
                    }
                    canPost = true;
                }

            }

            if (!canPost)
            {
                if (model.Id == 0 && model.LocalGuid == Guid.Empty) //new item                
                {
                    localItem = new LocalRefuel { Id = 0, JsonData = JsonConvert.SerializeObject(model), Key = Guid.NewGuid(), Date = model.RefuelTime.Value.ToString("yyyyMMdd"), Synced = false };
                    _db.Refuels.Add(localItem);
                }
                else
                {
                    if (model.LocalGuid == Guid.Empty)
                        localItem = _db.Refuels.FirstOrDefault(lr => lr.Id == model.Id);
                    else
                        localItem = _db.Refuels.FirstOrDefault(lr => lr.Key == model.LocalGuid);
                    if (localItem != null)
                    {
                        localItem.JsonData = JsonConvert.SerializeObject(model);
                        localItem.Id = model.Id;
                        localItem.Date = model.RefuelTime.Value.ToString("yyyyMMdd");
                        localItem.Synced = false;
                    }
                    else
                    {
                        localItem = new LocalRefuel { Id = model.Id, JsonData = JsonConvert.SerializeObject(model), Key = Guid.NewGuid(), Date = model.RefuelTime.Value.ToString("yyyyMMdd"), Synced = false };
                        _db.Refuels.Add(localItem);
                    }
                }
            }

            _db.SaveChanges();



            try
            {
                var respItem = JsonConvert.DeserializeObject<RefuelViewModel>(localItem.JsonData);
                respItem.LocalGuid = localItem.Key;

                return respItem;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public ICollection<TruckViewModel> GetTrucks()
        {
            ApiHelper client = new ApiHelper();
            return client.GetTrucks();
        }

        private bool UseRemote()
        {
            return AppSetting.CheckInternet();
        }
        internal InvoiceViewModel PostInvoice(InvoiceViewModel invoice)
        {
            return PostInvoice(invoice, true);
        }
        internal InvoiceViewModel PostInvoice(InvoiceViewModel invoice, bool remote = true)
        {
            var localItem = new LocalInvoice();
            var canPost = false;
            if (UseRemote() && remote)
            {
                ApiHelper client = new ApiHelper();
                if (!string.IsNullOrEmpty(invoice.ImagePath))
                {
                    if (File.Exists(invoice.ImagePath))
                    {
                        using (System.Drawing.Image image = System.Drawing.Image.FromFile(invoice.ImagePath))
                        {
                            using (MemoryStream m = new MemoryStream())
                            {
                                image.Save(m, image.RawFormat);
                                byte[] imageBytes = m.ToArray();
                                invoice.ImageString = Convert.ToBase64String(imageBytes);
                            }
                        }
                        
                    }
                }
                var response = client.PostInvoice(invoice);
                var respItem = JsonConvert.DeserializeObject<InvoiceViewModel>(response);
                if (respItem != null)
                {
                    localItem = _db.Invoices.FirstOrDefault(lr => lr.Id == respItem.Id);
                    if (localItem == null)
                    {
                        localItem = new LocalInvoice { Id = respItem.Id, JsonData = JsonConvert.SerializeObject(respItem), Key = Guid.NewGuid(), Synced = true };
                        _db.Invoices.Add(localItem);
                    }
                    else
                    {
                        localItem.JsonData = JsonConvert.SerializeObject(respItem);
                    }
                    canPost = true;
                }
            }

            if (!canPost)
            {
                invoice.ImageString = null;
                if (invoice.Id == 0) //new item                
                {
                    
                    localItem = new LocalInvoice
                    {
                        Id = 0,
                        JsonData = JsonConvert.SerializeObject(invoice),
                        Key = Guid.NewGuid(),
                        InvoiceNumber = invoice.InvoiceNumber,
                        Synced = false
                    };
                    _db.Invoices.Add(localItem);
                }
                else
                {
                    localItem = _db.Invoices.FirstOrDefault(lr => lr.Id == invoice.Id);
                    if (localItem != null)
                    {
                        localItem.JsonData = JsonConvert.SerializeObject(invoice);
                        localItem.Id = invoice.Id;
                        localItem.InvoiceNumber = invoice.InvoiceNumber;
                        localItem.Synced = false;
                        
                    }
                    else
                    {
                        localItem = new LocalInvoice
                        {
                            Id = invoice.Id,
                            JsonData = JsonConvert.SerializeObject(invoice),
                            Key = Guid.NewGuid(),
                            InvoiceNumber = invoice.InvoiceNumber,
                            Synced = false
                        };
                        _db.Invoices.Add(localItem);
                    }
                }
            }

            _db.SaveChanges();

            var refuel = _db.Refuels.FirstOrDefault(lr => lr.Id == invoice.RefuelItemId);
            if (refuel != null)
            {
                refuel.InvoiceGuid = localItem.Key;
                RefuelViewModel refuelModel = JsonConvert.DeserializeObject<RefuelViewModel>(refuel.JsonData);
                refuelModel.Printed = true;
                refuelModel.InvoiceGuid = localItem.Key;
                refuel.JsonData = JsonConvert.SerializeObject(refuelModel);
                refuel.Synced = false;
                _db.SaveChanges();
            }

            InvoiceViewModel item = JsonConvert.DeserializeObject<InvoiceViewModel>(localItem.JsonData);
            item.LocalGuid = localItem.Key;
            return item;
        }

        internal void CancelInvoice(int id)
        {
            ApiHelper client = new ApiHelper();
            var response = client.CancelInvoice(id);

        }


        internal InvoiceViewModel GetInvoice(Guid guid)
        {
            InvoiceViewModel model = null;
            var localItem = _db.Invoices.FirstOrDefault(lr => lr.Key == guid);
            if (localItem != null)
                model = JsonConvert.DeserializeObject<InvoiceViewModel>(localItem.JsonData);
            return model;
        }
        internal InvoiceViewModel GetInvoice(int id)
        {
            return GetInvoice(id, Guid.Empty);
        }
        internal InvoiceViewModel GetInvoice(int id, Guid localId )
        {
            using (var _db = new LocalDbContext())
            {
                var localItem = _db.Invoices.FirstOrDefault(lr => lr.Id == id && lr.Id > 0);
                if (UseRemote() && id > 0)
                {
                    ApiHelper client = new ApiHelper();
                    var remoteItem = client.GetInvoice(id);

                    if (remoteItem != null)
                    {
                        if (localItem == null)
                        {
                            var key = Guid.NewGuid();
                            remoteItem.LocalGuid = key;
                            localItem = new LocalInvoice { Key = key, Id = remoteItem.Id, InvoiceNumber = remoteItem.InvoiceNumber, JsonData = JsonConvert.SerializeObject(remoteItem), Synced = true };
                            _db.Invoices.Add(localItem);

                        }
                        else if (localItem.Synced)
                        {
                            localItem.JsonData = JsonConvert.SerializeObject(remoteItem);
                        }
                        try
                        {
                            _db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            if (ex.InnerException != null)
                                Console.WriteLine(ex.InnerException.Message);
                        }

                    }
                }
                InvoiceViewModel item = null;

                if (localItem == null)
                    localItem = _db.Invoices.FirstOrDefault(lr => lr.Key == localId);


                if (localItem != null)
                    item = JsonConvert.DeserializeObject<InvoiceViewModel>(localItem.JsonData);
                return item;
            }
        }

        object lockObject = new object();

        public void Synchronize(Object state)
        {
            using (var _db = new LocalDbContext())
            {
                var i = 0;
                if (Monitor.TryEnter(lockObject))
                {
                    try
                    {
                        bool canSync = true;
                        ApiHelper client = new ApiHelper();

                        // poset newly created refuel items first
                        var localList = _db.Refuels.Where(lr => !lr.Synced && lr.Id == 0).ToList();
                        i = 0;
                        while (canSync && i < localList.Count)
                        {
                            var item = localList[i++];
                            var model = JsonConvert.DeserializeObject<RefuelViewModel>(item.JsonData);
                            var response = client.PostRefuel(model);
                            var respItem = JsonConvert.DeserializeObject<RefuelViewModel>(response);
                            if (respItem == null)
                                canSync = false;
                            else
                            {
                                item.Id = respItem.Id;
                                item.Synced = true;
                                var inv = _db.Invoices.FirstOrDefault(li => li.Key == item.InvoiceGuid);
                                if (inv != null)
                                {
                                    var invObj = JsonConvert.DeserializeObject<InvoiceViewModel>(inv.JsonData);
                                    invObj.RefuelItemId = item.Id;
                                    inv.JsonData = JsonConvert.SerializeObject(invObj);
                                }
                            }
                        }
                        _db.SaveChanges();

                        //post invoices

                        var localInvoices = _db.Invoices.Where(lr => !lr.Synced ).ToList();

                        while (canSync && i < localInvoices.Count)
                        {
                            var item = localInvoices[i++];
                            
                            var model = JsonConvert.DeserializeObject<InvoiceViewModel>(item.JsonData);
                            if (model.CanSync)
                            {
                                if (!string.IsNullOrEmpty(model.ImagePath))
                                {
                                    if (File.Exists(model.ImagePath))
                                    {
                                        byte[] imageArray = File.ReadAllBytes(model.ImagePath);
                                        string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                                        model.ImageString = base64ImageRepresentation;

                                    }
                                }
                                var response = client.PostInvoice(model);
                                var respItem = JsonConvert.DeserializeObject<InvoiceViewModel>(response);
                                if (respItem != null)
                                    
                                {
                                    item.Id = respItem.Id;
                                    item.Synced = true;

                                    var refuel = _db.Refuels.FirstOrDefault(lr => lr.InvoiceGuid == item.Key);
                                    if (refuel != null)
                                    {
                                        var refuelModel = JsonConvert.DeserializeObject<RefuelViewModel>(refuel.JsonData);
                                        refuelModel.InvoiceId = respItem.Id;
                                        refuel.JsonData = JsonConvert.SerializeObject(refuelModel);
                                    }
                                }
                            }
                        }
                        _db.SaveChanges();

                        //post remaining refuels
                        localList = _db.Refuels.Where(lr => !lr.Synced).ToList();
                        i = 0;
                        while (canSync && i < localList.Count)
                        {
                            var item = localList[i++];
                            var model = JsonConvert.DeserializeObject<RefuelViewModel>(item.JsonData);
                            var response = client.PostRefuel(model);
                            var respItem = JsonConvert.DeserializeObject<RefuelViewModel>(response);
                            if (respItem == null)
                                canSync = false;
                            else
                            {
                                item.Id = respItem.Id;
                                item.Synced = true;
                            }
                        }
                        _db.SaveChanges();

                        GetRemoteUsers();
                        GetRemoteAirlines();
                    }
                    finally
                    {
                        Monitor.Exit(lockObject);
                    }

                }
            }
        }
    }
}
