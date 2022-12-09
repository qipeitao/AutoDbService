using AutoDbService.Interfaces;
using AutoDbService.Models;
using AutoDbService.DbPrism.Extends;
using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Reflection;

namespace AutoDbService.DbPrism.Interfaces
{
    /// <summary>
    /// VM自动创建时，重新创建绑定关系时 属性名称转换关系 
    /// </summary>
    public interface IPropertyAndCommandConvertName
    {
        /// <summary>
        /// 根据方法获取命令
        /// 默认方法名+Command
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        string GetPropertyCommandByMethod(string methodName);
        /// <summary>
        /// 根据方法名获取字段
        /// 默认my+方法名+Command
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        string GetFieldCommandByMethod(string methodName);
        /// <summary>
        /// 根据字段名获取方法名
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        string GetMethodNameByFieldCommandName(string fieldName);
        /// <summary>
        /// 根据属性名获取字段名
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        string GetFieldByProperty(string propertyName);
        bool IsMyCommandField(string fieldName);
    }
}
