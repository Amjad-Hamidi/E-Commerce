# TShop API Controllers & Endpoints - Detailed Documentation

This document describes all available API controllers in the TShop backend, their endpoints, authorization requirements, expected inputs/outputs, and special notes.

---

## 1. AccountController

**Base Route:** `/api/Account`

| Method | Route           | Description                          | Authorization | Request Body / Params                 | Response / Notes                                     |
|--------|-----------------|--------------------------------------|---------------|---------------------------------------|------------------------------------------------------|
| POST   | `/Register`     | Registers a new user with "Customer" role | Anonymous     | `RegisterDTO` (Username, Email, Password, ConfirmPassword) | 200 OK or 400 BadRequest (validation errors)        |
| GET    | `/ConfirmEmail` | Confirms user email via token       | Anonymous     | Query: `userId` (string), `token` (string) | 200 OK on success / 400 BadRequest                   |
| POST   | `/Login`        | User login and returns JWT token    | Anonymous     | `LoginDTO` (Username, Password)       | 200 OK with token and user info / 400 BadRequest     |
| POST   | `/Logout`       | Logs out authenticated user         | Authenticated | None                                  | 200 OK                                               |
| POST   | `/ChangePassword` | Changes password of authenticated user | Authenticated | `ChangePasswordDTO` (CurrentPassword, NewPassword, ConfirmNewPassword) | 200 OK or 400 BadRequest                            |
| POST   | `/ForgotPassword` | Sends password reset code to email | Anonymous     | `ForgotPasswordDTO` (Email)           | 200 OK or 400 BadRequest                             |
| POST   | `/ResetPassword` | Resets password with provided code  | Anonymous     | `ResetPasswordDTO` (Email, ResetCode, NewPassword, ConfirmNewPassword) | 200 OK or 400 BadRequest                            |

---

## 2. BrandsController

**Base Route:** `/api/Brands`

| Method | Route       | Description            | Authorization          | Request Body            | Response                                |
|--------|-------------|------------------------|-------------------------|-------------------------|----------------------------------------|
| GET    | `/`         | Retrieves all brands   | Public                  | None                    | 200 OK, returns list of `BrandDTO`     |
| GET    | `/{id}`     | Retrieves brand by ID  | Public                  | None                    | 200 OK with brand or 404 if not found  |
| POST   | `/`         | Creates a new brand    | Not explicitly restricted | `CreateBrandDTO`        | 201 Created with brand data / 400 BadRequest |
| PUT    | `/{id}`     | Updates existing brand | Not explicitly restricted | `CreateBrandDTO`        | 204 No Content / 400 BadRequest         |
| DELETE | `/{id}`     | Deletes brand by ID    | Not explicitly restricted | None                    | 204 No Content / 404 if not found       |

---

## 3. CartsController

**Base Route:** `/api/Carts`

| Method | Route       | Description                      | Authorization    | Request Body           | Response                                |
|--------|-------------|----------------------------------|------------------|------------------------|----------------------------------------|
| POST   | `/AddToCart`| Adds product to user’s cart     | Authenticated    | `AddToCartDTO` (ProductId, Quantity) | 200 OK with updated cart info / 400 BadRequest |
| GET    | `/GetCart`  | Retrieves authenticated user’s cart | Authenticated    | None                  | 200 OK with `CartDTO` including total price |

---

## 4. CategoriesController

**Base Route:** `/api/Categories`

| Method | Route       | Description                   | Authorization                      | Request Body          | Response                            |
|--------|-------------|-------------------------------|------------------------------------|------------------------|-------------------------------------|
| GET    | `/`         | Retrieves all categories       | Public                             | None                  | 200 OK with list of `CategoryDTO`   |
| GET    | `/{id}`     | Retrieves category by ID       | Public                             | None                  | 200 OK or 404 Not Found             |
| POST   | `/`         | Creates a new category         | Roles: SuperAdmin, Admin, Company  | `CreateCategoryDTO`   | 201 Created / 400 BadRequest        |
| PUT    | `/{id}`     | Updates existing category      | Roles: SuperAdmin, Admin, Company  | `CreateCategoryDTO`   | 204 No Content / 400 BadRequest     |
| DELETE | `/{id}`     | Deletes category by ID         | Roles: SuperAdmin, Admin, Company  | None                  | 204 No Content / 404 Not Found      |

