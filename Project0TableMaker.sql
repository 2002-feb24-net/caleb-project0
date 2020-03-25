--using Project0Db
--go
DROP TABLE IF EXISTS Orders;
DROP TABLE IF EXISTS Customers;
DROP TABLE IF EXISTS Stores;
DROP TABLE IF EXISTS Products;

CREATE TABLE Products (
ID int primary key,
Name nvarchar(100),
Price money
);
CREATE TABLE Stores (
ID int primary key,
Address nvarchar(200),
Stock int 
);
CREATE TABLE Customers (
ID int primary key,
Username nvarchar(50),
Password nvarchar(50),
Address nvarchar(200),
City nvarchar(100)
);
CREATE TABLE Orders (
ID int,
ProductID int,
CustomerID int,
StoreID int,
Price money,
Time datetime,
PRIMARY KEY (ID),
FOREIGN KEY (ProductID) REFERENCES Products(ID),
FOREIGN KEY (CustomerID) REFERENCES Customers(ID),
FOREIGN KEY (StoreID) REFERENCES Stores(ID)
);