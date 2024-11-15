using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AlloySampleSite.Extensions
{
    public static class HttpContextExtensions
    {
        private const string NullIpAddress = "::1";
        private static bool? _isLocalRequest = null;

        public static bool IsLocalRequest(this HttpContext httpContext)
        {
            if (!_isLocalRequest.HasValue)
            {
                var connection = httpContext.Connection;

                _isLocalRequest = connection.RemoteIpAddress.IsSet() ? connection.LocalIpAddress.IsSet()
                                //Is local is same as remote, then we are local
                                ? connection.RemoteIpAddress.Equals(connection.LocalIpAddress)
                                //else we are remote if the remote IP address is not a loopback address
                                : IPAddress.IsLoopback(connection.RemoteIpAddress)
                                : true;
            }

            return _isLocalRequest.Value;
        }

        private static bool IsSet(this IPAddress address)
        {
            return address != null && address.ToString() != NullIpAddress;
        }
    }
}
