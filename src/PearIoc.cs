using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Web.Services
{
    public abstract class PearIocConfiguration
    {
        protected readonly IServiceCollection _iocServices;
        protected TypeInfo _currentType;
        protected List<TypeInfo> _currentInterfaces;

        protected PearIocConfiguration(IServiceCollection iocServices)
        {
            _iocServices = iocServices;
            _currentInterfaces = new List<TypeInfo>();
        }

        protected abstract void CustomConfiguration();

        public void Execute(TypeInfo assembly)
        {
            _currentType = assembly;
            _currentInterfaces.Clear();
            _currentInterfaces.AddRange(_currentType.ImplementedInterfaces.Select(i => i.GetTypeInfo()));

            CustomConfiguration();         
        }
    }

    internal class StandardConvention : PearIocConfiguration
    {
        public StandardConvention(IServiceCollection iocServices) : base(iocServices)
        {
        }

        protected override void CustomConfiguration()
        {
            foreach (var interfaceType in this._currentInterfaces)
            {
                if (string.Equals(interfaceType.Name, ('i' + _currentType.Name),
                    StringComparison.CurrentCultureIgnoreCase))
                {
                    _iocServices.AddTransient(interfaceType.AsType(), _currentType.AsType());
                }
            }
        }
    }

    public class PearIoc
    {
        private readonly List<Assembly> _assemblies;
        private readonly List<PearIocConfiguration> _configurations;
        public IServiceCollection Services { get; set; }

        public PearIoc(IServiceCollection services)
        {
            Services = services;

            //By default we will check through the assembly that contains the startup class.
            //This should be your main project in your solution.
            _assemblies = new List<Assembly>
            {
                typeof(Startup).GetTypeInfo().Assembly
            };

            _configurations = new List<PearIocConfiguration>();
        }

        #region Convenience Methods
        public void AddTransient<TService, TImplementation>() where TService : class
            where TImplementation : class, TService
        {
            Services.AddTransient<TService, TImplementation>();
        }
        #endregion

        
        public void AddAssembly(params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                _assemblies.Add(assembly);
            }         
        }

        public void WithStandardConvention()
        {
            AddConfigurations(new StandardConvention(Services));
        }

        public void AddConfigurations(params PearIocConfiguration[] pearIocConfigurations)
        {
            foreach (var iocConfiguration in pearIocConfigurations)
            {
                _configurations.Add(iocConfiguration);
            }
        }

        public void RunConfigurations()
        {
            if(_assemblies.Count == 0) throw new ArgumentOutOfRangeException("No assemblies were added for the container to check through. Use AddAssembly().", new Exception(""));

            foreach (var assembly in _assemblies)
            {
                foreach (var type in assembly.ExportedTypes.Select(t => t.GetTypeInfo()).Where(t => t.IsClass && !t.IsAbstract))
                {
                    foreach (var iocConfiguration in _configurations)
                    {
                        iocConfiguration.Execute(type);
                    }
                }
            }
        }
    }
}
