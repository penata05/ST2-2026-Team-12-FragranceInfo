# FragranceInfo

A catalogue of fragrances in the style of Fragrantica. Course project for ST2 2026, Team 12.

Built with C#, ASP.NET Core MVC (.NET 10) and MySQL.

## How to run

1. Install the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).
2. Start **MySQL** in the XAMPP Control Panel.
3. In the project folder, run:

   ```
   dotnet run
   ```

4. Open http://localhost:5257

The application creates the `fragranceinfo` database and the `fragrances` table by itself on the first start, and adds six sample fragrances.

The connection string is in `appsettings.json`. It uses the XAMPP defaults: user `root`, no password, port 3306.

## Week 1

| Requirement | Where |
| --- | --- |
| Singleton design pattern | `Data/DatabaseConnection.cs` |
| Insert | `FragranceRepository.InsertAsync`, form at `/Fragrances/Create` |
| Select with filter | `FragranceRepository.GetFilteredAsync`, list at `/Fragrances` |

## Structure (MVC)

| Part | Files |
| --- | --- |
| Model | `Models/Fragrance.cs`, `Models/FragranceFilter.cs`, `Models/FragranceListViewModel.cs` |
| View | `Views/Fragrances/Index.cshtml`, `Views/Fragrances/Create.cshtml` |
| Controller | `Controllers/FragrancesController.cs` |
| Data access | `Data/DatabaseConnection.cs`, `Data/FragranceRepository.cs`, `Data/DatabaseInitializer.cs` |
