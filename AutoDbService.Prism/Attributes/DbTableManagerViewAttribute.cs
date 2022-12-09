using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDbService.DbPrism.Attributes
{
    /// <summary>
    /// 标记本节点为InfoManger
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DbTableManagerViewAttribute : Attribute
    {
        public Type TableType { set; get; }
        public DbTableManagerViewAttribute(Type type)
        {
            TableType = type;
        }
    }
    /// <summary>
    /// 标记本节点为AddView
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DbTableAddViewAttribute : Attribute
    {
        public Type TableType { set; get; }
        public DbTableAddViewAttribute(Type type)
        {
            TableType = type;
        }
    }
    /// <summary>
    /// 标记本节点为ModifyView
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DbTableModifyViewAttribute : Attribute
    {
        public Type TableType { set; get; }
        public DbTableModifyViewAttribute(Type type)
        {
            TableType = type;
        }
    }
}
