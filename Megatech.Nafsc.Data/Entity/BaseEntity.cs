using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace FMS.Data
{
    public  class BaseEntity: IBaseEntity
    {
        protected DataContext _dbContext = null;
        public BaseEntity()
        {
            this.DateCreated = this.DateUpdated = DateTime.Now;
        }

        public BaseEntity(DataContext context):this()
        {
            _dbContext = context;
        }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        public int? UserCreatedId { get; set; }

        public int? UserUpdatedId { get; set; }

        public  bool IsDeleted { get; set; }

        public DateTime? DateDeleted { get; set; }

        public  int? UserDeletedId { get; set; }
       
    }
}
