﻿namespace NServiceBus.AcceptanceTests.Sagas
{
    using System;
    using EndpointTemplates;
    using AcceptanceTesting;
    using Features;
    using NServiceBus.AcceptanceTests.Routing;
    using NUnit.Framework;
    using Saga;
    using ScenarioDescriptors;

    //Repro for #1323
    public class When_started_by_event_from_another_saga : NServiceBusAcceptanceTest
    {
        [Test]
        public void Should_start_the_saga_and_request_a_timeout()
        {
            Scenario.Define<Context>()
                .WithEndpoint<SagaThatPublishesAnEvent>(b =>
                    b.When(c => c.IsEventSubscriptionReceived,
                            bus =>
                                bus.SendLocal(new StartSaga
                                {
                                    DataId = Guid.NewGuid()
                                }))
                )
                .WithEndpoint<SagaThatIsStartedByTheEvent>(
                    b => b.Given((bus, context) =>
                    {
                        bus.Subscribe<SomethingHappenedEvent>();

                        if (context.HasNativePubSubSupport)
                            context.IsEventSubscriptionReceived = true;
                    }))
                .Done(c => c.DidSaga1Complete && c.DidSaga2Complete)
                .Repeat(r => r.For(Transports.Default))
                .Should(c => Assert.True(c.DidSaga1Complete && c.DidSaga2Complete))
                .Run();
        }

        public class Context : ScenarioContext
        {
            public bool DidSaga1Complete { get; set; }
            public bool DidSaga2Complete { get; set; }
            public bool IsEventSubscriptionReceived { get; set; }
        }

        public class SagaThatPublishesAnEvent : EndpointConfigurationBuilder
        {
            public SagaThatPublishesAnEvent()
            {
                EndpointSetup<DefaultPublisher>(b =>
                {
                    b.EnableFeature<TimeoutManager>();
                    b.OnEndpointSubscribed<Context>((s, context) =>
                    {
                        context.IsEventSubscriptionReceived = true;
                    });
                });
            }

            public class EventFromOtherSaga1 : Saga<EventFromOtherSaga1.EventFromOtherSaga1Data>, IAmStartedByMessages<StartSaga>, IHandleTimeouts<EventFromOtherSaga1.Timeout1>
            {
                public Context Context { get; set; }

                public void Handle(StartSaga message)
                {
                    Data.DataId = message.DataId;

                    //Publish the event, which will start the second saga
                    Bus.Publish<SomethingHappenedEvent>(m => { m.DataId = message.DataId; });

                    //Request a timeout
                    RequestTimeout<Timeout1>(TimeSpan.FromSeconds(5));
                }

                public void Timeout(Timeout1 state)
                {
                    MarkAsComplete();
                    Context.DidSaga1Complete = true;
                }

                public class EventFromOtherSaga1Data : ContainSagaData
                {
                    public virtual Guid DataId { get; set; }
                }

                public class Timeout1
                {
                }

                protected override void ConfigureHowToFindSaga(SagaPropertyMapper<EventFromOtherSaga1Data> mapper)
                {
                    mapper.ConfigureMapping<StartSaga>(m => m.DataId).ToSaga(s => s.DataId);
                }
            }
        }

        public class SagaThatIsStartedByTheEvent : EndpointConfigurationBuilder
        {
            public SagaThatIsStartedByTheEvent()
            {
                EndpointSetup<DefaultServer>(c =>
                {
                    c.EnableFeature<TimeoutManager>();
                    c.DisableFeature<AutoSubscribe>();
                })
                    .AddMapping<SomethingHappenedEvent>(typeof(SagaThatPublishesAnEvent));

            }

            public class EventFromOtherSaga2 : Saga<EventFromOtherSaga2.EventFromOtherSaga2Data>, IAmStartedByMessages<SomethingHappenedEvent>, IHandleTimeouts<EventFromOtherSaga2.Saga2Timeout>
            {
                public Context Context { get; set; }

                public void Handle(SomethingHappenedEvent message)
                {
                    Data.DataId = message.DataId;
                    //Request a timeout
                    RequestTimeout<Saga2Timeout>(TimeSpan.FromSeconds(5));
                }

                public void Timeout(Saga2Timeout state)
                {
                    MarkAsComplete();
                    Context.DidSaga2Complete = true;
                }

                public class EventFromOtherSaga2Data : ContainSagaData
                {
                    public virtual Guid DataId { get; set; }
                }

                public class Saga2Timeout
                {
                }

                protected override void ConfigureHowToFindSaga(SagaPropertyMapper<EventFromOtherSaga2Data> mapper)
                {
                    mapper.ConfigureMapping<SomethingHappenedEvent>(m => m.DataId).ToSaga(s => s.DataId);
                }
            }
        }

        [Serializable]
        public class StartSaga : ICommand
        {
            public Guid DataId { get; set; }
        }

        public interface SomethingHappenedEvent : BaseEvent
        {
          
        }

        public interface BaseEvent : IEvent
        {
            Guid DataId { get; set; }
        }
    }
}