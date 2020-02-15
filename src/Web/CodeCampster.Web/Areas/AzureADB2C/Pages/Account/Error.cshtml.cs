// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.using Microsoft.AspNetCore.Authorization;

using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CodeCampster.Web.Areas.AzureADB2C.Pages.Account
{
    /// <summary>
    /// This API supports infrastructure and is not intended to be used
    /// directly from your code.This API may change or be removed in future releases
    /// </summary>
    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ErrorModel : PageModel
    {
        /// <summary>
        /// This API supports infrastructure and is not intended to be used
        /// directly from your code.This API may change or be removed in future releases
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// This API supports infrastructure and is not intended to be used
        /// directly from your code.This API may change or be removed in future releases
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        /// <summary>
        /// This API supports infrastructure and is not intended to be used
        /// directly from your code.This API may change or be removed in future releases
        /// </summary>
        public void OnGet()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }
    }
}
