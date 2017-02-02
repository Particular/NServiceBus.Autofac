namespace NServiceBus.ObjectBuilder.Autofac
{
    using global::Autofac;

    class ParentBuilder : ContainerBuilder
    {
        public ILifetimeScope ParentScope { get; }

        public ParentBuilder(ILifetimeScope parentScope)
        {
            this.ParentScope = parentScope;
        }
    }
}
