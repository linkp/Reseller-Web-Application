// -----------------------------------------------------------------------
// <copyright file="PayUMoneyPaymentResponse.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.CustomerPortal.BusinessLogic.Commerce.PaymentGateways
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// PayUMoneyPaymentResponse class
    /// </summary>
    public class PayUMoneyPaymentResponse
    {
        /// <summary>
        /// Gets or sets error code
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets message code
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets response code
        /// </summary>
        public string ResponseCode { get; set; }

        /// <summary>
        /// Gets or sets result
        /// </summary>
        public List<Result> Result { get; set; }

        /// <summary>
        /// Gets or sets status
        /// </summary>
        public string Status { get; set; }
    }
}