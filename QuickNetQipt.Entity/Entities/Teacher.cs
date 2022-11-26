using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace QuickNetQipt.Entity.Entities
{
    public class Teacher
    {
        [Key]
        [Column(TypeName = "char(36)")]
        public Guid Id { set; get; }
        public string Name { set; get; }   
         
        
        public virtual List<User> Users { set; get; }
    }
}
