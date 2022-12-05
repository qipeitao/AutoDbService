using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AutoDbService.Entity.Entities
{
    public class UserLog
    {
        [Key]
        [Column(TypeName = "char(36)")]
        public Guid Id { set; get; }  
        public string Remark { set; get; }

        [Column(TypeName = "char(36)")]
        public Guid CreateUserId { set; get; } 

        [ForeignKey("CreateUserId")] 
        public virtual User CreateUser { set; get; }
    }
}
