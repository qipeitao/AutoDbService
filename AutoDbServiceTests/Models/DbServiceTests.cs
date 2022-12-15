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
        [TestMethod()]
        public void GetListFromDbTest()
        { 
            Assert.IsNotNull(AutoDbServiceEngine.Instance[typeof(IDbService<User>)]);
            Assert.IsNotNull(AutoDbServiceEngine.Instance.Get<IDbService<User>>()); 
            var result = AutoDbServiceEngine.Instance.Get<IDbService<User>>().GetListFromDb(out int n);
            Assert.IsNotNull(result);
            Assert.IsTrue(n!=0); 
        }
        public static IEnumerable<object[]> AddTypeTest_GetListData()
        {
            yield return new object[] { typeof(IDbService<User>), new User {Id=Guid.NewGuid(), Name="Test"} };
            yield return new object[] { typeof(IDbService<User>), null };
            yield return new object[] { typeof(IDbService<UserLog>), new UserLog { Id = Guid.NewGuid(),Remark="Test" } };
            yield return new object[] { typeof(IDbService<UserLog>), null };
            yield return new object[] { typeof(IDbService<Teacher>), new Teacher { Id = Guid.NewGuid(), Name = "Test" } };
            yield return new object[] { typeof(IDbService<Teacher>), null };
        }
        [DataTestMethod()]
        [DynamicData(nameof(AddTypeTest_GetListData), DynamicDataSourceType.Method)]  
        public void AddTest(Type type,object obj)
        {
            Assert.IsNotNull(AutoDbServiceEngine.Instance[type]);
            Assert.IsTrue(AutoDbServiceEngine.Instance.HasInstance(type));
            var result= type.GetMethod("Add").Invoke(AutoDbServiceEngine.Instance[type], new object[] { obj });
            if(obj==null)
            {
                Assert.IsFalse((bool)result);
                return;
            }
            Assert.IsTrue((bool)result);
            result = type.GetMethod("Remove").Invoke(AutoDbServiceEngine.Instance[type], new object[] { obj });
            Assert.IsTrue((bool)result);
        }

        [DataTestMethod()]
        [DynamicData(nameof(AddTypeTest_GetListData), DynamicDataSourceType.Method)]
        public void RemoveTest(Type type, object obj)
        {
            Assert.IsNotNull(AutoDbServiceEngine.Instance[type]);
            Assert.IsTrue(AutoDbServiceEngine.Instance.HasInstance(type));
            var result = type.GetMethod("Add").Invoke(AutoDbServiceEngine.Instance[type], new object[] { obj });
            if (obj == null)
            {
                Assert.IsFalse((bool)result);
                return;
            }
            Assert.IsTrue((bool)result);
            result = type.GetMethod("Remove").Invoke(AutoDbServiceEngine.Instance[type], new object[] { obj });
            Assert.IsTrue((bool)result);
        }

        public static IEnumerable<object[]> AddTypeTest_GetUpdateData()
        {
            yield return new object[] { typeof(IDbService<User>), new User { Id = Guid.NewGuid(), Name = "Test" },
                new Func<object, object>(entity => ((User)entity).Name="New"), (Expression<Func<User, bool>>)(entity => entity.Name == "Test") };
            yield return new object[] { typeof(IDbService<UserLog>), new UserLog { Id = Guid.NewGuid(), Remark = "Test" },
                new Func<object, object>(entity => ((UserLog)entity).Remark = "New"),(Expression<Func<UserLog, bool>>)(entity => entity.Remark == "Test") };
            yield return new object[] { typeof(IDbService<Teacher>), new Teacher { Id = Guid.NewGuid(), Name = "Test" },
                new Func<object, object>(entity => ((Teacher)entity).Name = "New"),(Expression<Func<Teacher, bool>>)(entity => entity.Name == "Test") };
        }
        [DataTestMethod()]
        [DynamicData(nameof(AddTypeTest_GetUpdateData), DynamicDataSourceType.Method)]
        public void SaveTest(Type type, object optEntity, Func<object, object> update, object filter)
        {
            Assert.IsNotNull(AutoDbServiceEngine.Instance[type]);
            Assert.IsTrue(AutoDbServiceEngine.Instance.HasInstance(type));
            var entity = type.GetMethod("GetBaseOneFromDb").Invoke(AutoDbServiceEngine.Instance[type], new object[] {filter });
            Assert.IsNull(entity);
            type.GetMethod("Add").Invoke(AutoDbServiceEngine.Instance[type], new object[] { optEntity });
            entity = type.GetMethod("GetBaseOneFromDb").Invoke(AutoDbServiceEngine.Instance[type], new object[] { filter });
            Assert.IsNotNull(entity);
            update(entity);
            type.GetMethod("Save").Invoke(AutoDbServiceEngine.Instance[type], new object[] { entity });
            entity = type.GetMethod("GetBaseOneFromDb").Invoke(AutoDbServiceEngine.Instance[type], new object[] { filter });
            Assert.IsNull(entity);
            type.GetMethod("Remove").Invoke(AutoDbServiceEngine.Instance[type], new object[] { entity });

        }
        public static IEnumerable<object[]> AddTypeTest_GetFillData()
        {
            yield return new object[] { typeof(IDbService<User>), (Expression<Func<User, bool>>)(entity => entity.Id != Guid.Empty), new Func<object, bool>(entity => ((User)entity).CreateTeacher !=null) };
            yield return new object[] { typeof(IDbService<UserLog>), (Expression<Func<UserLog, bool>>)(entity => entity.Id != Guid.Empty), new Func<object, bool>(entity => ((UserLog)entity).CreateUser != null) };
            yield return new object[] { typeof(IDbService<Teacher>), (Expression<Func<Teacher, bool>>)(entity => entity.Id != Guid.Empty), new Func<object, bool>(entity => ((Teacher)entity).Users != null) };
        }
        [DataTestMethod()]
        [DynamicData(nameof(AddTypeTest_GetFillData), DynamicDataSourceType.Method)] 
        public void FillDetailTest(Type type, object filter, Func<object, bool> resultB)
        {
            Assert.IsNotNull(AutoDbServiceEngine.Instance[type]);
            Assert.IsTrue(AutoDbServiceEngine.Instance.HasInstance(type));
            var task = type.GetMethod("FillDetail").Invoke(AutoDbServiceEngine.Instance[type], new object[] { filter,null }) as Task;
            var result = task.GetType().GetProperty("Result").GetValue(task);
            Assert.IsTrue(resultB.Invoke(result)); 
        }
    }
}