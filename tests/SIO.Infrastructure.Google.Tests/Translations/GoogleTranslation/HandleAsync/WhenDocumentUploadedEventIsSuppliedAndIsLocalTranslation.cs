﻿using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OpenEventSourcing.Events;
using SIO.Domain.Document.Events;
using SIO.Domain.Translation.Events;
using SIO.Domain.Translations;
using SIO.Testing.Attributes;

namespace SIO.Infrastructure.Google.Tests.Translations.GoogleTranslation.HandleAsync
{
    public class WhenDocumentUploadedEventIsSuppliedAndIsLocalTranslation : GoogleTranslationSpecification
    {
        public WhenDocumentUploadedEventIsSuppliedAndIsLocalTranslation(GoogleTranslationFixture googleTranslationFixture, DocumentUploadedFixture eventFixture) : base(googleTranslationFixture, eventFixture, TranslationType.Local)
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
        public async Task TranslationQueuedEventShouldNotBePublished()
        {
            var eventStore = _serviceProvider.GetRequiredService<IEventStore>();
            var page = await eventStore.GetEventsAsync(0);
            page.Events.Any(e => e.GetType() == typeof(TranslationQueued) && e.CausationId == _eventFixture.Id).Should().BeFalse();
        }
    }
}
