namespace NServiceBus.Autofac.Tests
{
    using System;
    using global::Autofac;
    using NUnit.Framework;
    using ObjectBuilder.Autofac;

    class ILifetimeScopeExtensionTests
    {
        [Test]
        public void Cloning_A_Container_Resolves_The_Same_Singleton_Instance()
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(new TestClass());
            var container = builder.Build();

            var clonedBuilder = container.CreateBuilderFromContainer(false);
            var clonedContainer = clonedBuilder.Build();

            Assert.AreSame(container.Resolve<TestClass>(), clonedContainer.Resolve<TestClass>());
        }

        [Test]
        public void Cloning_A_ChildContainer_Resolves_The_Same_Singleton_Instance()
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(new TestClass());
            var container = builder.Build();

            var clonedBuilder = container.BeginLifetimeScope().CreateBuilderFromContainer(true);
            var clonedContainer = clonedBuilder.Build();

            Assert.AreSame(container.Resolve<TestClass>(), clonedContainer.Resolve<TestClass>());
        }

        [Test]
        public void Cloning_A_Container_And_Changing_The_Builder_Does_Not_Affect_Original_Container()
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(new TestClass());
            var container = builder.Build();

            var clonedBuilder = container.CreateBuilderFromContainer(true);
            clonedBuilder.RegisterInstance(new TestClass());
            var clonedContainer = clonedBuilder.Build();

            Assert.AreNotSame(container.Resolve<TestClass>(), clonedContainer.Resolve<TestClass>());
        }

        [Test]
        public void Cloning_A_Container_And_Changing_The_Original_Container_Does_Not_Affect_Cloned_Container()
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(new TestClass());
            var container = builder.Build();

            var clonedBuilder = container.CreateBuilderFromContainer(true);
            var clonedContainer = clonedBuilder.Build();

            var newBuilder = new ContainerBuilder();
            newBuilder.RegisterInstance(new TestClass());

#pragma warning disable 612, 618
            newBuilder.Update(container.ComponentRegistry);
#pragma warning restore 612, 618

            Assert.AreNotSame(container.Resolve<TestClass>(), clonedContainer.Resolve<TestClass>());
        }
    }

    class TestClass
    {
        public Guid Id { get; } = Guid.NewGuid();
    }
}