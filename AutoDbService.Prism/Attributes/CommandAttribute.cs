using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoDbService.DbPrism.Attributes
{
    /// <summary>
    /// 转换为Command
    /// 参数不能为空
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)] 
    public class CommandAttribute : Attribute
    { 

    }
}
