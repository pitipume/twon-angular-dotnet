# API Contract — Store

Base path: `/api/store`

All endpoints require: `Authorization: Bearer {accessToken}`

---

## POST /api/store/orders

Create a new order for one or more products.

**Request body:**
```json
{
  "productIds": ["uuid-1", "uuid-2"]
}
```

**Success (200):**
```json
{
  "code": "A001",
  "status": "success",
  "data": {
    "id": "order-uuid",
    "status": "PENDING",
    "totalTHB": 598,
    "orderItems": [
      { "productId": "uuid-1", "priceTHB": 299 },
      { "productId": "uuid-2", "priceTHB": 299 }
    ]
  }
}
```

**Errors:**
- `A409` — one or more products already owned by this user
- `A404` — one or more products not found or not published

---

## GET /api/store/orders/:orderId

Get order detail including checkout info (bank QR, account number).

**Success (200):**
```json
{
  "code": "A001",
  "status": "success",
  "data": {
    "id": "order-uuid",
    "status": "PENDING",
    "totalTHB": 299,
    "createdAt": "2025-04-01T00:00:00.000Z",
    "orderItems": [
      {
        "id": "uuid",
        "productId": "uuid",
        "priceTHB": 299,
        "product": { "title": "The Art of Tarot", "productType": "EBOOK" }
      }
    ],
    "payment": {
      "id": "uuid",
      "status": "PENDING",
      "amountTHB": 299
    },
    "checkoutInfo": {
      "bankName": "กสิกรไทย",
      "accountName": "ชื่อบัญชี",
      "accountNumber": "xxx-x-xxxxx-x",
      "qrImageUrl": "https://r2.dev/signed..."
    }
  }
}
```

Notes:
- `checkoutInfo` is null if admin has not configured payment yet
- `checkoutInfo.qrImageUrl` is a signed URL valid for **30 minutes**

**Errors:**
- `A403` — order does not belong to this user
- `A404` — order not found

---

## GET /api/store/orders `[Planned — not yet in controller]`

Get all orders for the logged-in user, newest first.

**Success (200):**
```json
{
  "code": "A001",
  "status": "success",
  "data": [
    {
      "id": "order-uuid",
      "status": "COMPLETED",
      "totalTHB": 299,
      "createdAt": "2025-04-01T00:00:00.000Z",
      "orderItems": [
        {
          "productId": "uuid",
          "priceTHB": 299,
          "product": { "title": "The Art of Tarot", "productType": "EBOOK" }
        }
      ]
    }
  ]
}
```
