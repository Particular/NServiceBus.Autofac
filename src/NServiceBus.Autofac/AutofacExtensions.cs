namespace NServiceBus
{
    using Container;
    using Autofac;

    /// <summary>
    /// Autofac extension to pass an existing Autofac container instance.
    /// </summary>
    [ObsoleteEx(
         Message = "Support for Autofac is provided via the NServiceBus.Extensions.DependencyInjection package.",
         RemoveInVersion = "8.0.0",
         TreatAsErrorFromVersion = "7.0.0")]
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