# Repository 
依赖项 
* [Prism.Unity](#Prism.Unity)  

```cs
//注册使用
protected override void OnStartup(StartupEventArgs e)
        {
            AutoDbServiceEngine.Instance
                    .UsePrism()///////////////扩展
                    .Builder<MyContext>();
            base.OnStartup(e);
           
        }  

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
            AutoDbServiceEngine.Instance.ReConfigureViewModelLocator(); 
        }
```

