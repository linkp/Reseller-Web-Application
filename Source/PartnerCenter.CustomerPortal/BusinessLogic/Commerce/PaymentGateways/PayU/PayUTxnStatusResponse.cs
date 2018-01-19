// -----------------------------------------------------------------------
// <copyright file="PayUTxnStatusResponse.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.CustomerPortal.BusinessLogic.Commerce.PaymentGateways
{
    using System.Collections.Generic;

    /// <summary>
    /// Transaction status response class
    /// </summary>
    public class PayUTxnStatusResponse
    {
        /// <summary>
        /// Gets or sets
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Gets or sets
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets
        /// </summary>
        public List<TxnResult> Result { get; set; }

        /// <summary>
        /// Gets or sets
        /// </summary>
        public object ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets
        /// </summary>
        public object ResponseCode { get; set; }
    }
}