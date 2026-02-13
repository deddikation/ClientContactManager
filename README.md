# Client Contact Manager

An ASP.NET Core 8.0 MVC application for managing clients and their associated contacts. Built using **Clean Architecture** with the **CQRS** (Command Query Responsibility Segregation) pattern.

---

## Table of Contents

- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Database Setup](#database-setup)
- [Running the Application](#running-the-application)
- [Using the Application](#using-the-application)
  - [Dashboard](#dashboard)
  - [Managing Clients](#managing-clients)
  - [Managing Contacts](#managing-contacts)
  - [Linking and Unlinking](#linking-and-unlinking)
- [Project Structure](#project-structure)
- [Architecture and Design Decisions](#architecture-and-design-decisions)

---

## Prerequisites

Before running the application, ensure you have the following installed:

| Tool | Version | Download |
|------|---------|----------|
| .NET SDK | 8.0 or higher | https://dotnet.microsoft.com/download/dotnet/8.0 |
| SQL Server | 2019+ or LocalDB | Included with Visual Studio, or install separately |
| Visual Studio 2022 (optional) | 17.8+ | https://visualstudio.microsoft.com/ |

To verify your .NET installation, open a terminal and run:

```bash
dotnet --version
```

---

## Getting Started

### 1. Clone or Extract the Project

Place the project folder on your machine. The solution file is located at:

```
ClientContactManager/
    ClientContactManager.sln
    database/
        schema.sql
    src/
        ClientContactManager.Domain/
        ClientContactManager.Application/
        ClientContactManager.Infrastructure/
        ClientContactManager.WebUI/
```

### 2. Restore Dependencies

Open a terminal in the root project folder and run:

```bash
dotnet restore
```

This downloads all required NuGet packages (Entity Framework Core, MediatR, FluentValidation, etc.).

---

## Database Setup

The application uses **SQL Server** with a database called `ClientContactManagerDb`.

### Option A: Automatic (Recommended)

The application automatically creates the database and all tables on first startup using `Database.EnsureCreated()` in `Program.cs`. No manual setup is required if you have SQL Server or LocalDB running.

### Option B: Manual Setup

If you prefer to create the database manually, run the script located at `database/schema.sql` against your SQL Server instance. This creates:

- **Clients** table (Id, Name, ClientCode)
- **Contacts** table (Id, Name, Surname, Email)
- **ClientContacts** junction table (ClientId, ContactId) for the many-to-many relationship

### Connection String

The default connection string in `src/ClientContactManager.WebUI/appsettings.json` is:

```
Data Source=.;Initial Catalog=ClientContactManagerDb;Integrated Security=True;...
```

- `Data Source=.` connects to the local SQL Server default instance.
- If you use **LocalDB**, change it to `Data Source=(localdb)\MSSQLLocalDB`.
- If you use a **named instance**, change it to `Data Source=.\SQLEXPRESS` or your instance name.

---

## Running the Application

### Using the Command Line

```bash
cd src/ClientContactManager.WebUI
dotnet run
```

The application will start and display URLs in the terminal, typically:

```
https://localhost:7058
http://localhost:5058
```

Open **https://localhost:7058** in your browser.

### Using Visual Studio

1. Open `ClientContactManager.sln` in Visual Studio.
2. Set **ClientContactManager.WebUI** as the startup project (right-click the project and select "Set as Startup Project").
3. Press **F5** to run or **Ctrl+F5** to run without debugging.

---

## Using the Application

### Dashboard

The home page provides an overview with quick-action cards:

- **New Client** - Navigate directly to create a new client.
- **New Contact** - Navigate directly to create a new contact.
- **Link Records** - Go to the clients list to manage relationships.

Use the **navigation bar** at the top to switch between Dashboard, Clients, and Contacts at any time.

---

### Managing Clients

#### Viewing All Clients

1. Click **Clients** in the navigation bar.
2. You will see a table listing all clients with their **Name**, **Client Code**, and the number of **Linked Contacts**.

#### Creating a Client

1. From the Clients page, click the **New Client** button.
2. Enter the client **Name** (required, maximum 200 characters).
3. Click **Save Client**.
4. The system automatically generates a unique 6-character **Client Code** based on the name.
   - Example: "First National Bank" generates code `FNB001`.
   - If `FNB001` already exists, it increments to `FNB002`, `FNB003`, etc.
5. After saving, you are redirected to the client's Edit page.

#### Editing a Client

1. From the Clients list, click the **Edit** button next to a client.
2. The Edit page has two tabs:
   - **General** - Displays the client's name and auto-generated client code (read-only fields).
   - **Contacts** - Shows linked contacts and allows linking/unlinking (see [Linking and Unlinking](#linking-and-unlinking)).

---

### Managing Contacts

#### Viewing All Contacts

1. Click **Contacts** in the navigation bar.
2. You will see a table listing all contacts with their **Name**, **Surname**, **Email**, and the number of **Linked Clients**.

#### Creating a Contact

1. From the Contacts page, click the **New Contact** button.
2. Fill in the required fields:
   - **Name** - First name (required, maximum 100 characters).
   - **Surname** - Last name (required, maximum 100 characters).
   - **Email** - A valid, unique email address (required).
3. Click **Save Contact**.
4. If validation fails (empty fields, invalid email, or duplicate email), error messages will appear below the relevant fields.

#### Editing a Contact

1. From the Contacts list, click the **Edit** button next to a contact.
2. The Edit page has two tabs:
   - **General** - Displays the contact's name, surname, and email (read-only fields).
   - **Clients** - Shows linked clients and allows linking/unlinking (see [Linking and Unlinking](#linking-and-unlinking)).

---

### Linking and Unlinking

Clients and contacts have a **many-to-many** relationship. A client can have multiple contacts, and a contact can belong to multiple clients.

#### Linking a Contact to a Client

1. Go to the client's **Edit** page.
2. Switch to the **Contacts** tab.
3. If there are contacts available to link, a dropdown will appear showing all contacts that are **not yet linked** to this client.
4. Select a contact from the dropdown and click **Link Contact**.
5. The contact will now appear in the linked contacts table below.

#### Unlinking a Contact from a Client

1. On the client's Edit page, go to the **Contacts** tab.
2. In the linked contacts table, click the **Unlink** button next to the contact you want to remove.
3. The contact is removed from this client but is **not deleted** from the system.

#### Linking/Unlinking from the Contact Side

The same operations are available from the contact's Edit page under the **Clients** tab. You can link or unlink clients from a contact using the same dropdown and unlink buttons.

---

## Project Structure

```
ClientContactManager/
|
|-- ClientContactManager.sln            Solution file
|-- database/
|   |-- schema.sql                      SQL Server database schema
|
|-- src/
    |-- ClientContactManager.Domain/           Core business entities
    |   |-- Entities/
    |   |   |-- Client.cs                      Client aggregate
    |   |   |-- Contact.cs                     Contact aggregate
    |   |   |-- ClientContact.cs               Many-to-many join entity
    |   |-- Interfaces/
    |       |-- IRepositories.cs               Repository and UnitOfWork contracts
    |
    |-- ClientContactManager.Application/      Use cases (CQRS)
    |   |-- Commands/
    |   |   |-- Clients/                       CreateClient, LinkContact, UnlinkContact
    |   |   |-- Contacts/                      CreateContact, LinkClient, UnlinkClient
    |   |-- Queries/
    |   |   |-- Clients/                       GetAllClients, GetClientById, GetAvailableContacts
    |   |   |-- Contacts/                      GetAllContacts, GetContactById, GetAvailableClients
    |   |-- DTOs/                              Data transfer objects for views
    |   |-- Validators/                        FluentValidation rules
    |   |-- Common/
    |       |-- Behaviors/                     MediatR validation pipeline
    |       |-- Interfaces/                    IClientCodeGenerator
    |
    |-- ClientContactManager.Infrastructure/   Data access and external services
    |   |-- Persistence/
    |   |   |-- AppDbContext.cs                 EF Core DbContext
    |   |   |-- Repositories/                  Client and Contact repository implementations
    |   |-- Services/
    |       |-- ClientCodeGenerator.cs          Auto-generates 6-char client codes
    |
    |-- ClientContactManager.WebUI/            ASP.NET Core MVC front-end
        |-- Controllers/                        Client, Contact, Home controllers
        |-- Views/                              Razor views with Bootstrap styling
        |-- wwwroot/                            Static assets (CSS, JS, Bootstrap)
        |-- Program.cs                          App startup and dependency injection
```

---

## Architecture and Design Decisions

### Clean Architecture

The solution is split into four projects with dependencies flowing **inward only**:

```
WebUI  -->  Application  -->  Domain
               ^
        Infrastructure
```

- **Domain** has zero external dependencies. It contains entities, value objects, and repository interfaces.
- **Application** depends only on Domain. It contains commands, queries, DTOs, and validators.
- **Infrastructure** implements Domain and Application interfaces using EF Core and SQL Server.
- **WebUI** is the entry point that wires everything together via dependency injection.

### CQRS (Command Query Responsibility Segregation)

Every operation is modelled as either a **Command** (modifies data) or a **Query** (reads data):

| Type | Example | Purpose |
|------|---------|---------|
| Command | `CreateClientCommand` | Creates a new client |
| Command | `LinkContactToClientCommand` | Associates a contact with a client |
| Query | `GetAllClientsQuery` | Returns a list of all clients |
| Query | `GetClientByIdQuery` | Returns a single client with details |

Commands and queries are dispatched through **MediatR**, which routes each request to its handler class. A **ValidationBehavior** pipeline intercepts commands before they reach handlers, running FluentValidation rules automatically.

### Key Technologies

| Technology | Purpose |
|------------|---------|
| ASP.NET Core 8.0 MVC | Web framework and server-side rendering |
| Entity Framework Core 8.0 | Object-relational mapper for SQL Server |
| MediatR 12.2 | In-process messaging for CQRS pattern |
| FluentValidation 11.3 | Declarative input validation rules |
| Bootstrap 5.3 | Responsive UI styling |
| SQL Server | Relational database |
