namespace NServiceBus
{
    // An internal type referenced by the API approvals test as it can't reference obsoleted types.
    class AutofacInternalType
    {
    }

    /// <summary>
    /// Autofac Container
    /// </summary>
    [ObsoleteEx(
        Message = "Autofac is no longer supported via the NServiceBus.Autofac adapter. NServiceBus directly supports all containers compatible with Microsoft.Extensions.DependencyInjection.Abstractions via the externally managed container mode.",
        RemoveInVersion = "9.0.0",
        TreatAsErrorFromVersion = "8.0.0")]
    public class AutofacBuilder
    {
    }

    /// <summary>
    /// Autofac extension to pass an existing Autofac container instance.
    /// </summary>
    [ObsoleteEx(
        Message = "Autofac is no longer supported via the NServiceBus.Autofac adapter. NServiceBus directly supports all containers compatible with Microsoft.Extensions.DependencyInjection.Abstractions via the externally managed container mode.",
        RemoveInVersion = "9.0.0",
        TreatAsErrorFromVersion = "8.0.0")]
    public static class AutofacExtensions
    {
    }
}