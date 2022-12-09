using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDbService.DbPrism.Attributes
{
    /// <summary>
    /// 转换为Command
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class BindingPropertyAttribute : Attribute
    {
       
    }
}
