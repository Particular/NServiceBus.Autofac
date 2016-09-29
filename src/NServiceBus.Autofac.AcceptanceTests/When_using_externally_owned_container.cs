namespace NServiceBus.AcceptanceTests
{
    using global::Autofac;
    using NServiceBus.AcceptanceTesting;
    using NServiceBus.AcceptanceTests.EndpointTemplates;
    using NServiceBus.Autofac.AcceptanceTests;
    using NUnit.Framework;

    public class When_using_externally_owned_container : NServiceBusAcceptanceTest
    {
        [Test]
        public void Should_shutdown_properly()
        {
            Scenario.Define<Context>()
                .WithEndpoint<Endpoint>()
                .Done(c => c.EndpointsStarted)
                .Run();

            Assert.IsFalse(Endpoint.Context.Decorator.Disposed);
            Assert.DoesNotThrow(() => Endpoint.Context.Scope.Dispose());
        }

        class Context : ScenarioContext
        {
            public ScopeDecorator Decorator { get; set; }
            public ILifetimeScope Scope { get; set; }
        }

        class Endpoint : EndpointConfigurationBuilder
        {
            public static Context Context { get; set; }
            public Endpoint()
            {
                EndpointSetup<DefaultServer>(config =>
                {
                    Context = new Context();
                    
                    var container = new ContainerBuilder().Build();
                    var scopeDecorator = new ScopeDecorator(container);

                    Context.Decorator = scopeDecorator;
                    Context.Scope = container;

                    config.UseContainer<AutofacBuilder>(c => c.ExistingLifetimeScope(scopeDecorator));
                });
            }
        }
    }
}