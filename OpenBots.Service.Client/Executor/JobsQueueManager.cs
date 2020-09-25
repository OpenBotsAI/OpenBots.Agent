using OpenBots.Service.API.Model;
using System.Collections.Generic;

namespace OpenBots.Service.Client.Executor
{
    public class JobsQueueManager
    {
        private Queue<Job> _jobsQueue;

        public static JobsQueueManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new JobsQueueManager();

                return instance;
            }
        }
        private static JobsQueueManager instance;

        private JobsQueueManager()
        {
            _jobsQueue = new Queue<Job>();
        }

        public void EnqueueJob(Job job)
        {
            _jobsQueue.Enqueue(job);
        }

        public Job DequeueJob()
        {
            return _jobsQueue.Dequeue();
        }

        public bool IsQueueEmpty()
        {
            return _jobsQueue.Count == 0 ? true : false;
        }

        public void ClearJobsQueue()
        {
            _jobsQueue.Clear();
        }
    }
}
