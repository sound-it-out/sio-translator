using Amazon;
using Amazon.Polly;
using Amazon.Polly.Model;
using Amazon.Runtime;
using Microsoft.Extensions.Options;
using SIO.Infrastructure.Files;
using SIO.Infrastructure.Translations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SIO.Infrastructure.AWS.Translations
{
    internal class AWSSpeechSynthesizer : ISpeechSynthesizer<AWSSpeechRequest>
    {
        private readonly IAmazonPolly _pollyClient;
        private readonly IFileClient _fileClient;

        public AWSSpeechSynthesizer(IFileClient fileClient, IOptions<AWSCredentialOptions> awsCredentialOptions)
        {
            if (fileClient == null)
                throw new ArgumentNullException(nameof(fileClient));
            if (awsCredentialOptions == null)
                throw new ArgumentNullException(nameof(awsCredentialOptions));

            _fileClient = fileClient;
            _pollyClient = new AmazonPollyClient(new BasicAWSCredentials(awsCredentialOptions.Value.AccessKey, awsCredentialOptions.Value.SceretKey), RegionEndpoint.EUWest1);
        }
        public async ValueTask<ISpeechResult> TranslateTextAsync(AWSSpeechRequest request)
        {         
            var synthesizeSpeechRequest = new StartSpeechSynthesisTaskRequest
            {
                OutputFormat = request.OutputFormat,
                VoiceId = request.VoiceId,
                Text = request.Text
            };

            var response = await _pollyClient.StartSpeechSynthesisTaskAsync(synthesizeSpeechRequest);            

            using (var waitHandle = new AutoResetEvent(false))
            using (new Timer(
                    callback: async (e) => { await CheckSynthesisTask(response.SynthesisTask.TaskId, waitHandle); },
                    state: null,
                    dueTime: TimeSpan.Zero,
                    period: TimeSpan.FromSeconds(30)
                )
            )
            {
                waitHandle.WaitOne();
            }

            var task = await _pollyClient.GetSpeechSynthesisTaskAsync(new GetSpeechSynthesisTaskRequest { TaskId = response.SynthesisTask.TaskId });

            var fileResult = await _fileClient.DownloadAsync(task.SynthesisTask.OutputUri, "");
            
            return new AWSSpeechResult(fileResult.OpenStreamAsync);
        }

        private async Task CheckSynthesisTask(string id, EventWaitHandle waitHandle)
        {
            var task = await _pollyClient.GetSpeechSynthesisTaskAsync(new GetSpeechSynthesisTaskRequest { TaskId = id });

            if (task.SynthesisTask.TaskStatus == Amazon.Polly.TaskStatus.Completed)
                waitHandle.Set();
        }
    }
}
