using NServiceBus.ContainerTests;
using NServiceBus.ObjectBuilder.Autofac;
using NUnit.Framework;

[SetUpFixture]
public class SetUpFixture
{
    public SetUpFixture()
    {
        TestContainerBuilder.ConstructBuilder = () => new AutofacObjectBuilder(new Autofac.ContainerBuilder(), true, false);
    }
}