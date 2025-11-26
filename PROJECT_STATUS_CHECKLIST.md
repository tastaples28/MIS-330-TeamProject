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
- [x] Login page - Created with customer/employee login
- [x] Registration page - Created with customer/employee registration
- [x] User profile page - Created with view/edit functionality

## User Management Features
- [x] User registration page - Implemented with customer/employee registration
- [x] User login page - Implemented with customer/employee login
- [x] Authentication system - Login/logout with localStorage token management
- [x] Session management - Client-side session using localStorage
- [ ] Password hashing - Passwords stored as plaintext (security issue - needs improvement)
- [x] Customer CRUD operations - Create, Read, Delete implemented
- [x] Employee CRUD operations - Create, Read, Deactivate implemented
- [x] User profiles - Profile viewing/editing page with modals
- [ ] Password reset - Not implemented (password change available in profile)

## Furniture Listing & Management Features
- [x] Create furniture listing - Modal form with all fields
- [x] Read furniture listings - Table displays all active furniture
- [x] Edit furniture listing - Edit modal with pre-filled form implemented
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
