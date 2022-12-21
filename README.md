# Repository

依赖项 
* [Entity Framework Core](#entity-framework-core) 

### Basic usage

使用

```cs
//定义的数据上下文内容
//AutoMapContext 默认开启dbset 搜索，默认搜索命名空间下扩展.Entities的类。搜索规则可以自订
//也可以显式指定dbset<TEntity>
    public class MyContext : AutoMapContext
    {
        private readonly string connection = ConfigurationManager.AppSettings["DBConnection"] ?? "Server=localhost;port=3306;userid=root;password=admin;persist security info=false;charset=utf8mb4;database=bluefluent;sslMode=none;AllowPublicKeyRetrieval=true;";
        public MyContext() : base()
        {
            //Trace.WriteLine("++++++++++++++++++++++++++++++++++++++++++");
        } 

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(connection);
        } 
    }
```
```cs
//注册使用
AutoDbServiceEngine.Instance 
                    .Builder<MyContext>();
```
```cs
//使用 DbService
AutoDbServiceEngine.Instance 
                    .Get<IDbService<TEntity>>();
```
```cs
//示例

namespace AutoDbService.Entity
{
  public class MyContext : AutoMapContext
    { 。。。。 }
}
namespace AutoDbService.Entity.Entities
{ 
    public class User: EntityBase
    { }
}

AutoDbServiceEngine.Instance 
                    .Builder<MyContext>();
//// IDbService可以采用自定或者默认创建
////自定可以替换注册
////AutoDbServiceEngine.Instance.ReplaceServiceValue<IDbService<User>>(YourDbService<User>,false);
IDbService<User> UserService= AutoDbServiceEngine.Instance 
                    .Get<IDbService<User>>();
////funExtends 默认采用第一级Include,当然也可以自己指定
await UserService.GetListFromDb(out int n,u=>u.Name!=null,u=>u.OrderBy(p=>p.CreateTime),u=>u.Teacher,new EnginePage{Skip=100，Take=20 });
```


