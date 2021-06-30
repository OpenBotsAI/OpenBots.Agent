using System;
using System.Runtime.Serialization;

namespace OpenBots.DocumentsServer.SDK.Exceptions
{
    [Serializable]
    internal class CannotRetrieveQueuesException : Exception
    {
        public CannotRetrieveQueuesException()
        {
        }

        public CannotRetrieveQueuesException(string message) : base(message)
        {
        }

        public CannotRetrieveQueuesException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CannotRetrieveQueuesException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
