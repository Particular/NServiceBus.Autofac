namespace NServiceBus.ObjectBuilder.Autofac
{
    using global::Autofac;
    using global::Autofac.Core;
    using global::Autofac.Core.Activators.ProvidedInstance;
    using System.Linq;

    static class ILifetimeScopeExtensionMethods
    {
        public static ContainerBuilder CreateBuilderFromContainer(this ILifetimeScope container, bool isChild)
        {
            var builder = isChild ? new ContainerBuilder() : new ParentBuilder(container);

            if (container?.ComponentRegistry?.Registrations.Any() ?? false)
            {
                foreach (var reg in container.ComponentRegistry.Registrations)
                {
                    if (reg.Activator is ProvidedInstanceActivator)
                    {
                        foreach (TypedService service in reg.Services)
                        {
                            var registration = builder.RegisterInstance(container.Resolve(service.ServiceType)).As(service.ServiceType).PropertiesAutowired();
                            if (isChild)
                            {
                                registration.ExternallyOwned();
                            }
                        }
                    }
                    else
                    {
                        builder.RegisterComponent(reg);
                    }
                }
            }

            return builder;
        }
    }
}