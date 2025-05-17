# TShop API Controllers & Endpoints - Detailed Documentation

This document describes all available API controllers in the TShop backend, their endpoints, authorization requirements, expected inputs/outputs, and special notes.

---

## 1. AccountController

**Base Route:** `/api/Account`

| Method | Route           | Description                          | Authorization | Request Body / Params                 | Response / Notes                                     |
|--------|-----------------|------------------------------------|---------------|-------------------------------------|-----------------------------------------------------|
| POST   | `/Register`     | Registers a new user with "Customer" role | Anonymous     | `RegisterDTO` (Username, Email, Password, ConfirmPassword) | 200 OK or 400 BadRequest (validation errors)        |
| GET    | `/ConfirmEmail` | Confirms user email via token       | Anonymous     | Query: `userId` (string), `token` (string) | 200 OK on success / 400 BadRequest                   |
| POST   | `/Login`        | User login and returns JWT token    | Anonymous     | `LoginDTO` (Username, Password)    | 200 OK with token and user info / 400 BadRequest    |
| POST   | `/Logout`       | Logs out authenticated user         | Authenticated | None                                | 200 OK                                              |
| POST   | `/ChangePassword` | Changes password of authenticated user | Authenticated | `ChangePasswordDTO` (CurrentPassword, NewPassword, ConfirmNewPassword) | 200 OK or 400 BadRequest                            |
| POST   | `/ForgotPassword` | Sends password reset code to email | Anonymous     | `ForgotPasswordDTO` (Email)         | 200 OK or 400 BadRequest                            |
| POST   | `/ResetPassword` | Resets password with provided code  | Anonymous     | `ResetPasswordDTO` (Email, ResetCode, NewPassword, ConfirmNewPassword) | 200 OK or 400 BadRequest                            |

**Notes:**

- Uses ASP.NET Identity for user and role management.
- Sends emails for confirmation and password reset.
- JWT tokens include roles as claims.
- Validation applied on all relevant inputs.

---

## 2. BrandsController

**Base Route:** `/api/Brands`

| Method | Route       | Description            | Authorization          | Request Body            | Response                                |
|--------|-------------|------------------------|-----------------------|-------------------------|----------------------------------------|
| GET    | `/`         | Retrieves all brands   | Public                | None                    | 200 OK, returns list of `BrandDTO`    |
| GET    | `/{id}`     | Retrieves brand by ID  | Public                | None                    | 200 OK with brand or 404 if not found |
| POST   | `/`         | Creates a new brand    | Not explicitly restricted | `CreateBrandDTO`         | 201 Created with brand data / 400 BadRequest |
| PUT    | `/{id}`     | Updates existing brand | Not explicitly restricted | `CreateBrandDTO`         | 204 No Content / 400 BadRequest        |
| DELETE | `/{id}`     | Deletes brand by ID    | Not explicitly restricted | None                    | 204 No Content / 404 if not found      |

**Notes:**

- Uses `IBrandService` to perform CRUD operations.
- Mapster used for DTO to entity mapping.
- Supports cancellation tokens on create and delete.

---

## 3. CartsController

**Base Route:** `/api/Carts`

| Method | Route       | Description                      | Authorization    | Request Body           | Response                                |
|--------|-------------|--------------------------------|------------------|-----------------------|----------------------------------------|
| POST   | `/AddToCart`| Adds product to user’s cart     | Authenticated    | `AddToCartDTO` (ProductId, Quantity) | 200 OK with updated cart info / 400 BadRequest |
| GET    | `/GetCart`  | Retrieves authenticated user’s cart | Authenticated    | None                  | 200 OK with `CartDTO` including total price |

**Notes:**

- User identified via JWT claim.
- Uses `ICartService` to manage cart data.
- Returns cart with total price and product details.

---

## 4. CategoriesController

**Base Route:** `/api/Categories`

| Method | Route       | Description                   | Authorization                      | Request Body          | Response                            |
|--------|-------------|-------------------------------|----------------------------------|-----------------------|------------------------------------|
| GET    | `/`         | Retrieves all categories       | Public                           | None                  | 200 OK with list of `CategoryDTO`  |
| GET    | `/{id}`     | Retrieves category by ID       | Public                           | None                  | 200 OK or 404 Not Found             |
| POST   | `/`         | Creates a new category         | Roles: SuperAdmin, Admin, Company | `CreateCategoryDTO`     | 201 Created / 400 BadRequest        |
| PUT    | `/{id}`     | Updates existing category      | Roles: SuperAdmin, Admin, Company | `CreateCategoryDTO`     | 204 No Content / 400 BadRequest     |
| DELETE | `/{id}`     | Deletes category by ID         | Roles: SuperAdmin, Admin, Company | None                  | 204 No Content / 404 Not Found      |

**Notes:**

- Enforces role-based authorization for modifying categories.
- Uses `ICategoryService` for logic.
- Mapster used for DTO mapping.

---

## 5. CheckOutsController

**Base Route:** `/api/CheckOuts`

| Method | Route                  | Description                               | Authorization     | Request Body / Params              | Response                                    |
|--------|------------------------|-----------------------------------------|-------------------|----------------------------------|---------------------------------------------|
| GET    | `/Pay`                 | Processes payment (Stripe or Cash)       | Authenticated     | `PaymentRequest` (PaymentMethod: Cash/Visa) in Body | 200 OK with Stripe session URL or redirect to Success |
| GET    | `/Success/{orderId}`   | Handles successful payment and order finalization | Anonymous (AllowAnonymous) | `orderId` (int) in route         | 200 OK with confirmation message            |

### Pay Endpoint Details:

- Retrieves authenticated user's cart.
- If payment method is **Cash**, creates order, sets payment type, redirects to `Success`.
- If payment method is **Visa**, creates Stripe payment session, returns session URL.
- Calculates total price from cart (product price × quantity).
- Creates order with `Pending` status.
- Sets Stripe session and transaction IDs upon success.

### Success Endpoint Details:

- Retrieves order by ID.
- Retrieves user info and cart items.
- Creates `OrderItem` entries from cart items.
- Adjusts product quantities accordingly.
- Removes user's cart items after order creation.
- Updates order status:
  - For Cash: Marks order as received.
  - For Visa: Marks order as approved and stores Stripe payment intent.
- Sends order confirmation email to user.
- Returns success message.

**Notes:**

- Uses Stripe.NET SDK for payment session handling.
- Uses `IEmailSender` for sending emails.
- Order status updates based on payment method.
- Important: User must be logged in to create payment (except success route).

---

# Summary of Authentication & Authorization

| Controller       | Auth Requirement                | Notes                                 |
|------------------|--------------------------------|-------------------------------------|
| AccountController| Mostly Anonymous except Logout, ChangePassword | JWT authentication, role-based claims in tokens |
| BrandsController | Public                        | No explicit role-based restrictions  |
| CartsController  | Authenticated                | Uses JWT user claims                  |
| CategoriesController | Role-based (SuperAdmin/Admin/Company) on mutation | Read is public                      |
| CheckOutsController | Authenticated except Success (AllowAnonymous) | Payment and order processing secured |

---

# Common Technologies & Practices

- **DTOs:** Used throughout for request and response payloads.
- **Mapster:** For entity-DTO mapping.
- **Dependency Injection:** Services injected via constructor.
- **JWT:** For securing endpoints and user identity.
- **Stripe Integration:** For Visa payment processing.
- **Email Services:** For confirmation and notifications.
- **Role-based Access Control:** Enforced on sensitive data operations.
- **Cancellation Tokens:** Supported on some endpoints.

---

*Made by: Amjad Hamidi - 2025*

