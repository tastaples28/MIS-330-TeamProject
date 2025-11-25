# Campus ReHome - C# Backend API

A REST API backend built with C# .NET 8.0 that connects to Heroku JawsDB MySQL database.

## Prerequisites

- .NET 8.0 SDK or later
- Heroku JawsDB MySQL database (already configured)

## Project Structure

```
MIS-330-TeamProject/
├── api/                   # C# Backend API
│   ├── Controllers/       # API Controllers
│   │   ├── CustomersController.cs
│   │   ├── EmployeesController.cs
│   │   ├── FurnitureController.cs
│   │   └── TransactionsController.cs
│   ├── Models/            # Data models
│   │   ├── Customer.cs
│   │   ├── Employee.cs
│   │   ├── Furniture.cs
│   │   └── Transaction.cs
│   ├── Services/          # Database service
│   │   └── DatabaseService.cs
│   ├── Program.cs         # Application entry point
│   ├── appsettings.json   # Configuration (connection string)
│   └── CampusReHome.csproj # Project file
├── Client/                # Frontend HTML/CSS/JS files
└── Database/              # Database schema files
```

## Setup

1. **Install .NET SDK** (if not already installed)
   - Download from: https://dotnet.microsoft.com/download

2. **Restore Dependencies**
   ```bash
   dotnet restore
   ```

3. **Database Connection**
   - The connection string is already configured in `appsettings.json`
   - Make sure your JawsDB database tables are created (run `Database/schema_jawsdb.sql`)

## Running the Application

1. **Navigate to the api folder:**
   ```bash
   cd api
   ```

2. **Restore dependencies (first time only):**
   ```bash
   dotnet restore
   ```

3. **Start the API server:**
   ```bash
   dotnet run
   ```

4. **Access the application:**
   - Frontend: http://localhost:5000 (or the port shown in console)
   - API endpoints: http://localhost:5000/customers, /employees, /furniture, /transactions

## API Endpoints

### Customers
- `GET /customers?format=json` - Get all customers
- `GET /customers/{id}` - Get customer by ID
- `POST /customers` - Create new customer
- `PUT /customers/{id}` - Update customer
- `DELETE /customers/{id}` - Delete customer

### Employees
- `GET /employees?format=json` - Get all employees
- `GET /employees/{id}` - Get employee by ID
- `POST /employees` - Create new employee
- `PUT /employees/{id}` - Update employee
- `POST /employees/{id}/delete` - Deactivate employee

### Furniture
- `GET /furniture?format=json` - Get all furniture (with optional `search` and `category` query params)
- `GET /furniture/{id}` - Get furniture item by ID
- `POST /furniture` - Create new furniture
- `PUT /furniture/{id}` - Update furniture
- `POST /furniture/{id}/delete` - Soft delete furniture

### Transactions
- `GET /transactions?format=json` - Get all transactions
- `GET /transactions/{id}` - Get transaction by ID with details
- `POST /transactions` - Create new transaction (with automatic stock updates)

## How It Works

1. **Frontend (Client/)** - Static HTML files that make API calls using `fetch()`
2. **Backend (C# API)** - REST API that handles requests and connects to MySQL database
3. **Database (JawsDB)** - MySQL database hosted on Heroku

When you interact with the website:
- Frontend JavaScript makes API calls to the C# backend
- Backend processes requests and queries the JawsDB database
- Data is returned as JSON and displayed on the frontend

## Troubleshooting

### "Connection string not found"
- Check that `appsettings.json` exists and has the correct connection string

### "Cannot connect to database"
- Verify JawsDB is active (free tier may sleep after inactivity)
- Check connection string credentials in `appsettings.json`

### Port already in use
- Change the port in `Properties/launchSettings.json` or use:
  ```bash
  dotnet run --urls "http://localhost:5001"
  ```

