using AutoDbService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AutoDbService.Interfaces
{
    /// <summary>
    /// 通用start
    /// </summary>
    public interface IStart
    {
        /// <summary>
        /// 
        /// </summary>
        void Start();
    }
    /// <summary>
    /// 通用默认stop实现
    /// </summary>
    public interface IStop
    {
        /// <summary>
        /// 
        /// </summary>
        void Stop();
    }
}
