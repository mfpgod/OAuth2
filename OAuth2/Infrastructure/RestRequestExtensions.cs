﻿using RestSharp;
using RestSharp.Contrib;

namespace OAuth2.Infrastructure
{
    public static class RestRequestExtensions
    {
        public static void AddResourceWithQuery(this IRestRequest request, string query)
        {
            var queryParts = query.Split('?');
            if (queryParts.Length == 2)
            {
                request.Resource = queryParts[0];
                var parameters = HttpUtility.ParseQueryString(queryParts[1]);
                parameters.AllKeys.ForEach(key => request.AddParameter(key, parameters[key]));
            }
            else
            {
                request.Resource = query;
            }
        }
    }
}