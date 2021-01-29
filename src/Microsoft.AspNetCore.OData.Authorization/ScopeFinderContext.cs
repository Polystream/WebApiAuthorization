// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.OData.Authorization
{
    /// <summary>
    /// Contains information used to extract permission scopes
    /// available to the authenticated user
    /// </summary>
    public class ScopeFinderContext
    {
        /// <summary>
        /// Creates an instance of <see cref="ScopeFinderContext"/>
        /// </summary>
        /// <param name="user">The authenticated user</param>
        /// <param name="httpContext">The http context for the current request</param>
        public ScopeFinderContext(ClaimsPrincipal user, HttpContext httpContext)
        {
            User = user;
            HttpContext = httpContext;
        }
        
        /// <summary>
        /// The <see cref="ClaimsPrincipal"/> representing the current user.
        /// </summary>
        public ClaimsPrincipal User { get; private set; }
        
        /// <summary>
        /// The <see cref="HttpContext"/> for the current request
        /// </summary>
        public HttpContext HttpContext { get; private set; }
    }
}
