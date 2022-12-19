using IPHandlerWebAPI.Models;
using IPHandlerWebAPI.Services;
using IPHandlerWebAPI.Utils;
using IPInfoProvider.Services;
using System.Diagnostics;
using System.IO.IsolatedStorage;

namespace IPHandlerWebAPI.Jobs
{
    public class BackgroundIPService : BackgroundService
    {
        private readonly BackgroundJobs _backgroundJobs;
        private readonly ILogger<BackgroundIPService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly int bufferSize = 10;

        public BackgroundIPService(BackgroundJobs backgroundJobs, ILogger<BackgroundIPService> logger, IServiceProvider serviceProvider)
        {
            _backgroundJobs = backgroundJobs;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }


        /**
         * BackgroundService's ExecuteAsync is always running in the background and takes care of queued jobs created by the user.
         */
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Background IPService has been started.");
            while (!stoppingToken.IsCancellationRequested)
            {
                //The "await" keyword in the line below is important in order to let the API start running.
                await Task.Delay(1000);
                if (_backgroundJobs.BackgroundTasks.TryDequeue(out JobDTO job))
                {
                    _logger.LogInformation($"Job with ID '{job.JobId}' is being processed");
                    await ProcessJob(job);
                    _logger.LogInformation($"Job with ID '{job.JobId}' has finished");
                }
                
            }

        }

        /**
         * ProcessJob takes IP objects from the job's list of IPs and gives them to the service in batches of 10.
         * Service updates the database and then returns the result to this process which then updates the job's status.
         */
        private async Task ProcessJob(JobDTO job)
        {
            List<IP> ipsReady = new();
            List<bool> finished = new();
            int totalIps = job.QueuedIps.Count;

            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                IIPService _service = scope.ServiceProvider.GetRequiredService<IPServiceImpl>();

                while (job.QueuedIps.Count > 0)
                {
                    while (ipsReady.Count < bufferSize && job.QueuedIps.Count > 0)
                    {
                        ipsReady.Add(job.QueuedIps.Dequeue());
                    }

                    finished = await _service.UpdateIPs(ipsReady);
                    job.Progress[0] += finished.Count;


                    for (int i = 0; i < finished.Count(); i++)
                    {
                        job.Results.TryAdd(ipsReady.ElementAt(i).Id, finished.ElementAt(i));

                        if (finished.ElementAt(i)) job.Progress[2]++;
                        else job.Progress[3]++;
                    }

                    ipsReady.Clear();
                    
                    // Enable the line below to test a Job's Status from the API when it's in progress.
                    //await Task.Delay(100000);     
                }

            }
        }

    }
}
