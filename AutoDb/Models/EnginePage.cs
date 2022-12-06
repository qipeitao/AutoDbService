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
        /// <summary>
        /// 跳过
        /// </summary>
        public int Skip { get; set; }
        /// <summary>
        /// 获取
        /// </summary>
        public int Take { get; set; }
    }
}
