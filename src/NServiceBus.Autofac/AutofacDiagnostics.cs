namespace NServiceBus.Features
{
    /// <summary>
    /// Adds Diagnostics information
    /// </summary>
    public class AutofacDiagnostics : Feature
    {
        /// <summary>
        /// Constructor for AutoFac diagnostics feature
        /// </summary>
        public AutofacDiagnostics()
        {
            EnableByDefault();
        }

        /// <summary>
        /// Sets up NServiceBus.Autofac diagnostics
        /// </summary>
        protected override void Setup(FeatureConfigurationContext context)
        {
            context.Settings.AddStartupDiagnosticsSection("NServiceBus.Autofac", new
            {
                UsingExistingLifetimeScope = context.Settings.HasSetting<AutofacBuilder.LifetimeScopeHolder>()
            });
        }
    }
}
