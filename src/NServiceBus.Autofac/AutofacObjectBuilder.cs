namespace NServiceBus.ObjectBuilder.Autofac
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using global::Autofac;
    using global::Autofac.Builder;
    using global::Autofac.Core;

    class AutofacObjectBuilder : Common.IContainer
    {
        ILifetimeScope container;
        private bool owned;
        private bool isChild;

        AutofacObjectBuilder(ILifetimeScope container, bool owned, bool isChild)
        {
            this.isChild = isChild;
            this.owned = owned;
            this.container = container;
        }

        public AutofacObjectBuilder(ILifetimeScope container)
            : this(container, false, false)
        {
        }

        public AutofacObjectBuilder(ILifetimeScope container, bool owned)
            : this(container, owned, false)
        {
        }

        public AutofacObjectBuilder()
            : this(new ContainerBuilder().Build(), true, false)
        {
        }

        public void Dispose()
        {
            //Injected at compile time
        }

        void DisposeManaged()
        {
            if (!owned)
            {
                return;
            }
            container?.Dispose();
        }

        public Common.IContainer BuildChildContainer()
        {
            return new AutofacObjectBuilder(container.BeginLifetimeScope(), true, true);
        }

        public object Build(Type typeToBuild)
        {
            return container.Resolve(typeToBuild);
        }

        public IEnumerable<object> BuildAll(Type typeToBuild)
        {
            return ResolveAll(container, typeToBuild);
        }

        public void Configure(Type component, DependencyLifecycle dependencyLifecycle)
        {
            EnforceNotInChildContainer();

            var registration = GetComponentRegistration(component);

            if (registration != null)
            {
                return;
            }

            var builder = new ContainerBuilder();
            var services = GetAllServices(component).ToArray();
            var registrationBuilder = builder.RegisterType(component).As(services).PropertiesAutowired();

            SetLifetimeScope(dependencyLifecycle, registrationBuilder);

            builder.Update(container.ComponentRegistry);
        }

        public void Configure<T>(Func<T> componentFactory, DependencyLifecycle dependencyLifecycle)
        {
            EnforceNotInChildContainer();

            var registration = GetComponentRegistration(typeof(T));

            if (registration != null)
            {
                return;
            }

            var builder = new ContainerBuilder();
            var services = GetAllServices(typeof(T)).ToArray();
            var registrationBuilder = builder.Register(c => componentFactory.Invoke()).As(services).PropertiesAutowired();

            SetLifetimeScope(dependencyLifecycle, (IRegistrationBuilder<object, IConcreteActivatorData, SingleRegistrationStyle>) registrationBuilder);

            builder.Update(container.ComponentRegistry);
        }

        public void RegisterSingleton(Type lookupType, object instance)
        {
            EnforceNotInChildContainer();

            var builder = new ContainerBuilder();
            builder.RegisterInstance(instance).As(lookupType).PropertiesAutowired();
            builder.Update(container.ComponentRegistry);
        }

        public bool HasComponent(Type componentType)
        {
            return container.IsRegistered(componentType);
        }

        public void Release(object instance)
        {
        }

        static void SetPropertyValue(object instance, string propertyName, object value)
        {
            instance.GetType()
                .GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance)
                .SetValue(instance, value, null);
        }

        static void SetLifetimeScope(DependencyLifecycle dependencyLifecycle, IRegistrationBuilder<object, IConcreteActivatorData, SingleRegistrationStyle> registrationBuilder)
        {
            switch (dependencyLifecycle)
            {
                case DependencyLifecycle.InstancePerCall:
                    registrationBuilder.InstancePerDependency();
                    break;
                case DependencyLifecycle.SingleInstance:
                    registrationBuilder.SingleInstance();
                    break;
                case DependencyLifecycle.InstancePerUnitOfWork:
                    registrationBuilder.InstancePerLifetimeScope();
                    break;
                default:
                    throw new ArgumentException("Unhandled lifecycle - " + dependencyLifecycle);
            }
        }

        IComponentRegistration GetComponentRegistration(Type concreteComponent)
        {
            return container.ComponentRegistry.Registrations.FirstOrDefault(x => x.Activator.LimitType == concreteComponent);
        }

        private void EnforceNotInChildContainer()
        {
            if (isChild)
            {
                throw new InvalidOperationException("Can't perform configurations on child containers");
            }
        }

        static IEnumerable<Type> GetAllServices(Type type)
        {
            if (type == null)
            {
                return new List<Type>();
            }

            var result = new List<Type>(type.GetInterfaces())
                         {
                             type
                         };

            foreach (var interfaceType in type.GetInterfaces())
            {
                result.AddRange(GetAllServices(interfaceType));
            }

            return result.Distinct();
        }

        static IEnumerable<object> ResolveAll(IComponentContext container, Type componentType)
        {
            return container.Resolve(typeof(IEnumerable<>).MakeGenericType(componentType)) as IEnumerable<object>;
        }
    }
}