using FMS.Data;
using Megatech.FMS.WebAPI.Models;
using Megatech.NAFSC.WPFApp.Global;
using Megatech.NAFSC.WPFApp.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megatech.NAFSC.WPFApp.Data
{
    public class DataRepository
    {
        public DataRepository()
        {
            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromSeconds(30);
            var timer = new System.Threading.Timer((e) =>
            {
                Synchronize();
                }, null, startTimeSpan, periodTimeSpan);
        }
        private static DataRepository _repo;
        private LocalDbContext _db = LocalDbContext.GetInstance();


        public ICollection<AirlineViewModel> GetAirlines()
        {
            ApiHelper client = new ApiHelper();

            return client.GetAirlines();

        }

        public ICollection<RefuelViewModel> GetRefuelList(string sDate)
        {
            if (AppSetting.CheckInternet())
            {
                ApiHelper client = new ApiHelper();
                var remoteList = client.GetRefuels(sDate);
                foreach (var item in remoteList)
                {
                    var localItem = _db.Refuels.FirstOrDefault(lr => lr.Id == item.Id);
                    if (localItem == null)
                    {
                        localItem = new LocalRefuel { Id = item.Id, JsonData = JsonConvert.SerializeObject(item), Key = Guid.NewGuid(), Date = item.RefuelTime.Value.ToString("yyyyMMdd"), Synced = true };

                        _db.Refuels.Add(localItem);
                    }
                    else if (localItem.Synced)
                    {
                        localItem.JsonData = JsonConvert.SerializeObject(item);
                        localItem.Date = item.RefuelTime.Value.ToString("yyyyMMdd");
                    }

                }
                _db.SaveChanges();

            }
            var localList = _db.Refuels.Where(lr => lr.Date == sDate).ToList();
            var list = new List<RefuelViewModel>();
            foreach (var item in localList)
            {
                list.Add(JsonConvert.DeserializeObject<RefuelViewModel>(item.JsonData));
            }
            return list;

        }

        public RefuelViewModel GetRefuel(int id)
        {
            ApiHelper client = new ApiHelper();
            var item = client.GetRefuel(id);

            if (item == null)
            {
                var localItem = _db.Refuels.FirstOrDefault(lr => lr.Id == id);
                if (localItem != null)
                    item = JsonConvert.DeserializeObject<RefuelViewModel>(localItem.JsonData);
            }
            return item;
        }

        internal ICollection<UserViewModel> GetUsers()
        {
            ApiHelper client = new ApiHelper();
            return client.GetUsers();
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

            ApiHelper client = new ApiHelper();
            var response = client.PostRefuel(model);
            var respItem = JsonConvert.DeserializeObject<RefuelViewModel>(response);
            if (respItem != null)
            {
                localItem = _db.Refuels.FirstOrDefault(lr => lr.Id == respItem.Id);
                if (localItem == null)
                {
                    localItem = new LocalRefuel { Id = respItem.Id, JsonData = JsonConvert.SerializeObject(respItem), Key = Guid.NewGuid(), Date = respItem.RefuelTime.Value.ToString("yyyyMMdd"), Synced = true };
                    _db.Refuels.Add(localItem);
                }
                else
                {
                    localItem.JsonData = JsonConvert.SerializeObject(respItem);
                    localItem.Date = respItem.RefuelTime.Value.ToString("yyyyMMdd");
                }

            }
            else {
                if (model.Id == 0) //new item                
                {
                    localItem = new LocalRefuel { Id = 0, JsonData = JsonConvert.SerializeObject(model), Key = Guid.NewGuid(), Date = model.RefuelTime.Value.ToString("yyyyMMdd"), Synced = false };
                    _db.Refuels.Add(localItem);
                }
                else
                {
                    localItem = _db.Refuels.FirstOrDefault(lr => lr.Id == model.Id);
                    if (localItem != null)
                    {
                        localItem.JsonData = JsonConvert.SerializeObject(model);
                        localItem.Date = model.RefuelTime.Value.ToString("yyyyMMdd");
                        localItem.Synced = false;
                    }
                    else
                    {
                        localItem = new LocalRefuel { Id = 0, JsonData = JsonConvert.SerializeObject(model), Key = Guid.NewGuid(), Date = model.RefuelTime.Value.ToString("yyyyMMdd"), Synced = false };
                        _db.Refuels.Add(localItem);
                    }
                }
            }

            _db.SaveChanges();

            return JsonConvert.DeserializeObject<RefuelViewModel>(localItem.JsonData);
        }

        public ICollection<TruckViewModel> GetTrucks()
        {
            ApiHelper client = new ApiHelper();
            return client.GetTrucks();
        }

        internal InvoiceViewModel PostInvoice(InvoiceViewModel invoice)
        {
            ApiHelper client = new ApiHelper();
            var response = client.PostInvoice(invoice);
            var respItem = JsonConvert.DeserializeObject<InvoiceViewModel>(response);
            return respItem;
        }

        internal void CancelInvoice(int id)
        {
            ApiHelper client = new ApiHelper();
            var response = client.CancelInvoice(id);

        }

        internal InvoiceViewModel GetInvoice(int id)
        {
            ApiHelper client = new ApiHelper();
            return client.GetInvoice(id);

        }


        public void Synchronize()
        {
            bool canSync = true;
            ApiHelper client = new ApiHelper();
            
            var localList = _db.Refuels.Where(lr=>!lr.Synced).ToList();
            var i = 0;
            while (canSync && i<localList.Count)
            {
                var item = localList[i++];
                var model = JsonConvert.DeserializeObject<RefuelViewModel>(item.JsonData);
                var response = client.PostRefuel(model);
                var respItem = JsonConvert.DeserializeObject<RefuelViewModel>(response);
                if (respItem == null)
                    canSync = false;
                else item.Synced = true;
            }
            _db.SaveChanges();
        }
    }
}
