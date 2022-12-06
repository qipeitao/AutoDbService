using AutoDbService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDbService.DbPrism.Interfaces
{
    /// <summary>
    /// 清理动态生成的类型及对象
    /// </summary>
    public interface IDynamicTypeClear:IStop
    {
        /// <summary>
        /// 添加新的对象
        /// </summary>
        /// <param name="obj">实例</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        bool AddObj(object obj, object type); 
    }
}
