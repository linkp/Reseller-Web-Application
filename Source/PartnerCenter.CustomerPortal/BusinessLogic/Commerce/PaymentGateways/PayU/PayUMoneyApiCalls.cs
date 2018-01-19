// -----------------------------------------------------------------------
// <copyright file="PayUMoneyApiCalls.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.CustomerPortal.BusinessLogic.Commerce.PaymentGateways
{
    using System;
    using System.Collections.Specialized;
    using System.Threading.Tasks;
    using Microsoft.Store.PartnerCenter.CustomerPortal.BusinessLogic.Utility;
    using Microsoft.Store.PartnerCenter.CustomerPortal.Models;

    /// <summary>
    /// class definition
    /// </summary>
    public class PayUMoneyApiCalls
    {
        /// <summary>
        /// Get payment response. 
        /// </summary>
        /// <param name="paymentId">The PaymentId.</param>
        /// <returns>returns PayUMoneyPaymentResponse.</returns>
        public static async Task<PayUMoneyPaymentResponse> GetPaymentDetails(string paymentId)
        {
            PaymentConfiguration payconfig = await GetAPaymentConfigAsync();
            NameValueCollection header = new NameValueCollection();
            header.Add("Authorization", payconfig.WebExperienceProfileId);
            PayUMoneyPaymentResponse response = await ApiClient<PayUMoneyPaymentResponse>.PostAsync(header, string.Format(PayUConstant.PaymentResponseUrl, payconfig.ClientId, paymentId));
            return await Task.FromResult(response);
        }

        /// <summary>
        /// Get Payment status. 
        /// </summary>
        /// <param name="paymentId">The PaymentId.</param>
        /// <returns>returns transaction response.</returns>
        public static async Task<PayUTxnStatusResponse> GetPaymentStatus(string paymentId)
        {
            PaymentConfiguration payconfig = await GetAPaymentConfigAsync();
            NameValueCollection header = new NameValueCollection();
            header.Add("Authorization", payconfig.WebExperienceProfileId);
            PayUTxnStatusResponse response = await ApiClient<PayUTxnStatusResponse>.PostAsync(header, string.Format(PayUConstant.PaymentStatusUrl, payconfig.ClientId, paymentId));
            return await Task.FromResult(response);
        }

        /// <summary>
        /// Initiate Refund. 
        /// </summary>
        /// <param name="paymentId">The PaymentId.</param>
        /// <param name="amount">The Amount.</param>
        /// <returns>returns PayUMoneyRefundResponse.</returns>
        public static async Task<PayUMoneyRefundResponse> RefundPayment(string paymentId, string amount)
        {
            PaymentConfiguration payconfig = await GetAPaymentConfigAsync();
            NameValueCollection header = new NameValueCollection();
            header.Add("Authorization", payconfig.WebExperienceProfileId);
            PayUMoneyRefundResponse response = await ApiClient<PayUMoneyRefundResponse>.PostAsync(header, string.Format(PayUConstant.PaymentRefundUrl, payconfig.ClientId, paymentId, amount));
            return await Task.FromResult(response);
        }

        /// <summary>
        /// Throws PartnerDomainException by parsing PayPal exception. 
        /// </summary>
        /// <returns>return payment configuration</returns>
        public static async Task<PaymentConfiguration> GetAPaymentConfigAsync()
        {
            //// The GetAccessToken() of the SDK Returns the currently cached access token. 
            //// If no access token was previously cached, or if the current access token is expired, then a new one is generated and returned. 
            //// See more - https://github.com/paypal/PayPal-NET-SDK/blob/develop/Source/SDK/Api/OAuthTokenCredential.cs

            // Before getAPIContext ... set up PayPal configuration. This is an expensive call which can benefit from caching. 
            PaymentConfiguration paymentConfig = await ApplicationDomain.Instance.PaymentConfigurationRepository.RetrieveAsync();

            return paymentConfig;
        }
    }
}