using System;
using System.Runtime.Serialization;

namespace OAuth2.Client
{
    public class OauthException : Exception
    {
        public OauthException()
        {
        }

        public OauthException(string message) : base(message)
        {
        }

        public OauthException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected OauthException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}