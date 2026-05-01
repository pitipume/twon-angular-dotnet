# API Contract — Library

Base path: `/api/library`

All endpoints require: `Authorization: Bearer {accessToken}`

---

## GET /api/library

Get all purchased items for the logged-in user.

**Success (200):**
```json
{
  "code": "A001",
  "status": "success",
  "data": [
    {
      "id": "uuid",
      "productId": "uuid",
      "grantedAt": "2025-04-01T00:00:00.000Z",
      "product": {
        "id": "uuid",
        "productType": "EBOOK",
        "title": "The Art of Tarot",
        "priceTHB": 299,
        "coverImageUrl": "https://...",
        "author": "Jane Doe"
      }
    }
  ]
}
```

---

## GET /api/library/ebook/:productId/session

Start an ebook reading session. Returns a signed PDF URL.

**Success (200):**
```json
{
  "code": "A001",
  "status": "success",
  "data": {
    "productId": "uuid",
    "signedUrl": "https://r2.dev/signed-url...",
    "totalPages": 240,
    "currentPage": 12
  }
}
```

Notes:
- `signedUrl` is a signed URL valid for **2 hours**
- `currentPage` is the last saved reading position (1 if never opened)
- `signedUrl` is never a raw R2 URL — always signed

**Errors:**
- `A403` — user does not own this product
- `A404` — product not found

---

## POST /api/library/ebook/:productId/progress

Save reading progress.

**Request body:**
```json
{ "currentPage": 42 }
```

**Success (200):**
```json
{ "code": "A001", "status": "success", "data": null }
```

---

## GET /api/library/tarot/:productId/session

Start a tarot session. Returns signed URLs for all card images.

**Success (200):**
```json
{
  "code": "A001",
  "status": "success",
  "data": {
    "productId": "uuid",
    "deckName": "Classic Rider Waite",
    "backImageUrl": "https://r2.dev/signed...",
    "cards": [
      {
        "cardNumber": 0,
        "name": "The Fool",
        "imageUrl": "https://r2.dev/signed...",
        "uprightMeaning": "New beginnings...",
        "reversedMeaning": "Recklessness...",
        "keywords": ["beginnings", "innocence", "spontaneity"]
      }
    ]
  }
}
```

Notes:
- `imageUrl` is a signed URL valid for **1 hour** per card
- `imageKey` (R2 key) is **never** included in the response
- `backImageUrl` is null if no back image was uploaded

**Errors:**
- `A403` — user does not own this product
