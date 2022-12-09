using AutoDbService.Interfaces;
using AutoDbService.Models;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDbService.DbPrism.Interfaces
{
    /// <summary>
    /// VM自动创建时，重新创建绑定关系时 
    /// 关联属性名称转换关系
    /// 自动创建Command，自动创建属性通知
    /// 需要标注特性BindingPropertyAttribute CommandAttribute
    /// </summary>
    public interface IBuildDynamicType
    {  
        TType BuildType<TType>() where TType : BindableBase;
        object BuildType(Type type);
    }
}
