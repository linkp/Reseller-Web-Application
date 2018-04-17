// -----------------------------------------------------------------------
// <copyright file="TransactionStatusResponse.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace PayUMoney.Api
{
    using System.Collections.Generic;

    /// <summary>
    /// Transaction status response class
    /// </summary>
    public class TransactionStatusResponse
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
        public List<TransactionResult> Result { get; set; }

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