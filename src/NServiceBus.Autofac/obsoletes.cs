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
        Message = "NServiceBus.Autofac is no longer supported. Use the externally managed container mode to integrate third party dependency injection containers with NServiceBus instead.",
        RemoveInVersion = "9.0.0",
        TreatAsErrorFromVersion = "8.0.0")]
    public class AutofacBuilder
    {
    }

    /// <summary>
    /// Autofac extension to pass an existing Autofac container instance.
    /// </summary>
    [ObsoleteEx(
        Message = "NServiceBus.Autofac is no longer supported. Use the externally managed container mode to integrate third party dependency injection containers with NServiceBus instead.",
        RemoveInVersion = "9.0.0",
        TreatAsErrorFromVersion = "8.0.0")]
    public static class AutofacExtensions
    {
    }
}