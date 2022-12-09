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
    public interface IBuildDynamicType
    {  
        TType BuildType<TType>() where TType : BindableBase;
        object BuildType(Type type);
    }
}
