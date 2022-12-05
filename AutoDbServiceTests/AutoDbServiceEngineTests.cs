using Microsoft.VisualStudio.TestTools.UnitTesting;
using AutoDbService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoDbService.Interfaces;
using AutoDbService.Models;
using AutoDbService.Entity;
using AutoDbService.Dbs;

namespace AutoDbService.Tests
{
    [TestClass()]
    public class AutoDbServiceEngineTests
    {

        private static DbTableSearch _DbTableSearch;
        private static DbLinqInclude _DbLinqInclude;
        [TestInitialize]
        public void Init()
        {
            _DbTableSearch=new DbTableSearch();
            _DbLinqInclude = new DbLinqInclude();
        }
        [TestCleanup]
        public void Clear()
        {
            AutoDbServiceEngine.Instance.Dispose();
        }
        public static IEnumerable<object[]> AddTypeTest_GetData()
        {
            yield return new object[] { typeof(IDbTableSearch), typeof(DbTableSearch), null, false };
            yield return new object[] { typeof(IDbTableSearch), typeof(DbTableSearch), null, true };
            yield return new object[] { typeof(IDbTableSearch), typeof(DbTableSearch), _DbTableSearch, false };
            yield return new object[] { typeof(IDbTableSearch), typeof(DbTableSearch), _DbTableSearch, true };
            ////////////////////////////////////////
            yield return new object[] { typeof(IDbLinqInclude), typeof(DbTableSearch), null, false };
            yield return new object[] { typeof(IDbLinqInclude), typeof(DbTableSearch), null, true };
            yield return new object[] { typeof(IDbLinqInclude), typeof(DbTableSearch), _DbLinqInclude, false };
            yield return new object[] { typeof(IDbLinqInclude), typeof(DbTableSearch), _DbLinqInclude, true };
        } 

        [DataTestMethod()]
        [DynamicData(nameof(AddTypeTest_GetData), DynamicDataSourceType.Method)]
        public void AddTypeTest(Type key,Type targetType,object obj,bool inst=true)
        {
            AutoDbServiceEngine.Instance.AddType(key,targetType,obj,inst); 
            Assert.IsTrue(AutoDbServiceEngine.Instance.IsRegister(key));
            if(inst&&obj!=null)
            {
                Assert.IsTrue(AutoDbServiceEngine.Instance.HasInstance(key));
                Assert.AreEqual(AutoDbServiceEngine.Instance[key],obj);
            }
            else if(inst && obj == null)
            {
                Assert.IsTrue(AutoDbServiceEngine.Instance.HasInstance(key));
                Assert.AreNotEqual(AutoDbServiceEngine.Instance[key], obj);
            }
            else if (!inst && obj != null)
            {
                Assert.IsFalse(AutoDbServiceEngine.Instance.HasInstance(key));
                Assert.AreNotEqual(AutoDbServiceEngine.Instance[key], obj);
            }
            else if (!inst && obj == null)
            {
                Assert.IsFalse(AutoDbServiceEngine.Instance.HasInstance(key));
                Assert.AreNotEqual(AutoDbServiceEngine.Instance[key], obj);
            } 
        }

        [DataTestMethod()]
        [DynamicData(nameof(AddTypeTest_GetData), DynamicDataSourceType.Method)]
        public void RemoveTypeTest(Type key, Type targetType, object obj, bool inst = true)
        {
            AutoDbServiceEngine.Instance.AddType(key, targetType, obj, inst);
            Assert.IsTrue(AutoDbServiceEngine.Instance.IsRegister(key));
            AutoDbServiceEngine.Instance.RemoveType(key);
            Assert.IsFalse(AutoDbServiceEngine.Instance.IsRegister(key));
        }


        [DataTestMethod()]
        [DynamicData(nameof(AddTypeTest_GetData), DynamicDataSourceType.Method)]
        public void ReplaceServiceValueTest(Type key, Type targetType, object obj, bool inst = true)
        {
            AutoDbServiceEngine.Instance.Builder<MyContext>();
            Assert.IsTrue(AutoDbServiceEngine.Instance.IsRegister(key));
            Assert.IsNotNull(AutoDbServiceEngine.Instance[key]);
           var result= AutoDbServiceEngine.Instance.ReplaceServiceValue<IDbTableSearch>(obj as IDbTableSearch, inst); 
            if(inst&& obj!=null&& result)
            {
                Assert.AreEqual(AutoDbServiceEngine.Instance[key],obj);
            }  
        }

        [TestMethod()]
        public void SetDbSearchTypeTest()
        {
            AutoDbServiceEngine.Instance.SetDbSearchType<DbTableSearch>();
            Assert.IsTrue(AutoDbServiceEngine.Instance.IsRegister(typeof(IDbTableSearch)));
        }

        [TestMethod()]
        public void SetDbServiceTypeTest()
        {
            AutoDbServiceEngine.Instance.SetDbServiceType<DbService<object>>();
            Assert.IsTrue(AutoDbServiceEngine.Instance.IsRegister(typeof(IDbService<object>)));
        }

        [DataTestMethod()]
        [DataRow(typeof(IDbTableSearch))]
        [DataRow(typeof(IDbLinqInclude))]
        [DataRow(typeof(AutoMapContext))]
        public void BuilderTest(Type type)
        {
            Assert.IsNull(AutoDbServiceEngine.Instance[type]);
            Assert.IsFalse(AutoDbServiceEngine.Instance.IsRegister(type));

            AutoDbServiceEngine.Instance.Builder<MyContext>();

            Assert.IsTrue(AutoDbServiceEngine.Instance.IsRegister(type));
            Assert.IsNotNull(AutoDbServiceEngine.Instance[type]);

            AutoDbServiceEngine.Instance.Dispose();

            Assert.IsFalse(AutoDbServiceEngine.Instance.IsRegister(type));
            Assert.IsNull(AutoDbServiceEngine.Instance[type]);
        }

        [DataTestMethod()]
        [DataRow(typeof(IDbTableSearch))]
        [DataRow(typeof(IDbLinqInclude))]
        [DataRow(typeof(AutoMapContext))]
        public void GetTest(Type type)
        {
            Assert.IsNull(AutoDbServiceEngine.Instance[typeof(IDbTableSearch)]);
            AutoDbServiceEngine.Instance.Builder<MyContext>();
            Assert.IsNotNull(AutoDbServiceEngine.Instance.Get<IDbTableSearch>());
            Assert.AreEqual(AutoDbServiceEngine.Instance.Get<IDbTableSearch>(), AutoDbServiceEngine.Instance[typeof(IDbTableSearch)]);
            AutoDbServiceEngine.Instance.Dispose();
            Assert.IsNull(AutoDbServiceEngine.Instance.Get<IDbTableSearch>());
        }

        [TestMethod()]
        public void DisposeTest()
        {
            AutoDbServiceEngine.Instance.Builder<MyContext>();
            Assert.IsTrue(AutoDbServiceEngine.Instance.IsRegister(typeof(IDbTableSearch)));
            AutoDbServiceEngine.Instance.Dispose();
            Assert.IsFalse(AutoDbServiceEngine.Instance.IsRegister(typeof(IDbTableSearch)));
        }
    }
}