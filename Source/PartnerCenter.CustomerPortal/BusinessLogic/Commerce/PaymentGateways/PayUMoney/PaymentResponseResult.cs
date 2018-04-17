// -----------------------------------------------------------------------
// <copyright file="PaymentResponseResult.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace PayUMoney.Api
{
    /// <summary>
    /// result class
    /// </summary>
    public class PaymentResponseResult
    {
        /// <summary>
        /// Gets or sets merchant transaction id
        /// </summary>
        public string MerchantTransactionId { get; set; }

        /// <summary>
        /// Gets or sets post back 
        /// </summary>
        public PostBackParameter PostBackParam { get; set; }
    }
}