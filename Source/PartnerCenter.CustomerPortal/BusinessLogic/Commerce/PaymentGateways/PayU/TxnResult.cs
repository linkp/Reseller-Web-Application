// -----------------------------------------------------------------------
// <copyright file="TxnResult.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.CustomerPortal.BusinessLogic.Commerce.PaymentGateways
{
    /// <summary>
    /// Transaction result class
    /// </summary>
    public class TxnResult
    {
        /// <summary>
        /// Gets or sets
        /// </summary>
        public string MerchantTransactionId { get; set; }

        /// <summary>
        /// Gets or sets
        /// </summary>
        public int PaymentId { get; set; }

        /// <summary>
        /// Gets or sets
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets
        /// </summary>
        public double Amount { get; set; }
    }
}