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
    public class User
    {
        [Key]
        [Column(TypeName = "char(36)")]
        public Guid Id { set; get; }
        public string Name { set; get; }   
        [Column(TypeName = "char(36)")]
        public Guid CreateTeacherId { set; get; }

        [ForeignKey("CreateTeacherId")]
        public virtual Teacher CreateTeacher { set; get; }

 
        public virtual List<UserLog> UserLogs { set; get; }
    }
}
