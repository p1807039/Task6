﻿using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Task6.Models;

namespace Task6.Controllers
{
    public class CartController : Controller
    {
        string key = "stripe api key id"; // input Stripe API Key here
        string customerId = "customer id key id"; // input customer id here
        string productPrice1 = "product1 price id"; // input product1 price here
        string productPrice2 = "product2 price id"; // input product2 price here
        string subscriptionId = "subscription id"; // input subscription id here
        string paymentIntent = "payment intent key"; // input payment intent here

        // GET: Cart/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Cart/Subscribe
        [HttpPost]
        public ActionResult Subscribe()
        {
            try
            {
                // Use Stripe's library to make request
                StripeConfiguration.ApiKey = key;
                StripeConfiguration.MaxNetworkRetries = 2;

                var options = new SubscriptionCreateOptions
                {
                    Customer = customerId,
                    Items = new List<SubscriptionItemOptions>
                    {
                        new SubscriptionItemOptions
                        {
                        Price = productPrice1,
                        },
                    },
                };

                var service = new SubscriptionService();
                Subscription subscription = service.Create(options);

                var model = new SubscriptionViewModel();
                model.SubscriptionId = subscription.Id;

                //return View("OrderStatus", model);
                return View("OrderStatus");
            }
            catch (StripeException e)
            {
                var x = new
                {
                    status = "Failed",
                    message = e.Message
                };
                return this.Json(x);
            }
        }

        // POST: Cart/Upgrade
        [HttpPost]
        public ActionResult Upgrade()
        {
            try
            {
                StripeConfiguration.ApiKey = key;
                StripeConfiguration.MaxNetworkRetries = 2;

                var service = new SubscriptionService();
                Subscription subscription = service.Get(subscriptionId);

                var items = new List<SubscriptionItemOptions> {
                new SubscriptionItemOptions {
                Id = subscription.Items.Data[0].Id,
                Price = productPrice2,
                },
            };

                var options = new SubscriptionUpdateOptions
                {
                    CancelAtPeriodEnd = false,
                    ProrationBehavior = "create_prorations",
                    Items = items,
                };
                subscription = service.Update(subscriptionId, options);

                return View("OrderStatus");
            }
            catch (StripeException e)
            {
                var x = new
                {
                    status = "Failed",
                    message = e.Message
                };
                return this.Json(x);
            }
        }

        // POST: Cart/Downgrade
        [HttpPost]
        public ActionResult Downgrade()
        {
            try
            {
                StripeConfiguration.ApiKey = key;
                StripeConfiguration.MaxNetworkRetries = 2;

                var service = new SubscriptionService();
                Subscription subscription = service.Get(subscriptionId);

                var items = new List<SubscriptionItemOptions> {
                new SubscriptionItemOptions {
                Id = subscription.Items.Data[0].Id,
                Price =productPrice1,
                },
            };

                var options = new SubscriptionUpdateOptions
                {
                    CancelAtPeriodEnd = false,
                    ProrationBehavior = "create_prorations",
                    Items = items,
                };
                subscription = service.Update(subscriptionId, options);

                return View("OrderStatus");
            }
            catch (StripeException e)
            {
                var x = new
                {
                    status = "Failed",
                    message = e.Message
                };
                return this.Json(x);
            }
        }

        // POST: Cart/Pause
        [HttpPost]
        public ActionResult Pause()
        {
            try
            {
                StripeConfiguration.ApiKey = key;
                StripeConfiguration.MaxNetworkRetries = 2;

                var options = new SubscriptionUpdateOptions
                {
                    PauseCollection = new SubscriptionPauseCollectionOptions
                    {
                        Behavior = "void",
                        ResumesAt = DateTime.Today.AddDays(1)
                    },
                };
                var service = new SubscriptionService();
                service.Update(subscriptionId, options);

                return View("OrderStatus");
            }
            catch (StripeException e)
            {
                var x = new
                {
                    status = "Failed",
                    message = e.Message
                };
                return this.Json(x);
            }
        }

        // POST: Cart/Resume
        [HttpPost]
        public ActionResult Resume()
        {
            try
            {
                StripeConfiguration.ApiKey = key;
                StripeConfiguration.MaxNetworkRetries = 2;

                var options = new SubscriptionUpdateOptions();
                options.AddExtraParam("pause_collection", "");
                var service = new SubscriptionService();
                service.Update(subscriptionId, options);

                return View("OrderStatus");
            }
            catch (StripeException e)
            {
                var x = new
                {
                    status = "Failed",
                    message = e.Message
                };
                return this.Json(x);
            }
        }

        // POST: Cart/Refund
        [HttpPost]
        public ActionResult Refund()
        {
            try
            {
                StripeConfiguration.ApiKey = key;
                StripeConfiguration.MaxNetworkRetries = 2;

                var refunds = new RefundService();
                var refundOptions = new RefundCreateOptions
                {
                    PaymentIntent = paymentIntent
                };
                var refund = refunds.Create(refundOptions);

                return View("OrderStatus");
            }
            catch (StripeException e)
            {
                var x = new
                {
                    status = "Failed",
                    message = e.Message
                };
                return this.Json(x);
            }
        }

        // POST: Cart/Cancel
        [HttpPost]
        public ActionResult Cancel()
        {
            try
            {
                StripeConfiguration.ApiKey = key;
                StripeConfiguration.MaxNetworkRetries = 2;

                var service = new SubscriptionService();
                var cancelOptions = new SubscriptionCancelOptions
                {
                    InvoiceNow = false,
                    Prorate = false,
                };
                Subscription subscription = service.Cancel(subscriptionId, cancelOptions);

                return View("OrderStatus");
            }
            catch (StripeException e)
            {
                var x = new
                {
                    status = "Failed",
                    message = e.Message
                };
                return this.Json(x);
            }
        }
    }
}
