using System;
using System.Runtime.Serialization;

namespace OpenBots.Server.SDK.Exceptions
{
    [Serializable]
    internal class CannotSubmitDocumentException : Exception
    {
        public CannotSubmitDocumentException()
        {
        }

        public CannotSubmitDocumentException(string message) : base(message)
        {
        }

        public CannotSubmitDocumentException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CannotSubmitDocumentException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
