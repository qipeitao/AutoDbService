using AutoDbService.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AutoDbService.Interfaces
{
    /// <summary>
    /// 基础服务接口
    /// </summary>
    public interface IDbServiceCreator
    { 
        /// <summary>
        /// 获取库表服务
        /// </summary>
        /// <returns>key:Type of TEntity; Value:Service</returns>
        Dictionary<Type,object> CreateDbService();
    }
}
