# Repository

������ 
* [Entity Framework Core](#entity-framework-core) 

### Basic usage

ʹ��

```cs
//�������������������
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
//ע��ʹ��
AutoDbServiceEngine.Instance
                    .UsePrism() 
                    .Builder<MyContext>();
```

