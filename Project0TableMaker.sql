--use Project0Db
--go
DROP TABLE IF EXISTS Cart;
DROP TABLE IF EXISTS Inventory;
DROP TABLE IF EXISTS Orders;
DROP TABLE IF EXISTS Customers;
DROP TABLE IF EXISTS Stores;
DROP TABLE IF EXISTS Products;

CREATE TABLE Products (
ID int,
Name nvarchar(100),
Price money,
PRIMARY KEY (ID)
);
CREATE TABLE Stores (
ID int,
Address nvarchar(200),
--Stock int,
--removed stock b/c new Inventory table has Quantity
PRIMARY KEY (ID)
);
CREATE TABLE Customers (
ID int,
Username nvarchar(50),
Password nvarchar(50),
FirstName nvarchar(200),
LastName nvarchar(100),
PRIMARY KEY (ID)
);
CREATE TABLE Orders (
ID int,
ProductID int,
CustomerID int,
StoreID int,
Price money,
OrderTime datetime,
Quantity int,
PRIMARY KEY (ID),
FOREIGN KEY (ProductID) REFERENCES Products(ID),
FOREIGN KEY (CustomerID) REFERENCES Customers(ID),
FOREIGN KEY (StoreID) REFERENCES Stores(ID)
);
CREATE TABLE Inventory(
ID int,
ProductID int,
StoreID int,
Quantity int,
PRIMARY KEY (ID),
FOREIGN KEY (ProductID) REFERENCES Products(ID),
FOREIGN KEY (StoreID) REFERENCES Stores(ID)
);
--CREATE TABLE Cart (
--ID int,
--OrderID int,
--ProductID int,
--Quantity int,
--PRIMARY KEY (ID),
--FOREIGN KEY (OrderID) REFERENCES Orders(ID),
--FOREIGN KEY (ProductID) REFERENCES Products(ID)
--);
SELECT * FROM Inventory;