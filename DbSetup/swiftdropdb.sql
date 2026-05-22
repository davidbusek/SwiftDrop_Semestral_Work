-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: localhost
-- Generation Time: May 23, 2026 at 12:40 AM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `swiftdropdb`
--
CREATE DATABASE IF NOT EXISTS `swiftdropdb` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
USE `swiftdropdb`;

-- --------------------------------------------------------

--
-- Table structure for table `Addresses`
--

DROP TABLE IF EXISTS `Addresses`;
CREATE TABLE `Addresses` (
  `Id` int(11) NOT NULL,
  `Street` varchar(255) NOT NULL,
  `City` varchar(100) NOT NULL,
  `ZipCode` varchar(10) NOT NULL,
  `Latitude` decimal(10,8) DEFAULT NULL,
  `Longitude` decimal(11,8) DEFAULT NULL,
  `UserId` int(11) NOT NULL,
  `IsDeliveryAddress` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `Addresses`
--

INSERT INTO `Addresses` (`Id`, `Street`, `City`, `ZipCode`, `Latitude`, `Longitude`, `UserId`, `IsDeliveryAddress`) VALUES
(1, 'Dlouhá 12', 'Praha 1', '11000', 50.08920730, 14.42251690, 3, 1),
(2, 'Náměstí Míru 5', 'Praha 2', '12000', 50.07550000, 14.43780000, 4, 1),
(3, 'Václavské náměstí 15', 'Praha 1', '11000', 50.08150000, 14.42720000, 5, 0),
(4, 'Náměstí Republiky 3', 'Praha 1', '11000', 50.08780000, 14.43020000, 6, 0),
(5, 'Křižíkova 20', 'Praha 8', '18600', 50.09210000, 14.45330000, 7, 0),
(6, 'Vinohradská 25', 'Praha 2', '12000', 50.07510000, 14.43940000, 8, 0),
(7, 'U Michelského mlýna 380/4', 'Praha 4', '140 00', 50.05426380, 14.45096560, 3, 1);

-- --------------------------------------------------------

--
-- Table structure for table `Categories`
--

DROP TABLE IF EXISTS `Categories`;
CREATE TABLE `Categories` (
  `Id` int(11) NOT NULL,
  `RestaurantId` int(11) NOT NULL,
  `Name` varchar(100) NOT NULL,
  `DisplayOrder` int(11) DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `Categories`
--

INSERT INTO `Categories` (`Id`, `RestaurantId`, `Name`, `DisplayOrder`) VALUES
(1, 1, 'Smash Burgery', 1),
(2, 1, 'Hranolky & Omáčky', 2),
(3, 1, 'Nápoje', 3),
(4, 2, 'Pizzy 32cm', 1),
(5, 2, 'Těstoviny', 2),
(6, 2, 'Nápoje', 3),
(7, 3, 'Sushi Sety', 1),
(8, 3, 'Nigiri', 2),
(9, 3, 'Nápoje', 3),
(10, 4, 'Plant Bowls', 1),
(11, 4, 'Smoothies', 2),
(12, 4, 'Dezerty', 3);

-- --------------------------------------------------------

--
-- Table structure for table `MenuItems`
--

DROP TABLE IF EXISTS `MenuItems`;
CREATE TABLE `MenuItems` (
  `Id` int(11) NOT NULL,
  `CategoryId` int(11) NOT NULL,
  `Name` varchar(150) NOT NULL,
  `Description` text DEFAULT NULL,
  `Price` decimal(10,2) NOT NULL,
  `ImageUrl` varchar(500) DEFAULT NULL,
  `Allergens` varchar(255) DEFAULT NULL,
  `WeightOrVolume` varchar(50) DEFAULT NULL,
  `IsAvailable` tinyint(1) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `MenuItems`
--

INSERT INTO `MenuItems` (`Id`, `CategoryId`, `Name`, `Description`, `Price`, `ImageUrl`, `Allergens`, `WeightOrVolume`, `IsAvailable`) VALUES
(1, 1, 'Cyber Smash', 'Dvojitý smash patty, cheddar, karamelizovaná cibule, burger sauce.', 229.00, NULL, '1,3,7', '300g', 1),
(2, 1, 'Double Glitch Burger', 'Dvojitý patty, slanina, jalapeños, chipotle mayo.', 289.00, NULL, '1,3,7', '350g', 1),
(3, 1, 'Classic Smash', 'Jednoduchý smash patty, americký cheddar, okurky, hořčice.', 199.00, NULL, '1,3,7', '250g', 1),
(4, 2, 'Hranolky s cheddarem', 'Křupavé hranolky zalité cheddar omáčkou.', 95.00, NULL, '1,7', '250g', 1),
(5, 2, 'Batátové hranolky', 'Sladké batátové hranolky se zakysanou smetanou.', 115.00, NULL, '7', '250g', 1),
(6, 3, 'Cola Zero', 'Coca-Cola Zero 0,33l.', 45.00, NULL, NULL, '330ml', 1),
(7, 3, 'Domácí limonáda', 'Citron, máta, zázvor.', 65.00, NULL, NULL, '400ml', 1),
(8, 4, 'Margherita', 'Rajčatová omáčka, mozzarella fior di latte, čerstvá bazalka.', 199.00, NULL, '1,7', '32cm', 1),
(9, 4, 'Diavola', 'Pikantní salám, mozzarella, jalapeños, chilli olej.', 229.00, NULL, '1,7', '32cm', 1),
(10, 4, 'Quattro Formaggi', 'Mozzarella, gorgonzola, parmezán, ricotta.', 249.00, NULL, '1,7', '32cm', 1),
(11, 5, 'Spaghetti Carbonara', 'Pancetta, vejce, parmezán, černý pepř.', 189.00, NULL, '1,3,7', '380g', 1),
(12, 5, 'Penne Arrabbiata', 'Pikantní rajčatová omáčka, česnek, chilli.', 169.00, NULL, '1', '360g', 1),
(13, 6, 'Acqua Panna (0,5l)', 'Neperlivá minerální voda.', 45.00, NULL, NULL, '500ml', 1),
(14, 6, 'Pinot Grigio (0,2l)', 'Italské bílé víno.', 79.00, NULL, NULL, '200ml', 1),
(15, 7, 'Maki Set (12 ks)', 'Lososové a tuňákové maki rolky.', 349.00, NULL, '2,4,11', '12ks', 1),
(16, 7, 'Salmon Night Set', 'Lososové nigiri (4ks) + lososové maki (8ks) + miso polévka.', 499.00, NULL, '2,4,11', '1set', 1),
(17, 8, 'Losos Nigiri (2 ks)', 'Čerstvý atlantický losos na rýži.', 99.00, NULL, '2,4', '2ks', 1),
(18, 8, 'Tuňák Nigiri (2 ks)', 'Prémiový tuňák na rýži.', 109.00, NULL, '2,4', '2ks', 1),
(19, 9, 'Zelený čaj (0,3l)', 'Japonský sencha zelený čaj.', 55.00, NULL, NULL, '300ml', 1),
(20, 9, 'Saké (0,1l)', 'Tradiční japonské rýžové víno.', 89.00, NULL, NULL, '100ml', 1),
(21, 10, 'Tofu Buddha Bowl', 'Grilované tofu, quinoa, avokádo, edamame, tahini dresink.', 219.00, NULL, '6,11', '500g', 1),
(22, 10, 'Avocado Dream Bowl', 'Avokádo, sladké brambory, červená cibule, citronový dresink.', 239.00, NULL, NULL, '480g', 1),
(23, 11, 'Green Restart', 'Špenát, banán, zelené jablko, kokosová voda.', 109.00, NULL, NULL, '400ml', 1),
(24, 11, 'Berry Boost', 'Lesní ovoce, acai, lněná semínka, mandlové mléko.', 119.00, NULL, '8', '400ml', 1),
(25, 12, 'Chia Pudding', 'Chia semínka, kokosové mléko, mango, granola.', 99.00, NULL, NULL, '300g', 1);

-- --------------------------------------------------------

--
-- Table structure for table `Openinghours`
--

DROP TABLE IF EXISTS `Openinghours`;
CREATE TABLE `Openinghours` (
  `Id` int(11) NOT NULL,
  `RestaurantId` int(11) NOT NULL,
  `DayOfWeek` enum('Monday','Tuesday','Wednesday','Thursday','Friday','Saturday','Sunday') NOT NULL,
  `OpenTime` time DEFAULT NULL,
  `ClosingTime` time DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `Orderitems`
--

DROP TABLE IF EXISTS `Orderitems`;
CREATE TABLE `Orderitems` (
  `Id` int(11) NOT NULL,
  `SubOrderId` int(11) NOT NULL,
  `MenuItemId` int(11) NOT NULL,
  `Quantity` int(11) NOT NULL,
  `UnitPrice` decimal(10,2) NOT NULL,
  `ItemNotes` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `Orderitems`
--

INSERT INTO `Orderitems` (`Id`, `SubOrderId`, `MenuItemId`, `Quantity`, `UnitPrice`, `ItemNotes`) VALUES
(1, 1, 1, 1, 229.00, NULL),
(2, 1, 4, 1, 95.00, NULL),
(3, 2, 15, 1, 349.00, NULL),
(4, 3, 8, 1, 199.00, NULL),
(5, 3, 9, 1, 229.00, NULL),
(6, 4, 2, 1, 289.00, NULL),
(7, 5, 21, 1, 219.00, NULL),
(8, 6, 11, 1, 189.00, NULL),
(9, 7, 1, 1, 229.00, NULL),
(10, 8, 16, 1, 499.00, NULL),
(11, 9, 1, 1, 229.00, NULL),
(12, 9, 2, 1, 289.00, NULL),
(13, 10, 16, 1, 499.00, NULL),
(14, 11, 9, 1, 229.00, NULL),
(15, 12, 2, 1, 289.00, NULL),
(16, 13, 24, 1, 119.00, NULL);

-- --------------------------------------------------------

--
-- Table structure for table `Orders`
--

DROP TABLE IF EXISTS `Orders`;
CREATE TABLE `Orders` (
  `Id` int(11) NOT NULL,
  `UserId` int(11) NOT NULL,
  `AddressId` int(11) NOT NULL,
  `Status` enum('Pending','Paid','CourierAssigned','PickupsInProgress','Delivering','Delivered','Canceled') NOT NULL,
  `ItemPrice` decimal(10,2) NOT NULL,
  `DeliveryFee` decimal(10,2) NOT NULL,
  `TotalPrice` decimal(10,2) NOT NULL,
  `CreatedAt` datetime DEFAULT current_timestamp(),
  `DeliveredAt` datetime DEFAULT NULL,
  `CourierId` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `Orders`
--

INSERT INTO `Orders` (`Id`, `UserId`, `AddressId`, `Status`, `ItemPrice`, `DeliveryFee`, `TotalPrice`, `CreatedAt`, `DeliveredAt`, `CourierId`) VALUES
(1, 3, 1, 'Paid', 673.00, 89.00, 762.00, '2026-05-10 12:30:00', NULL, NULL),
(2, 4, 2, 'Delivered', 428.00, 49.00, 477.00, '2026-05-10 13:00:00', '2026-05-10 13:55:00', NULL),
(3, 3, 1, 'Paid', 508.00, 64.00, 572.00, '2026-05-11 18:15:00', NULL, NULL),
(4, 4, 2, 'Pending', 189.00, 39.00, 228.00, '2026-05-13 11:00:00', NULL, NULL),
(5, 3, 7, 'Delivered', 728.00, 64.00, 792.00, '2026-05-13 16:55:31', '2026-05-13 16:56:42', NULL),
(6, 3, 7, 'Delivered', 1246.00, 89.00, 1335.00, '2026-05-22 21:10:20', '2026-05-22 21:21:56', 2),
(7, 3, 1, 'Paid', 408.00, 64.00, 472.00, '2026-05-23 00:10:56', NULL, NULL);

-- --------------------------------------------------------

--
-- Table structure for table `Payments`
--

DROP TABLE IF EXISTS `Payments`;
CREATE TABLE `Payments` (
  `Id` int(11) NOT NULL,
  `OrderId` int(11) NOT NULL,
  `PaymentMethod` enum('CardOnline','ApplePay','GooglePay','CashOnDelivery') NOT NULL,
  `PaymentStatus` enum('Unpaid','Paid','Refunded') NOT NULL,
  `Amount` decimal(10,2) NOT NULL,
  `CreatedAt` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `Payments`
--

INSERT INTO `Payments` (`Id`, `OrderId`, `PaymentMethod`, `PaymentStatus`, `Amount`, `CreatedAt`) VALUES
(1, 1, 'CardOnline', 'Paid', 762.00, '2026-05-10 12:30:05'),
(2, 2, 'ApplePay', 'Paid', 477.00, '2026-05-10 13:00:05'),
(3, 3, 'GooglePay', 'Paid', 572.00, '2026-05-11 18:15:05'),
(4, 4, 'CardOnline', 'Unpaid', 228.00, '2026-05-13 11:00:05'),
(5, 5, 'CardOnline', 'Paid', 792.00, '2026-05-13 16:55:33'),
(6, 6, 'CardOnline', 'Paid', 1335.00, '2026-05-22 21:10:21'),
(7, 7, 'CardOnline', 'Paid', 472.00, '2026-05-23 00:10:58');

-- --------------------------------------------------------

--
-- Table structure for table `Restaurants`
--

DROP TABLE IF EXISTS `Restaurants`;
CREATE TABLE `Restaurants` (
  `Id` int(11) NOT NULL,
  `Name` varchar(150) NOT NULL,
  `Description` text DEFAULT NULL,
  `ContactPhone` varchar(20) DEFAULT NULL,
  `ContactEmail` varchar(255) DEFAULT NULL,
  `LogoUrl` varchar(500) DEFAULT NULL,
  `AverageRating` decimal(3,2) DEFAULT 0.00,
  `ReviewCount` int(11) DEFAULT 0,
  `EstimatedPrepTimeMinutes` int(11) DEFAULT NULL,
  `MinimumOrderAmount` decimal(10,2) DEFAULT NULL,
  `IsActive` tinyint(1) DEFAULT 1,
  `IsAcceptingOrders` tinyint(1) DEFAULT 1,
  `AddressId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `Restaurants`
--

INSERT INTO `Restaurants` (`Id`, `Name`, `Description`, `ContactPhone`, `ContactEmail`, `LogoUrl`, `AverageRating`, `ReviewCount`, `EstimatedPrepTimeMinutes`, `MinimumOrderAmount`, `IsActive`, `IsAcceptingOrders`, `AddressId`) VALUES
(1, 'CyberBurger', 'Nejlepší smash burgery ve městě.', NULL, 'burger@swiftdrop.cz', NULL, 0.00, 0, 20, 150.00, 1, 1, 3),
(2, 'Neon Pizza', 'Klasická neapolská pizza s křupavým okrajem.', NULL, 'pizza@swiftdrop.cz', NULL, 0.00, 0, 25, 150.00, 1, 1, 4),
(3, 'Tokyo Drift Sushi', 'Autentické čerstvé sushi.', NULL, 'sushi@swiftdrop.cz', NULL, 0.00, 0, 30, 200.00, 1, 1, 5),
(4, 'Green Node', '100% rostlinná strava – vegan bowls a smoothies.', NULL, 'green@swiftdrop.cz', NULL, 0.00, 0, 20, 120.00, 1, 1, 6);

-- --------------------------------------------------------

--
-- Table structure for table `Reviews`
--

DROP TABLE IF EXISTS `Reviews`;
CREATE TABLE `Reviews` (
  `Id` int(11) NOT NULL,
  `UserId` int(11) NOT NULL,
  `RestaurantId` int(11) NOT NULL,
  `Rating` int(11) DEFAULT NULL CHECK (`Rating` >= 1 and `Rating` <= 5),
  `Comment` text DEFAULT NULL,
  `CreatedAt` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `Suborders`
--

DROP TABLE IF EXISTS `Suborders`;
CREATE TABLE `Suborders` (
  `Id` int(11) NOT NULL,
  `OrderId` int(11) NOT NULL,
  `RestaurantId` int(11) NOT NULL,
  `Status` enum('Pending','Preparing','ReadyForPickUp','PickedUp') NOT NULL,
  `EstimatedReadyTime` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `Suborders`
--

INSERT INTO `Suborders` (`Id`, `OrderId`, `RestaurantId`, `Status`, `EstimatedReadyTime`) VALUES
(1, 1, 1, 'Pending', NULL),
(2, 1, 3, 'Pending', NULL),
(3, 2, 2, 'PickedUp', NULL),
(4, 3, 1, 'Preparing', NULL),
(5, 3, 4, 'Pending', NULL),
(6, 4, 2, 'Pending', NULL),
(7, 5, 1, 'Pending', NULL),
(8, 5, 3, 'Pending', NULL),
(9, 6, 1, 'Pending', NULL),
(10, 6, 3, 'Pending', NULL),
(11, 6, 2, 'Pending', NULL),
(12, 7, 1, 'Pending', NULL),
(13, 7, 4, 'Pending', NULL);

-- --------------------------------------------------------

--
-- Table structure for table `Users`
--

DROP TABLE IF EXISTS `Users`;
CREATE TABLE `Users` (
  `Id` int(11) NOT NULL,
  `FirstName` varchar(100) NOT NULL,
  `LastName` varchar(100) NOT NULL,
  `Email` varchar(255) NOT NULL,
  `PhoneNumber` varchar(20) DEFAULT NULL,
  `PasswordHash` varchar(255) NOT NULL,
  `Role` enum('Customer','Courier','Admin','RestaurantManager') NOT NULL,
  `RegisteredAt` datetime DEFAULT current_timestamp(),
  `IsActive` tinyint(1) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `Users`
--

INSERT INTO `Users` (`Id`, `FirstName`, `LastName`, `Email`, `PhoneNumber`, `PasswordHash`, `Role`, `RegisteredAt`, `IsActive`) VALUES
(1, 'Admin', 'SwiftDrop', 'admin@swiftdrop.cz', NULL, '$2a$11$q3sZzNORRQ2cadloCiGwRu5xQtHGyQQHJU8wJtE36xqcZ46p103sK', 'Admin', '2026-05-01 10:00:00', 1),
(2, 'Petr', 'Rychlý', 'courier@swiftdrop.cz', NULL, '$2a$11$q3sZzNORRQ2cadloCiGwRu5xQtHGyQQHJU8wJtE36xqcZ46p103sK', 'Courier', '2026-05-01 10:00:00', 1),
(3, 'Karel', 'Novák', 'customer@swiftdrop.cz', NULL, '$2a$11$q3sZzNORRQ2cadloCiGwRu5xQtHGyQQHJU8wJtE36xqcZ46p103sK', 'Customer', '2026-05-01 10:00:00', 1),
(4, 'Jana', 'Svobodová', 'customer2@swiftdrop.cz', NULL, '$2a$11$q3sZzNORRQ2cadloCiGwRu5xQtHGyQQHJU8wJtE36xqcZ46p103sK', 'Customer', '2026-05-01 10:00:00', 1),
(5, 'Tomáš', 'Burger', 'manager.burger@swiftdrop.cz', NULL, '$2a$11$q3sZzNORRQ2cadloCiGwRu5xQtHGyQQHJU8wJtE36xqcZ46p103sK', 'RestaurantManager', '2026-05-01 10:00:00', 1),
(6, 'Lucie', 'Pizza', 'manager.pizza@swiftdrop.cz', NULL, '$2a$11$q3sZzNORRQ2cadloCiGwRu5xQtHGyQQHJU8wJtE36xqcZ46p103sK', 'RestaurantManager', '2026-05-01 10:00:00', 1),
(7, 'Martin', 'Sushi', 'manager.sushi@swiftdrop.cz', NULL, '$2a$11$q3sZzNORRQ2cadloCiGwRu5xQtHGyQQHJU8wJtE36xqcZ46p103sK', 'RestaurantManager', '2026-05-01 10:00:00', 1),
(8, 'Eva', 'Green', 'manager.green@swiftdrop.cz', NULL, '$2a$11$q3sZzNORRQ2cadloCiGwRu5xQtHGyQQHJU8wJtE36xqcZ46p103sK', 'RestaurantManager', '2026-05-01 10:00:00', 1),
(9, 'dfv', 'fdsg', 'pepega@sssvrt.cz', '765543219', '$2a$11$WBcgNKIOOZ5pe0lRVH4UKOy8Otuzs1W8W9JFjFB0BoyaQHZn9bstW', 'Customer', '2026-05-22 21:54:57', 1);

--
-- Indexes for dumped tables
--

--
-- Indexes for table `Addresses`
--
ALTER TABLE `Addresses`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `UserId` (`UserId`);

--
-- Indexes for table `Categories`
--
ALTER TABLE `Categories`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `RestaurantId` (`RestaurantId`);

--
-- Indexes for table `MenuItems`
--
ALTER TABLE `MenuItems`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `CategoryId` (`CategoryId`);

--
-- Indexes for table `Openinghours`
--
ALTER TABLE `Openinghours`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `RestaurantId` (`RestaurantId`);

--
-- Indexes for table `Orderitems`
--
ALTER TABLE `Orderitems`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `SubOrderId` (`SubOrderId`),
  ADD KEY `MenuItemId` (`MenuItemId`);

--
-- Indexes for table `Orders`
--
ALTER TABLE `Orders`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `UserId` (`UserId`),
  ADD KEY `AddressId` (`AddressId`),
  ADD KEY `idx_orders_courier` (`CourierId`);

--
-- Indexes for table `Payments`
--
ALTER TABLE `Payments`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `OrderId` (`OrderId`);

--
-- Indexes for table `Restaurants`
--
ALTER TABLE `Restaurants`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `AddressId` (`AddressId`);

--
-- Indexes for table `Reviews`
--
ALTER TABLE `Reviews`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `UserId` (`UserId`),
  ADD KEY `RestaurantId` (`RestaurantId`);

--
-- Indexes for table `Suborders`
--
ALTER TABLE `Suborders`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `OrderId` (`OrderId`),
  ADD KEY `RestaurantId` (`RestaurantId`);

--
-- Indexes for table `Users`
--
ALTER TABLE `Users`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `Email` (`Email`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `Addresses`
--
ALTER TABLE `Addresses`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT for table `Categories`
--
ALTER TABLE `Categories`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=13;

--
-- AUTO_INCREMENT for table `MenuItems`
--
ALTER TABLE `MenuItems`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=26;

--
-- AUTO_INCREMENT for table `Openinghours`
--
ALTER TABLE `Openinghours`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `Orderitems`
--
ALTER TABLE `Orderitems`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=17;

--
-- AUTO_INCREMENT for table `Orders`
--
ALTER TABLE `Orders`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT for table `Payments`
--
ALTER TABLE `Payments`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT for table `Restaurants`
--
ALTER TABLE `Restaurants`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT for table `Reviews`
--
ALTER TABLE `Reviews`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `Suborders`
--
ALTER TABLE `Suborders`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=14;

--
-- AUTO_INCREMENT for table `Users`
--
ALTER TABLE `Users`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `Addresses`
--
ALTER TABLE `Addresses`
  ADD CONSTRAINT `addresses_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE;

--
-- Constraints for table `Categories`
--
ALTER TABLE `Categories`
  ADD CONSTRAINT `categories_ibfk_1` FOREIGN KEY (`RestaurantId`) REFERENCES `Restaurants` (`Id`) ON DELETE CASCADE;

--
-- Constraints for table `MenuItems`
--
ALTER TABLE `MenuItems`
  ADD CONSTRAINT `menuitems_ibfk_1` FOREIGN KEY (`CategoryId`) REFERENCES `Categories` (`Id`) ON DELETE CASCADE;

--
-- Constraints for table `Openinghours`
--
ALTER TABLE `Openinghours`
  ADD CONSTRAINT `openinghours_ibfk_1` FOREIGN KEY (`RestaurantId`) REFERENCES `Restaurants` (`Id`) ON DELETE CASCADE;

--
-- Constraints for table `Orderitems`
--
ALTER TABLE `Orderitems`
  ADD CONSTRAINT `orderitems_ibfk_1` FOREIGN KEY (`SubOrderId`) REFERENCES `Suborders` (`Id`) ON DELETE CASCADE,
  ADD CONSTRAINT `orderitems_ibfk_2` FOREIGN KEY (`MenuItemId`) REFERENCES `MenuItems` (`Id`);

--
-- Constraints for table `Orders`
--
ALTER TABLE `Orders`
  ADD CONSTRAINT `orders_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`),
  ADD CONSTRAINT `orders_ibfk_2` FOREIGN KEY (`AddressId`) REFERENCES `Addresses` (`Id`),
  ADD CONSTRAINT `orders_ibfk_courier` FOREIGN KEY (`CourierId`) REFERENCES `Users` (`Id`) ON DELETE SET NULL;

--
-- Constraints for table `Payments`
--
ALTER TABLE `Payments`
  ADD CONSTRAINT `payments_ibfk_1` FOREIGN KEY (`OrderId`) REFERENCES `Orders` (`Id`);

--
-- Constraints for table `Restaurants`
--
ALTER TABLE `Restaurants`
  ADD CONSTRAINT `restaurants_ibfk_1` FOREIGN KEY (`AddressId`) REFERENCES `Addresses` (`Id`);

--
-- Constraints for table `Reviews`
--
ALTER TABLE `Reviews`
  ADD CONSTRAINT `reviews_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`),
  ADD CONSTRAINT `reviews_ibfk_2` FOREIGN KEY (`RestaurantId`) REFERENCES `Restaurants` (`Id`);

--
-- Constraints for table `Suborders`
--
ALTER TABLE `Suborders`
  ADD CONSTRAINT `suborders_ibfk_1` FOREIGN KEY (`OrderId`) REFERENCES `Orders` (`Id`) ON DELETE CASCADE,
  ADD CONSTRAINT `suborders_ibfk_2` FOREIGN KEY (`RestaurantId`) REFERENCES `Restaurants` (`Id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
