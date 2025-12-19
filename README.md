# Mijn Bibliotheek

**Mijn Bibliotheek** is een desktopapplicatie ontwikkeld in **WPF (.NET 9)** met ondersteuning voor **Entity Framework Core** en **Identity Framework**.  
De applicatie maakt het mogelijk om boeken te beheren, categorieën te organiseren en uitleningen bij te houden op een overzichtelijke manier.  

Dit project werd gerealiseerd als examenopdracht binnen de opleiding **Toegepaste Informatica (EHB)**.

---

## 1. Functionaliteiten

### 1.1 Rollen en toegangsbeheer
De applicatie gebruikt **Identity Framework** voor gebruikersbeheer.  
Elke gebruiker heeft een rol die bepaalt welke acties mogelijk zijn:

| Rol | Mogelijkheden |
|----|---------------|
| **Admin** | Alle gegevens beheren (Boeken, Categorieën, Uitleningen) |
| **Medewerker** | Zelfde mogelijkheden als Admin |
| **Lid** | Alleen bekijken, geen wijzigingen uitvoeren |

De interface past zich aan afhankelijk van de rol (knoppen, invoervelden, opties).

---

### 1.2 Boekenbeheer
- Nieuwe boeken aanmaken en bestaande boeken aanpassen
- Zoeken op titel, auteur of ISBN
- Filteren op categorie
- Soft-delete i.p.v. permanent verwijderen
- Automatische beschikbaarheidsstatus bij uitlenen

### 1.3 Categoriebeheer
- Nieuwe categorieën toevoegen
- Categorieën worden direct gebruikt in het boekenscherm
- Soft-delete is toegepast voor consistente databeheer

### 1.4 Uitleningen
- Uitlenen van boeken aan geregistreerde gebruikers
- Boek wordt automatisch als *niet beschikbaar* gemarkeerd
- Terugbrengen bijhouden en boek opnieuw beschikbaar maken
- Overzicht met gebruikers, datums en status

---

## 2. Architectuur
Solution
MijnBibliotheek/
│
├── MijnBibliotheekModels/               # Class Library (Model + Database)
│   ├── Data/
│   │   ├── BibliotheekContext.cs        # DbContext (afgeleid van IdentityDbContext)
│   │   └── BibliotheekSeeder.cs         # Seeder voor basisdata
│   │
│   ├── Identity/
│   │   └── AppUser.cs                   # Eigen Identity-gebruiker met extra veld 'VolledigeNaam'
│   │
│   ├── Migrations/                      # EF Core migratiebestanden (automatisch gegenereerd)
│   │   └── ...                          # Up/Down database schema bestanden
│   │
│   └── Models/
│       ├── Boek.cs                       # Boek model (Titel, Auteur, ISBN, Beschikbaar ...)
│       ├── Categorie.cs                  # Categorie model (Naam + Soft Delete)
│       └── Uitlening.cs                  # Uitleningen model (Boek <-> User)
│
│
└── MijnBibliotheekWPF/                  # WPF Desktop UI
    ├── App.xaml                          # Applicatie entry + resource setup
    ├── App.xaml.cs
    │
    ├── Services/
    │   └── AuthService.cs                # Login & registratie logica (Identity wrapper)
    │
    ├── Windows/                          # Alle schermen (UI + code-behind)
    │   ├── LoginWindow.xaml              # Inloggen + Registreren
    │   ├── LoginWindow.xaml.cs
    │   │
    │   ├── MainWindow.xaml               # Hoofdmenu + navigatie
    │   ├── MainWindow.xaml.cs
    │   │
    │   ├── BoekenWindow.xaml             # Beheer boeken (CRUD + zoek + categorie filter)
    │   ├── BoekenWindow.xaml.cs
    │   │
    │   ├── CategorienWindow.xaml         # Beheer categorieën (CRUD)
    │   ├── CategorienWindow.xaml.cs
    │   │
    │   ├── UitleningWindow.xaml          # Uitlenen & terugbrengen
    │   └── UitleningWindow.xaml.cs
    │
    └── Properties/
        └── AssemblyInfo.cs


- **Code First** database met **Entity Framework Core**
- **SQLite** databank wordt automatisch aangemaakt
- **IdentityDbContext** gebruikt voor gebruikersbeheer
- Soft-delete op alle modellen via IsDeleted + DeletedAt

---

## 3. Technologieën

| Technologie | Toepassing |
|------------|-----------|
| WPF (XAML) | User Interface |
| C# (.NET 9) | Applicatielogica |
| Entity Framework Core | ORM & migraties |
| Identity Framework | Rollen en authenticatie |
| SQLite | Lokale database-opslag |

---

## 4. Installatie en gebruik

