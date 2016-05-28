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

Next, in your startup file new it up and run it:


```c#
public void ConfigureServices(IServiceCollection services)
{
    ...some code here...
    
    var ioc = new PearIoc(services);

    ioc.WithStandardConvention();
    ioc.RunConfigurations();
}
```
