using MySql.Data.MySqlClient;
using CampusReHome.Models;
using System.Data;

namespace CampusReHome.Services;

public class DatabaseService
{
    private readonly string _connectionString;

    public DatabaseService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string not found");
    }

    private async Task<MySqlConnection> GetConnectionAsync()
    {
        var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }

    // Customer methods
    public async Task<List<Customer>> GetAllCustomersAsync()
    {
        var customers = new List<Customer>();
        using var connection = await GetConnectionAsync();
        var command = new MySqlCommand(
            "SELECT customer_id, first_name, last_name, email, phone, userpassword, address, join_date FROM customer ORDER BY customer_id",
            connection);
        
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            customers.Add(new Customer
            {
                CustomerId = reader.GetInt32("customer_id"),
                FirstName = reader.GetString("first_name"),
                LastName = reader.GetString("last_name"),
                Email = reader.GetString("email"),
                Phone = reader.IsDBNull("phone") ? null : reader.GetString("phone"),
                UserPassword = reader.GetString("userpassword"),
                Address = reader.IsDBNull("address") ? null : reader.GetString("address"),
                JoinDate = reader.GetDateTime("join_date")
            });
        }
        return customers;
    }

    public async Task<Customer?> GetCustomerByIdAsync(int id)
    {
        using var connection = await GetConnectionAsync();
        var command = new MySqlCommand(
            "SELECT customer_id, first_name, last_name, email, phone, userpassword, address, join_date FROM customer WHERE customer_id = @id",
            connection);
        command.Parameters.AddWithValue("@id", id);
        
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Customer
            {
                CustomerId = reader.GetInt32("customer_id"),
                FirstName = reader.GetString("first_name"),
                LastName = reader.GetString("last_name"),
                Email = reader.GetString("email"),
                Phone = reader.IsDBNull("phone") ? null : reader.GetString("phone"),
                UserPassword = reader.GetString("userpassword"),
                Address = reader.IsDBNull("address") ? null : reader.GetString("address"),
                JoinDate = reader.GetDateTime("join_date")
            };
        }
        return null;
    }

    public async Task<int> CreateCustomerAsync(Customer customer)
    {
        // Validate required fields
        if (string.IsNullOrWhiteSpace(customer.FirstName))
            throw new ArgumentException("First name is required", nameof(customer));
        if (string.IsNullOrWhiteSpace(customer.LastName))
            throw new ArgumentException("Last name is required", nameof(customer));
        if (string.IsNullOrWhiteSpace(customer.Email))
            throw new ArgumentException("Email is required", nameof(customer));
        if (string.IsNullOrWhiteSpace(customer.UserPassword))
            throw new ArgumentException("Password is required", nameof(customer));
        
        using var connection = await GetConnectionAsync();
        var command = new MySqlCommand(
            "INSERT INTO customer (first_name, last_name, email, phone, userpassword, address) VALUES (@firstName, @lastName, @email, @phone, @password, @address)",
            connection);
        
        // Ensure values are not null
        command.Parameters.AddWithValue("@firstName", customer.FirstName ?? string.Empty);
        command.Parameters.AddWithValue("@lastName", customer.LastName ?? string.Empty);
        command.Parameters.AddWithValue("@email", customer.Email ?? string.Empty);
        command.Parameters.AddWithValue("@phone", (object?)customer.Phone ?? DBNull.Value);
        // Ensure password is not null or empty
        var password = customer.UserPassword ?? string.Empty;
        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentException("Password cannot be empty", nameof(customer));
        }
        command.Parameters.AddWithValue("@password", password);
        Console.WriteLine($"Inserting password: Length={password.Length}, Value={(password.Length > 0 ? "***" : "EMPTY")}"); // Debug
        command.Parameters.AddWithValue("@address", (object?)customer.Address ?? DBNull.Value);
        
        await command.ExecuteNonQueryAsync();
        return (int)command.LastInsertedId;
    }

    public async Task<bool> UpdateCustomerAsync(int id, Customer customer)
    {
        using var connection = await GetConnectionAsync();
        var command = new MySqlCommand(
            "UPDATE customer SET first_name = @firstName, last_name = @lastName, email = @email, phone = @phone, userpassword = @password, address = @address WHERE customer_id = @id",
            connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@firstName", customer.FirstName);
        command.Parameters.AddWithValue("@lastName", customer.LastName);
        command.Parameters.AddWithValue("@email", customer.Email);
        command.Parameters.AddWithValue("@phone", (object?)customer.Phone ?? DBNull.Value);
        command.Parameters.AddWithValue("@password", customer.UserPassword);
        command.Parameters.AddWithValue("@address", (object?)customer.Address ?? DBNull.Value);
        
        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteCustomerAsync(int id)
    {
        using var connection = await GetConnectionAsync();
        var command = new MySqlCommand("DELETE FROM customer WHERE customer_id = @id", connection);
        command.Parameters.AddWithValue("@id", id);
        return await command.ExecuteNonQueryAsync() > 0;
    }

    // Employee methods
    public async Task<List<Employee>> GetAllEmployeesAsync()
    {
        var employees = new List<Employee>();
        using var connection = await GetConnectionAsync();
        var command = new MySqlCommand(
            "SELECT employee_id, first_name, last_name, email, phone_number, userpassword, role, hire_date, is_active FROM employee ORDER BY employee_id",
            connection);
        
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            employees.Add(new Employee
            {
                EmployeeId = reader.GetInt32("employee_id"),
                FirstName = reader.GetString("first_name"),
                LastName = reader.GetString("last_name"),
                Email = reader.GetString("email"),
                PhoneNumber = reader.IsDBNull("phone_number") ? null : reader.GetString("phone_number"),
                UserPassword = reader.GetString("userpassword"),
                Role = reader.GetString("role"),
                HireDate = reader.IsDBNull("hire_date") ? null : reader.GetDateTime("hire_date"),
                IsActive = reader.GetBoolean("is_active")
            });
        }
        return employees;
    }

    public async Task<Employee?> GetEmployeeByIdAsync(int id)
    {
        using var connection = await GetConnectionAsync();
        var command = new MySqlCommand(
            "SELECT employee_id, first_name, last_name, email, phone_number, userpassword, role, hire_date, is_active FROM employee WHERE employee_id = @id",
            connection);
        command.Parameters.AddWithValue("@id", id);
        
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Employee
            {
                EmployeeId = reader.GetInt32("employee_id"),
                FirstName = reader.GetString("first_name"),
                LastName = reader.GetString("last_name"),
                Email = reader.GetString("email"),
                PhoneNumber = reader.IsDBNull("phone_number") ? null : reader.GetString("phone_number"),
                UserPassword = reader.GetString("userpassword"),
                Role = reader.GetString("role"),
                HireDate = reader.IsDBNull("hire_date") ? null : reader.GetDateTime("hire_date"),
                IsActive = reader.GetBoolean("is_active")
            };
        }
        return null;
    }

    public async Task<int> CreateEmployeeAsync(Employee employee)
    {
        // Validate required fields
        if (string.IsNullOrWhiteSpace(employee.FirstName))
            throw new ArgumentException("First name is required", nameof(employee));
        if (string.IsNullOrWhiteSpace(employee.LastName))
            throw new ArgumentException("Last name is required", nameof(employee));
        if (string.IsNullOrWhiteSpace(employee.Email))
            throw new ArgumentException("Email is required", nameof(employee));
        if (string.IsNullOrWhiteSpace(employee.UserPassword))
            throw new ArgumentException("Password is required", nameof(employee));
        if (string.IsNullOrWhiteSpace(employee.Role))
            throw new ArgumentException("Role is required", nameof(employee));
        
        using var connection = await GetConnectionAsync();
        var command = new MySqlCommand(
            "INSERT INTO employee (first_name, last_name, email, phone_number, userpassword, role, hire_date, is_active) VALUES (@firstName, @lastName, @email, @phone, @password, @role, @hireDate, @isActive)",
            connection);
        
        // Ensure values are not null
        command.Parameters.AddWithValue("@firstName", employee.FirstName ?? string.Empty);
        command.Parameters.AddWithValue("@lastName", employee.LastName ?? string.Empty);
        command.Parameters.AddWithValue("@email", employee.Email ?? string.Empty);
        command.Parameters.AddWithValue("@phone", (object?)employee.PhoneNumber ?? DBNull.Value);
        command.Parameters.AddWithValue("@password", employee.UserPassword ?? string.Empty);
        command.Parameters.AddWithValue("@role", employee.Role ?? string.Empty);
        command.Parameters.AddWithValue("@hireDate", (object?)employee.HireDate ?? DBNull.Value);
        command.Parameters.AddWithValue("@isActive", employee.IsActive);
        
        await command.ExecuteNonQueryAsync();
        return (int)command.LastInsertedId;
    }

    public async Task<bool> UpdateEmployeeAsync(int id, Employee employee)
    {
        using var connection = await GetConnectionAsync();
        var command = new MySqlCommand(
            "UPDATE employee SET first_name = @firstName, last_name = @lastName, email = @email, phone_number = @phone, userpassword = @password, role = @role, hire_date = @hireDate, is_active = @isActive WHERE employee_id = @id",
            connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@firstName", employee.FirstName);
        command.Parameters.AddWithValue("@lastName", employee.LastName);
        command.Parameters.AddWithValue("@email", employee.Email);
        command.Parameters.AddWithValue("@phone", (object?)employee.PhoneNumber ?? DBNull.Value);
        command.Parameters.AddWithValue("@password", employee.UserPassword);
        command.Parameters.AddWithValue("@role", employee.Role);
        command.Parameters.AddWithValue("@hireDate", (object?)employee.HireDate ?? DBNull.Value);
        command.Parameters.AddWithValue("@isActive", employee.IsActive);
        
        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeactivateEmployeeAsync(int id)
    {
        using var connection = await GetConnectionAsync();
        var command = new MySqlCommand("UPDATE employee SET is_active = 0 WHERE employee_id = @id", connection);
        command.Parameters.AddWithValue("@id", id);
        return await command.ExecuteNonQueryAsync() > 0;
    }

    // Furniture methods
    public async Task<List<Furniture>> GetAllFurnitureAsync(string? search = null, string? category = null, decimal? minPrice = null, decimal? maxPrice = null)
    {
        var furniture = new List<Furniture>();
        using var connection = await GetConnectionAsync();
        
        var query = "SELECT furniture_id, item_name, category, description, item_condition, price, stock_quantity, is_active FROM furniture WHERE 1=1";
        var command = new MySqlCommand(query, connection);
        
        if (!string.IsNullOrEmpty(search))
        {
            query += " AND (item_name LIKE @search OR description LIKE @search)";
            command.CommandText = query;
            command.Parameters.AddWithValue("@search", $"%{search}%");
        }
        
        if (!string.IsNullOrEmpty(category))
        {
            query += " AND category = @category";
            command.CommandText = query;
            command.Parameters.AddWithValue("@category", category);
        }
        
        if (minPrice.HasValue)
        {
            query += " AND price >= @minPrice";
            command.CommandText = query;
            command.Parameters.AddWithValue("@minPrice", minPrice.Value);
        }
        
        if (maxPrice.HasValue)
        {
            query += " AND price <= @maxPrice";
            command.CommandText = query;
            command.Parameters.AddWithValue("@maxPrice", maxPrice.Value);
        }
        
        query += " AND is_active = 1 ORDER BY furniture_id";
        command.CommandText = query;
        
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            furniture.Add(new Furniture
            {
                FurnitureId = reader.GetInt32("furniture_id"),
                ItemName = reader.GetString("item_name"),
                Category = reader.GetString("category"),
                Description = reader.IsDBNull("description") ? null : reader.GetString("description"),
                ItemCondition = reader.GetString("item_condition"),
                Price = reader.GetDecimal("price"),
                StockQuantity = reader.GetInt32("stock_quantity"),
                IsActive = reader.GetBoolean("is_active")
            });
        }
        return furniture;
    }

    public async Task<List<string>> GetCategoriesAsync()
    {
        var categories = new List<string>();
        using var connection = await GetConnectionAsync();
        var command = new MySqlCommand("SELECT DISTINCT category FROM furniture WHERE is_active = 1 ORDER BY category", connection);
        
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            categories.Add(reader.GetString("category"));
        }
        return categories;
    }

    public async Task<Furniture?> GetFurnitureByIdAsync(int id)
    {
        using var connection = await GetConnectionAsync();
        var command = new MySqlCommand(
            "SELECT furniture_id, item_name, category, description, item_condition, price, stock_quantity, is_active FROM furniture WHERE furniture_id = @id",
            connection);
        command.Parameters.AddWithValue("@id", id);
        
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Furniture
            {
                FurnitureId = reader.GetInt32("furniture_id"),
                ItemName = reader.GetString("item_name"),
                Category = reader.GetString("category"),
                Description = reader.IsDBNull("description") ? null : reader.GetString("description"),
                ItemCondition = reader.GetString("item_condition"),
                Price = reader.GetDecimal("price"),
                StockQuantity = reader.GetInt32("stock_quantity"),
                IsActive = reader.GetBoolean("is_active")
            };
        }
        return null;
    }

    public async Task<int> CreateFurnitureAsync(Furniture furniture)
    {
        using var connection = await GetConnectionAsync();
        var command = new MySqlCommand(
            "INSERT INTO furniture (item_name, category, description, item_condition, price, stock_quantity, is_active) VALUES (@itemName, @category, @description, @condition, @price, @stock, @isActive)",
            connection);
        command.Parameters.AddWithValue("@itemName", furniture.ItemName);
        command.Parameters.AddWithValue("@category", furniture.Category);
        command.Parameters.AddWithValue("@description", (object?)furniture.Description ?? DBNull.Value);
        command.Parameters.AddWithValue("@condition", furniture.ItemCondition);
        command.Parameters.AddWithValue("@price", furniture.Price);
        command.Parameters.AddWithValue("@stock", furniture.StockQuantity);
        command.Parameters.AddWithValue("@isActive", furniture.IsActive);
        
        await command.ExecuteNonQueryAsync();
        return (int)command.LastInsertedId;
    }

    public async Task<bool> UpdateFurnitureAsync(int id, Furniture furniture)
    {
        using var connection = await GetConnectionAsync();
        var command = new MySqlCommand(
            "UPDATE furniture SET item_name = @itemName, category = @category, description = @description, item_condition = @condition, price = @price, stock_quantity = @stock, is_active = @isActive WHERE furniture_id = @id",
            connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@itemName", furniture.ItemName);
        command.Parameters.AddWithValue("@category", furniture.Category);
        command.Parameters.AddWithValue("@description", (object?)furniture.Description ?? DBNull.Value);
        command.Parameters.AddWithValue("@condition", furniture.ItemCondition);
        command.Parameters.AddWithValue("@price", furniture.Price);
        command.Parameters.AddWithValue("@stock", furniture.StockQuantity);
        command.Parameters.AddWithValue("@isActive", furniture.IsActive);
        
        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteFurnitureAsync(int id)
    {
        using var connection = await GetConnectionAsync();
        var command = new MySqlCommand("UPDATE furniture SET is_active = 0 WHERE furniture_id = @id", connection);
        command.Parameters.AddWithValue("@id", id);
        return await command.ExecuteNonQueryAsync() > 0;
    }

    // Transaction methods
    public async Task<List<Transaction>> GetAllTransactionsAsync()
    {
        var transactions = new List<Transaction>();
        using var connection = await GetConnectionAsync();
        var command = new MySqlCommand(
            @"SELECT t.transaction_id, t.customer_id, t.employee_id, t.transaction_date, t.total_amount, t.payment_method, t.status,
                     c.first_name AS customer_first_name, c.last_name AS customer_last_name,
                     e.first_name AS employee_first_name, e.last_name AS employee_last_name
              FROM CustomerPurchaseTransaction t
              LEFT JOIN customer c ON t.customer_id = c.customer_id
              LEFT JOIN employee e ON t.employee_id = e.employee_id
              ORDER BY t.transaction_id DESC",
            connection);
        
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            transactions.Add(new Transaction
            {
                TransactionId = reader.GetInt32("transaction_id"),
                CustomerId = reader.GetInt32("customer_id"),
                EmployeeId = reader.GetInt32("employee_id"),
                TransactionDate = reader.GetDateTime("transaction_date"),
                TotalAmount = reader.GetDecimal("total_amount"),
                PaymentMethod = reader.GetString("payment_method"),
                Status = reader.GetString("status"),
                CustomerFirstName = reader.IsDBNull("customer_first_name") ? null : reader.GetString("customer_first_name"),
                CustomerLastName = reader.IsDBNull("customer_last_name") ? null : reader.GetString("customer_last_name"),
                EmployeeFirstName = reader.IsDBNull("employee_first_name") ? null : reader.GetString("employee_first_name"),
                EmployeeLastName = reader.IsDBNull("employee_last_name") ? null : reader.GetString("employee_last_name")
            });
        }
        return transactions;
    }

    public async Task<List<Transaction>> GetTransactionsByCustomerIdAsync(int customerId)
    {
        var transactions = new List<Transaction>();
        using var connection = await GetConnectionAsync();
        var command = new MySqlCommand(
            @"SELECT t.transaction_id, t.customer_id, t.employee_id, t.transaction_date, t.total_amount, t.payment_method, t.status,
                     c.first_name AS customer_first_name, c.last_name AS customer_last_name,
                     e.first_name AS employee_first_name, e.last_name AS employee_last_name
              FROM CustomerPurchaseTransaction t
              LEFT JOIN customer c ON t.customer_id = c.customer_id
              LEFT JOIN employee e ON t.employee_id = e.employee_id
              WHERE t.customer_id = @customerId
              ORDER BY t.transaction_date DESC, t.transaction_id DESC",
            connection);
        command.Parameters.AddWithValue("@customerId", customerId);
        
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var transaction = new Transaction
            {
                TransactionId = reader.GetInt32("transaction_id"),
                CustomerId = reader.GetInt32("customer_id"),
                EmployeeId = reader.GetInt32("employee_id"),
                TransactionDate = reader.GetDateTime("transaction_date"),
                TotalAmount = reader.GetDecimal("total_amount"),
                PaymentMethod = reader.GetString("payment_method"),
                Status = reader.GetString("status"),
                CustomerFirstName = reader.IsDBNull("customer_first_name") ? null : reader.GetString("customer_first_name"),
                CustomerLastName = reader.IsDBNull("customer_last_name") ? null : reader.GetString("customer_last_name"),
                EmployeeFirstName = reader.IsDBNull("employee_first_name") ? null : reader.GetString("employee_first_name"),
                EmployeeLastName = reader.IsDBNull("employee_last_name") ? null : reader.GetString("employee_last_name")
            };
            
            // Load transaction details
            transaction.Details = await GetTransactionDetailsAsync(transaction.TransactionId);
            transactions.Add(transaction);
        }
        return transactions;
    }

    public async Task<Transaction?> GetTransactionByIdAsync(int id)
    {
        using var connection = await GetConnectionAsync();
        var command = new MySqlCommand(
            @"SELECT t.transaction_id, t.customer_id, t.employee_id, t.transaction_date, t.total_amount, t.payment_method, t.status,
                     c.first_name AS customer_first_name, c.last_name AS customer_last_name,
                     e.first_name AS employee_first_name, e.last_name AS employee_last_name
              FROM CustomerPurchaseTransaction t
              LEFT JOIN customer c ON t.customer_id = c.customer_id
              LEFT JOIN employee e ON t.employee_id = e.employee_id
              WHERE t.transaction_id = @id",
            connection);
        command.Parameters.AddWithValue("@id", id);
        
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            var transaction = new Transaction
            {
                TransactionId = reader.GetInt32("transaction_id"),
                CustomerId = reader.GetInt32("customer_id"),
                EmployeeId = reader.GetInt32("employee_id"),
                TransactionDate = reader.GetDateTime("transaction_date"),
                TotalAmount = reader.GetDecimal("total_amount"),
                PaymentMethod = reader.GetString("payment_method"),
                Status = reader.GetString("status"),
                CustomerFirstName = reader.IsDBNull("customer_first_name") ? null : reader.GetString("customer_first_name"),
                CustomerLastName = reader.IsDBNull("customer_last_name") ? null : reader.GetString("customer_last_name"),
                EmployeeFirstName = reader.IsDBNull("employee_first_name") ? null : reader.GetString("employee_first_name"),
                EmployeeLastName = reader.IsDBNull("employee_last_name") ? null : reader.GetString("employee_last_name")
            };
            
            // Load transaction details
            transaction.Details = await GetTransactionDetailsAsync(id);
            return transaction;
        }
        return null;
    }

    public async Task<List<TransactionDetail>> GetTransactionDetailsAsync(int transactionId)
    {
        var details = new List<TransactionDetail>();
        using var connection = await GetConnectionAsync();
        var command = new MySqlCommand(
            @"SELECT td.transaction_detail_id, td.transaction_id, td.furniture_id, td.quantity, td.sale_price,
                     f.item_name AS furniture_name
              FROM TransactionDetail td
              LEFT JOIN furniture f ON td.furniture_id = f.furniture_id
              WHERE td.transaction_id = @transactionId",
            connection);
        command.Parameters.AddWithValue("@transactionId", transactionId);
        
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            details.Add(new TransactionDetail
            {
                TransactionDetailId = reader.GetInt32("transaction_detail_id"),
                TransactionId = reader.GetInt32("transaction_id"),
                FurnitureId = reader.GetInt32("furniture_id"),
                Quantity = reader.GetInt32("quantity"),
                SalePrice = reader.GetDecimal("sale_price"),
                FurnitureName = reader.IsDBNull("furniture_name") ? null : reader.GetString("furniture_name")
            });
        }
        return details;
    }

    public async Task<int> CreateTransactionAsync(Transaction transaction, List<TransactionDetail> details)
    {
        using var connection = await GetConnectionAsync();
        // Connection is already open from GetConnectionAsync()
        using var trans = await connection.BeginTransactionAsync();
        
        try
        {
            // Create transaction
            var command = new MySqlCommand(
                "INSERT INTO CustomerPurchaseTransaction (customer_id, employee_id, transaction_date, total_amount, payment_method, status) VALUES (@customerId, @employeeId, @date, @total, @payment, @status)",
                connection, (MySqlTransaction)trans);
            command.Parameters.AddWithValue("@customerId", transaction.CustomerId);
            command.Parameters.AddWithValue("@employeeId", transaction.EmployeeId);
            command.Parameters.AddWithValue("@date", transaction.TransactionDate);
            command.Parameters.AddWithValue("@total", transaction.TotalAmount);
            command.Parameters.AddWithValue("@payment", transaction.PaymentMethod);
            command.Parameters.AddWithValue("@status", transaction.Status);
            
            await command.ExecuteNonQueryAsync();
            var transactionId = (int)command.LastInsertedId;
            
            // Create transaction details and update stock
            foreach (var detail in details)
            {
                // Insert detail
                var detailCommand = new MySqlCommand(
                    "INSERT INTO TransactionDetail (transaction_id, furniture_id, quantity, sale_price) VALUES (@transactionId, @furnitureId, @quantity, @salePrice)",
                    connection, (MySqlTransaction)trans);
                detailCommand.Parameters.AddWithValue("@transactionId", transactionId);
                detailCommand.Parameters.AddWithValue("@furnitureId", detail.FurnitureId);
                detailCommand.Parameters.AddWithValue("@quantity", detail.Quantity);
                detailCommand.Parameters.AddWithValue("@salePrice", detail.SalePrice);
                await detailCommand.ExecuteNonQueryAsync();
                
                // Update stock
                var stockCommand = new MySqlCommand(
                    "UPDATE furniture SET stock_quantity = stock_quantity - @quantity WHERE furniture_id = @furnitureId",
                    connection, (MySqlTransaction)trans);
                stockCommand.Parameters.AddWithValue("@quantity", detail.Quantity);
                stockCommand.Parameters.AddWithValue("@furnitureId", detail.FurnitureId);
                await stockCommand.ExecuteNonQueryAsync();
            }
            
            await trans.CommitAsync();
            return transactionId;
        }
        catch
        {
            await trans.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> UpdateTransactionStatusAsync(int id, string status)
    {
        using var connection = await GetConnectionAsync();
        var command = new MySqlCommand(
            "UPDATE CustomerPurchaseTransaction SET status = @status WHERE transaction_id = @id",
            connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@status", status);
        
        return await command.ExecuteNonQueryAsync() > 0;
    }
}

