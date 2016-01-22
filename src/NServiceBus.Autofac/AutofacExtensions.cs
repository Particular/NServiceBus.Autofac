namespace NServiceBus
{
	using System;
	using Container;
    using global::Autofac;
	using NServiceBus.Settings;

	/// <summary>
    /// Autofac extension to pass an existing Autofac container instance.
    /// </summary>
    public static class AutofacExtensions
    {
        /// <summary>
        /// Use the a pre-configured AutoFac lifetime scope.
        /// </summary>
        /// <param name="customizations"></param>
        /// <param name="lifetimeScope">The existing lifetime scope to use.</param>
        public static void ExistingLifetimeScope(this ContainerCustomizations customizations, ILifetimeScope lifetimeScope)
        {
            customizations.Settings.Set("ExistingLifetimeScope", lifetimeScope);
        }

		/// <summary>
		/// Use the pre-configured Autofac lifetime scope.
		/// </summary>
		/// <param name="customizations"></param>
		/// <param name="factory">The factory that will retrieve the lifetime scope to use.</param>
		public static void ChildScopeFactory(this ContainerCustomizations customizations,
			Func<ReadOnlySettings, ILifetimeScope> factory)
		{
			customizations.Settings.Set("ChildScopeContainerFactory", factory);
		}
	}
}