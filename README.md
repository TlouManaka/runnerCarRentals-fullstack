# runnerCarRentals-fullstack:
RunnerCarRentals is a complete full-stack web application for managing car rentals, built to demonstrate proficiency in modern web development and Clean Architecture principles.

💡 Project Focus: Clean Architecture & SOLID
The core objective of this project was to implement a robust, scalable, and maintainable structure using Clean Architecture. This approach separates the application into layers, ensuring that the business logic (the "Core") is independent of the frameworks, databases, and UI.

Backend Architecture Highlights
Separation of Concerns (SoC): Distinct layers for Domain, Application, Infrastructure, and Presentation.

Dependency Inversion Principle (DIP): Dependencies point inward; the Core layer is independent of the Infrastructure and Presentation layers.

SOLID Principles: Focus on creating highly maintainable and testable code.

Repository Pattern: Used for abstracting data access and keeping the Domain/Application layers clean of database-specific logic.

✨ Core Functionality

The application provides end-to-end management for a car rental business:

- User Management: Secure user registration and authentication.

- Car Catalog: Displaying available vehicles with detailed information.

- Booking System: Creating, retrieving, updating, and deleting rental reservations (full CRUD operations).

- Admin Tools: Dedicated features for inventory and booking oversight.
