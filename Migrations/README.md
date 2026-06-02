# EF Core Migrations

Po zainstalowaniu narzedzi .NET uruchom:

```bash
dotnet tool install --global dotnet-ef
dotnet restore
dotnet ef migrations add InitialCreate
dotnet ef database update
```

Migracje beda tworzone w tym katalogu.
