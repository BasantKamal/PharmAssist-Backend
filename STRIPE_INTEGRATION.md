# Stripe Payment Integration for PharmAssist

This document outlines the implementation of Stripe payment processing in the PharmAssist backend system.

## Overview

The Stripe integration provides secure payment processing capabilities for pharmacy orders, including:
- Creating payment intents for order payments
- Handling payment confirmations and failures
- Webhook processing for payment status updates
- Support for multiple payment methods through Stripe

## Architecture

### Core Components

1. **Payment Entities**
   - `PaymentIntent` - Tracks Stripe payment intents
   - Updated `CustomerBasket` - Includes payment intent information
   - Updated `Order` - Linked to payment intents

2. **Services**
   - `IPaymentService` / `PaymentService` - Core payment logic
   - Integration with existing `OrderService`

3. **Controllers**
   - `PaymentsController` - API endpoints for payment operations

4. **DTOs**
   - `PaymentIntentDTO` - Payment response data
   - Updated `CustomerBasketDTO` - Includes payment fields

## Setup Instructions

### 1. Stripe Account Setup

1. Create a Stripe account at https://stripe.com
2. Get your API keys from the Stripe Dashboard:
   - Publishable Key (starts with `pk_`)
   - Secret Key (starts with `sk_`)
3. Set up webhooks for payment status updates

### 2. Configuration

Update your `appsettings.json` and `appsettings.Development.json` with your Stripe credentials:

```json
{
  "StripeSettings": {
    "PublishableKey": "pk_test_your_publishable_key_here",
    "SecretKey": "sk_test_your_secret_key_here",
    "WebhookSecret": "whsec_your_webhook_secret_here"
  }
}
```

**Important**: Never commit real API keys to version control. Use environment variables or secure configuration for production.

### 3. Webhook Configuration

Set up a webhook endpoint in your Stripe Dashboard:
- URL: `https://yourdomain.com/api/payments/webhook`
- Events to listen for:
  - `payment_intent.succeeded`
  - `payment_intent.payment_failed`

## API Endpoints

### Create or Update Payment Intent

```http
POST /api/payments/{basketId}
Authorization: Bearer {jwt_token}
```

**Response:**
```json
{
  "id": "basket_id",
  "items": [...],
  "deliveryMethodId": 1,
  "paymentIntentId": "pi_1234567890",
  "clientSecret": "pi_1234567890_secret_abc123"
}
```

### Webhook Endpoint

```http
POST /api/payments/webhook
Stripe-Signature: {stripe_signature_header}
```

This endpoint handles Stripe webhook events for payment status updates.

## Payment Flow

### 1. Customer Checkout Process

1. **Create/Update Payment Intent**
   ```
   Customer adds items to basket
   → Frontend calls POST /api/payments/{basketId}
   → Backend creates Stripe Payment Intent
   → Returns client secret to frontend
   ```

2. **Process Payment**
   ```
   Frontend uses Stripe.js with client secret
   → Customer enters payment details
   → Stripe processes payment
   → Stripe sends webhook to backend
   ```

3. **Order Completion**
   ```
   Webhook updates order status
   → Payment successful: Order status = "Payment Received"
   → Payment failed: Order status = "Payment Failed"
   ```

### 2. Backend Payment Processing

```csharp
// Create payment intent
var basket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);

// Handle webhook events
await _paymentService.UpdateOrderPaymentSucceeded(paymentIntentId);
await _paymentService.UpdateOrderPaymentFailed(paymentIntentId);
```

## Database Changes

### Updated Entities

1. **CustomerBasket**
   ```csharp
   public class CustomerBasket
   {
       public string Id { get; set; }
       public List<BasketItem> Items { get; set; }
       public int? DeliveryMethodId { get; set; }      // New
       public string PaymentIntentId { get; set; }     // New
       public string ClientSecret { get; set; }        // New
   }
   ```

2. **Order** (existing PaymentIntentId field)
   ```csharp
   public class Order : BaseEntity
   {
       // ... existing properties
       public string PaymentIntentId { get; set; } = string.Empty;
   }
   ```

## Security Considerations

1. **API Key Security**
   - Store secret keys securely
   - Use environment variables in production
   - Never expose secret keys in client-side code

2. **Webhook Security**
   - Verify webhook signatures
   - Use HTTPS endpoints
   - Validate event data

3. **Payment Data**
   - Never store credit card information
   - Log payment events for audit trails
   - Implement proper error handling

## Testing

### Test Mode

Stripe provides test mode with test cards:
- Success: `4242424242424242`
- Decline: `4000000000000002`
- Authentication Required: `4000002500003155`

### Testing Webhooks

Use Stripe CLI for local webhook testing:
```bash
stripe listen --forward-to localhost:7195/api/payments/webhook
```

## Error Handling

The system handles various payment scenarios:

1. **Payment Success**: Order status updated to "Payment Received"
2. **Payment Failure**: Order status updated to "Payment Failed"
3. **Network Issues**: Retry logic for webhook processing
4. **Invalid Baskets**: Proper error responses

## Frontend Integration

Your frontend should:

1. Call the payment intent endpoint to get client secret
2. Use Stripe.js to handle payment form
3. Confirm payment with the client secret
4. Handle payment results and redirect accordingly

Example frontend flow:
```javascript
// 1. Create payment intent
const response = await fetch(`/api/payments/${basketId}`, {
  method: 'POST',
  headers: { 'Authorization': `Bearer ${token}` }
});
const basket = await response.json();

// 2. Use Stripe.js to process payment
const stripe = Stripe('pk_test_...');
const result = await stripe.confirmCardPayment(basket.clientSecret, {
  payment_method: {
    card: cardElement,
    billing_details: { name: 'Customer Name' }
  }
});

// 3. Handle result
if (result.error) {
  // Payment failed
} else {
  // Payment succeeded - redirect to success page
}
```

## Production Deployment

1. Replace test keys with live keys
2. Set up production webhook endpoints
3. Configure proper logging and monitoring
4. Implement proper error handling and retries
5. Set up SSL/TLS certificates

## Support

For issues with the Stripe integration:
1. Check Stripe Dashboard for payment logs
2. Review application logs for errors
3. Verify webhook delivery in Stripe Dashboard
4. Test with Stripe's test cards in development

## Additional Features

Future enhancements could include:
- Support for multiple currencies
- Subscription payments for recurring orders
- Refund processing
- Payment method saving for returning customers
- Apple Pay / Google Pay integration 