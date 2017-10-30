NetCore.Standalone
----------

[![NetCore.Standalone](https://img.shields.io/nuget/v/NetCore.Standalone.svg?maxAge=2592000)](https://www.nuget.org/packages/NetCore.Standalone/)


Getting Started
----------

![](http://www.plantuml.com/plantuml/svg/Y_PqB2Z8oKnEBCdCptCgpSn9IIsAvOe6iu2qStvUMcPwQL4ga8si4iWDBaaiAW7IGQKGNdv1B1Ug8fXn2hfs2d0J8JHO2Dbm11iiXMON2XUi06OxcWn82k8W40dXGvm6rQ405m40)




Blocking Application
----------


```charp

//Async Main requires C#7.1 to be configured in the project build options currently.
public static async Task Main(string[] args)
{
	await Builder.New()
		.UseStartup<ExampleStartup>(options => {
			options.UseArguments(args);
		})
		.UseInstallables()
		.UseInjectables()
		.UseAppServices()
		.Build()
		.RunAsync();
}


```

Non-Blocking Application
----------

```csharp

//Async Main requires C#7.1 to be configured in the project build options currently.
public static async Task Main(string[] args)
{
	var application = await Builder.New()
		.UseStartup<ExampleStartup>(options => {
			options.UseArguments(args);
		})
		.UseInstallables()
		.UseInjectables()
		.Build()
		.StartAsync();

	await SomethingThatBlocks();

	await application.StopAsync();
}


```


Configuration
----------



```charp

public static IApplicationBuilder UseCustom(this IApplicationBuilder applicationBuilder)
{
	
	Func<IApplicationBuilder, Task> onBuild = async (IApplicationBuilder builder) =>
	{
		//Setup objects to be used in configure,start,stop
		await Task.CompletedTask;
	};	
	Func<IServiceCollection, Task> onConfigure = async (IServiceCollection collection) =>
	{
		//Add services into the container before its build
		await Task.CompletedTask;
	};	
	Func<IServiceProvider, Task> onStart = async (IServiceProvider container) =>
	{
		//Configure what should be done when the application starts
		await Task.CompletedTask;
	};	
	Func<IServiceProvider, Task> onStop = async (IServiceProvider container) =>
	{
		//Configure what to do when the application stops
		await Task.CompletedTask;
	};

	var config = new ConfiguredOption
	{
		OnBuild = onBuild,
		OnConfigure = onConfigure,
		OnStart = onStart,
		OnStop = onStop
	};

	applicationBuilder.ConfiguredOptions.Add(config);

	return applicationBuilder;
}


```


Installables
----------

When UseInstallables method is invoked on your IApplicationBuilder any class visible to the executing assembly that inherits from IInstallable will automatically have their Install method invoked during the Configure step.


Injectables
----------

When UseInjectables method is called on the IApplicationBuilder any class visible to the executing assembly that inherits from IInjectable will automatically be registered to the container with the lifestyles based on the interface inherited.

- Singleton for ISingletonInjectable
- Transient for ITransientInjectable
- Scoped for IScopedInjectable

A good source of information on how lifestyles work can be found [here](https://github.com/castleproject/Windsor/blob/master/docs/lifestyles.md "Windsor/docs/lifestyles.md"):


AppServices
----------

When the UseAppServices is called on the IApplicationBuilder any class visible to the executing assembly that inherits from AppService automatically have be created, started and stopped with the application through the IAppServiceManager.
