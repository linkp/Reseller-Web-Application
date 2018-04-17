// -----------------------------------------------------------------------
// <copyright file="RefundResponse.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace PayUMoney.Api
{
    /// <summary>
    /// Refund response class
    /// </summary>
    public class RefundResponse
    {
        /// <summary>
        /// Gets or sets
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// Gets or sets
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// Gets or sets
        /// </summary>
        public string Rows { get; set; }

        /// <summary>
        /// Gets or sets
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// Gets or sets
        /// </summary>
        public int Status { get; set; }
    }
}