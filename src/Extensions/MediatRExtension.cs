using System.Linq;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace PearIoc
{
    public static class MediatRExtension
    {
        public static PearIoc WithMediatR(this PearIoc pearIoc)
        {
            pearIoc.Services.AddTransient<IMediator>(x => new Mediator(x.GetService<SingleInstanceFactory>(), x.GetService<MultiInstanceFactory>()));
            pearIoc.Services.AddTransient<SingleInstanceFactory>(x => t => x.GetRequiredService(t));
            pearIoc.Services.AddTransient<MultiInstanceFactory>(x => t => x.GetServices(t));

            pearIoc.AddConfigurations(new MediatoRConfiguration(pearIoc.Services));
            return pearIoc;
        }
    }

    public class MediatoRConfiguration : PearIocConfiguration {
        public MediatoRConfiguration(IServiceCollection serviceCollection) : base(serviceCollection)
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

            foreach (var handlerType in _currentInterfaces.Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)))
            {
                _iocServices.AddTransient(handlerType.AsType(), _currentType.AsType());
            }
        }
    }
}
