API Specification
This document provides a specification of all endpoints in this API.

---

### Products
**GET /api/Products**  
Returns a list of all products in an array with the following response:

* id: number
* productNumber: string
* name: string
* description: string
* price: number with two decimals
* category: string
* isDiscontinued: boolean

**GET /api/Products/{id}**  
Returns a specific product by ID.

**GET /api/Products/search?name={name}**  
Searches for products by name.

**GET /api/Products/number/{productNumber}**  
Returns a specific product by its product number.

**POST /api/Products**  
Creates a new product with the following body:

* productNumber: string
* name: string
* description: string
* price: number with two decimals
* category: string

**PUT /api/Products/{id}**  
Updates an existing product with a certain ID.

**DELETE /api/Products/{id}**  
Deletes a product with a certain ID.

---

### Customers
**GET /api/Customers**  
Returns a list of all customers in an array with the following response:

* id: number
* firstName: string
* lastName: string
* email: string
* phoneNumber: string
* address: string
* city: string
* postalCode: string
* country: string

**GET /api/Customers/{id}**  
Returns a specific customer by ID.

**GET /api/Customers/email/{email}**  
Finds a customer by email address.

**POST /api/Customers**  
Creates a new customer with the following body.

* firstName: string
* lastName: string
* email: string
* phoneNumber: string
* address: string
* city: string
* postalCode: string
* country: string

**PUT /api/Customers/{id}**  
Updates an existing customer with a certain ID.

---

### Orders

**GET /api/Orders**  
Returns a list of all orders in an array with the following response:

* id: number
* customerId: number
* customerName: string
* orderDate: date
* totalAmount: number
* orderItems: array of order item objects

**GET /api/Orders/{id}**  
Returns a specific order by ID.

**GET /api/Orders/customer/{customerId}**  
Returns all orders for a specific customer by their ID.

**POST /api/Orders**  
Creates a new order with the following body:

* customerId: number
* orderItems: array of order item information containing:  
productId: number  
quantity: number

**DELETE /api/Orders/{id}**  
Deletes an order with a certain ID.