# Data Model — Library

## LibraryItem (PostgreSQL)

One record per user per product purchased. Created when admin approves payment.

| Field | Type | Notes |
|---|---|---|
| `id` | UUID | Primary key |
| `userId` | UUID | Foreign key → User |
| `productId` | UUID | Foreign key → Product |
| `orderId` | UUID | Which order granted this access |
| `grantedAt` | datetime | When access was granted (= payment approval time) |

Unique constraint: `(userId, productId)` — no duplicate access grants.

## ReadingProgress (MongoDB)

Tracks where a user is in an ebook. One doc per user per ebook.

| Field | Type | Notes |
|---|---|---|
| `_id` | ObjectId | |
| `userId` | string | PostgreSQL User ID |
| `productId` | string | PostgreSQL Product ID |
| `currentPage` | number | Last page the user was on |
| `updatedAt` | datetime | |

## Notes

- `LibraryItem` is created with `skipDuplicates: true` — safe to call multiple times
- Purchases are permanent — no expiry, no revocation (except future refund flow)
- `ReadingProgress` is optional — if missing, ebook reader starts at page 1
- Tarot decks have no reading progress — each session starts fresh
