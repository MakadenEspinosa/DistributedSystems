# Citations and Resources

## AI Assistance
- **GitHub Copilot** (Claude Sonnet 4.5) - Used for code generation, architecture design, and implementation guidance throughout the project development (January 2026)

## Frameworks and Libraries

### Backend (ASP.NET Core Web API)
- **ASP.NET Core** - Microsoft's cross-platform framework for building web APIs
  - Version: .NET 9.0
  - Documentation: https://docs.microsoft.com/en-us/aspnet/core/
  - License: MIT License

- **MongoDB.Driver** - Official MongoDB driver for .NET
  - Version: 2.x+
  - Used for database operations and data persistence
  - Documentation: https://www.mongodb.com/docs/drivers/csharp/
  - License: Apache License 2.0

- **Swashbuckle.AspNetCore** - Swagger/OpenAPI tools for ASP.NET Core
  - Used for API documentation generation
  - Documentation: https://github.com/domaindrivendev/Swashbuckle.AspNetCore
  - License: MIT License

### Authentication and Security

- **Microsoft.AspNetCore.Authentication.JwtBearer** - JWT Bearer authentication for ASP.NET Core
  - Version: 9.0.0
  - Used for JWT token validation and authentication middleware
  - Documentation: https://docs.microsoft.com/en-us/aspnet/core/security/authentication/
  - License: MIT License

- **System.IdentityModel.Tokens.Jwt** - JWT token creation and validation
  - Version: 8.0.1 (dependency of JwtBearer)
  - Used for generating JWT tokens
  - Documentation: https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet
  - License: MIT License

- **BCrypt.Net-Next** - BCrypt password hashing library
  - Version: 4.0.3
  - Used for secure password hashing and verification
  - Documentation: https://github.com/BcryptNet/bcrypt.net
  - License: MIT License
  - Algorithm: https://en.wikipedia.org/wiki/Bcrypt

### Client SDK
- **OpenAPI Generator** - Tool for generating API client libraries from OpenAPI specifications
  - Used to generate C# client SDK from OpenAPI/Swagger specification
  - Documentation: https://openapi-generator.tech/
  - License: Apache License 2.0

- **RestSharp** / **.NET HttpClient** - HTTP client libraries (depending on generator configuration)
  - Used in generated client for making HTTP requests
  - Documentation: https://restsharp.dev/ or https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient

## Standards and Specifications

### RFC 7807 - Problem Details for HTTP APIs
- **Title**: Problem Details for HTTP APIs
- **Authors**: M. Nottingham, E. Wilde
- **Date**: March 2016
- **URL**: https://tools.ietf.org/html/rfc7231
- **Description**: Standard format for returning error responses in REST APIs
- **Usage**: Implemented in `Problem.cs` model for consistent error handling

### RFC 7519 - JSON Web Token (JWT)
- **Title**: JSON Web Token (JWT)
- **Authors**: M. Jones, J. Bradley, N. Sakimura
- **Date**: May 2015
- **URL**: https://tools.ietf.org/html/rfc7519
- **Description**: Standard for creating secure tokens for authentication
- **Usage**: Used for stateless authentication with bearer tokens

### RFC 6750 - The OAuth 2.0 Authorization Framework: Bearer Token Usage
- **Title**: Bearer Token Usage
- **Date**: October 2012
- **URL**: https://tools.ietf.org/html/rfc6750
- **Description**: Defines how to use bearer tokens in HTTP requests
- **Usage**: Authorization header format for JWT tokens

### OpenAPI Specification (formerly Swagger)
- **Version**: 3.1.1
- **Organization**: OpenAPI Initiative
- **URL**: https://swagger.io/specification/
- **Description**: Standard specification for describing RESTful APIs
- **Usage**: API documentation and client generation via `api.yml`

### Richardson Maturity Model
- **Author**: Leonard Richardson
- **Description**: Model for evaluating REST API maturity (Levels 0-3)
- **Reference**: https://martinfowler.com/articles/richardsonMaturityModel.html
- **Usage**: API implements Level 3 (HATEOAS) compliance

## Database
- **MongoDB** - NoSQL document database
  - Version: Compatible with MongoDB 4.x+
  - Documentation: https://docs.mongodb.com/
  - License: Server Side Public License (SSPL)
  - Usage: Data persistence for Users and Games collections
  - Download: https://www.mongodb.com/try/download/community

## Development Tools
- **Visual Studio Code** - Code editor and development environment
  - Extensions used for .NET development
  - Documentation: https://code.visualstudio.com/docs
  - Download: https://code.visualstudio.com/

- **.NET CLI** - Command-line interface for .NET development
  - Used for building, running, and managing the project
  - Documentation: https://docs.microsoft.com/en-us/dotnet/core/tools/
  - Download: https://dotnet.microsoft.com/download

## Design Patterns and Architecture

### Repository Pattern
- Service layer abstraction (`GameService`, `UserService`, `AuthService`) separating business logic from data access

