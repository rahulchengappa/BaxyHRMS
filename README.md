# Baxy HRMS

A full-stack Human Resource Management System (HRMS) developed as part of a training project at **BAXY Limited**.

The application is designed to streamline employee management, role-based access control, organizational hierarchy management, document handling, reporting, and secure authentication.


## Project Overview

Baxy HRMS is an enterprise-style web application built using ASP.NET Core MVC and SQL Server following a layered architecture and clean architecture approach.

The system enables organizations to manage employees, departments, designations, roles, permissions, reporting structures, and employee documents while maintaining secure access control using JWT Authentication and Role-Based Access Control (RBAC).



## Tech Stack

### Frontend
- ASP.NET Core MVC
- Razor Views
- HTML
- CSS
- Bootstrap
- JavaScript
- jQuery

### Backend
- ASP.NET Core
- C#
- ADO.NET
- REST APIs
- JWT Authentication

### Database
- Microsoft SQL Server
- Stored Procedures
- SQL Server Express

### Tools & Deployment
- Swagger UI
- Postman
- IIS (Internet Information Services)
- Cloudflare Tunnel
- Git & GitHub



## Architecture

The project follows a layered architecture and inspiration from clean architecture principles:


HRMS.Web

│

├── HRMS.Application

├── HRMS.Domain

├── HRMS.Infrastructure

└── SQL Server Database


### Layers

#### HRMS.Web
Presentation Layer containing:
- Controllers
- Views
- Authentication
- UI Logic

#### HRMS.Application
Business Logic Layer containing:
- DTOs
- Interfaces
- Application Services

#### HRMS.Domain
Core Domain Layer containing:
- Entities
- Models
- Business Objects

#### HRMS.Infrastructure
Data Access Layer containing:
- Repositories
- Database Operations
- Stored Procedure Integration



## Key Features

### Authentication & Security

- User Login
- JWT Authentication
- Cookie-based Session Management
- Forgot Password
- Reset Password
- Change Password using email verification
- Authorization Filters
- Symmetric Encryption
- Secure Route Access



### Employee Management

- Add Employee
- Edit Employee
- Delete Employee
- View Employee Details
- Dynamic Dashboard
- Employee Code Generation
- Employee Profile Photo Upload
- Employee Document Upload
- Employee Status Management



### Department Management

- Create Department
- View Department List


### Designation Management

- Create Designation
- View Designation List


### Role-Based Access Control (RBAC)

- Role Management( Super Admin, Admin, HR, Employee)
- Menu Management
- Role-Menu Mapping
- Dynamic Sidebar Menu
- URL-Based Authorization
- Access Denied Handling


### Profile Management

- View Profile
- Update Profile
- Change Password
- Profile Image Management


### Reporting Structure

- Reporting Manager Assignment
- Employee Hierarchy Management
- Organization Chart Visualization


### Employee Documents

- Upload Employee Documents
- Store File References
- Manage Employee Records


### Audit Logging

- Employee Audit Tracking
- Action History Recording
- Change Monitoring


### Reports & Exports

- Excel Export
- PDF Export


## API Development

REST APIs were developed to support:

- Authentication
- Employee Management
- Department Management
- Designation Management
- Role Management
- Menu Management
- Profile Management


## Swagger Integration

Swagger was integrated for API documentation and testing.

Features:

- Interactive API Testing
- Endpoint Documentation
- JWT Token Authorization
- Request/Response Validation


## Postman Testing

The APIs were tested using Postman for:

- Authentication Validation
- CRUD Operations
- JWT Token Verification
- Authorization Testing
- Endpoint Validation



## Database

The system uses Microsoft SQL Server with:

- Stored Procedures
- Primary Keys
- Foreign Keys
- Constraints
- Views
- Audit Logging

Database schema script is included in:

Database/Database_Schema.sql



## Deployment

### Local Deployment

Hosted using:

- IIS
- SQL Server Express
- ASP.NET Core Hosting Bundle

### Public Deployment

Exposed securely through:

- Cloudflare Tunnel



## Learning Outcomes

This project provided hands-on experience with:

- ASP.NET Core MVC
- Layered Architecture
- SQL Server Design
- Stored Procedures
- JWT Authentication
- RBAC Implementation
- REST API Development
- Swagger Integration
- Postman Testing
- IIS Deployment
- Cloudflare Tunnel Deployment
- Git & GitHub Version Control



## Project Status

Completed



## Author

Rahul Chengappa

Full Stack Developer Intern

Baxy Limited


## Acknowledgement

This project was developed as part of a professional training program at **BAXY**, focusing on enterprise application development using ASP.NET Core, SQL Server, API development, authentication, authorization, and deployment practices.
