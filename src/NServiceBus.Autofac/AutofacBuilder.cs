namespace NServiceBus
{
    using Container;
    using global::Autofac;
    using ObjectBuilder.Autofac;
    using Settings;

    /// <summary>
    /// Autofac Container
    /// </summary>
    public class AutofacBuilder : ContainerDefinition
    {
        /// <summary>
        ///     Implementers need to new up a new container.
        /// </summary>
        /// <param name="settings">The settings to check if an existing container exists.</param>
        /// <returns>The new container wrapper.</returns>
        public override ObjectBuilder.Common.IContainer CreateContainer(ReadOnlySettings settings)
        {
            ILifetimeScope existingLifetimeScope;

            if (settings.TryGet("ExistingLifetimeScope", out existingLifetimeScope))
            {
                return new AutofacObjectBuilder(existingLifetimeScope);
            }

            return new AutofacObjectBuilder();
        }
    }
}