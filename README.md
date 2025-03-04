# Inventory and Sales Management System

## Project Overview
This is a simple web-based inventory and sales management system built using **ASP.NET Core MVC, jQuery, and AJAX**. The system provides user authentication, product management (CRUD operations), and a sales invoice module with real-time calculations.

## Features
### 1. User Authentication (Login Screen)
- Users can log in using their username and password.
- Authentication and session management using **ASP.NET Core Identity**.
- AJAX-based login validation (no full-page reload).
- Redirects users to the dashboard upon successful login.

### 2. Product Management (CRUD Operations)
- Add, update, delete, and display products.
- Required fields:
  - **Product Name** (Text input)
  - **Price** (Numeric input)
  - **Stock Quantity** (Numeric input)
  - **Product Type** (Radio buttons: مخزني - Stored, خدمي - Service)
  - **Category Group** (Dropdown with predefined categories)
- Soft delete functionality.
- DataTable integration for displaying products with sorting and filtering.
- All operations performed via **AJAX for a seamless experience**.

### 3. Sales Invoice Module
- Create sales invoices with:
  - **Product Selection** (Dropdown: Choose from available products)
  - **Quantity** (Numeric input)
  - **Price** (Auto-filled from product details)
  - **Total Price** (Auto-calculated: Quantity × Price)
- Users can dynamically add multiple products via AJAX.
- Summary table showing added products and total price.
- Real-time invoice calculation.
- Saves invoices to the database and maintains a history table.

## Technologies Used
### **Back-end:**
- **ASP.NET Core MVC** (Latest Stable Version)
- **Entity Framework Core (EF Core)** for database operations
- **SQL Server** as the database
- **Dependency Injection** for maintainability
- **Repository Pattern** (optional, recommended for scalability)

### **Front-end:**
- **jQuery & AJAX** for async data operations
- **Bootstrap** for responsive design
- **DataTables** for sorting and filtering records

### **Security Features:**
- **User Authentication & Authorization**
- **CSRF Protection** (Anti-forgery tokens used in AJAX requests)
- **Input Validation** to prevent SQL Injection and XSS
- **Secure AJAX requests** using anti-forgery tokens

## Setup Instructions
### Prerequisites:
- .NET SDK (Latest stable version)
- SQL Server
- Visual Studio 2022 or VS Code (Recommended)
- Node.js (Optional for front-end package management)

### Installation Steps:
1. **Clone the repository:**
   ```sh
   git clone https://github.com/yourusername/InventorySalesSystem.git
   cd InventorySalesSystem
   ```

2. **Configure the database connection:**
   - Open `appsettings.json`
   - Update the `ConnectionStrings` with your SQL Server details.

3. **Apply migrations and seed data:**
   ```sh
   dotnet ef database update
   ```

4. **Run the application:**
   ```sh
   dotnet run
   ```
   OR using Visual Studio, press **F5** to start debugging.

5. **Access the application:**
   Open your browser and go to:
   ```
   http://localhost:5000
   ```



## Future Enhancements
- Implement **role-based access control (RBAC)** for better security.
- Add **unit tests and integration tests**.
- Improve **UI/UX** with additional styling and animations.
- Implement **export functionality** for product and invoice data (CSV/PDF).

## License
This project is licensed under the MIT License.

## Author
Developed by [Ahmed Mostafa Attia] - [Your GitHub Profile](https://github.com/ahmedmostafa-cell/)

---
### 🎉 Happy Coding! 🚀

