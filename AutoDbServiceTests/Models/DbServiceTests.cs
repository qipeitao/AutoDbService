using Microsoft.VisualStudio.TestTools.UnitTesting;
using AutoDbService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoDbService.Dbs;
using AutoDbService.Interfaces;
using AutoDbService.Entity.Entities;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using AutoDbService.Entity;
using Microsoft.EntityFrameworkCore;

namespace AutoDbService.Models.Tests
{
    [TestClass()]
    public class DbServiceTests
    {
        
        [TestInitialize]
        public void Initialize()
        {
            AutoDbServiceEngine.Instance.Builder<MyContext>(); 
        }

        [TestCleanup]
        public void Cleanup()
        {
            AutoDbServiceEngine.Instance.Dispose();
        }
        public static IEnumerable<object[]> AddTypeTest_GetNotNullData()
        { 
            yield return new object[] { typeof(IDbService<User>), (Expression<Func<User, bool>>)(entity => entity.Id!=Guid.Empty) };
            yield return new object[] { typeof(IDbService<UserLog>), (Expression<Func<UserLog, bool>>)(entity => entity.Id != Guid.Empty) };
            yield return new object[] { typeof(IDbService<Teacher>), (Expression<Func<Teacher, bool>>)(entity => entity.Id != Guid.Empty) };
        }
        [DataTestMethod()]
        [DynamicData(nameof(AddTypeTest_GetNotNullData), DynamicDataSourceType.Method)]
        public void GetBaseOneFromDbTest(Type type, object filter)
        { 
            Assert.IsTrue(AutoDbServiceEngine.Instance.IsRegister(type));
            Assert.IsTrue(AutoDbServiceEngine.Instance.HasInstance(type));
            Assert.IsNotNull(AutoDbServiceEngine.Instance[type]);
           var result= AutoDbServiceEngine.Instance[type].GetType()
                .GetMethod(nameof(IDbService<EntityBase>.GetBaseOneFromDb))
                .Invoke(AutoDbServiceEngine.Instance[type], new object[] { filter });
            Assert.IsNotNull(result);
            Assert.IsTrue(result is EntityBase);
        }
        public static IEnumerable<object[]> AddTypeTest_GetListData()
        {
            yield return new object[] { typeof(IDbService<User>), (Expression<Func<User, bool>>)(entity => true) };
            yield return new object[] { typeof(IDbService<UserLog>), (Expression<Func<UserLog, bool>>)(entity => true) };
            yield return new object[] { typeof(IDbService<Teacher>), (Expression<Func<Teacher, bool>>)(entity => true) };
        }
        [DataTestMethod()]
        [DynamicData(nameof(AddTypeTest_GetNotNullData), DynamicDataSourceType.Method)]
        public void GetListFromDbTest(Type type, object filter)
        { 
            Assert.IsNotNull(AutoDbServiceEngine.Instance[type]);
            var parms = new object[] { 0, filter, null, null };
            var result = AutoDbServiceEngine.Instance[type].GetType()
                 .GetMethod(nameof(IDbService<EntityBase>.GetListFromDb))
                 .Invoke(AutoDbServiceEngine.Instance[type], parms);
            Assert.IsNotNull(result);
            Assert.IsTrue(result is EntityBase);
        }

        [TestMethod()]
        public void AddTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RemoveTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SaveTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void FillDetailTest()
        {
            Assert.Fail();
        }
    }
}