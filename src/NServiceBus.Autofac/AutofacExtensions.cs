namespace NServiceBus
{
    using Container;
    using Autofac;

    /// <summary>
    /// Autofac extension to pass an existing Autofac container instance.
    /// </summary>
    [ObsoleteEx(
         Message = "Support for external dependency injection containers is no longer provided by NServiceBus adapters for each container library. Instead, the NServiceBus.Extensions.DependencyInjection library provides the ability to use any container that conforms to the Microsoft.Extensions.DependencyInjection container abstraction.",
         RemoveInVersion = "9.0.0",
         TreatAsErrorFromVersion = "8.0.0")]
    public static class AutofacExtensions
    {
        /// <summary>
        /// Use the a pre-configured AutoFac lifetime scope.
        /// </summary>
        /// <param name="customizations"></param>
        /// <param name="lifetimeScope">The existing lifetime scope to use.</param>
        public static void ExistingLifetimeScope(this ContainerCustomizations customizations, ILifetimeScope lifetimeScope)
        {
            customizations.Settings.Set<AutofacBuilder.LifetimeScopeHolder>(new AutofacBuilder.LifetimeScopeHolder(lifetimeScope));
        }
    }
}