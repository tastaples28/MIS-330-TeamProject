-- JawsDB Schema Setup
-- This schema is designed for Heroku JawsDB (database already exists)
-- Connect to your JawsDB database first, then run this script

-- Use the existing database (already connected via JawsDB)
-- No need to create database or use statement

-- Drop tables if they exist (for clean setup)
DROP TABLE IF EXISTS TransactionDetail;
DROP TABLE IF EXISTS CustomerPurchaseTransaction;
DROP TABLE IF EXISTS furniture;
DROP TABLE IF EXISTS employee;
DROP TABLE IF EXISTS customer;

-- Create customer table
CREATE TABLE customer (
  customer_id INT AUTO_INCREMENT PRIMARY KEY,
  first_name VARCHAR(50) NOT NULL,
  last_name VARCHAR(50) NOT NULL,
  email VARCHAR(50) NOT NULL UNIQUE,
  phone VARCHAR(15),
  userpassword VARCHAR(30) NOT NULL,
  address VARCHAR(40),
  join_date DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Create employee table
CREATE TABLE employee (
  employee_id INT AUTO_INCREMENT PRIMARY KEY,
  first_name VARCHAR(50) NOT NULL,
  last_name VARCHAR(50) NOT NULL,
  email VARCHAR(50) NOT NULL UNIQUE,
  phone_number VARCHAR(15),
  userpassword VARCHAR(30) NOT NULL,
  role VARCHAR(40) NOT NULL,
  hire_date DATE,
  is_active BOOLEAN DEFAULT TRUE
);

-- Create furniture table
CREATE TABLE furniture (
  furniture_id INT AUTO_INCREMENT PRIMARY KEY,
  item_name VARCHAR(50) NOT NULL,
  category VARCHAR(40) NOT NULL,
  description VARCHAR(255),
  item_condition VARCHAR(20) NOT NULL,
  price DECIMAL(10,2) NOT NULL,
  stock_quantity INT NOT NULL,
  is_active BOOLEAN DEFAULT TRUE
);

-- Create CustomerPurchaseTransaction table
CREATE TABLE CustomerPurchaseTransaction (
  transaction_id INT AUTO_INCREMENT PRIMARY KEY,
  customer_id INT,
  CONSTRAINT fk_customer
    FOREIGN KEY (customer_id) REFERENCES customer(customer_id),
  employee_id INT,
  CONSTRAINT fk_employee
    FOREIGN KEY (employee_id) REFERENCES employee(employee_id),
  transaction_date DATE NOT NULL,
  total_amount DECIMAL(10,2) NOT NULL,
  payment_method VARCHAR(25) NOT NULL,
  status VARCHAR(10) NOT NULL
);

-- Create TransactionDetail table
CREATE TABLE TransactionDetail (
  transaction_detail_id INT AUTO_INCREMENT PRIMARY KEY,
  transaction_id INT,
  CONSTRAINT fk_transaction
    FOREIGN KEY (transaction_id) REFERENCES CustomerPurchaseTransaction(transaction_id),
  furniture_id INT,
  CONSTRAINT fk_furniture
    FOREIGN KEY (furniture_id) REFERENCES furniture(furniture_id),
  quantity INT NOT NULL,
  sale_price DECIMAL(10,2) NOT NULL
);

