# Campus ReHome Project Status Checklist

## Database Requirements Collection
- [x] Database requirements identified - All required tables defined in schema
- [ ] ERD documentation created - No ERD file found
- [x] Relational schema designed - Schema includes all required tables with relationships

## ERD and Relational Schema Design
- [ ] ERD diagram file - No ERD image/document found
- [x] Schema SQL file - schema.sql and schema_jawsdb.sql exist
- [x] Table relationships defined - Foreign keys implemented in schema

## Database Implementation
- [x] Customer table - Created with all required fields, primary key, constraints
- [x] Furniture table - Created with all required fields, primary key, is_active flag
- [x] CustomerPurchaseTransaction table - Created with foreign keys to customer and employee
- [x] TransactionDetail table - Created with foreign keys to transaction and furniture
- [x] Employee table - Created with all required fields, is_active flag
- [x] Foreign key constraints - All relationships properly defined
- [x] Database connection configured - JawsDB connection string in appsettings.json
- [x] Database deployed - Schema created on JawsDB

## Data Loading / Seed Data
- [ ] Seed data SQL file - No seed data file found
- [ ] Sample customer data - No seed data
- [ ] Sample furniture data - No seed data
- [ ] Sample employee data - No seed data
- [ ] Sample transaction data - No seed data

## Front-end Pages and Components
- [x] Home page (index.html) - Created with navigation cards
- [x] Customers page - Created with table and add form
- [x] Employees page - Created with table and add form
- [x] Furniture page - Created with table, search, filter, and add form
- [x] Transactions page - Created with table and add form
- [x] Navigation menu - Bootstrap navbar on all pages
- [x] Responsive design - Bootstrap 5 used throughout
- [ ] Login page - Not found
- [ ] Registration page - Not found
- [ ] User profile page - Not found

## User Management Features
- [ ] User registration page - Not implemented
- [ ] User login page - Not implemented
- [ ] Authentication system - No login/logout functionality
- [ ] Session management - Not implemented
- [ ] Password hashing - Passwords stored as plaintext (security issue)
- [x] Customer CRUD operations - Create, Read, Delete implemented
- [x] Employee CRUD operations - Create, Read, Deactivate implemented
- [ ] User profiles - No profile viewing/editing page
- [ ] Password reset - Not implemented

## Furniture Listing & Management Features
- [x] Create furniture listing - Modal form with all fields
- [x] Read furniture listings - Table displays all active furniture
- [ ] Edit furniture listing - Edit button exists but no edit form/modal
- [x] Delete furniture listing - Soft delete (sets is_active = false)
- [x] Furniture detail view - Data displayed in table
- [x] Category management - Categories stored and filterable

## Search and Filtering Features
- [x] Keyword search - Search by item name or description implemented
- [x] Category filtering - Filter by category dropdown implemented
- [ ] Sorting options - No sorting functionality visible
- [ ] Price range filter - Not implemented
- [ ] Condition filter - Not implemented
- [x] Search/filter UI - Search box and category dropdown on furniture page

## Customer Purchase Transaction Management Features
- [x] Order placement - Transaction creation form with multiple items
- [x] Transaction creation - POST endpoint with transaction details
- [x] Transaction history - List view showing all transactions
- [x] Transaction details - View transaction with customer/employee names
- [x] Multiple items per transaction - Dynamic item list in form
- [x] Payment method selection - Dropdown with payment options
- [x] Transaction status - Status field (Pending, Completed, Cancelled)
- [ ] Order confirmation page - Not implemented (only success alert)
- [ ] Transaction editing - Not implemented
- [ ] Transaction cancellation - Status can be set but no cancel workflow

## Inventory Management Features
- [x] Stock quantity tracking - stock_quantity field in furniture table
- [x] Automatic stock updates - Stock decreases when transaction created
- [x] Stock display - Stock quantity shown in furniture table
- [x] Inventory listing - All furniture with stock shown
- [ ] Low stock alerts - Not implemented
- [ ] Inventory reports - Not implemented
- [ ] Stock adjustment (manual) - Not implemented
- [x] Soft delete for inventory - is_active flag prevents showing deleted items

## Backend API Implementation
- [x] REST API structure - Controllers for all entities
- [x] Customer API endpoints - GET, POST, PUT, DELETE
- [x] Employee API endpoints - GET, POST, PUT, DELETE
- [x] Furniture API endpoints - GET, POST, PUT, DELETE with search/filter
- [x] Transaction API endpoints - GET, POST with details
- [x] Database service layer - DatabaseService with all CRUD methods
- [x] Error handling - Try-catch blocks in frontend
- [x] JSON serialization - Snake_case naming for API responses
- [ ] Authentication middleware - Not implemented
- [ ] Input validation - Basic HTML5 validation, no server-side validation

## Additional Features
- [x] Bootstrap UI framework - Bootstrap 5.3.3 via CDN
- [x] Responsive design - Mobile-friendly layout
- [x] Modal forms - All add operations use Bootstrap modals
- [x] Error messages - User-friendly error display
- [x] Success notifications - Alert messages after operations
- [ ] Data export - Not implemented
- [ ] Reports/dashboard - Basic stats placeholders on home page (not functional)

