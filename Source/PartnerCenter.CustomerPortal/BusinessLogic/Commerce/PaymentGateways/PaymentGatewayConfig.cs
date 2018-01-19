// -----------------------------------------------------------------------
// <copyright file="PaymentGatewayConfig.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.CustomerPortal.BusinessLogic.Commerce.PaymentGateways
{
    /// <summary>
    /// Payment configuration class
    /// </summary>
    public class PaymentGatewayConfig
    {
        /// <summary>
        /// country code
        /// </summary>
        private static string countryCode = ApplicationDomain.Instance.PortalLocalization.CountryIso2Code;

        /// <summary>
        /// this method is use to get the payment configuration page
        /// </summary>
        /// <returns> return view name</returns>
        public static string GetPaymentConfigView()
        {
            if (countryCode.Equals("IN"))
            {
                return "PayUPaymentSetup";
            }

            return "PaymentSetup";
        }

        /// <summary>
        /// Get web configuration path
        /// </summary>
        /// <returns>returns web configuration name</returns>
        public static string GetWebConfigPath()
        {
            if (countryCode.Equals("IN"))
            {
                return "WebPortalConfigurationPayU.json";
            }

            return "WebPortalConfiguration.json";
        }

        /// <summary>
        /// creates a payment gateway instance
        /// </summary>
        /// <param name="applicationDomain">Application domain</param>
        /// <param name="description">the description</param>
        /// <returns>returns payment gateway instance</returns>
        public static IPaymentGateway GetPaymentGatewayInstance(ApplicationDomain applicationDomain, string description)
        {
            if (countryCode.Equals("IN"))
            {
                return new PayUGateway(applicationDomain, description);
            }

            return new PayPalGateway(applicationDomain, description);
        }
    }
}