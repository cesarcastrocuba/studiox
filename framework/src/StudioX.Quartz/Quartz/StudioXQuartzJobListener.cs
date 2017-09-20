﻿using Castle.Core.Logging;
using Quartz;

namespace StudioX.Quartz.Quartz
{
    public class StudioXQuartzJobListener : IJobListener
    {
        public string Name { get; } = "StudioXJobListener";

        public ILogger Logger { get; set; }

        public StudioXQuartzJobListener()
        {
            Logger = NullLogger.Instance;
        }

        public virtual void JobExecutionVetoed(IJobExecutionContext context)
        {
            Logger.Info($"Job {context.JobDetail.JobType.Name} executing operation vetoed...");
        }

        public virtual void JobToBeExecuted(IJobExecutionContext context)
        {
            Logger.Debug($"Job {context.JobDetail.JobType.Name} executing...");
        }

        public virtual void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
        {
            if (jobException == null)
            {
                Logger.Debug($"Job {context.JobDetail.JobType.Name} sucessfully executed.");
            }
            else
            {
                Logger.Error($"Job {context.JobDetail.JobType.Name} failed with exception: {jobException}");
            }
        }
    }
}