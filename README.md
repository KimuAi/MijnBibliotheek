# Mijn Bibliotheek

Dit is mijn examenproject voor de opleiding **Toegepaste Informatica** aan de **Erasmushogeschool Brussel (EHB)**. 

**Mijn Bibliotheek** is een bibliotheeksysteem gebouwd in **.NET 9**. De solution bestaat uit drie applicaties die samenwerken op dezelfde database:
1. Een **WPF desktopapplicatie** voor lokaal beheer.
2. Een **ASP.NET Core MVC webapplicatie** die ook dient als **REST API**.
3. Een **.NET MAUI mobiele app** die gegevens ophaalt via de REST API.

Alle applicaties maken gebruik van één gedeelde Class Library (`MijnBibliotheekModels`) waarin de database-entiteiten, migraties en het **Identity Framework** staan.

---

## Ontwikkelproces, AI-hulp en gebruikte bronnen

Ik had de basis van dit project opgebouwd, maar daarna heeft het ongeveer **8 maanden stilgelegen**. Toen ik de code na al die tijd opnieuw opende om het project af te werken, liep ik tegen een paar problemen aan:
- Het project wilde niet meer meteen bouwen of opstarten door foutmeldingen in de dependencies.
- Ik wist niet goed meer hoe bepaalde delen van de code in elkaar zaten (zoals de sessies en cookies tussen de MAUI app en de Web API).
- Er waren wat fouten met migraties en de SQLite database.

Omdat ik er zelf niet direct uitkwam, heb ik verschillende hulpmiddelen gebruikt om de fouten op te lossen:

- **ChatGPT (OpenAI)**: Ik heb ChatGPT gebruikt om me te helpen bij het debuggen van de foutmeldingen, het opschonen van kapotte code en om me stap voor stap uit te leggen wat bepaalde stukken code deden.
- **Microsoft Learn**: Om documentatie op te zoeken over .NET 9, Entity Framework Core (soft deletes, IdentityDbContext) en .NET MAUI (`HttpClient` met cookies).

---

## Projectstructuur

De solution is verdeeld in vier projecten:

```text
MijnBibliotheek/
│
├── MijnBibliotheekModels/               # Gedeelde database & modellen
│   ├── Data/
│   │   ├── BibliotheekContext.cs        # DbContext (afgeleid van IdentityDbContext)
│   │   └── BibliotheekSeeder.cs         # Seeder voor rollen, admin-account & data
│   ├── Identity/
│   │   └── AppUser.cs                   # Custom gebruiker met extra veld 'VolledigeNaam'
│   ├── Migrations/                      # EF Core migratiebestanden
│   └── Models/
│       ├── Boek.cs                      # Boek entiteit (Titel, Auteur, ISBN, Status, CategorieId)
│       ├── Categorie.cs                 # Categorie entiteit (Naam, IsDeleted)
│       └── Uitlening.cs                 # Koppeling tussen gebruiker en boek (Datums, Status)
│
├── MijnBibliotheekWPF/                  # WPF Desktop App
│   ├── Services/                        # AuthService (Wrapper rond Identity/DbContext)
│   ├── Windows/                         # Schermen (Login, Boeken, Categorieën, Uitleningen)
│   └── App.xaml                         # Applicatiestart & styling
│
├── MijnBibliotheekWeb/                  # ASP.NET Core MVC Webapp + REST API
│   ├── Controllers/
│   │   ├── Api/                         # REST API (AuthController, BoekenApiController, etc.)
│   │   ├── BoekenController.cs          # MVC Controller voor Boeken Razor views
│   │   ├── CategorieenController.cs      # MVC Controller voor Categorieën Razor views
│   │   └── UitleningenController.cs     # MVC Controller voor Uitleningen Razor views
│   ├── Views/                           # Razor Views + Bootstrap UI
│   └── Program.cs                       # DI, Middleware & Cookie authenticatie
│
└── MijnBibliotheekMAUI/                 # Mobiele App (.NET MAUI)
    ├── Services/                        # ApiSession, AuthApiService & BibliotheekApiService
    ├── ViewModels/                      # MVVM ViewModels (LoginVm, BoekenVm, etc.)
    └── Pages/                           # XAML schermen (LoginPage, BoekenPage, etc.)
```

---

## Functionaliteiten

### Rollen en toegangsbeheer
Met **ASP.NET Core Identity** zijn er drie rollen ingesteld:

- **Admin**: Mag alles doen (boeken, categorieën en uitleningen toevoegen, aanpassen en verwijderen).
- **Medewerker**: Heeft dezelfde beheerrechten als een Admin.
- **Lid**: Kan de catalogus bekijken, boeken uitlenen en eigen uitleningen inzien of terugbrengen.

