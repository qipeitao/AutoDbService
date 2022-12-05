using AutoDbService.Models;
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
    public class Teacher: EntityBase
    { 
        public string Name { set; get; }   
         
        
        public virtual List<User> Users { set; get; }
    }
}
