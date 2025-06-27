using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megatech.NAFSC.WPFApp.Data
{
    public class BaseLocalEntity
    {
        
        [Key]
        public Guid Key { get; set; }

        public string JsonData  { get; set; }

        public bool Synced { get; set; }

        //public string SyncError { get; set; }

        public int Id { get; set; }

        public UPDATED_LOCATION  UpdatedLocation { get; set; }
    }

    public enum UPDATED_LOCATION
    {
        WEB,
        APP
    }
}
