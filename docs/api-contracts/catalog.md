# API Contract — Catalog

Base path: `/api/catalog`

Public endpoints — no auth required.

---

## GET /api/catalog/products

List all published products. Optionally filter by type.

**Query params:**
| Param | Values | Notes |
|---|---|---|
| `type` | `ebook` / `tarot_deck` | Optional — omit for all |

**Success (200):**
```json
{
  "code": "A001",
  "status": "success",
  "data": [
    {
      "id": "uuid",
      "mongoRefId": "mongo-id",
      "productType": "EBOOK",
      "title": "The Art of Tarot",
      "priceTHB": 299,
      "isPublished": true,
      "author": "Jane Doe",
      "description": "...",
      "coverImageUrl": "https://...",
      "language": "th",
      "categories": ["spirituality"],
      "tags": ["tarot", "beginner"]
    }
  ]
}
```

---

## GET /api/catalog/products/:id

Get single product detail (enriched with MongoDB metadata).

**Success (200):** Same shape as single item above, plus:
- Ebook: includes `totalPages`, `previewPages`
- Tarot deck: includes `cardCount`

**Errors:**
- `A404` — product not found or not published
