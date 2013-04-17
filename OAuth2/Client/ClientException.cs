using System;
using System.Runtime.Serialization;

namespace OAuth2.Client
{
    public class ClientException : Exception
    {
        public ClientException()
        {
        }

        public ClientException(string message) : base(message)
        {
        }

        public ClientException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ClientException(string message, string code, Exception innerException = null) : base(message, innerException)
        {
            Code = code;
        }

        public ClientException(string message, string code, string description, Exception innerException = null) : base(message, innerException)
        {
            Code = code;
            Description = description;
        }
        
        protected ClientException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public string Code { get; set; }

        public string Description { get; set; }
    }
}