using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OpenEventSourcing.Events;
using SIO.Domain.Document.Events;
using SIO.Domain.Translation.Events;
using SIO.Domain.Translations;
using SIO.Testing.Attributes;

namespace SIO.Infrastructure.Local.Tests.Translations.LocalTranslation.HandleAsync
{
    public class WhenDocumentUploadedEventIsSuppliedAndIsLocalTranslation : LocalTranslationSpecification
    {
        public WhenDocumentUploadedEventIsSuppliedAndIsLocalTranslation(LocalTranslationFixture localTranslationFixture, DocumentUploadedFixture eventFixture) : base(localTranslationFixture, eventFixture, TranslationType.Local)
        {
        }

        protected override async Task Given()
        {
            var @event = new DocumentUploaded(_eventFixture.Id, _eventFixture.AggregateId, new Guid(_eventFixture.UserId), _eventFixture.TranslationType, _eventFixture.TranslationSubject, _eventFixture.FileName);
            await Translation.HandleAsync(@event);
        }

        protected override Task When()
        {
            return Task.CompletedTask;
        }

        [Then]
        public async Task TranslationQueuedEventShouldBePublished()
        {
            var eventStore = _serviceProvider.GetRequiredService<IEventStore>();
            var page = await eventStore.GetEventsAsync(0);
            page.Events.Any(e => e.GetType() == typeof(TranslationQueued) && e.CausationId == _eventFixture.Id).Should().BeTrue();
        }

        [Then]
        public async Task TranslationQueuedEventShouldBePublishedWithExpectedUserId()
        {
            var eventStore = _serviceProvider.GetRequiredService<IEventStore>();
            var page = await eventStore.GetEventsAsync(0);
            page.Events.First(e => e.GetType() == typeof(TranslationQueued)).UserId.Should().Be(_eventFixture.UserId);
        }

        [Then]
        public async Task TranslationQueuedEventShouldBePublishedWithExpectedCorrelationId()
        {
            var eventStore = _serviceProvider.GetRequiredService<IEventStore>();
            var page = await eventStore.GetEventsAsync(0);
            page.Events.First(e => e.GetType() == typeof(TranslationQueued)).CorrelationId.Should().Be(_eventFixture.AggregateId);
        }

        [Then]
        public async Task TranslationQueuedEventShouldBePublishedWithExpectedCausationId()
        {
            var eventStore = _serviceProvider.GetRequiredService<IEventStore>();
            var page = await eventStore.GetEventsAsync(0);
            page.Events.First(e => e.GetType() == typeof(TranslationQueued)).CausationId.Should().Be(_eventFixture.Id);
        }

        [Then]
        public async Task TranslationQueuedEventShouldBePublishedWithExpectedVersion()
        {
            var eventStore = _serviceProvider.GetRequiredService<IEventStore>();
            var page = await eventStore.GetEventsAsync(0);
            page.Events.First(e => e.GetType() == typeof(TranslationQueued)).Version.Should().Be(_eventFixture.Version + 1);
        }
    }
}
