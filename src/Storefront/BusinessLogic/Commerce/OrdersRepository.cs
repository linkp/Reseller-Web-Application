﻿// -----------------------------------------------------------------------
// <copyright file="OrdersRepository.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Storefront.BusinessLogic.Commerce
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Exceptions;
    using Models;
    using Newtonsoft.Json;
    using WindowsAzure.Storage.Table;

    /// <summary>
    /// Encapsulates persistence for orders during customer purchases.
    /// </summary>
    public class OrdersRepository : DomainObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrdersRepository"/> class.
        /// </summary>
        /// <param name="applicationDomain">An instance of the application domain.</param>
        public OrdersRepository(ApplicationDomain applicationDomain) : base(applicationDomain)
        {
        }

        /// <summary>
        /// Adds a new order into persistence.
        /// </summary>
        /// <param name="newOrder">The new customer order to add.</param>
        /// <returns>The resulting customer order that got added.</returns>
        public async Task<OrderViewModel> AddAsync(OrderViewModel newOrder)
        {
            newOrder.AssertNotNull(nameof(newOrder));

            CloudTable customerOrdersTable = await ApplicationDomain.AzureStorageService.GetCustomerOrdersTableAsync().ConfigureAwait(false);
            CustomerOrderTableEntity orderEntity = new CustomerOrderTableEntity(newOrder);

            TableResult insertionResult = await customerOrdersTable.ExecuteAsync(TableOperation.Insert(orderEntity)).ConfigureAwait(false);
            insertionResult.HttpStatusCode.AssertHttpResponseSuccess(ErrorCode.PersistenceFailure, "Failed to add customer order", insertionResult.Result);

            return newOrder;
        }

        /// <summary>
        /// Removes an order from persistence.
        /// </summary>
        /// <param name="orderId">Id of the order to remove.</param>
        /// <param name="customerId">Id of the customer whose order to remove.</param>
        /// <returns>A task.</returns>
        public async Task DeleteAsync(string orderId, string customerId)
        {
            orderId.AssertNotEmpty(nameof(orderId));
            customerId.AssertNotEmpty(nameof(customerId));

            CloudTable customerOrdersTable = await ApplicationDomain.AzureStorageService.GetCustomerOrdersTableAsync().ConfigureAwait(false);

            TableResult deletionResult = await customerOrdersTable.ExecuteAsync(
                TableOperation.Delete(new CustomerOrderTableEntity() { PartitionKey = customerId, RowKey = orderId, ETag = "*" })).ConfigureAwait(false);

            deletionResult.HttpStatusCode.AssertHttpResponseSuccess(ErrorCode.PersistenceFailure, "Failed to delete customer order", deletionResult.Result);
        }

        /// <summary>
        /// Retrieves specific order made by a customer from persistence.
        /// </summary>
        /// <param name="orderId">The order ID.</param>
        /// <param name="customerId">The customer ID.</param>
        /// <returns>The customer's order.</returns>
        public async Task<OrderViewModel> RetrieveAsync(string orderId, string customerId)
        {
            orderId.AssertNotEmpty(nameof(orderId));
            customerId.AssertNotEmpty(nameof(customerId));

            CloudTable customerOrdersTable = await ApplicationDomain.AzureStorageService.GetCustomerOrdersTableAsync().ConfigureAwait(false);

            string tableQueryFilter = TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, customerId),
                TableOperators.And,
                TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, orderId));

            TableQuery<CustomerOrderTableEntity> getCustomerOrdersQuery = new TableQuery<CustomerOrderTableEntity>().Where(tableQueryFilter);

            TableQuerySegment<CustomerOrderTableEntity> resultSegment = null;
            OrderViewModel customerOrder = null;
            do
            {
                resultSegment = await customerOrdersTable.ExecuteQuerySegmentedAsync(getCustomerOrdersQuery, resultSegment?.ContinuationToken).ConfigureAwait(false);

                foreach (CustomerOrderTableEntity orderResult in resultSegment.AsEnumerable())
                {
                    if (orderResult.RowKey == orderId)
                    {
                        customerOrder = JsonConvert.DeserializeObject<OrderViewModel>(orderResult.OrderBlob);
                    }
                }
            }
            while (resultSegment.ContinuationToken != null);

            return customerOrder;
        }

        /// <summary>
        /// An azure table entity that describes a customer order.
        /// </summary>
        private class CustomerOrderTableEntity : TableEntity
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CustomerOrderTableEntity"/> class.
            /// </summary>
            public CustomerOrderTableEntity()
            {
                RowKey = Guid.NewGuid().ToString();
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="CustomerOrderTableEntity"/> class.
            /// </summary>
            /// <param name="order">The order details.</param>            
            public CustomerOrderTableEntity(OrderViewModel order)
            {
                PartitionKey = order.CustomerId;
                RowKey = order.OrderId;
                OrderBlob = JsonConvert.SerializeObject(order, Formatting.None);
            }

            /// <summary>
            /// Gets or sets the blob which contains the order details. 
            /// </summary>
            public string OrderBlob { get; set; }
        }
    }
}