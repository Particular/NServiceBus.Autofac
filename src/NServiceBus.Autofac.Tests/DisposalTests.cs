namespace NServiceBus.Autofac.Tests
{
    using System;
    using System.Collections.Generic;
    using global::Autofac;
    using global::Autofac.Core;
    using global::Autofac.Core.Lifetime;
    using global::Autofac.Core.Resolving;
    using ObjectBuilder.Autofac;
    using NUnit.Framework;

    [TestFixture]
    public class DisposalTests
    {
        [Test]
        public void Owned_container_should_be_disposed()
        {
            var fakeScope = new FakeLifetimeScope();

            var container = new AutofacObjectBuilder(fakeScope.CreateBuilderFromContainer(false), true, false);
            container.Dispose();

            Assert.True(fakeScope.Disposed);
        }

        [Test]
        public void Externally_owned_container_should_not_be_disposed()
        {
            var fakeScope = new FakeLifetimeScope();

            var container = new AutofacObjectBuilder(fakeScope.CreateBuilderFromContainer(false), false, false);
            container.Dispose();

            Assert.False(fakeScope.Disposed);
        }

        sealed class FakeLifetimeScope : ILifetimeScope
        {
            public bool Disposed { get; private set; }

            public object ResolveComponent(IComponentRegistration registration, IEnumerable<Parameter> parameters)
            {
                return null;
            }

            public IComponentRegistry ComponentRegistry { get; }
            public void Dispose()
            {
                Disposed = true;
            }

            public ILifetimeScope BeginLifetimeScope()
            {
                return null;
            }

            public ILifetimeScope BeginLifetimeScope(object tag)
            {
                return null;
            }

            public ILifetimeScope BeginLifetimeScope(Action<ContainerBuilder> configurationAction)
            {
                return null;
            }

            public ILifetimeScope BeginLifetimeScope(object tag, Action<ContainerBuilder> configurationAction)
            {
                return null;
            }

            public IDisposer Disposer { get; }
            public object Tag { get; }
            public event EventHandler<LifetimeScopeBeginningEventArgs> ChildLifetimeScopeBeginning;
            public event EventHandler<LifetimeScopeEndingEventArgs> CurrentScopeEnding;
            public event EventHandler<ResolveOperationBeginningEventArgs> ResolveOperationBeginning;

            void OnChildLifetimeScopeBeginning(LifetimeScopeBeginningEventArgs e)
            {
                ChildLifetimeScopeBeginning?.Invoke(this, e);
            }

            void OnCurrentScopeEnding(LifetimeScopeEndingEventArgs e)
            {
                CurrentScopeEnding?.Invoke(this, e);
            }

            void OnResolveOperationBeginning(ResolveOperationBeginningEventArgs e)
            {
                ResolveOperationBeginning?.Invoke(this, e);
            }
        }
    }
}