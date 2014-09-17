using NServiceBus.ContainerTests;
using NServiceBus.ObjectBuilder.Autofac;
using NUnit.Framework;

[SetUpFixture]
public class SetUpFixture
{
    [SetUp]
    public void Setup()
    {
        TestContainerBuilder.ConstructBuilder = () => new AutofacObjectBuilder();
    }

}