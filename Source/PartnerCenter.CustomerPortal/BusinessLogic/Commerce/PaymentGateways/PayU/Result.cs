// -----------------------------------------------------------------------
// <copyright file="Result.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.CustomerPortal.BusinessLogic.Commerce.PaymentGateways
{
    /// <summary>
    /// result class
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Gets or sets merchant transaction id
        /// </summary>
        public string MerchantTransactionId { get; set; }

        /// <summary>
        /// Gets or sets post back 
        /// </summary>
        public PostBackParam PostBackParam { get; set; }
    }
}