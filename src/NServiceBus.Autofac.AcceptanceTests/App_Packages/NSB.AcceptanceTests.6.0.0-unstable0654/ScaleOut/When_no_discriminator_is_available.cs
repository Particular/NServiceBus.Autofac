﻿namespace NServiceBus.AcceptanceTests.ScaleOut
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NServiceBus.AcceptanceTesting;
    using NServiceBus.AcceptanceTests.EndpointTemplates;
    using NServiceBus.ConsistencyGuarantees;
    using NServiceBus.Features;
    using NServiceBus.ObjectBuilder;
    using NServiceBus.Transports;
    using NUnit.Framework;

    public class When_no_discriminator_is_available : NServiceBusAcceptanceTest
    {
        [Test]
        public void Should_blow_up()
        {
            var ex = Assert.Throws<AggregateException>(()=> Scenario.Define<Context>()
                    .WithEndpoint<IndividualizedEndpoint>().Done(c =>c.EndpointsStarted)
                    .AllowExceptions()
                    .Run());

            var configEx = ex.InnerExceptions.First()
                .InnerException;

            Assert.True(configEx.Message.StartsWith("No endpoint instance discriminator found"));

        }

        public class Context : ScenarioContext
        {
        }

        public class IndividualizedEndpoint : EndpointConfigurationBuilder
        {
       
            public IndividualizedEndpoint()
            {
                EndpointSetup<DefaultServer>(c =>
                {
                    c.ScaleOut().UniqueQueuePerEndpointInstance();
                    c.UseTransport<TransportThatDoesNotSetADefaultDiscriminator>();
                });
            }
        }

        public class TransportThatDoesNotSetADefaultDiscriminator:TransportDefinition
        {
            protected override void Configure(BusConfiguration config)
            {
                config.EnableFeature<TransportThatDoesNotSetADefaultDiscriminatorConfigurator>();
            }

            public override string GetSubScope(string address, string qualifier)
            {
                return address + "." + qualifier;
            }

            public override IEnumerable<Type> GetSupportedDeliveryConstraints()
            {
                return new List<Type>();
            }

            public override ConsistencyGuarantee GetDefaultConsistencyGuarantee()
            {
                return new AtomicWithReceiveOperation();
            }

            public override IManageSubscriptions GetSubscriptionManager()
            {
                throw new NotImplementedException();
            }
        }

            public class TransportThatDoesNotSetADefaultDiscriminatorConfigurator : ConfigureTransport
            {
                protected override Func<IBuilder, ReceiveBehavior> GetReceiveBehaviorFactory(ReceiveOptions settings)
                {
                    throw new NotImplementedException();
                }

                protected override void Configure(FeatureConfigurationContext context, string connectionString)
                {
                    
                }

                protected override string ExampleConnectionStringForErrorMessage
                {
                    get { return ""; }
                }
            }
    }

    
}