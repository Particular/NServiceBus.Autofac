namespace NServiceBus.ObjectBuilder.Autofac
{
    using global::Autofac;

    class ParentBuilder : ContainerBuilder
    {
        public ILifetimeScope parentScope { get; }

        public ParentBuilder(ILifetimeScope parentScope)
        {
            this.parentScope = parentScope;
        }
    }
}
