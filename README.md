# Web-Lab2 - Robin Andersson, .NET24 GBG

Ett enkelt e-handelssystem utvecklat med Blazor WebAssembly .NET 8 och ASP.NET Core Web API. Projektet följer Repository Pattern och Unit of Work-mönstret med rollbaserad autentisering (JWT).

## Teknologier
- ASP.NET Core 8
- Blazor WebAssembly
- Entity Framework Core
- SQL Server
- JWT-autentisering

## Funktioner
- Produkthantering (lägg till, uppdatera, radera, markera som utgångna)
- Kundhantering
- Orderhantering med orderhistorik
- Rollbaserad åtkomst (Admin/Användare)
- Säker autentisering med JWT

## Installation och körning

### Förutsättningar
- .NET 8 SDK
- SQL Server

### Steg för att komma igång
1. Klona repositoryt: `git clone https://github.com/rojona/Web-Lab2.git`
2. Uppdatera connection string i Web-Lab2.Server/appsettings.json för att matcha din SQL Server-instans.
3. Öppna en kommandotolk i din IDE och kör detta kommando mot mappen Web-Lab2.Server för att ladda databasmigreringar: `dotnet ef database update`
4. Starta projektet genom att köra både Server och Client:
- Server: `dotnet run` från mappen Web-Lab2.Server
- Client: Öppna en ny kommandotolk och kör `dotnet run` från mappen Web-Lab2.Client

Alternativt kan du köra båda projekten samtidigt från Rider eller Visual Studio.

## Testanvändare

För att testa applikationen finns följande användare förkonfigurerade:

- **Admin**
- E-post: admin@admin.com
- Lösenord: admin

- **Vanlig användare**
- E-post: user@user.com
- Lösenord: user

## Projektstruktur

- **Web-Lab2.Server**: Backend API med repository pattern och dataåtkomst
- **Web-Lab2.Client**: Blazor WebAssembly frontend
- Auth: Autentiseringshantering
- Layout: Sidlayout-komponenter
- Models: Klientmodeller
- Pages: Alla sidor i applikationen
- Services: Tjänster för API-kommunikation
