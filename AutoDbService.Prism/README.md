# Repository
 
### Basic usage

Use

```cs
//ע��ʹ��
protected override void OnStartup(StartupEventArgs e)
        {
            AutoDbServiceEngine.Instance
                    .UsePrism()///////////////��չ
                    .Builder<MyContext>();
            base.OnStartup(e);
           
        }  

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
            AutoDbServiceEngine.Instance.ReConfigureViewModelLocator(); 
        }
```

