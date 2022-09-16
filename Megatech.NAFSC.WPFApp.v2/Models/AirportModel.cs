using Megatech.NAFSC.WPFApp.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megatech.NAFSC.WPFApp.Models
{
    class AirportModel
    {
        public string Code { get; set; }

        private static ICollection<AirportModel> _list;

        public static ICollection<AirportModel> GetList()
        {

            if (_list == null)
            {
                try
                {
                    _list = DataRepository.GetInstance().GetAirports();
                    if (_list == null)
                    {
                        string data = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "airports.json"));
                        _list = JsonConvert.DeserializeObject<ICollection<AirportModel>>(data);
                    }
                }
                catch {
                    _list = null;
                }
            }
            return _list;
        }

        public static bool Check(string airport)
        {
            if (_list == null)
                GetList();
            return _list.Any(am => am.Code.Equals(airport.Trim(), StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