De UI in WPF, Web en MAUI past zich automatisch aan op basis van de rol van de ingelogde gebruiker.

### Wat kan het systeem?
- **Boekenbeheer**: Boeken toevoegen, bewerken, zoeken (titel, auteur, ISBN) en filteren op categorie.
- **Soft Delete**: Als een boek of categorie wordt verwijderd, krijgt `IsDeleted` de waarde `true`. De data blijft zo bewaard in de database.
- **Categoriebeheer**: Categorieën toevoegen en koppelen aan boeken.
- **Uitleningen**: Boeken uitlenen aan leden. Het boek wordt automatisch op *niet beschikbaar* gezet tot het weer is teruggebracht.

---

## Overzicht van de applicaties

### 1. WPF Desktop App (`MijnBibliotheekWPF`)
- Desktop-interface gemaakt voor beheerders.
- Werkt rechtstreeks op de SQLite database via `MijnBibliotheekModels`.
- Bevat losse schermen voor inloggen, boekenbeheer, categorieën en uitleningen.

### 2. Webapplicatie & REST API (`MijnBibliotheekWeb`)
- Webapp gebouwd met ASP.NET Core MVC en Bootstrap.
- Bevat ook de **REST API endpoints** voor de MAUI app:

| Methode | Endpoint | Omschrijving | Toegang |
|:---|:---|:---|:---|
| `POST` | `/api/auth/login` | Inloggen | Anoniem |
| `POST` | `/api/auth/register` | Account aanmaken | Anoniem |
| `GET` | `/api/auth/me` | Gegevens van ingelogde gebruiker ophalen | Ingelogd |
| `POST` | `/api/auth/logout` | Uitloggen | Ingelogd |
| `GET` | `/api/boekenapi` | Alle boeken ophalen | Ingelogd |
| `GET` | `/api/categorieenapi` | Alle categorieën ophalen | Ingelogd |
| `GET` | `/api/uitleningenapi/mijn` | Eigen uitleningen ophalen | Ingelogd (Lid) |
| `POST` | `/api/uitleningenapi/leen/{boekId}` | Boek uitlenen | Ingelogd (Lid) |

- Werkt met cookie-authenticatie. API-fouten sturen HTTP statuscodes (401/403) terug zonder redirects.

### 3. MAUI Mobiele App (`MijnBibliotheekMAUI`)
- Mobiele app gebouwd met .NET MAUI (MVVM-patroon).
- Gebruikt `ApiSession` (`HttpClient` met `CookieContainer`) om de inlogsessie te bewaren bij verzoeken naar de REST API.
- Leden kunnen via hun telefoon boeken bekijken, uitlenen en hun actieve uitleningen opvolgen.

---

## Gebruikte technologieën

- **Frameworks**: .NET 9, WPF, ASP.NET Core MVC, .NET MAUI
- **Talen**: C#, XAML, HTML, CSS (Bootstrap)
- **Database**: SQLite met Entity Framework Core 9 (Code-First)
- **Authenticatie**: ASP.NET Core Identity Framework
- **API & Netwerk**: REST API, `HttpClient`, `CookieContainer`
- **Hulpmiddelen**: Visual Studio 2022, ChatGPT, Microsoft Learn, Stack Overflow

---

## Hoe het project op te starten

### Vereisten
- Visual Studio 2022 (v17.8 of hoger) met de workloads:
  - *.NET desktop development*
  - *ASP.NET and web development*
  - *.NET Multi-platform App UI development*

### Stappen

1. **Repository clonen**:
   ```bash
   git clone https://github.com/jouw-gebruikersnaam/MijnBibliotheek.git
   cd MijnBibliotheek
   ```

2. **Solution openen**:
   Open `MijnBibliotheek.sln` in Visual Studio.

3. **Database aanmaken**:
   - Open de **Package Manager Console** (`Tools` > `NuGet Package Manager` > `Package Manager Console`).
   - Selecteer `MijnBibliotheekModels` als **Default project**.
   - Run het commando:
     ```powershell
     Update-Database
     ```

4. **Webapp & API starten**:
   - Klik met de rechtermuisknop op `MijnBibliotheekWeb` en kies **Set as Startup Project**.
   - Druk op `F5`. De database wordt automatisch gevuld met testdata en de standaard accounts/rollen.

5. **MAUI App starten**:
   - Zorg dat `MijnBibliotheekWeb` op de achtergrond draait.
   - Klik met de rechtermuisknop op `MijnBibliotheekMAUI` -> **Set as Startup Project**.
   - Kies Windows Machine of een Android Emulator en start de app.

6. **WPF App starten**:
   - Klik met de rechtermuisknop op `MijnBibliotheekWPF` -> **Set as Startup Project**.
   - Druk op `run`.
