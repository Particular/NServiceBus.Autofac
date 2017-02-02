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
        readonly ContainerBuilder builder;
        readonly HashSet<Type> registeredTypes;

        Lazy<ILifetimeScope> container;
        bool owned;
        bool isChild;

        public AutofacObjectBuilder(ContainerBuilder builder, bool owned, bool isChild)
        {
            this.owned = owned;
            this.isChild = isChild;
            this.builder = builder;

            this.container = new Lazy<ILifetimeScope>(() => builder.Build());

            registeredTypes = new HashSet<Type>();
        }

        public ILifetimeScope Container => container.Value;

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

            if (container.IsValueCreated)
            {
                container.Value.Dispose();
            }

            (builder as ParentBuilder)?.parentScope?.Dispose();
        }

        public Common.IContainer BuildChildContainer()
        {
            var childScope = Container.BeginLifetimeScope();

            return new AutofacObjectBuilder(childScope.CreateBuilderFromContainer(true), true, true);
        }

        public object Build(Type typeToBuild)
        {
            return Container.Resolve(typeToBuild);
        }

        public IEnumerable<object> BuildAll(Type typeToBuild)
        {
            return ResolveAll(Container, typeToBuild);
        }

        public void Configure(Type component, DependencyLifecycle dependencyLifecycle)
        {
            EnforceNotInChildContainer();

            if (HasComponent(component))
            {
                return;
            }

            var services = GetAllServices(component).ToArray();
            var registrationBuilder = builder.RegisterType(component).As(services).PropertiesAutowired();

            AddServicesToRegisteredTypes(services);
            SetLifetimeScope(dependencyLifecycle, registrationBuilder);
        }

        void AddServicesToRegisteredTypes(params Type[] services)
        {
            foreach (var svc in services)
            {
                registeredTypes.Add(svc);
            }
        }

        public void Configure<T>(Func<T> componentFactory, DependencyLifecycle dependencyLifecycle)
        {
            EnforceNotInChildContainer();

            if (HasComponent(typeof(T)))
            {
                return;
            }

            var services = GetAllServices(typeof(T)).ToArray();
            var registrationBuilder = builder.Register(c => componentFactory.Invoke()).As(services).PropertiesAutowired();

            AddServicesToRegisteredTypes(services);

            SetLifetimeScope(dependencyLifecycle, (IRegistrationBuilder<object, IConcreteActivatorData, SingleRegistrationStyle>) registrationBuilder);
        }

        public void RegisterSingleton(Type lookupType, object instance)
        {
            EnforceNotInChildContainer();

            AddServicesToRegisteredTypes(lookupType);
            builder.RegisterInstance(instance).As(lookupType).PropertiesAutowired();
        }

        public bool HasComponent(Type componentType)
        {
            return registeredTypes.Contains(componentType);
        }

        public void Release(object instance)
        {
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

        private void EnforceNotInChildContainer()
        {
            if (isChild)
            {
                throw new InvalidOperationException("Can't perform configurations on child containers");
            }
        }
    }
}