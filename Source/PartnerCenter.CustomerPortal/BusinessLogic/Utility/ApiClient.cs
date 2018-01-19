// -----------------------------------------------------------------------
// <copyright file="ApiClient.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.CustomerPortal.BusinessLogic.Utility
{
    using System;
    using System.Collections.Specialized;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    /// <summary>
    /// client class
    /// </summary>
    /// <typeparam name="T">Generic class</typeparam>
    public class ApiClient<T>
    {
        /// <summary>
        /// Post Async method
        /// </summary>
        /// <param name="header">the header data</param>
        /// <param name="path">The path string</param>
        /// <returns> response of post call</returns>
        public static async Task<T> PostAsync(NameValueCollection header, string path)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(path);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", header.Get("Authorization"));
            T data = default(T);
            HttpResponseMessage response = await client.PostAsync(path, null);
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsAsync<T>();
            }

            return data;
        }
    }
}