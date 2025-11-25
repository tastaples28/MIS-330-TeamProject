create database CampusRehome;
use CampusRehome;

create table customer (
  customer_id int auto_increment primary key,
  first_name varchar(50) not null,
  last_name varchar(50) not null,
  email varchar(50) not null unique,
  phone varchar(15),
  userpassword varchar(30) not null,
  address varchar(40),
  join_date datetime default current_timestamp
);

create table employee (
  employee_id int auto_increment primary key,
  first_name varchar(50) not null,
  last_name varchar(50) not null,
  email varchar(50) not null unique,
  phone_number varchar(15),
  userpassword varchar(30) not null,
  role varchar(40) not null,
  hire_date date,
  is_active boolean default true
);

create table CustomerPurchaseTransaction(
	transaction_id int auto_increment primary key,
    customer_id int,
    CONSTRAINT fk_customer
        FOREIGN KEY (customer_id) REFERENCES customer(customer_id),
    employee_id int,
    CONSTRAINT fk_employee
        FOREIGN KEY (employee_id) REFERENCES employee(employee_id),
    transaction_date date not null,
    total_amount decimal (10,2) not null,
    payment_method varchar(25) not null,
    status varchar (10) not null
);

create table furniture(
  furniture_id int auto_increment primary key,
  item_name varchar(50) not null,
  category varchar(40) not null,
  description varchar(255),
  item_condition varchar(20) not null,  
  price decimal(10,2) not null,
  stock_quantity int not null,
  is_active boolean default true
);

create table TransactionDetail(
	transaction_detail_id int auto_increment primary key,
    transaction_id int,
    CONSTRAINT fk_transaction
        FOREIGN KEY (transaction_id) REFERENCES CustomerPurchaseTransaction(transaction_id),
    furniture_id int,
    CONSTRAINT fk_furniture
        FOREIGN KEY (furniture_id) REFERENCES Furniture(furniture_id),
    quantity int not null,
    sale_price decimal (10,2)not null
);