---

## 5. CheckOutsController

**Base Route:** `/api/CheckOuts`

| Method | Route                  | Description                               | Authorization     | Request Body / Params              | Response                                    |
|--------|------------------------|-------------------------------------------|-------------------|------------------------------------|---------------------------------------------|
| GET    | `/Pay`                 | Processes payment (Stripe or Cash)        | Authenticated     | `PaymentRequest` in body (PaymentMethod: Cash/Visa) | 200 OK with Stripe session URL or redirect |
| GET    | `/Success/{orderId}`   | Handles successful payment and order finalization | Anonymous (AllowAnonymous) | Route: `orderId` (int)             | 200 OK with confirmation message            |

---

## 6. ProductsController

**Base Route:** `/api/Products`

| Method | Route       | Description                          | Authorization                | Request Body                     | Response                                 |
|--------|-------------|--------------------------------------|------------------------------|----------------------------------|------------------------------------------|
| GET    | `/`         | Retrieves all products (with query)  | Public                       | Query: `query`, `page`, `limit` | 200 OK with paginated list of products   |
| GET    | `/{id}`     | Retrieves product by ID              | Public                       | None                            | 200 OK or 404 Not Found                  |
| POST   | `/`         | Creates a new product                | Roles: SuperAdmin, Admin, Company | `ProductRequest` (form-data with image) | 201 Created / 400 BadRequest             |
| PUT    | `/{id}`     | Updates product                      | Roles: SuperAdmin, Admin, Company | `ProductUpdateRequest`          | 204 No Content / 404 Not Found           |
| DELETE | `/{id}`     | Deletes product                      | Roles: SuperAdmin, Admin, Company | None                            | 204 No Content / 404 Not Found           |

---

## 7. UsersController

**Base Route:** `/api/Users`

| Method | Route                     | Description                        | Authorization       | Request Body / Params         | Response                                  |
|--------|---------------------------|------------------------------------|---------------------|-------------------------------|-------------------------------------------|
| GET    | `/`                       | Retrieves all users                | Role: SuperAdmin    | None                          | 200 OK with list of `UserDto`            |
| GET    | `/{id}`                   | Retrieves a user by ID             | Role: SuperAdmin    | Route: `id`                   | 200 OK or 404 Not Found                   |
| POST   | `/Change Role {userId}`   | Changes role of a user             | Role: SuperAdmin    | Query: `newRoleName`          | 200 OK with result                        |
| PATCH  | `/LockUnLock/{userId}`    | Locks or unlocks a user account    | Role: SuperAdmin    | Route: `userId`               | 200 OK or 404 Not Found with message      |

---

## Summary of Authentication & Authorization

| Controller         | Auth Requirement                          | Notes                                         |
|--------------------|-------------------------------------------|-----------------------------------------------|
| AccountController  | Mostly Anonymous                          | JWT authentication, includes role claims     |
| BrandsController   | Public                                    | No explicit restrictions                     |
| CartsController    | Authenticated                             | Uses JWT claims                              |
| CategoriesController| Role-based (SuperAdmin/Admin/Company)   | Read is public                               |
| CheckOutsController| Authenticated except `/Success`          | Payment and order logic secured              |
| ProductsController | Role-based for modification, public read | Pagination & search supported                |
| UsersController    | Role: SuperAdmin                          | Full user management and role control        |

---

## Common Technologies & Practices

- **DTOs:** Used for all request/response formats.
- **Mapster:** For mapping between entities and DTOs.
- **Dependency Injection:** Used throughout for services.
- **JWT Authentication:** Token-based secured APIs.
- **Stripe Integration:** Payment handling (Visa).
- **Email Service:** Used for confirmations and notifications.
- **Role-based Access Control:** Enforced on sensitive routes.
- **Cancellation Tokens:** Implemented on selected operations.

---

*Made by: Amjad Hamidi - 2025*
