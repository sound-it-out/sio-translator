﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OpenEventSourcing.Events;
using SIO.Domain.Translation.Events;
using SIO.Infrastructure.AWS.Tests.Stubs;
using SIO.Infrastructure.AWS.Tests.Translations.AWSTranslationWorker;
using SIO.Infrastructure.AWS.Translations;
using SIO.Infrastructure.Files;
using SIO.Infrastructure.Translations;
using SIO.Testing.Attributes;

namespace SIO.Infrastructure.Google.Tests.Translations.AWSTranslationWorker.StartAsync
{
    public class WhenTranslationFails : AWSTranslationWorkerSpecification
    {
        private TranslationRequest _translationRequest;
        private readonly Guid _aggregateId = Guid.NewGuid();

        protected override Task Given()
        {
            return TranslationWorker.StartAsync(_translationRequest);
        }

        protected override async Task When()
        {
            var fileClient = _serviceProvider.GetRequiredService<IFileClient>();

            _translationRequest = new TranslationRequest(_aggregateId, Guid.NewGuid(), Guid.NewGuid(), 1, Guid.NewGuid().ToString(), "test.txt", "en-GB-Standard-F");

            using (var ms = new MemoryStream())
            using(TextWriter tw = new StreamWriter(ms))
            {
                await tw.WriteAsync("some test text.");
                await tw.FlushAsync();
                ms.Position = 0;
                await fileClient.UploadAsync($"{_translationRequest.CorrelationId}{Path.GetExtension(_translationRequest.FileName)}", _translationRequest.UserId, ms);
            }
        }

        protected override void BuildServices(IServiceCollection services)
        {
            base.BuildServices(services);
            services.AddSingleton<ISpeechSynthesizer<AWSSpeechRequest>>(new InMemoryAWSSpeechSynthesizer(true));
        }

        [Then]
        public async Task TranslationFailedEventShouldBePublished()
        {
            var eventStore = _serviceProvider.GetRequiredService<IEventStore>();
            var events = await eventStore.GetEventsAsync(_aggregateId);
            events.Any(e => e.GetType() == typeof(TranslationFailed)).Should().BeTrue();
        }
    }
}
