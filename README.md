# ⚡ SwiftDrop 
> **Multi-Restaurant Delivery System** > *Academic project for KIV/MNS (Software Design and Modeling) and KIV/PNET (ASP.NET Web Applications).*

---

## 📖 Project Overview
SwiftDrop is a next-generation food delivery platform developed as a **semester project for the University of West Bohemia (ZČU)**. 
It specifically targets the "Multi-Restaurant Dilemma," allowing students and users to combine items from multiple restaurants into a single order with an optimized delivery fee.

### Core Features (Use Cases)
1.  **UC-01: Multi-Restaurant Cart Management** – Users can mix dishes from different providers in one session.
2.  **UC-02: User Identity Management** – Secure registration and login system with persistent cookie-based authentication.
3.  **UC-03: Dynamic Delivery Pricing** – Automated calculation of delivery fees based on order complexity.
4.  **UC-04: Order Orchestration** – Seamless transition from selection to final checkout and persistence.

---

## 🏗️ Architecture & Design Patterns
The project is built using the **ASP.NET Core MVC** framework and follows the **Clean Architecture** principles (separation of concerns).



### Implemented Design Patterns
* **Strategy Pattern**: Used for the delivery cost calculation (`IDeliveryCostStrategy`). It allows the system to switch between different pricing models (e.g., flat rate vs. multi-stop complexity) without modifying the core service.
* **Service Layer Pattern**: Business logic is decoupled from Controllers into dedicated services (e.g., `CartService`, `RestaurantService`), ensuring high modularity and testability.
* **Repository Pattern (via EF Core)**: Data access is abstracted to allow easy switching of database providers.
* **ViewModel Pattern**: Strict separation between the database models and the data displayed in Views, preventing over-posting and ensuring type safety.

---

## 🛠️ Tech Stack
- **Backend:** .NET 9.0 (C#)
- **Frontend:** Razor Pages & MVC (ASP.NET Core), Bootstrap 5, Custom CSS (Cyberpunk/Dark Theme)
- **Database:** MariaDB / MySQL (Entity Framework Core)
- **Security:** BCrypt for password hashing, Cookie Authentication for session management

---

## ⚠️ Academic Disclaimer
This project is developed strictly for **educational purposes** within the following courses at **University of West Bohemia (KIV/FAV)**:
* **KIV/MNS** (Modelování a navrhování softwaru)
* **KIV/PNET** (Programování v prostředí .NET)

The implementation focuses on demonstrating object-oriented design principles, design patterns, and modern web development practices. It is **not** a production-ready service and does not handle real financial transactions or logistics.
