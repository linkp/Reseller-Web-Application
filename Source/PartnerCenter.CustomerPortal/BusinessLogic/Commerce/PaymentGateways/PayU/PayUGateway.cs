// -----------------------------------------------------------------------
// <copyright file="PayUGateway.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.CustomerPortal.BusinessLogic.Commerce.PaymentGateways
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Exceptions;
    using Models;
    using PartnerCenter.Models.Customers;
    using PayPal;
    using PayPal.Api;

    /// <summary>
    /// PayPal payment gateway implementation.
    /// </summary>
    public class PayUGateway : DomainObject, IPaymentGateway
    {
        /// <summary>
        /// Test url.
        /// </summary>
        private static readonly string TESTPAYUURL = "https://test.payu.in/_payment";

        /// <summary>
        /// Live url.
        /// </summary>
        private static readonly string LIVEPAYUURL = "https://secure.payu.in/_payment";

        /// <summary>
        /// Hash sequence.
        /// </summary>
        private static readonly string HASHSEQUENCE = "key|txnid|amount|productinfo|firstname|email|udf1|udf2|udf3|udf4|udf5|udf6|udf7|udf8|udf9|udf10";

        /// <summary>
        /// Maintains the payment id for the payment gateway.
        /// </summary>
        private static readonly string PAYUPAISASERVICEPROVIDER = "payu_paisa";

        /// <summary>
        /// Maintains the description for this payment. 
        /// </summary>
        private readonly string paymentDescription;

        /// <summary>
        /// Maintains the payer id for the payment gateway. 
        /// </summary>
        private string payerId;

        /// <summary>
        /// Maintains the payment id for the payment gateway.
        /// </summary>
        private string paymentId;

        /// <summary>
        /// Initializes a new instance of the <see cref="PayUGateway" /> class. 
        /// </summary>
        /// <param name="applicationDomain">The ApplicationDomain</param>        
        /// <param name="description">The description which will be added to the Payment Card authorization call.</param>
        public PayUGateway(ApplicationDomain applicationDomain, string description) : base(applicationDomain)
        {
            description.AssertNotEmpty(nameof(description));
            this.paymentDescription = description;

            this.payerId = string.Empty;
            this.paymentId = string.Empty;
        }

        /// <summary>
        /// Validates payment configuration. 
        /// </summary>
        /// <param name="paymentConfig">The Payment configuration.</param>
        public void ValidateConfiguration(PaymentConfiguration paymentConfig)
        {
            ////Payu does not provide payment profile validation api.
        }

        /// <summary>
        /// Creates Web Experience profile using portal branding and payment configuration. 
        /// </summary>
        /// <param name="paymentConfig">The Payment configuration.</param>
        /// <param name="brandConfig">The branding configuration.</param>
        /// <param name="countryIso2Code">The locale code used by the web experience profile. Example-US.</param>
        /// <returns>The created web experience profile id.</returns>
        public string CreateWebExperienceProfile(PaymentConfiguration paymentConfig, BrandingConfiguration brandConfig, string countryIso2Code)
        {
            ////Payu does not provide the concept of webprofile
            ////stored authorization in WebExperienceProfileId
            return paymentConfig.WebExperienceProfileId;
        }

        /// <summary>
        /// Creates a payment transaction and returns the PayPal generated payment URL. 
        /// </summary>
        /// <param name="returnUrl">The redirect url for PayPal callback to web store portal.</param>                
        /// <param name="order">The order details for which payment needs to be made.</param>        
        /// <returns>Payment URL from PayPal.</returns>
        public async Task<string> GeneratePaymentUriAsync(string returnUrl, OrderViewModel order)
        {
            returnUrl.AssertNotEmpty(nameof(returnUrl));
            order.AssertNotNull(nameof(order));
            RemotePost myremotepost = await this.PrepareRemotePost(order, returnUrl);
            return myremotepost.Post();
        }

        /// <summary>
        /// get payment url.
        /// </summary>
        /// <param name="paymentData">payment data.</param>
        /// <returns>return boolean.</returns>
        public async Task<bool> IsPaymentDataValid(System.Web.Mvc.FormCollection paymentData)
        {
            string[] merc_hash_vars_seq;
            string merc_hash_string = string.Empty;
            string merc_hash = string.Empty;
            PaymentConfiguration payconfig = await this.GetAPaymentConfigAsync();

            merc_hash_vars_seq = HASHSEQUENCE.Split('|');
            Array.Reverse(merc_hash_vars_seq);
            merc_hash_string = payconfig.ClientSecret + "|" + paymentData["status"].ToString();

            foreach (string merc_hash_var in merc_hash_vars_seq)
            {
                merc_hash_string += "|";
                merc_hash_string = merc_hash_string + (paymentData[merc_hash_var] != null ? paymentData[merc_hash_var] : string.Empty);
            }

            merc_hash = this.Generatehash512(merc_hash_string).ToLower();

            if (merc_hash != paymentData["hash"])
            {
                throw new PartnerDomainException(ErrorCode.PaymentGatewayIdentityFailureDuringConfiguration).AddDetail("ErrorMessage", Resources.PaymentGatewayIdentityFailureDuringConfiguration);
            }

            return true;
        }

        /// <summary>
        /// Executes a PayU payment.
        /// </summary>
        /// <returns>Capture string id.</returns>
        public async Task<string> ExecutePaymentAsync()
        {
            PayUTxnStatusResponse paymentResponse = await PayUMoneyApiCalls.GetPaymentStatus(this.paymentId);

            try
            {
                if (paymentResponse != null && paymentResponse.Result.Count > 0 && paymentResponse.Result[0].Status.Equals(PayUConstant.MoneyWithPayU))
                {
                    return paymentResponse.Result[0].Amount.ToString();
                }
            }
            catch (PayPalException ex)
            {
                this.ParsePayPalException(ex);
            }

            return await Task.FromResult(string.Empty);
        }

        /// <summary>
        /// Finalizes an authorized payment with PayU.
        /// </summary>
        /// <param name="authorizationCode">The authorization code for the payment to capture.</param>
        /// <returns>A task.</returns>
        public async Task CaptureAsync(string authorizationCode)
        {
            ////PayU api not provided
            await Task.FromResult(string.Empty);
        }

        /// <summary>
        /// Voids an authorized payment with PayPal.
        /// </summary>
        /// <param name="authorizationCode">The authorization code for the payment to void.</param>
        /// <returns>a Task</returns>
        public async Task VoidAsync(string authorizationCode)
        {
            authorizationCode.AssertNotEmpty(nameof(authorizationCode));

            // given the authorizationId string... Lookup the authorization to void it. 
            try
            {
                PayUMoneyRefundResponse refundResponse = await PayUMoneyApiCalls.RefundPayment(this.payerId, authorizationCode);
                if (refundResponse.Status != 0 || !refundResponse.Message.Equals("Refund Initiated"))
                {
                    throw new Exception("Error in refund");
                }
            }
            catch (PayPalException ex)
            {
                this.ParsePayPalException(ex);
            }
        }

        /// <summary>
        /// Retrieves the order details maintained for the payment gateway.  
        /// </summary>
        /// <param name="payerId">The Payer Id.</param>
        /// <param name="paymentId">The Payment Id.</param>
        /// <param name="orderId">The Order Id.</param>
        /// <param name="customerId">The Customer Id.</param>
        /// <returns>The order associated with this payment transaction.</returns>
        public async Task<OrderViewModel> GetOrderDetailsFromPaymentAsync(string payerId, string paymentId, string orderId, string customerId)
        {
            // this payment gateway ignores orderId & customerId. 
            payerId.AssertNotEmpty(nameof(payerId));
            paymentId.AssertNotEmpty(nameof(paymentId));

            this.payerId = payerId;
            this.paymentId = paymentId;

            return await this.GetOrderDetails();
        }

        /// <summary>
        /// Retrieves order view.
        /// </summary>
        /// <param name="paymentData">payment data.</param>
        /// <returns>returns order view.</returns>
        public async Task<OrderViewModel> GetOrderDetailsFromPaymentAsync(System.Web.Mvc.FormCollection paymentData)
        {
            return await this.GetOrderDetails(paymentData["udf1"], paymentData["productinfo"], paymentData["udf2"]);
        }

        /// <summary>
        /// Generate hash. 
        /// </summary>
        /// <param name="text">hash string.</param>
        /// <returns>return string</returns>
        public string Generatehash512(string text)
        {
            byte[] message = Encoding.UTF8.GetBytes(text);

            UnicodeEncoding ue = new UnicodeEncoding();
            byte[] hashValue;
            System.Security.Cryptography.SHA512Managed hashString = new System.Security.Cryptography.SHA512Managed();
            string hex = string.Empty;
            hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += string.Format("{0:x2}", x);
            }

            return hex;
        }

        /// <summary>
        /// Generate transaction id. 
        /// </summary>
        /// <returns>return string</returns>
        public string Generatetxnid()
        {
            Random rnd = new Random();
            string strHash = this.Generatehash512(rnd.ToString() + DateTime.Now);
            string txnid1 = strHash.ToString().Substring(0, 20);
            return txnid1;
        }

        /// <summary>
        /// Retrieves the order details maintained for the payment gateway.  
        /// </summary>
        /// <returns>return order data.</returns>
        private async Task<OrderViewModel> GetOrderDetails()
        {
            OrderViewModel orderFromPayment = null;
            PayUMoneyPaymentResponse paymentResponse = await PayUMoneyApiCalls.GetPaymentDetails(this.paymentId);

            try
            {
                if (paymentResponse != null && paymentResponse.Result.Count > 0)
                {
                    orderFromPayment = await this.GetOrderDetails(paymentResponse.Result[0].PostBackParam.Udf1, paymentResponse.Result[0].PostBackParam.Productinfo, paymentResponse.Result[0].PostBackParam.Udf2);
                }
            }
            catch (PayPalException ex)
            {
                this.ParsePayPalException(ex);
            }

            return await Task.FromResult(orderFromPayment);
        }

        /// <summary>
        /// get payment url.
        /// </summary>
        /// <param name="mode">mode of payment gateway.</param>
        /// <returns>return string.</returns>
        private string GetPaymentUrl(string mode)
        {
            ////two modes are possible sandbox and live
            if (mode.Equals("sandbox"))
            {
                return TESTPAYUURL;
            }

            return LIVEPAYUURL;
        }

        /// <summary>
        /// prepare remote post. 
        /// </summary>
        /// <param name="order">order details.</param>                
        /// <param name="returnUrl">return url.</param>        
        /// <returns>return remote post.</returns>
        private async Task<RemotePost> PrepareRemotePost(OrderViewModel order, string returnUrl)
        {
            string fname = string.Empty;
            string phone = string.Empty;
            string email = string.Empty;
            ////TODO: check if customer does not exist
            CustomerRegistrationRepository customerRegistrationRepository = new CustomerRegistrationRepository(ApplicationDomain.Instance);
            CustomerViewModel customerRegistrationInfo = await customerRegistrationRepository.RetrieveAsync(order.CustomerId);
            if (customerRegistrationInfo == null)
            {
                Customer customer = await ApplicationDomain.Instance.PartnerCenterClient.Customers.ById(order.CustomerId).GetAsync();
                fname = customer.BillingProfile.DefaultAddress.FirstName;
                phone = customer.BillingProfile.DefaultAddress.PhoneNumber;
                email = customer.BillingProfile.Email;
            }
            else
            {
                fname = customerRegistrationInfo.FirstName;
                phone = customerRegistrationInfo.Phone;
                email = customerRegistrationInfo.Email;
            }

            decimal paymentTotal = 0;
            //// PayPal wouldnt manage decimal points for few countries (example Hungary & Japan). 
            string moneyFixedPointFormat = (Resources.Culture.NumberFormat.CurrencyDecimalDigits == 0) ? "F0" : "F";
            StringBuilder productSubs = new StringBuilder();
            StringBuilder prodQuants = new StringBuilder();
            ////  StringBuilder prodPrices = new StringBuilder();
            //// Create itemlist and add item objects to it.
            var itemList = new ItemList() { items = new List<Item>() };
            foreach (var subscriptionItem in order.Subscriptions)
            {
                productSubs.Append(":").Append(subscriptionItem.SubscriptionId);
                prodQuants.Append(":").Append(subscriptionItem.Quantity.ToString());
                paymentTotal += Math.Round(subscriptionItem.Quantity * subscriptionItem.SeatPrice, Resources.Culture.NumberFormat.CurrencyDecimalDigits);
            }

            productSubs.Remove(0, 1);
            prodQuants.Remove(0, 1);
            System.Collections.Specialized.NameValueCollection inputs = new System.Collections.Specialized.NameValueCollection();
            PaymentConfiguration payconfig = await this.GetAPaymentConfigAsync();
            //// Inputs.Add("key", payconfig.ClientId);
            inputs.Add("key", payconfig.ClientId);
            inputs.Add("txnid", this.Generatetxnid());
            ////   decimal totalAmount = this.calculateTotalAmount(order.Subscriptions);
            inputs.Add("amount", paymentTotal.ToString());
            ////TODO : put correct product info
            inputs.Add("productinfo", productSubs.ToString());
            inputs.Add("firstname", fname);
            inputs.Add("phone", phone);
            inputs.Add("email", email);
            inputs.Add("udf1", order.OperationType.ToString());
            inputs.Add("udf2", prodQuants.ToString());

            ////TODO : make it configurable from outside
            inputs.Add("surl", returnUrl + "&payment=success&PayerId=" + inputs.Get("txnid")); ////Change the success url here depending upon the port number of your local system.
            inputs.Add("furl", returnUrl + "&payment=failure&PayerId=" + inputs.Get("txnid")); ////Change the failure url here depending upon the port number of your local system.
            inputs.Add("service_provider", PAYUPAISASERVICEPROVIDER);
            string hashString = inputs.Get("key") + "|" + inputs.Get("txnid") + "|" + inputs.Get("amount") + "|" + inputs.Get("productInfo") + "|" + inputs.Get("firstName") + "|" + inputs.Get("email") + "|" + inputs.Get("udf1") + "|" + inputs.Get("udf2") + "|||||||||" + payconfig.ClientSecret; // payconfig.ClientSecret;
            string hash = this.Generatehash512(hashString);
            inputs.Add("hash", hash);

            RemotePost myremotepost = new RemotePost();
            myremotepost.SetUrl(this.GetPaymentUrl(payconfig.AccountType)); // getPaymentUrl(payconfig.AccountType);
            myremotepost.SetInputs(inputs);
            return myremotepost;
        }

        /// <summary>
        /// Throws PartnerDomainException by parsing PayPal exception. 
        /// </summary>
        /// <param name="ex">Exceptions from PayPal SDK.</param>        
        private void ParsePayPalException(PayPalException ex)
        {
            if (ex is PaymentsException)
            {
                PaymentsException pe = ex as PaymentsException;

                // Get the details of this exception with ex.Details and format the error message in the form of "We are unable to process your payment –  {Errormessage} :: [err1, err2, .., errN]".                
                StringBuilder errorString = new StringBuilder();
                errorString.Append(Resources.PaymentGatewayErrorPrefix);

                // build error string for errors returned from financial institutions.
                if (pe.Details != null)
                {
                    string errorName = pe.Details.name.ToUpper();

                    if (errorName == null || errorName.Length < 1)
                    {
                        errorString.Append(pe.Details.message);
                        throw new PartnerDomainException(ErrorCode.PaymentGatewayFailure).AddDetail("ErrorMessage", errorString.ToString());
                    }
                    else if (errorName.Contains("UNKNOWN_ERROR"))
                    {
                        throw new PartnerDomainException(ErrorCode.PaymentGatewayPaymentError);
                    }
                    else if (errorName.Contains("VALIDATION") && pe.Details.details != null)
                    {
                        // Check if there are sub collection details and build error string.                                       
                        errorString.Append("[");
                        foreach (ErrorDetails errorDetails in pe.Details.details)
                        {
                            // removing extrataneous information.                     
                            string errorField = errorDetails.field;
                            if (errorField.Contains("payer.funding_instruments[0]."))
                            {
                                errorField = errorField.Replace("payer.funding_instruments[0].", string.Empty).ToString();
                            }

                            errorString.AppendFormat("{0} - {1},", errorField, errorDetails.issue);
                        }

                        errorString.Replace(',', ']', errorString.Length - 2, 2); // remove the last comma and replace it with ]. 
                    }
                    else
                    {
                        errorString.Append(Resources.PayPalUnableToProcessPayment);
                    }
                }

                throw new PartnerDomainException(ErrorCode.PaymentGatewayFailure).AddDetail("ErrorMessage", errorString.ToString());
            }

            if (ex is IdentityException)
            {
                // ideally this shouldn't be raised from customer experience calls. 
                // can occur when admin has generated a new secret for an existing app id in PayPal but didnt update portal payment configuration.                                
                throw new PartnerDomainException(ErrorCode.PaymentGatewayIdentityFailureDuringPayment).AddDetail("ErrorMessage", Resources.PaymentGatewayIdentityFailureDuringPayment);
            }

            // few PayPalException types contain meaningfull exception information only in InnerException. 
            if (ex is PayPalException && ex.InnerException != null)
            {
                throw new PartnerDomainException(ErrorCode.PaymentGatewayFailure).AddDetail("ErrorMessage", ex.InnerException.Message);
            }
            else
            {
                throw new PartnerDomainException(ErrorCode.PaymentGatewayFailure).AddDetail("ErrorMessage", ex.Message);
            }
        }

        /// <summary>
        /// Retrieves the Order from a payment transaction.
        /// </summary>
        /// <param name="operation">operation data.</param>
        /// <param name="prod">product data.</param>
        /// <param name="quant">quantity data.</param>
        /// <returns>The Order for which payment was made.</returns>
        private async Task<OrderViewModel> GetOrderDetails(string operation, string prod, string quant)
        {
            OrderViewModel orderFromPayment = null;
            try
            {
                orderFromPayment = new OrderViewModel();
                List<OrderSubscriptionItemViewModel> orderSubscriptions = new List<OrderSubscriptionItemViewModel>();

                orderFromPayment.OperationType = (CommerceOperationType)Enum.Parse(typeof(CommerceOperationType), operation, true);
                string[] prodList = prod.Split(':');
                string[] quantList = quant.Split(':');

                for (int i = 0; i < prodList.Length; i++)
                {
                    orderSubscriptions.Add(new OrderSubscriptionItemViewModel()
                    {
                        SubscriptionId = prodList[i],
                        OfferId = prodList[i],
                        Quantity = Convert.ToInt32(quantList[i], CultureInfo.InvariantCulture)
                    });
                }

                orderFromPayment.Subscriptions = orderSubscriptions;
            }
            catch (PayPalException ex)
            {
                this.ParsePayPalException(ex);
            }

            return await Task.FromResult(orderFromPayment);
        }

        /// <summary>
        /// Retrieves the API Context for PayPal. 
        /// </summary>
        /// <returns>PayPal APIContext</returns>
        private async Task<APIContext> GetAPIContextAsync()
        {
            //// The GetAccessToken() of the SDK Returns the currently cached access token. 
            //// If no access token was previously cached, or if the current access token is expired, then a new one is generated and returned. 
            //// See more - https://github.com/paypal/PayPal-NET-SDK/blob/develop/Source/SDK/Api/OAuthTokenCredential.cs

            // Before getAPIContext ... set up PayPal configuration. This is an expensive call which can benefit from caching. 
            PaymentConfiguration paymentConfig = await ApplicationDomain.Instance.PaymentConfigurationRepository.RetrieveAsync();

            Dictionary<string, string> configMap = new Dictionary<string, string>();
            configMap.Add("clientId", paymentConfig.ClientId);
            configMap.Add("clientSecret", paymentConfig.ClientSecret);
            configMap.Add("mode", paymentConfig.AccountType);
            configMap.Add("WebExperienceProfileId", paymentConfig.WebExperienceProfileId);
            configMap.Add("connectionTimeout", "120000");

            string accessToken = new OAuthTokenCredential(configMap).GetAccessToken();
            var apiContext = new APIContext(accessToken);
            apiContext.Config = configMap;
            return apiContext;
        }

        /// <summary>
        /// Throws PartnerDomainException by parsing PayPal exception. 
        /// </summary>
        /// <returns>return payment configuration</returns>
        private async Task<PaymentConfiguration> GetAPaymentConfigAsync()
        {
            //// The GetAccessToken() of the SDK Returns the currently cached access token. 
            //// If no access token was previously cached, or if the current access token is expired, then a new one is generated and returned. 
            //// See more - https://github.com/paypal/PayPal-NET-SDK/blob/develop/Source/SDK/Api/OAuthTokenCredential.cs

            // Before getAPIContext ... set up PayPal configuration. This is an expensive call which can benefit from caching. 
            PaymentConfiguration paymentConfig = await ApplicationDomain.Instance.PaymentConfigurationRepository.RetrieveAsync();

            return paymentConfig;
        }

        /// <summary>
        /// Remote post class.
        /// </summary>
        public class RemotePost
        {
            /// <summary>
            /// Maintains Url. 
            /// </summary>
            private string url = string.Empty;

            /// <summary>
            /// Maintains Method. 
            /// </summary>
            private string method = "post";

            /// <summary>
            /// Maintains form name. 
            /// </summary>
            private string formName = "form1";

            /// <summary>
            /// Maintains input collection. 
            /// </summary>
            private System.Collections.Specialized.NameValueCollection inputs = new System.Collections.Specialized.NameValueCollection();

            /// <summary>
            /// Retrieves the API Context for PayPal. 
            /// </summary>
            /// <param name="u">url string.</param>
            public void SetUrl(string u)
            {
                this.url = u;
            }

            /// <summary>
            /// Retrieves the API Context for PayPal. 
            /// </summary>
            /// <param name="name">name string.</param>
            /// <param name="value">value string.</param>
            public void Add(string name, string value)
            {
                this.inputs.Add(name, value);
            }

            /// <summary>
            /// Retrieves the API Context for PayPal. 
            /// </summary>
            /// <param name="inputs">collection of values.</param>
            public void SetInputs(System.Collections.Specialized.NameValueCollection inputs)
            {
                ////TODO : clone it, so that changes will not impact it
                this.inputs = inputs;
            }

            /// <summary>
            /// prepare form string.
            /// </summary>
            /// <returns>return form string</returns>
            public string Post()
            {
                System.Web.HttpContext.Current.Response.Clear();
                StringBuilder responseForm = new StringBuilder();
                responseForm.Append(string.Format("<form name=\"{0}\" method=\"{1}\" action=\"{2}\" >", this.formName, this.method, this.url));
                for (int i = 0; i < this.inputs.Keys.Count; i++)
                {
                    responseForm.Append(string.Format("<input name=\"{0}\" type=\"hidden\" value=\"{1}\">", this.inputs.Keys[i], this.inputs[this.inputs.Keys[i]]));
                }

                responseForm.Append("</form>");
                responseForm.Append(string.Format("<script language='javascript'>document.{0}.submit();</script>", this.formName));
                return responseForm.ToString();
            }
        }
    }
}