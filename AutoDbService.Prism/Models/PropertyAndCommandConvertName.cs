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
using AutoDbService.DbPrism.Interfaces;
using System.Reflection;

namespace AutoDbService.DbPrism.Models
{
    public  class PropertyAndCommandConvertName: IPropertyAndCommandConvertName
    {
        public const string FieldPreName = "my";
        public const string FieldLastName = "Command";

       public string GetPropertyCommandByMethod(string methodName)
        {
            if (string.IsNullOrEmpty(methodName))
                throw new ArgumentNullException();
            if (!methodName.EndsWith(FieldLastName))
            {
                return methodName + FieldLastName;
            }
            else
            {
                return methodName;
            }
        } 
        public string GetFieldCommandByMethod(string methodName)
        {
            var commandName = GetPropertyCommandByMethod(methodName);
            return GetFieldByProperty(commandName);
        }

        public string GetFieldByProperty(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException();
            if (propertyName.StartsWith(FieldPreName))
            {
                return propertyName;
            }
            else
            {
                return FieldPreName + propertyName;
            } 
        }

        public string GetMethodNameByFieldCommandName(string fieldName)
        {
            if (IsMyCommandField(fieldName))
            { 
                return fieldName.Substring(FieldPreName.Length,fieldName.Length- FieldPreName.Length- FieldLastName.Length);
            }
            throw new ArgumentNullException();
        }
        public bool IsMyCommandField(string fieldName)
        {
            return fieldName.EndsWith(FieldLastName) && fieldName.StartsWith(FieldPreName);
        }
    }
}
