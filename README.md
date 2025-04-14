# ASP.NET Web API Project

This is an ASP.NET Web API project created to serve as the backend for your application.

## Getting Started

First, ensure you have the following installed:

- [.NET SDK](https://dotnet.microsoft.com/download) (latest stable version)
- A code editor like [Visual Studio](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)

### Running the Development Server

To run the development server, use the following command:

```
dotnet run
```

By default, the application will be hosted at:

API Base URL: https://localhost:7102 (HTTPS)

API Base URL: http://localhost:5130 (HTTP)


Install Dotnet Globally:
```
dotnet tool -install --global dotnet -ef
```


To update database, use the following command:

```
dotnet ef migrations add InitMigrations
```

```
dotnet ef migrations update database
```

```
dotnet run seeddata
```

```
dotnet run
```
