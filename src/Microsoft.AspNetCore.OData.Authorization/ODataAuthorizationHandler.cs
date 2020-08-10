﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Microsoft.AspNetCore.OData.Authorization
{
    /// <summary>
    /// Decides whether an OData request should be authorized or denied.
    /// </summary>
    public class ODataAuthorizationHandler : AuthorizationHandler<ODataAuthorizationScopesRequirement>
    {
        private Func<ScopeFinderContext, Task<IEnumerable<string>>> _scopesFinder;

        /// <summary>
        /// Creates an instance of <see cref="ODataAuthorizationHandler"/>.
        /// </summary>
        /// <param name="scopesFinder">User-define function used to retrieve the current user's scopes from the authorization context</param>
        public ODataAuthorizationHandler(Func<ScopeFinderContext, Task<IEnumerable<string>>> scopesFinder = null) : base()
        {
            this._scopesFinder = scopesFinder;
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
            var scopeFinderContext = new ScopeFinderContext(context.User);
            var getScopes = this._scopesFinder ?? DefaultFindScopes;
            var scopes = await getScopes(scopeFinderContext);

            if (requirement.PermissionHandler.VerifyScopes(scopes))
            {
                context.Succeed(requirement);
            }
        }

        private Task<IEnumerable<string>> DefaultFindScopes(ScopeFinderContext context)
        {
            var claims = context.User?.FindAll("Scope");
            var scopes = claims?.Select(c => c.Value) ?? Enumerable.Empty<string>();
            return Task.FromResult(scopes);
        }
    }
}