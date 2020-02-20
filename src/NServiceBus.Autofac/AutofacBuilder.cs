namespace NServiceBus
{
    using Container;
    using Autofac;
    using ObjectBuilder.Autofac;
    using Settings;

    /// <summary>
    /// Autofac Container
    /// </summary>
    [ObsoleteEx(
          Message = "Support for external dependency injection containers is no longer provided by NServiceBus adapters for each container library. Instead, the NServiceBus.Extensions.DependencyInjection library provides the ability to use any container that conforms to the Microsoft.Extensions.DependencyInjection container abstraction.",
          RemoveInVersion = "9.0.0",
          TreatAsErrorFromVersion = "8.0.0")]
    public class AutofacBuilder : ContainerDefinition
    {
        /// <summary>
        ///     Implementers need to new up a new container.
        /// </summary>
        /// <param name="settings">The settings to check if an existing container exists.</param>
        /// <returns>The new container wrapper.</returns>
        public override ObjectBuilder.Common.IContainer CreateContainer(ReadOnlySettings settings)
        {
            LifetimeScopeHolder scopeHolder;

            if (settings.TryGet(out scopeHolder))
            {
                settings.AddStartupDiagnosticsSection("NServiceBus.Autofac", new
                {
                    UsingExistingLifetimeScope = true
                });

                return new AutofacObjectBuilder(scopeHolder.ExistingLifetimeScope);
            }

            settings.AddStartupDiagnosticsSection("NServiceBus.Autofac", new
            {
                UsingExistingLifetimeScope = false
            });

            return new AutofacObjectBuilder();
        }

        internal class LifetimeScopeHolder
        {
            public LifetimeScopeHolder(ILifetimeScope lifetimeScope)
            {
                ExistingLifetimeScope = lifetimeScope;
            }

            public ILifetimeScope ExistingLifetimeScope { get; }
        }
    }
}