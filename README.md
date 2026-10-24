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

## AI advisor (Gemini)

The AI advisor page (`/Advisor`) recommends fragrances from the catalogue using the Gemini API.

1. Create a free API key at https://aistudio.google.com/apikey
2. In the project folder, run:

   ```
   dotnet user-secrets set "Gemini:ApiKey" "YOUR_API_KEY"
   ```

3. Restart the application.

The key is stored in your user profile, outside the project folder, so it never ends up on GitHub. Every team member sets their own key. Without a key the rest of the site works and the advisor page shows a warning.

The model and the other settings are in the `Gemini` section of `appsettings.json`.

## Features

| Requirement | Where |
| --- | --- |
| Singleton design pattern | `Data/DatabaseConnection.cs` |
| Insert | `FragranceRepository.InsertAsync`, form at `/Fragrances/Create` |
| Select with filter | `FragranceRepository.GetFilteredAsync`, list at `/Fragrances` |
| Update | `FragranceRepository.UpdateAsync`, form at `/Fragrances/Edit/{id}` |
| Delete | `FragranceRepository.DeleteAsync`, confirmation at `/Fragrances/Delete/{id}` |
| AI model (Gemini) | `Services/GeminiService.cs`, page at `/Advisor` |

## Good practices in the AI integration

- The API key is kept out of the code and the repository (user secrets), and it is sent in a header, not in the URL.
- The controller depends on the `IAiService` interface, not on Gemini, so the model can be replaced without changing the controller.
- `HttpClient` is created by `IHttpClientFactory` (typed client) with a timeout.
- The settings are bound with the options pattern and checked on startup.
- Errors (wrong key, usage limit, timeout, blocked answers, no connection) are logged and shown as clear messages instead of crashing the page.
- The question is validated (required, at most 500 characters).
- The prompt has separate system instructions, and the answer is grounded in the fragrances from the database.
- The answer length and the thinking level are limited to keep requests fast and within the free quota.
- The answer is shown as plain text, so it cannot inject HTML into the page.

## Structure (MVC)

| Part | Files |
| --- | --- |
| Model | `Models/Fragrance.cs`, `Models/FragranceFilter.cs`, `Models/FragranceListViewModel.cs`, `Models/AdvisorViewModel.cs` |
| View | `Views/Fragrances/Index.cshtml`, `Create.cshtml`, `Edit.cshtml`, `Delete.cshtml`, `Views/Advisor/Index.cshtml` |
| Controller | `Controllers/FragrancesController.cs`, `Controllers/AdvisorController.cs` |
| Data access | `Data/DatabaseConnection.cs`, `Data/FragranceRepository.cs`, `Data/DatabaseInitializer.cs` |
| AI service | `Services/IAiService.cs`, `Services/GeminiService.cs`, `Services/GeminiOptions.cs` |
