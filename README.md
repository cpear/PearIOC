# PearIOC
An IOC helper for Asp.Net Core RC2
PearIoc looks through all of the types in your assembly and maps any that are implementing an interface using the same name convention prefixed with "I" to the built in Depencency Injection container that comes out of the box with the new Asp.Net Core projects.
## Features
- Adds basic IOC mapping convention (For IFruit use Fruit) for transient types.
- Maps types from your main project assembly be default. Provides an API for adding additional assemblies.
- Provides mechanism for adding custom mapping configurations
- Exposes the services collection incase you need/want to do something weird (not really a feature sinve you have this in the startup.cs anyway.) :-)

## How to use it
First downlaod the PearIoc.cs file and add it to your project.

Next, in the ConfigureServices() method of your startup.cs file new it up and run it:


```c#
public void ConfigureServices(IServiceCollection services)
{
    ...some code here...
    
    var ioc = new PearIoc(services);

    ioc.WithStandardConvention();
    ioc.RunConfigurations();
}
```

## Custom Configuration
To add a custom configuration you will need to create a class and implement the abstract class PearIocConfiguration.
The PearIocConfiguration exposes the current type being examined from the current assembly and its interfaces. It also exposes the services collection.

```c#
public class MyConfiguration : PearIocConfiguration {
        public MyConfiguration(IServiceCollection serviceCollection) : base(serviceCollection)
        {
        }

        protected override void CustomConfiguration()
        {
            foreach (var handlerType in _currentInterfaces.Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAsyncRequestHandler<,>)))
            {
                _iocServices.AddTransient(handlerType.AsType(), _currentType.AsType());
            }

            foreach (var handlerType in _currentInterfaces.Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(INotificationHandler<>)))
            {
                _iocServices.AddTransient(handlerType.AsType(), _currentType.AsType());
            }

        }
    }
```

Then you can add the custom configuration to PearIoc in your startup.cs file:

```c#
public void ConfigureServices(IServiceCollection services)
{
    ...some code here...
    
    var ioc = new PearIoc(services);

    ioc.WithStandardConvention();
    ioc.AddConfigurations(new MyConfiguration(services));
    ioc.RunConfigurations();
}
```
