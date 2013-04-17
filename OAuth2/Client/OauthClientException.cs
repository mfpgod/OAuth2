using System;
using System.Runtime.Serialization;

namespace OAuth2.Client
{
    public class OauthClientException : ClientException
    {
        public OauthClientException()
        {
        }

        public OauthClientException(string message) : base(message)
        {
        }

        public OauthClientException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public OauthClientException(string message, string code, Exception innerException = null) : base(message, code, innerException)
        {
        }

        public OauthClientException(string message, string code, string description, Exception innerException = null)
            : base(message, code, description, innerException)
        {
        }

        protected OauthClientException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}