// -----------------------------------------------------------------------
// <copyright file="TransactionResult.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace PayUMoney.Api
{
    /// <summary>
    /// Transaction result class
    /// </summary>
    public class TransactionResult
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