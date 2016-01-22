using NServiceBus.ContainerTests;
using NServiceBus.ObjectBuilder.Autofac;
using NServiceBus.Settings;
using NUnit.Framework;

[SetUpFixture]
public class SetUpFixture
{
    [SetUp]
    public void Setup()
    {
        TestContainerBuilder.ConstructBuilder = () => new AutofacObjectBuilder(new SettingsHolder());
    }
}