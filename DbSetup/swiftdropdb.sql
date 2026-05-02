-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: localhost
-- Generation Time: May 02, 2026 at 07:19 PM
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
  `UserId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `Addresses`
--

INSERT INTO `Addresses` (`Id`, `Street`, `City`, `ZipCode`, `Latitude`, `Longitude`, `UserId`) VALUES
(1, 'Dlouhá 12', 'Praha', '11000', 50.08700000, 14.42100000, 1),
(101, 'Václavské náměstí 1', 'Praha', '11000', 50.08150000, 14.42720000, 3),
(102, 'Staroměstské náměstí 1', 'Praha', '11000', 50.08740000, 14.42050000, 3),
(103, 'Křižíkova 20', 'Praha 8', '18600', 50.09210000, 14.45330000, 3),
(104, 'Náměstí Míru 5', 'Praha 2', '12000', 50.07550000, 14.43780000, 3),
(105, 'Seifertova 15', 'Praha 3', '13000', 50.08220000, 14.45300000, 101),
(106, 'Nádražní 25', 'Praha 5', '15000', 50.07110000, 14.40240000, 102),
(107, 'Milady Horákové 30', 'Praha 7', '17000', 50.09840000, 14.42390000, 103),
(108, 'Evropská 50', 'Praha 6', '16000', 50.10090000, 14.38610000, 104),
(109, 'U garáží 1611/1', 'Praha 7', '170 00', 50.10127590, 14.44422390, 105);

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
(1, 1, 'Hlavní jídla', 1),
(2, 2, 'Přílohy', 1),
(101, 101, 'Smash Burgery', 0),
(102, 101, 'Hranolky & Omáčky', 0),
(103, 102, 'Pizza 32cm', 0),
(104, 102, 'Nápoje', 0),
(105, 103, 'Sushi Sety', 0),
(106, 103, 'Nigiri', 0),
(107, 104, 'Plant Bowls', 0),
(108, 104, 'Smoothies', 0);

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
(1, 1, 'Butter Chicken', NULL, 189.00, NULL, NULL, NULL, 1),
(2, 2, 'Smažené Nudle', NULL, 120.00, NULL, NULL, NULL, 1),
(101, 101, 'Cyber Smash', NULL, 229.00, NULL, NULL, NULL, 1),
(102, 101, 'Double Glitch Burger', NULL, 289.00, NULL, NULL, NULL, 1),
(103, 102, 'Hranolky s cheddarem', NULL, 95.00, NULL, NULL, NULL, 1),
(104, 102, 'Batátové hranolky', NULL, 115.00, NULL, NULL, NULL, 1),
(105, 103, 'Margherita', NULL, 199.00, NULL, NULL, NULL, 1),
(106, 103, 'Quattro Formaggi', NULL, 249.00, NULL, NULL, NULL, 1),
(107, 104, 'Cola Zero', NULL, 45.00, NULL, NULL, NULL, 1),
(108, 105, 'Maki Set (12pcs)', NULL, 349.00, NULL, NULL, NULL, 1),
(109, 105, 'Salmon Night Set', NULL, 499.00, NULL, NULL, NULL, 1),
(110, 107, 'Tofu Buddha Bowl', NULL, 219.00, NULL, NULL, NULL, 1),
(111, 107, 'Avocado Dream Bowl', NULL, 239.00, NULL, NULL, NULL, 1),
(112, 108, 'Green Restart Smoothie', NULL, 109.00, NULL, NULL, NULL, 1);

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
(1, 1, 1, 1, 189.00, NULL),
(2, 2, 2, 1, 120.00, NULL),
(3, 3, 1, 1, 189.00, NULL),
(4, 4, 2, 1, 120.00, NULL),
(5, 5, 1, 1, 189.00, NULL),
(6, 6, 1, 1, 189.00, NULL),
(7, 6, 2, 1, 120.00, NULL),
(8, 7, 1, 1, 189.00, NULL),
(9, 8, 1, 1, 189.00, NULL),
(10, 8, 2, 1, 120.00, NULL),
(101, 101, 101, 1, 229.00, NULL),
(102, 101, 103, 1, 95.00, NULL),
(103, 102, 105, 1, 199.00, NULL),
(104, 102, 106, 1, 249.00, NULL),
(105, 103, 102, 1, 289.00, NULL),
(106, 104, 106, 1, 244.00, NULL),
(107, 105, 109, 1, 499.00, NULL),
(108, 106, 111, 2, 239.00, NULL),
(109, 107, 105, 1, 69.00, NULL),
(110, 108, 101, 1, 229.00, NULL),
(111, 109, 1, 1, 189.00, NULL),
(112, 109, 2, 1, 120.00, NULL),
(113, 110, 1, 1, 189.00, NULL),
(114, 110, 111, 1, 239.00, NULL),
(115, 111, 1, 1, 189.00, NULL),
(116, 111, 109, 1, 499.00, NULL),
(117, 111, 102, 1, 289.00, NULL),
(118, 111, 111, 1, 239.00, NULL),
(119, 112, 1, 1, 189.00, NULL),
(120, 113, 102, 1, 289.00, NULL),
(121, 114, 1, 1, 189.00, NULL),
(122, 115, 2, 1, 120.00, NULL),
(123, 116, 104, 1, 115.00, NULL),
(124, 117, 106, 1, 249.00, NULL),
(125, 118, 102, 1, 289.00, NULL),
(126, 119, 1, 1, 189.00, NULL),
(127, 120, 109, 1, 499.00, NULL),
(128, 121, 2, 1, 120.00, NULL),
(129, 122, 104, 1, 115.00, NULL);

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
  `DeliveredAt` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `Orders`
--

INSERT INTO `Orders` (`Id`, `UserId`, `AddressId`, `Status`, `ItemPrice`, `DeliveryFee`, `TotalPrice`, `CreatedAt`, `DeliveredAt`) VALUES
(1, 1, 1, 'Paid', 309.00, 49.00, 358.00, '2026-04-18 20:14:33', NULL),
(2, 5, 1, 'Pending', 189.00, 39.00, 228.00, '2026-04-19 23:38:32', NULL),
(3, 5, 1, 'Pending', 120.00, 39.00, 159.00, '2026-04-19 23:40:06', NULL),
(4, 4, 1, 'Pending', 189.00, 39.00, 228.00, '2026-04-20 10:30:35', NULL),
(5, 4, 1, 'Pending', 309.00, 64.00, 373.00, '2026-04-20 10:35:26', NULL),
(6, 4, 1, 'Delivered', 189.00, 39.00, 228.00, '2026-04-20 14:06:55', '2026-04-20 14:19:04'),
(7, 4, 1, 'Delivered', 309.00, 64.00, 373.00, '2026-04-20 14:24:44', '2026-04-26 23:05:41'),
(101, 101, 105, 'Paid', 324.00, 49.00, 373.00, '2026-04-26 11:17:17', NULL),
(102, 102, 106, 'Delivered', 448.00, 39.00, 487.00, '2026-04-26 11:17:17', '2026-04-26 23:05:55'),
(103, 103, 107, 'Delivered', 533.00, 89.00, 622.00, '2026-04-26 11:17:17', '2026-04-26 23:06:01'),
(104, 104, 108, 'PickupsInProgress', 1046.00, 129.00, 1175.00, '2026-04-26 11:17:17', NULL),
(105, 101, 105, 'Delivered', 229.00, 49.00, 278.00, '2026-04-26 11:17:17', NULL),
(106, 4, 1, 'Pending', 309.00, 64.00, 373.00, '2026-04-26 16:33:11', NULL),
(107, 4, 1, 'Paid', 428.00, 64.00, 492.00, '2026-04-26 16:47:56', NULL),
(108, 4, 1, 'Delivered', 1216.00, 114.00, 1330.00, '2026-04-26 19:14:39', '2026-04-26 23:06:03'),
(109, 105, 1, 'Delivered', 478.00, 64.00, 542.00, '2026-04-26 22:48:46', '2026-04-26 23:06:05'),
(110, 105, 1, 'Canceled', 673.00, 114.00, 787.00, '2026-04-26 22:51:24', NULL),
(111, 105, 106, 'Delivered', 977.00, 89.00, 1066.00, '2026-04-26 22:51:47', '2026-04-26 23:06:07'),
(112, 105, 109, 'CourierAssigned', 235.00, 64.00, 299.00, '2026-04-26 23:05:01', NULL);

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
(1, 2, 'CardOnline', 'Unpaid', 228.00, '2026-04-19 23:38:33'),
(2, 3, 'CardOnline', 'Unpaid', 159.00, '2026-04-19 23:40:07'),
(3, 4, 'CardOnline', 'Unpaid', 228.00, '2026-04-20 10:30:36'),
(4, 5, 'CardOnline', 'Unpaid', 373.00, '2026-04-20 10:35:27'),
(5, 6, 'CardOnline', 'Unpaid', 228.00, '2026-04-20 14:06:56'),
(6, 7, 'CardOnline', 'Unpaid', 373.00, '2026-04-20 14:24:45'),
(101, 101, 'CardOnline', 'Paid', 373.00, '2026-04-26 11:17:17'),
(102, 102, 'ApplePay', 'Paid', 487.00, '2026-04-26 11:17:17'),
(103, 103, 'CardOnline', 'Paid', 622.00, '2026-04-26 11:17:17'),
(104, 104, 'GooglePay', 'Paid', 1175.00, '2026-04-26 11:17:17'),
(105, 105, 'CardOnline', 'Paid', 278.00, '2026-04-26 11:17:17'),
(106, 107, 'CardOnline', 'Paid', 492.00, '2026-04-26 16:47:58'),
(107, 108, 'CardOnline', 'Paid', 1330.00, '2026-04-26 19:14:41'),
(108, 109, 'CardOnline', 'Paid', 542.00, '2026-04-26 22:48:47'),
(109, 110, 'CardOnline', 'Unpaid', 787.00, '2026-04-26 22:51:25'),
(110, 111, 'CardOnline', 'Paid', 1066.00, '2026-04-26 22:51:48'),
(111, 112, 'CardOnline', 'Paid', 299.00, '2026-04-26 23:05:02');

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
(1, 'Indická Kuchyně', 'Autentické kari a naan', NULL, 'indie@email.cz', NULL, 0.00, 0, 30, NULL, 1, 1, 1),
(2, 'Čínský Drak', 'Nejlepší nudle ve městě', NULL, 'drak@email.cz', NULL, 0.00, 0, 20, NULL, 1, 1, 1),
(101, 'CyberBurger', 'Nejlepší smash burgery ve městě.', NULL, NULL, NULL, 0.00, 0, NULL, NULL, 1, 1, 101),
(102, 'Neon Pizza', 'Klasická neapolská pizza s křupavým okrajem.', NULL, NULL, NULL, 0.00, 0, NULL, NULL, 1, 1, 102),
(103, 'Tokyo Drift Sushi', 'Autentické čerstvé sushi.', NULL, NULL, NULL, 0.00, 0, NULL, NULL, 1, 1, 103),
(104, 'Green Node', '100% rostlinná strava, vegan burgery a bowls.', NULL, NULL, NULL, 0.00, 0, NULL, NULL, 1, 1, 104);

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
(1, 1, 1, 'Preparing', NULL),
(2, 1, 2, 'Pending', NULL),
(3, 2, 1, 'Pending', NULL),
(4, 3, 2, 'Pending', NULL),
(5, 4, 1, 'Pending', NULL),
(6, 5, 1, 'Pending', NULL),
(7, 6, 1, 'Pending', NULL),
(8, 7, 1, 'Pending', NULL),
(101, 101, 101, 'Pending', NULL),
(102, 102, 102, 'ReadyForPickUp', NULL),
(103, 103, 101, 'ReadyForPickUp', NULL),
(104, 103, 102, 'ReadyForPickUp', NULL),
(105, 104, 103, 'PickedUp', NULL),
(106, 104, 104, 'ReadyForPickUp', NULL),
(107, 104, 102, 'Preparing', NULL),
(108, 105, 101, 'PickedUp', NULL),
(109, 106, 1, 'Pending', NULL),
(110, 107, 1, 'Pending', NULL),
(111, 108, 1, 'Pending', NULL),
(112, 109, 1, 'Pending', NULL),
(113, 109, 101, 'Pending', NULL),
(114, 110, 1, 'Pending', NULL),
(115, 110, 2, 'Pending', NULL),
(116, 110, 101, 'Pending', NULL),
(117, 110, 102, 'Pending', NULL),
(118, 111, 101, 'Pending', NULL),
(119, 111, 1, 'Pending', NULL),
(120, 111, 103, 'Pending', NULL),
(121, 112, 2, 'Pending', NULL),
(122, 112, 101, 'Pending', NULL);

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
(1, 'Honza', 'Novak', 'zakaznik@email.cz', NULL, 'hash123', 'Customer', '2026-04-18 20:14:33', 1),
(3, 'Admin', 'Vsemocny', 'admin@swiftdrop.cz', NULL, 'hash789', 'Admin', '2026-04-18 20:14:33', 1),
(4, 'Makre', 'fsdfdsasdf', '123456@sssvt.cz', NULL, '$2a$11$q3sZzNORRQ2cadloCiGwRu5xQtHGyQQHJU8wJtE36xqcZ46p103sK', 'Customer', '2026-04-18 23:32:35', 1),
(5, 'Admin', 'Admin', 'admin@sssvt.cz', NULL, '$2a$11$VRDlF5GFyFzLtlF8fYaa1O1/cAh0NqTfc/9rS.UqobgNeMP66odPy', 'Admin', '2026-04-19 19:41:11', 1),
(6, 'Manager', 'Manager', 'manager@sssvt.cz', NULL, '$2a$11$tTsJzG9tu0Tn4IXJ/zCZhOzBSncZYTcc.J8MvYz59No2UYMlAMRMq', 'RestaurantManager', '2026-04-19 19:41:37', 1),
(7, 'courier', 'courier', 'courier@sssvt.cz', NULL, '$2a$11$tr.xwI5ty.vkK66CTtNozO2UECQT2ODb8FkY6Dqo/p9GyLiLwztVS', 'Courier', '2026-04-20 14:08:05', 1),
(101, 'Karel', 'Zákazník', 'karel@test.cz', NULL, '$2a$11$q3sZzNORRQ2cadloCiGwRu5xQtHGyQQHJU8wJtE36xqcZ46p103sK', 'Customer', '2026-04-26 11:17:17', 1),
(102, 'Jana', 'Nováková', 'jana@test.cz', NULL, '$2a$11$q3sZzNORRQ2cadloCiGwRu5xQtHGyQQHJU8wJtE36xqcZ46p103sK', 'Customer', '2026-04-26 11:17:17', 1),
(103, 'Petr', 'Rychlý', 'petr@test.cz', NULL, '$2a$11$q3sZzNORRQ2cadloCiGwRu5xQtHGyQQHJU8wJtE36xqcZ46p103sK', 'Customer', '2026-04-26 11:17:17', 1),
(104, 'Firemní', 'Účet', 'firma@test.cz', NULL, '$2a$11$q3sZzNORRQ2cadloCiGwRu5xQtHGyQQHJU8wJtE36xqcZ46p103sK', 'Customer', '2026-04-26 11:17:17', 1),
(105, 'Marek', 'Praha', 'marek@sssvt.cz', NULL, '$2a$11$XkonJ46wDbKANZ8x/IWjoeoluaSnLxmdjLGxzpZ0vdaTHPXzG5CQq', 'Customer', '2026-04-26 22:48:19', 1);

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
  ADD KEY `AddressId` (`AddressId`);

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
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=110;

--
-- AUTO_INCREMENT for table `Categories`
--
ALTER TABLE `Categories`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=109;

--
-- AUTO_INCREMENT for table `MenuItems`
--
ALTER TABLE `MenuItems`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=113;

--
-- AUTO_INCREMENT for table `Openinghours`
--
ALTER TABLE `Openinghours`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `Orderitems`
--
ALTER TABLE `Orderitems`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=130;

--
-- AUTO_INCREMENT for table `Orders`
--
ALTER TABLE `Orders`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=113;

--
-- AUTO_INCREMENT for table `Payments`
--
ALTER TABLE `Payments`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=112;

--
-- AUTO_INCREMENT for table `Restaurants`
--
ALTER TABLE `Restaurants`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=105;

--
-- AUTO_INCREMENT for table `Reviews`
--
ALTER TABLE `Reviews`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `Suborders`
--
ALTER TABLE `Suborders`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=123;

--
-- AUTO_INCREMENT for table `Users`
--
ALTER TABLE `Users`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=106;

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
  ADD CONSTRAINT `orders_ibfk_2` FOREIGN KEY (`AddressId`) REFERENCES `Addresses` (`Id`);

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
