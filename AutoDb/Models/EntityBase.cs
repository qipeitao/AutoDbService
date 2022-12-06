using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDbService.Models
{
    public class EntityBase
    {
        [Key]
        [Column(TypeName = "char(36)")]
        public Guid Id { set; get; }
    }
}
