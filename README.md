![.NET CI](https://github.com/salvatoreallegra/TaskTracker/actions/workflows/dotnet-ci.yml/badge.svg)

# TaskTracker API

TaskTracker is a **.NET 9 ASP.NET Core Web API** that demonstrates **enterprise-grade architecture and cloud-native practices**.  
It includes Clean Architecture, EF Core with relationships, authentication, testing, CI/CD, observability, and cloud deployment.

---

## 🚀 Features
- ASP.NET Core 9 Web API with **RESTful endpoints**
- **Entity Framework Core** with SQL Server & InMemory provider for testing
- **Relationships**: One-to-Many (Projects ↔ Tasks), Many-to-Many (Tags ↔ Tasks)
- **DTOs + AutoMapper** for clean contracts
- **Service + Repository Pattern** for testable business logic
- **Authentication & Authorization** with JWT / OAuth2
- **Validation** with FluentValidation
- **xUnit + Moq** unit tests
- **Integration tests** with WebApplicationFactory + InMemory EF
- **Contract tests** with Pact
- **Load testing** with k6
- **Observability**: OpenTelemetry + Grafana + Azure App Insights
- **Secure configuration** with User Secrets, GitHub Secrets, Azure Key Vault
- **CI/CD pipelines** with GitHub Actions
- **Containerization & Orchestration** with Docker, Docker Compose, and Kubernetes (AKS)
- **Infrastructure as Code** using Bicep & Terraform
- **Deployment** to Azure App Service with blue/green rollout

---

# 📁 TaskTracker Project Structure

```plaintext
TaskTracker/
├── Controllers/              # Controllers (Tasks, Projects, Tags, Auth)
├── Data/                     # EF Core DbContext + repositories
├── Dtos/                     # Data Transfer Objects
├── Mapping/                  # AutoMapper profiles
├── Models/                   # Entities (TaskItem, Project, Tag, User)
├── Options/                  # Config options (paging, JWT, etc.)
├── Services/                 # Business logic services
├── TaskTracker.Api.csproj    # Main ASP.NET Core project file
└── tests/                    # Test projects
    ├── TaskTracker.Tests/           # Unit tests
    └── TaskTracker.IntegrationTests # Integration & contract tests
```

---

## 🛠️ Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- SQL Server (for dev DB)

### Run Locally
```bash
cd TaskTracker
dotnet restore
dotnet run --project TaskTracker.Api
Swagger: https://localhost:7034/swagger

## ✅ Run Tests

To run all tests in the solution, use the following command:

```bash
dotnet test
```

This will build the solution and execute all test projects (unit and integration tests) in the workspace.

## 🔐 Configuration & Secrets

- **Local Development**: Use [`dotnet user-secrets`](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) to securely store secrets on your local machine without committing them to source control.
  
- **CI/CD**: Store secrets using **GitHub Repository Secrets**. These can be used in GitHub Actions workflows for testing, builds, and deployments.

- **Production**: Use **Azure Key Vault** in combination with **Managed Identity** to securely retrieve secrets in deployed environments.

---

## ⚡ CI/CD

### GitHub Actions Workflow

The CI/CD pipeline includes:

- ✅ **Build & Restore**  
  Restores NuGet packages and builds the solution.

- 🧪 **Run Unit & Integration Tests**  
  Executes all test projects with `dotnet test`, verifying functionality and catching regressions.

- 📈 **Upload Code Coverage**  
  Optionally integrates with tools like Coverlet and Codecov to track test coverage.

- 🚀 **Deploy to Azure App Service**  
  Automatically deploys the application to Azure once tests pass and code is merged to the main branch.

  ## 📚 Example Endpoints

### 📝 Tasks
- `GET /api/tasks`  
  → List tasks (supports paging, filter, and search)

- `POST /api/tasks`  
  → Create a new task

- `PUT /api/tasks/{id}`  
  → Update an existing task

- `DELETE /api/tasks/{id}`  
  → Delete a task

### 📁 Projects
- `GET /api/projects`  
  → Retrieve projects with their nested tasks

### 🔐 Authentication
- `POST /api/auth/login`  
  → Issue a JWT token

### 👤 User
- `GET /api/users/me`  
  → Get the authenticated user’s profile

  ## 🚀 Advanced Features & Architecture Highlights

This project already integrates a robust set of modern development practices and tools:

- ✅ **EF Core Many-to-Many Support**  
  - Fully implemented relationship between `Tags` and `Tasks`.

- 🔐 **Authentication & Authorization**  
  - Supports JWT tokens and OAuth2 authentication flows.

- 🧠 **CQRS Pattern**  
  - Clear separation between Commands (writes) and Queries (reads).





- 
