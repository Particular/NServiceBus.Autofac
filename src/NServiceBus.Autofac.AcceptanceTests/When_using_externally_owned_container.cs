namespace NServiceBus.Autofac.AcceptanceTests
{
    using System.Threading.Tasks;
    using global::Autofac;
    using AcceptanceTesting;
    using NServiceBus.AcceptanceTests;
    using NServiceBus.AcceptanceTests.EndpointTemplates;
    using NUnit.Framework;

    public class When_using_externally_owned_container : NServiceBusAcceptanceTest
    {
        [Test]
        public async Task Should_shutdown_properly()
        {
            var context = await Scenario.Define<Context>()
                .WithEndpoint<Endpoint>()
                .Done(c => c.EndpointsStarted)
                .Run();

            Assert.IsFalse(context.Decorator.Disposed);
            Assert.DoesNotThrow(() => context.Scope.Dispose());
        }

        class Context : ScenarioContext
        {
            public ScopeDecorator Decorator { get; set; }
            public ILifetimeScope Scope { get; set; }
        }

        class Endpoint : EndpointConfigurationBuilder
        {
            public Endpoint()
            {
                EndpointSetup<DefaultServer>((config, desc) =>
                {
                    var container = new ContainerBuilder().Build();
                    var scopeDecorator = new ScopeDecorator(container);
#pragma warning disable 0618
                    config.UseContainer<AutofacBuilder>(c => c.ExistingLifetimeScope(scopeDecorator));
#pragma warning restore 0618
                    config.SendFailedMessagesTo("error");

                    var context = (Context) desc.ScenarioContext;
                    context.Decorator = scopeDecorator;
                    context.Scope = container;
                });
            }
        }
    }
}