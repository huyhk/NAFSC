using FMS.Data;
using Megatech.FMS.WebAPI.Models;
using Megatech.NAFSC.WPFApp.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megatech.NAFSC.WPFApp.Data
{
    public class DataRepository
    {
        private static DataRepository _repo;

        public ICollection<AirlineViewModel> GetAirlines()
        {
            ApiHelper client = new ApiHelper();

            return  client.GetAirlines();
            
        }

        public ICollection<RefuelViewModel> GetRefuelList(string sDate)
        {
            ApiHelper client = new ApiHelper();

            return client.GetRefuels(sDate);
           
        }

        public RefuelViewModel GetRefuel(int id)
        {
            ApiHelper client = new ApiHelper();

            return client.GetRefuel(id);

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

        public RefuelItem PostRefuel(RefuelViewModel model)
        {
            ApiHelper client = new ApiHelper();
            var response =  client.PostRefuel(model);
            var respItem = JsonConvert.DeserializeObject<RefuelItem>(response);
            return respItem;

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
    }
}
