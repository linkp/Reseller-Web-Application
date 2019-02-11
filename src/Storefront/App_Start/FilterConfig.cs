﻿// -----------------------------------------------------------------------
// <copyright file="FilterConfig.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Storefront
{
    using System.Web.Http.Filters;
    using System.Web.Mvc;
    using Filters.Mvc;

    /// <summary>
    /// Configures application filters.
    /// </summary>
    public static class FilterConfig
    {
        /// <summary>
        /// Registers global MVC filters.
        /// </summary>
        /// <param name="filters">The global MVC filter collection.</param>
        public static void RegisterGlobalMvcFilters(GlobalFilterCollection filters)
        {
            filters.Add(new AuthenticationFilterAttribute());
            filters.Add(new AiHandleErrorAttribute());
        }

        /// <summary>
        /// Registers global Web API filters.
        /// </summary>
        /// <param name="filters">The global web API filter collection.</param>
        public static void RegisterWebApiFilters(HttpFilterCollection filters)
        {
            filters.Add(new Filters.WebApi.AuthenticationFilterAttribute());
            filters.Add(new Filters.WebApi.ErrorHandlerAttribute());
        }
    }
}