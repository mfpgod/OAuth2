using System;
using System.Runtime.Serialization;

namespace OAuth2.Client
{
    public class ServiceDataException : Exception
    {
        public ServiceDataException()
        {
        }

        public ServiceDataException(string message)
            : base(message)
        {
        }

        public ServiceDataException(string message, string type, string code, Exception innerException = null)
            : base(message, innerException)
        {
            Type = type;
            Code = code;
        }

        public ServiceDataException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ServiceDataException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public string Type { get; set; }

        public string Code { get; set; }
    }
}