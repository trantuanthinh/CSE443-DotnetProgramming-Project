# ASP.NET Web MVC Project

This is an ASP.NET Web MVC project to create web application.

## Getting Started

First, ensure you have the following installed:

- [.NET SDK](https://dotnet.microsoft.com/download) (latest stable version)
- A code editor like [Visual Studio](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)

### Running the Development Server

Because we use TailwindCss, so we use the following command:
```
dotnet run
```

By default, the application will be hosted at:

API Base URL: https://localhost:7091 (HTTPS)

API Base URL: http://localhost:5085 (HTTP)


Install Dotnet Globally:
```
dotnet tool -install --global dotnet -ef
```

Install Node_Modules:
```
npm i
```

To update database, use the following command:

```
dotnet ef migrations add InitMigrations
```

```
dotnet ef database update
```

```
dotnet run seeddata
```

```
dotnet run
```
