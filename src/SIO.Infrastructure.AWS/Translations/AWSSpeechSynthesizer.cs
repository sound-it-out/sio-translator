using Amazon;
using Amazon.Polly;
using Amazon.Polly.Model;
using Amazon.Runtime;
using Microsoft.Extensions.Options;
using SIO.Infrastructure.AWS.Files;
using SIO.Infrastructure.Extensions;
using SIO.Infrastructure.Files;
using SIO.Infrastructure.Translations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SIO.Infrastructure.AWS.Translations
{
    internal class AWSSpeechSynthesizer : ISpeechSynthesizer<AWSSpeechRequest>
    {
        private readonly IAmazonPolly _pollyClient;
        private readonly IFileClient _fileClient;
        private readonly AWSTranslationOptions _translationOptions;

        public AWSSpeechSynthesizer(IFileClient fileClient, IOptions<AWSCredentialOptions> awsCredentialOptions, IOptions<AWSTranslationOptions> translationOptions)
        {
            if (fileClient == null)
                throw new ArgumentNullException(nameof(fileClient));
            if (awsCredentialOptions == null)
                throw new ArgumentNullException(nameof(awsCredentialOptions));
            if (translationOptions == null)
                throw new ArgumentNullException(nameof(translationOptions));

            _fileClient = fileClient;
            _pollyClient = new AmazonPollyClient(new BasicAWSCredentials(awsCredentialOptions.Value.AccessKey, awsCredentialOptions.Value.SecretKey), RegionEndpoint.EUWest2);
            _translationOptions = translationOptions.Value;
        }
        public async ValueTask<ISpeechResult> TranslateTextAsync(AWSSpeechRequest request)
        {
            var result = new AWSSpeechResult();

            var chunks = request.Content.Chunk(30).ToArray();

            var textIndex = 0;

            for (int i = 0; i < chunks.Length; i++)
            {
                if (i > 0)
                    await Task.Delay(60000);

                var tasks = new List<Task>();

                foreach (var chunk in chunks[i])
                {
                    tasks.Add(QueueText(chunk, textIndex, request, result));
                    textIndex++;
                }

                await Task.WhenAll(tasks);
            }

            return result;
        }

        private async Task CheckSynthesisTask(string id, EventWaitHandle waitHandle)
        {
            var task = await _pollyClient.GetSpeechSynthesisTaskAsync(new GetSpeechSynthesisTaskRequest { TaskId = id });

            if (task.SynthesisTask.TaskStatus != Amazon.Polly.TaskStatus.InProgress && task.SynthesisTask.TaskStatus != Amazon.Polly.TaskStatus.Scheduled)
                waitHandle.Set();
        }

        private async Task QueueText(string text, int index, AWSSpeechRequest request, AWSSpeechResult result)
        {
            var synthesizeSpeechRequest = new StartSpeechSynthesisTaskRequest
            {
                OutputFormat = request.OutputFormat,
                VoiceId = request.VoiceId,
                Text = text,
                OutputS3BucketName = _translationOptions.Bucket,
                OutputS3KeyPrefix = Guid.NewGuid().ToString()
            };

            var response = await _pollyClient.StartSpeechSynthesisTaskAsync(synthesizeSpeechRequest);

            using (var waitHandle = new AutoResetEvent(false))
            using (new Timer(
                    callback: async (e) => { await CheckSynthesisTask(response.SynthesisTask.TaskId, waitHandle); },
                    state: null,
                    dueTime: TimeSpan.Zero,
                    period: TimeSpan.FromSeconds(_translationOptions.PollRate)
                )
            )
            {
                waitHandle.WaitOne();
            }

            var task = await _pollyClient.GetSpeechSynthesisTaskAsync(new GetSpeechSynthesisTaskRequest { TaskId = response.SynthesisTask.TaskId });

            if (task.SynthesisTask.TaskStatus == Amazon.Polly.TaskStatus.Failed)
                throw new InvalidOperationException();

            var key = task.SynthesisTask.OutputUri.Split('/').Last();
            var fileResult = await _fileClient.DownloadAsync(key, "");
            using(var stream = await fileResult.OpenStreamAsync())
            {
                await result.DigestBytes(index, stream);
            }

            if(request.CallBack != null)
                await request.CallBack(text.Length);

            await _fileClient.DeleteAsync(key, "");
        }
    }
}
