using OpenBots.Server.SDK.Model;
using System.Collections.Generic;
using System.Linq;

namespace OpenBots.Service.Client.Manager.Execution
{
    public class JobsQueueManager
    {
        private Queue<Job> _jobsQueue;

        public JobsQueueManager()
        {
            _jobsQueue = new Queue<Job>();
        }

        public void EnqueueJob(Job job)
        {
            if (_jobsQueue.ToList().Where(j => j.Id == job.Id).Count() == 0)
                _jobsQueue.Enqueue(job);
        }

        public Job DequeueJob()
        {
            return _jobsQueue.Dequeue();
        }

        public Job PeekJob()
        {
            return _jobsQueue.Peek();
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