### Dependency Injection
- ASP.NET Core built-in DI container used for service registration and management

### RESTful API Design
- Standard HTTP methods (GET, POST, PUT, PATCH, DELETE)
- Resource-based URL structure
- Appropriate status codes following HTTP specifications
- HATEOAS implementation for hypermedia controls

### Security Patterns
- **JWT (JSON Web Tokens)** for stateless authentication
- **BCrypt** for password hashing with salt
- **Claims-based authorization** for user identity and permissions
- **Ownership validation** ensuring users can only modify their own resources

## Configuration Management
- **appsettings.json** - Standard ASP.NET Core configuration pattern
- **Options Pattern** - Strongly-typed configuration using `IOptions<T>`
- **Environment-specific configuration** - appsettings.Development.json support

## Data Validation
- **System.ComponentModel.DataAnnotations** - .NET validation attributes
  - `[Required]`, `[EmailAddress]`, `[Range]` attributes used in models
  - Documentation: https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations

## Security Best Practices Implemented
- **Password Hashing**: BCrypt with automatic salt generation
- **Token Expiration**: JWT tokens expire after 60 minutes
- **Claims-based Authorization**: User identity stored in token claims
- **Ownership Validation**: Users can only modify their own resources
- **Secure Password Storage**: Passwords never stored in plain text
- **HTTPS Ready**: Configuration supports secure communications

---

## Prerequisites to Run This Project

### Required Software
1. **.NET 9.0 SDK**
   - Download: https://dotnet.microsoft.com/download/dotnet/9.0
   - Used to build and run the ASP.NET Core application

2. **MongoDB Server**
   - Download: https://www.mongodb.com/try/download/community
   - Version: 4.x or higher
   - Must be running on `localhost:27017` (default)
   - Alternative: MongoDB Atlas (cloud) - https://www.mongodb.com/cloud/atlas

3. **Visual Studio Code** (Recommended)
   - Download: https://code.visualstudio.com/
   - Extensions:
     - C# Dev Kit
     - REST Client (for testing .http files)

### Optional Tools
- **Postman** - For API testing: https://www.postman.com/
- **MongoDB Compass** - GUI for MongoDB: https://www.mongodb.com/products/compass
- **Git** - Version control: https://git-scm.com/

### Environment Setup Steps
1. **Install .NET 9.0 SDK**
   ```powershell
   dotnet --version  # Verify installation
   ```

2. **Install and Start MongoDB**
   ```powershell
   # Windows - Start MongoDB service
   net start MongoDB
   
   # Or run mongod manually
   mongod --dbpath C:\data\db
   ```

3. **Clone/Download Project**
   ```powershell
   cd C:\Classes\DistributedSystems\VideoGameExchange
   ```

4. **Restore NuGet Packages**
   ```powershell
   cd VideoGameExchange.Server
   dotnet restore
   ```

5. **Build Project**
   ```powershell
   dotnet build
   ```

6. **Run Application**
   ```powershell
   dotnet run
   ```
   - Server will start at: http://localhost:5000
   - Swagger UI: http://localhost:5000/swagger

### Configuration Notes
- **MongoDB Connection**: Update `appsettings.json` if MongoDB is not on localhost:27017
- **JWT Secret Key**: Change the secret key in production environments
- **Port Configuration**: Modify `launchSettings.json` to change port

### NuGet Packages (Automatically Restored)
```xml
<PackageReference Include="MongoDB.Driver" />
<PackageReference Include="Swashbuckle.AspNetCore" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="9.0.0" />
<PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
```

---

## Project Structure
This project follows standard ASP.NET Core Web API conventions and best practices as documented in Microsoft's official ASP.NET Core documentation.

### Key Implementation Details
1. **User Management System** - Custom implementation with email uniqueness constraint and immutable email after creation
2. **Game Ownership Model** - Relationship between users and games using MongoDB ObjectId references
3. **Authentication & Authorization** - JWT-based stateless authentication with ownership validation
4. **Password Security** - BCrypt hashing with automatic salt generation (work factor: 11)
5. **Partial Update Support** - PATCH endpoints using dedicated patch models
6. **Query Filtering** - Optional query parameters for filtering games by user
7. **HATEOAS Implementation** - Hypermedia links in all resource responses for Level 3 REST maturity

---

## Additional Resources

### Learning Resources
- **ASP.NET Core Tutorial**: https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api
- **MongoDB with .NET**: https://www.mongodb.com/docs/drivers/csharp/current/quick-start/
- **JWT Authentication in ASP.NET Core**: https://learn.microsoft.com/en-us/aspnet/core/security/authentication/jwt-authn
- **RESTful API Design**: https://restfulapi.net/
- **HATEOAS**: https://en.wikipedia.org/wiki/HATEOAS

### Security References
- **OWASP API Security Top 10**: https://owasp.org/www-project-api-security/
- **JWT Best Practices**: https://tools.ietf.org/html/rfc8725
- **Password Hashing**: https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html

---

*Last Updated: January 16, 2026*

