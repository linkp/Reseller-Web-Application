// -----------------------------------------------------------------------
// <copyright file="PayUConstant.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.CustomerPortal.BusinessLogic.Commerce.PaymentGateways
{
    /// <summary>
    /// PayUConstant class
    /// </summary>
    public static class PayUConstant
    {
        /// <summary>
        /// PaymentResponseUrl url.
        /// </summary>
        public const string PaymentResponseUrl = "https://www.payumoney.com/payment/op/getPaymentResponse?merchantKey={0}&merchantTransactionIds={1}/";

        /// <summary>
        /// PaymentStatusUrl url.
        /// </summary>
        public const string PaymentStatusUrl = "https://www.payumoney.com/payment/payment/chkMerchantTxnStatus?merchantKey={0}&merchantTransactionIds={1}/";

        /// <summary>
        /// PaymentRefundUrl url.
        /// </summary>
        public const string PaymentRefundUrl = "https://www.payumoney.com/treasury/merchant/refundPayment?merchantKey={0}&paymentId={1}&refundAmount={2}/";

        /// <summary>
        /// MoneyWithPayU url.
        /// </summary>
        public const string MoneyWithPayU = "Money with Payumoney";
    }
}