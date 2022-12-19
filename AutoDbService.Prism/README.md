# Repository
 
### Basic usage

Use

```cs
//×¢²áÊ¹ÓÃ
protected override void OnStartup(StartupEventArgs e)
        {
            AutoDbServiceEngine.Instance
                    .UsePrism()///////////////À©Õ¹
                    .Builder<MyContext>();
            base.OnStartup(e);
           
        }  

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
            AutoDbServiceEngine.Instance.ReConfigureViewModelLocator(); 
        }
```

