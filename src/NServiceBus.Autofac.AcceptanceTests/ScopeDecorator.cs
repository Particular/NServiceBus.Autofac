namespace NServiceBus.Autofac.AcceptanceTests
{
    using System;
    using System.Collections.Generic;
    using global::Autofac;
    using global::Autofac.Core;
    using global::Autofac.Core.Lifetime;
    using global::Autofac.Core.Resolving;

    class ScopeDecorator : ILifetimeScope
    {
        public ScopeDecorator(ILifetimeScope decorated)
        {
            this.decorated = decorated;
        }

        public bool Disposed { get; private set; }

        public object ResolveComponent(IComponentRegistration registration, IEnumerable<Parameter> parameters)
        {
            return decorated.ResolveComponent(registration, parameters);
        }

        public IComponentRegistry ComponentRegistry => decorated.ComponentRegistry;

        public void Dispose()
        {
            decorated.Dispose();
            Disposed = true;
        }

        public ILifetimeScope BeginLifetimeScope()
        {
            return decorated.BeginLifetimeScope();
        }

        public ILifetimeScope BeginLifetimeScope(object tag)
        {
            return decorated.BeginLifetimeScope(tag);
        }

        public ILifetimeScope BeginLifetimeScope(Action<ContainerBuilder> configurationAction)
        {
            return decorated.BeginLifetimeScope(configurationAction);
        }

        public ILifetimeScope BeginLifetimeScope(object tag, Action<ContainerBuilder> configurationAction)
        {
            return decorated.BeginLifetimeScope(tag, configurationAction);
        }

        public IDisposer Disposer => decorated.Disposer;

        public object Tag => decorated.Tag;

        public event EventHandler<LifetimeScopeBeginningEventArgs> ChildLifetimeScopeBeginning
        {
            add { decorated.ChildLifetimeScopeBeginning += value; }
            remove { decorated.ChildLifetimeScopeBeginning -= value; }
        }

        public event EventHandler<LifetimeScopeEndingEventArgs> CurrentScopeEnding
        {
            add { decorated.CurrentScopeEnding += value; }
            remove { decorated.CurrentScopeEnding -= value; }
        }
        public event EventHandler<ResolveOperationBeginningEventArgs> ResolveOperationBeginning
        {
            add { decorated.ResolveOperationBeginning += value; }
            remove { decorated.ResolveOperationBeginning -= value; }
        }

        private readonly ILifetimeScope decorated;
    }
}