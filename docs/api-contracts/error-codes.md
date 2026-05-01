# API Response Codes

All responses follow this envelope:
```json
{ "code": "A001", "status": "success|failure", "message": "...", "data": {} }
```

---

## Code Reference

| Code | Status | HTTP | Meaning |
|---|---|---|---|
| `A001` | `success` | 200 | Request succeeded |
| `A002` | `failure` | 400 | Validation error or bad request |
| `A401` | `failure` | 401 | Unauthenticated — missing or invalid token |
| `A403` | `failure` | 403 | Forbidden — authenticated but not authorized |
| `A404` | `failure` | 404 | Resource not found |
| `A409` | `failure` | 409 | Conflict — e.g. duplicate purchase, wrong status |

---

## Examples

**Success:**
```json
{ "code": "A001", "status": "success", "data": { "accessToken": "eyJ..." } }
```

**Validation error:**
```json
{ "code": "A002", "status": "failure", "message": "Invalid or expired OTP." }
```

**Not found:**
```json
{ "code": "A404", "status": "failure", "message": "Product not found." }
```

**Conflict:**
```json
{ "code": "A409", "status": "failure", "message": "You already own this product." }
```

**Unauthorized:**
```json
{ "code": "A401", "status": "failure", "message": "Unauthorized." }
```

**Forbidden:**
```json
{ "code": "A403", "status": "failure", "message": "Access denied." }
```
