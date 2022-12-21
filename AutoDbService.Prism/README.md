# Repository 
依赖项 
* [Prism.Unity](#Prism.Unity)  

```cs  

///注册新增窗体
[DbTableAddView(typeof(User))]
public partial class AddUserView : UserControl{}
///注册修改窗体
[DbTableModifyView(typeof(User))]
public partial class ModifyUserView : UserControl{}
///注册管理窗体
[DbTableManagerView(typeof(User))]
public partial class UserView : UserControl{}

///子模块注册
///自动搜索模块内要注册的窗体，对于符合数据实体管理的View自动创建ViweModel
///优先使用自建的ViewModel
public class TestBModule : AutoDbModule{}


//注册使用
protected override void OnStartup(StartupEventArgs e)
        {
            AutoDbServiceEngine.Instance
                    .UsePrism()///////////////扩展
                    .Builder<MyContext>();
            base.OnStartup(e); 
        }  
        //重定向ViewModel生成
        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
            AutoDbServiceEngine.Instance.ReConfigureViewModelLocator(); 
        }
```

