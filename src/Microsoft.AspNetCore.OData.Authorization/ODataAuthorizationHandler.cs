﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.OData.Authorization
{
    /// <summary>
    /// Decides whether an OData request should be authorized or denied.
    /// </summary>
    internal class ODataAuthorizationHandler : AuthorizationHandler<ODataAuthorizationScopesRequirement>
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private Func<ScopeFinderContext, Task<IEnumerable<string>>> _scopesFinder;

        /// <summary>
        /// Creates an instance of <see cref="ODataAuthorizationHandler"/>.
        /// </summary>
        /// <param name="scopesFinder">User-defined function used to retrieve the current user's scopes from the authorization context</param>
        public ODataAuthorizationHandler(IHttpContextAccessor contextAccessor, Func<ScopeFinderContext, Task<IEnumerable<string>>> scopesFinder = null) : base()
        {
            _contextAccessor = contextAccessor;
            _scopesFinder = scopesFinder;
        }

        /// <summary>
        /// Makes decision whether authorization should be allowed based on the provided scopes.
        /// </summary>
        /// <param name="context">The authorization context.</param>
        /// <param name="requirement">The <see cref="ODataAuthorizationScopesRequirement"/> defining the scopes required
        /// for authorization to succeed.</param>
        /// <returns></returns>
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ODataAuthorizationScopesRequirement requirement)
        {
            var scopeFinderContext = new ScopeFinderContext(context.User, _contextAccessor.HttpContext);
            var getScopes = _scopesFinder ?? DefaultFindScopes;
            var scopes = await getScopes(scopeFinderContext).ConfigureAwait(false);

            if (requirement.PermissionHandler.AllowsScopes(scopes))
            {
                context.Succeed(requirement);
            }
        }

        private Task<IEnumerable<string>> DefaultFindScopes(ScopeFinderContext context)
        {
            var claims = context.User?.FindAll("Scope");
            // RFC6749 Section 3.3 specifies that the scope claim is a space-delimited set of strings https://tools.ietf.org/html/rfc6749#section-3.3
            var scopes = claims?.SelectMany(c => c.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries)) ?? Enumerable.Empty<string>();
            return Task.FromResult(scopes);
        }
    }
}
