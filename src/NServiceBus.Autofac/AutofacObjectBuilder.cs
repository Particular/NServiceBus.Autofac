namespace NServiceBus.ObjectBuilder.Autofac
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using global::Autofac;
    using global::Autofac.Builder;
    using global::Autofac.Core;

    class ParentBuilder : ContainerBuilder
    {
        public ILifetimeScope parentScope;

        public ParentBuilder(ILifetimeScope parentScope)
        {
            this.parentScope = parentScope;
        }
    }

    class AutofacObjectBuilder : Common.IContainer
    {
        ContainerBuilder builder;
        ILifetimeScope container;
        bool owned;
        bool isChild;

        AutofacObjectBuilder(ContainerBuilder builder, bool owned, bool isChild)
        {
            this.owned = owned;
            this.isChild = isChild;
            this.builder = builder;
        }

        public AutofacObjectBuilder(ILifetimeScope container, bool owned)
            : this(container.CreateBuilderFromContainer(false), owned, false)
        {
        }

        public AutofacObjectBuilder(ILifetimeScope container)
            : this(container.CreateBuilderFromContainer(false), false, false)
        {
        }

        public AutofacObjectBuilder()
            : this(new ContainerBuilder(), true, false)
        {
        }

        public ILifetimeScope Container
        {
            get
            {
                container = container ?? builder.Build();
                return container;
            }
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
            
            if (!isChild)
            {
                (builder as ParentBuilder)?.parentScope?.Dispose();
            }
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

            EnsureInBuildMode();

            var services = GetAllServices(component).ToArray();
            var registrationBuilder = builder.RegisterType(component).As(services).PropertiesAutowired();

            SetLifetimeScope(dependencyLifecycle, registrationBuilder);
        }

        public void Configure<T>(Func<T> componentFactory, DependencyLifecycle dependencyLifecycle)
        {
            EnforceNotInChildContainer();

            if (HasComponent(typeof(T)))
            {
                return;
            }

            EnsureInBuildMode();

            var services = GetAllServices(typeof(T)).ToArray();
            var registrationBuilder = builder.Register(c => componentFactory.Invoke()).As(services).PropertiesAutowired();

            SetLifetimeScope(dependencyLifecycle, (IRegistrationBuilder<object, IConcreteActivatorData, SingleRegistrationStyle>) registrationBuilder);
        }

        public void RegisterSingleton(Type lookupType, object instance)
        {
            EnforceNotInChildContainer();

            EnsureInBuildMode();

            builder.RegisterInstance(instance).As(lookupType).PropertiesAutowired();
        }

        public bool HasComponent(Type componentType)
        {
            // This is a problem as we can't check a builder's registrations
            // so we are forced to build a container to check if a type has
            // been registered.
            return Container.IsRegistered(componentType);
        }

        public void Release(object instance)
        {
        }

        void EnsureInBuildMode()
        {
            if (IsBuilt)
            {
                builder = container.CreateBuilderFromContainer(isChild);
                container = null;
            }
        }

        bool IsBuilt
        {
            get { return container != null; }
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
            return Container.ComponentRegistry.Registrations.FirstOrDefault(x => x.Activator.LimitType == concreteComponent);
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