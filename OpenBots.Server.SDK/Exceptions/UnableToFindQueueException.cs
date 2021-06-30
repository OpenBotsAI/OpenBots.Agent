using System;
using System.Runtime.Serialization;

namespace OpenBots.Server.SDK.Exceptions
{
    [Serializable]
    internal class UnableToFindQueueException : Exception
    {
        public UnableToFindQueueException()
        {
        }

        public UnableToFindQueueException(string message) : base(message)
        {
        }

        public UnableToFindQueueException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnableToFindQueueException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
