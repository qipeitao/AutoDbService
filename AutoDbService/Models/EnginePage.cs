using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDbService.Models
{
    /// <summary>
    /// 分页项
    /// </summary>
    public class EnginePage
    {
        public int Skip { get; set; }
        public int Take { get; set; }
    }
}