1. Clone de repository
2. Open de solution in **Visual Studio 2022 of hoger**
3. Open **Package Manager Console**
4. Voer de migraties uit:
   ```powershell
   Update-Database

---------------------------------------------------------------------------------------------------

# Mijn Bibliotheek – Webapplicatie

MijnBibliotheekWeb is een ASP.NET Core MVC webapplicatie gebouwd met .NET 9,
die fungeert als Razor frontend, authenticatieplatform en RESTFull API
voor externe clients zoals de .NET MAUI mobiele app.

Dit project maakt deel uit van een grotere solution en werd gerealiseerd als
examenproject binnen Toegepaste Informatica (EHB).

---

## 1. Doel

- Beheren van een bibliotheeksysteem via een webinterface

- Authenticatie en autorisatie met Identity Framework

- Aanbieden van een beveiligde REST API voor mobiele apps

- Centraliseren van businesslogica via een gedeelde Class Library

---

## 2. Functionaliteiten

### 2.1 Authenticatie & Autorisatie

- Login en registratie met Identity

- Custom AppUser met extra eigenschappen

- Rollen:

    - Admin

    - Medewerker

    - Lid

- Rol-gebaseerde toegang:

    - UI (Razor)

    - Controllers

    - API endpoints

---

### 2.2 Boeken

- Overzicht met filtering en sortering

- CRUD-functionaliteit (Admin/Medewerker)

- Soft-delete

- Automatische beschikbaarheid

---

### 2.3 Categorieën

- Beheer van categorieën

- Relatie met boeken

- Soft-delete

---

### 2.4 Uitleningen

- Uitlenen en terugbrengen van boeken

- Koppeling boek ↔ gebruiker

- Overzicht per gebruiker

- Statusbeheer

---

## 3. REST API

De webapp voorziet RESTFull API-endpoints voor gebruik door de MAUI app.

Voorbeelden:
POST   /api/auth/login
POST   /api/auth/register
GET    /api/auth/me

GET    /api/boekenapi
GET    /api/categorieenapi
GET    /api/uitleningenapi/mijn
POST   /api/uitleningenapi/leen/{boekId}


- Cookie-based authenticatie

- API’s respecteren dezelfde autorisatieregels als de webpagina’s

- Geen redirects bij API-auth fouten (401/403)

---

## 4. Architectuur

- ASP.NET Core MVC (.NET 9)

- Razor Views + Bootstrap

- Entity Framework Core

- Identity Framework

- SQLite database

- Gedeelde Class Library (MijnBibliotheekModels)

- Custom middleware & cookie-configuratie

- Logging & foutafhandeling

---

## 5. Projectstructuur (vereenvoudigd)

MijnBibliotheekWeb/
│
├── Controllers/
│   ├── Api/
│   │   ├── AuthController.cs
│   │   ├── BoekenApiController.cs
│   │   ├── CategorieenApiController.cs
│   │   └── UitleningenApiController.cs
│
├── Views/              # Razor frontend
├── Program.cs
└── appsettings.json

---

## 6. Opstarten

1. Open de solution in Visual Studio 2022+

2. Zet MijnBibliotheekWeb als Startup Project

3. Run het project

4. Database + rollen + admin-user worden automatisch geseed

---

## 7. Gebruikte technologieën

- ASP.NET Core MVC

- Razor

- Entity Framework Core

- Identity Framework

- SQLite

- Bootstrap

- .NET 9

---------------------------------------------------------------------------------------------------

# Mijn Bibliotheek – MAUI App

MijnBibliotheekMAUI is een .NET MAUI mobiele applicatie
die via RESTFull API communiceert met de MijnBibliotheekWeb applicatie.

De app is gebouwd met XAML + MVVM en maakt gebruik van dezelfde
modellen en database-structuur via een gedeelde Class Library.

---

## 1. Doel

- Mobiele toegang tot het bibliotheeksysteem

- Inloggen en registreren via de web-API

- Gegevens ophalen en bewerken via REST

- Rol-afhankelijk gedrag (Admin vs User)

---

## 2. Functionaliteiten

### 2.1 Authenticatie

- Login via API (/api/auth/login)

- Registratie via API

- Cookie-based authenticatie

- Automatische sessiebeheer via ApiSession

---

### 2.2 Boeken

- Lijst van beschikbare boeken

- Uitlenen van boeken (User)

- Beheer en verwijderen (Admin)

- Live synchronisatie met webdatabase

---

### 2.3 Categorieën

- Overzicht van categorieën

- Alleen beheerbaar door Admin

---

### 2.4 Uitleningen

- Overzicht van eigen uitleningen

- Status van boeken

- Alleen toegankelijk voor ingelogde gebruikers

---

### 2.5 Logout

- Logout via API

- Terugkeer naar LoginPage

---

## 3. Architectuur
- .NET MAUI (.NET 9)

- XAML frontend

- MVVM-architectuur

- Dependency Injection

- HttpClient + CookieContainer

- REST API consumptie

- Gedeelde Class Library (MijnBibliotheekModels)

---

## 4. Projectstructuur (vereenvoudigd)

MijnBibliotheekMAUI/
│
├── Pages/
│   ├── LoginPage.xaml
│   ├── BoekenPage.xaml
│   ├── CategorieenPage.xaml
│   └── UitleningenPage.xaml
│
├── ViewModels/
│   ├── LoginVm.cs
│   ├── BoekenVm.cs
│   ├── CategorieenVm.cs
│   └── UitleningenVm.cs
│
├── Services/
│   ├── AuthApiService.cs
│   ├── BibliotheekApiService.cs
│   └── ApiSession.cs
│
├── AppShell.xaml
├── MauiProgram.cs
└── App.xaml

---

## 5. API-afhankelijkheid

De MAUI app vereist dat MijnBibliotheekWeb draait.

Base URL (voorbeeld):

https://localhost:7234/

---

## 6. Opstarten

1. Start MijnBibliotheekWeb

2. Start daarna MijnBibliotheekMAUI

3. Log in met een bestaande gebruiker

4. Functionaliteit wordt aangepast op basis van rol

---

## 7. Gebruikte technologieën

- .NET MAUI

- XAML

- MVVM

- HttpClient

- RESTFull API

- .NET 9
