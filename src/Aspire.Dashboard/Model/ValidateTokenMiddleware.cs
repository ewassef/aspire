// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Security.Claims;
using Aspire.Dashboard.Configuration;
using Aspire.Dashboard.Utils;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Web;

namespace Aspire.Dashboard.Model;

internal sealed class ValidateTokenMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IOptionsMonitor<DashboardOptions> _options;
    private readonly ILogger<ValidateTokenMiddleware> _logger;

    public ValidateTokenMiddleware(RequestDelegate next, IOptionsMonitor<DashboardOptions> options, ILogger<ValidateTokenMiddleware> logger)
    {
        _next = next;
        _options = options;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.Equals("/login", StringComparisons.UrlPath))
        {
            if (_options.CurrentValue.Frontend.AuthMode != FrontendAuthMode.BrowserToken)
            {
                _logger.LogDebug($"Request to validate token URL but auth mode isn't set to {FrontendAuthMode.BrowserToken}.");

                RedirectAfterValidation(context);
            }
            else if (context.Request.Query.TryGetValue("t", out var value) && _options.CurrentValue.Frontend.AuthMode == FrontendAuthMode.BrowserToken)
            {
                var dashboardOptions = context.RequestServices.GetRequiredService<IOptionsMonitor<DashboardOptions>>();
                if (await TryAuthenticateAsync(value.ToString(), context, dashboardOptions).ConfigureAwait(false))
                {
                    // Success. Redirect to the app.
                    RedirectAfterValidation(context);
                }
                else
                {
                    // Failure.
                    // The bad token in the query string could be confusing with the token in the text box.
                    // Remove it before the presenting the UI to the user.
                    var qs = HttpUtility.ParseQueryString(context.Request.QueryString.ToString());
                    qs.Remove("t");

                    // Collection created by ParseQueryString handles escaping names and values.
                    var newQuerystring = qs.ToString();
                    if (!string.IsNullOrEmpty(newQuerystring))
                    {
                        newQuerystring = "?" + newQuerystring;
                    }
                    var pathBase = context.Request.PathBase.HasValue ? context.Request.PathBase.ToString() : "/";
                    var redirectUrl = $"{pathBase}{context.Request.Path}{newQuerystring}";
                    context.Response.Redirect(redirectUrl);
                }

                return;
            }
        }

        await _next(context).ConfigureAwait(false);
    }

    private static void RedirectAfterValidation(HttpContext context)
    {
        if (context.Request.Query.TryGetValue("returnUrl", out var returnUrl))
        {
            context.Response.Redirect(returnUrl.ToString());
        }
        else
        {
            var pathBase = context.Request.PathBase.HasValue ? context.Request.PathBase.ToString() : "/";
            var redirectUrl = $"{pathBase}{DashboardUrls.HomeUrl()}";
            context.Response.Redirect(redirectUrl);
        }
    }

    public static async Task<bool> TryAuthenticateAsync(string incomingBrowserToken, HttpContext httpContext, IOptionsMonitor<DashboardOptions> dashboardOptions)
    {
        if (string.IsNullOrEmpty(incomingBrowserToken) || dashboardOptions.CurrentValue.Frontend.GetBrowserTokenBytes() is not { } expectedBrowserTokenBytes)
        {
            return false;
        }

        if (!CompareHelpers.CompareKey(expectedBrowserTokenBytes, incomingBrowserToken))
        {
            return false;
        }

        var claimsIdentity = new ClaimsIdentity(
            [new Claim(ClaimTypes.NameIdentifier, "Local")],
            authenticationType: CookieAuthenticationDefaults.AuthenticationScheme);
        var claims = new ClaimsPrincipal(claimsIdentity);

        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            claims,
            new AuthenticationProperties { IsPersistent = true }).ConfigureAwait(false);
        return true;
    }
}
