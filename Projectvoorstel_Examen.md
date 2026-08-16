# Projectvoorstel Examen .NET Advanced & .NET Project (MAUI)

**Studentgegevens**
- **Naam initiatiefnemer:** Kimberley Thill  
- **Opleiding:** Toegepaste Informatica (EHB)  
- **Vakken:** Examenopdracht .NET Advanced (Deel 1) & Examenopdracht .NET Project (MAUI)  
- **Projectvorm:** Individueel project  
- **Werknaam project:** Bibliotheekbeheer - Cultureel Centrum  

---

## 1. Korte Omschrijving van het Project

Het project **Bibliotheekbeheer - Cultureel Centrum** betreft de ontwikkeling van een geïntegreerd informatiesysteem voor het beheren van een bibliotheek binnen een fictief cultureel centrum. 

Het systeem bestaat uit drie hoofdonderdelen:
1. **Gedeelde Class Library (`MijnBibliotheekModels`)**: Bevat de Entity Framework Core databankcontext (`BibliotheekContext`), gegevensmodellen (`Boek`, `Categorie`, `Uitlening`) en custom Identity-gebruiker (`AppUser`).
2. **Webapplicatie & REST API (`MijnBibliotheekWeb`)**: Gebouwd in ASP.NET Core MVC (.NET 9) met Bootstrap UI, AJAX-functionaliteiten, meertalige ondersteuning (NL/EN), rol-gebaseerde autorisatie en een RESTful API.
3. **Mobiele Applicatie (`MijnBibliotheekMAUI`)**: Gebouwd in .NET MAUI (.NET 9) volgens MVVM-architectuur. Consumeert de REST API en slaat gegevens lokaal op in een SQLite-databank (`local_bibliotheek.db3`) voor offline gebruik.

---

## 2. Motivatie & Bedrijfscontext

### Motivatie
Een bibliotheekbeheersysteem is een klassiek maar veelzijdig voorbeeld van een administratief informatiesysteem. Het stelt mij in staat om alle aspecten van moderne .NET-ontwikkeling te demonstreren: van gegevensmodellering en asynchrone API-communicatie tot dynamische web-interfaces en mobiele applicaties met offline gegevenscaching.

### Bedrijfscontext
Het fictieve Cultureel Centrum biedt een uitgebreide collectie boeken en media aan haar leden. Om de administratieve druk te verlagen en de service naar de leden te verbeteren, heeft de organisatie nood aan:
- Centraal beheer van boeken, categorieën en uitleningen.
- Rol-gebaseerde toegangscontrole voor beheerders, medewerkers en leden.
- Een responsieve webomgeving voor administratieve taken.
- Een mobiele app waarmee leden onderweg hun uitleningen kunnen inzien en boeken kunnen reserveren of lenen, zelfs als de internetverbinding tijdelijk wegvalt.

---

## 3. Analyse & User Stories

### User Stories - Webapplicatie (ASP.NET Core MVC)
- **Als Beheerder / Medewerker** wil ik boeken kunnen toevoegen, bewerken, soft-deleten en hun status aanpassen via AJAX, zodat het assortiment up-to-date blijft.
- **Als Beheerder / Medewerker** wil ik categorieën kunnen beheren en gekoppelde boeken overzien.
- **Als Gebruiker (Lid)** wil ik boeken kunnen doorzoeken op titel, auteur en ISBN, filteren op categorie, en sorteren op verschillende kolommen.
- **Als Gebruiker** wil ik door middel van paginering snel door grote lijsten met boeken kunnen navigeren.
- **Als Gebruiker** wil ik de taal van de interface eenvoudig kunnen omschakelen tussen Nederlands en Engels.
- **Als Gebruiker** wil ik duidelijke fout- en succesboodschappen ontvangen bij acties.

### User Stories - Mobiele App (.NET MAUI)
- **Als Mobiele Gebruiker** wil ik veilig kunnen inloggen met mijn account.
- **Als Mobiele Gebruiker** wil ik een overzicht van beschikbare boeken bekijken en direct een boek kunnen lenen.
- **Als Mobiele Gebruiker** wil ik mijn actieve uitleningen en inleverdatums bekijken.
- **Als Mobiele Gebruiker** wil ik de app kunnen blijven gebruiken wanneer ik geen netwerkverbinding heb, door gebruik te maken van lokaal gecachte gegevens (SQLite).

---

## 4. Overzicht van Pagina's en Schermen

### Webapplicatie (`MijnBibliotheekWeb`)
1. **Boeken Index (`/Boeken/Index`)**: Dynamisch overzicht van boeken met live AJAX-zoekbalk, categorie-filter, kolom-sortering, paginering, meertalige weergave en snelle AJAX status-toggle.
2. **Boek Toevoegen / Bewerken (`/Boeken/Create`, `/Boeken/Edit/{id}`)**: Formulier met data-annotatie validaties en categorie-selectie.
3. **Categorieën Beheer (`/Categorieen/Index`)**: Overzicht en beheer van boekcategorieën.
4. **Uitleningen Overzicht (`/Uitleningen/Index`)**: Beheer van actieve en afgeronde uitleningen per lid.
5. **Authenticatie Schermen (`/Identity/Account/Login`, `/Identity/Account/Register`)**: Inloggen en registreren.

### Mobiele App (`MijnBibliotheekMAUI`)
1. **LoginPage (`LoginPage.xaml`)**: Inlogscherm met API-authenticatie en opslag van de sessie-cookie.
2. **BoekenPage (`BoekenPage.xaml`)**: Lijstweergave van boeken, mogelijkheid tot lenen en verwijderen (afhankelijk van rol).
3. **CategorieenPage (`CategorieenPage.xaml`)**: Categorieënoverzicht met filtermogelijkheid.
4. **UitleningenPage (`UitleningenPage.xaml`)**: Overzicht van de eigen uitleningen van de ingelogde gebruiker.

---

## 5. Fundamentele Eisen Matrix & Compliance

### Deel 1: ASP.NET Core (.NET Advanced)
| Criterium / Requirement | Geïmplementeerd In Project | Status |
|---|---|---|
| **ASP.NET Core (MVC)** | `MijnBibliotheekWeb` project | ✅ Voltooid |
| **Database met ≥3 gerelateerde tabellen in Class Library** | `MijnBibliotheekModels` met `Boeken`, `Categorieen`, `Uitleningen` & `AspNetUsers` via EF Core | ✅ Voltooid |
| **Asynchrone communicatie** | Systematisch `async`/`await` in DbContext, Controllers, Services en API | ✅ Voltooid |
| **Ergonomische UI met Bootstrap** | Responsief design met Bootstrap 5, badges, alerts en navigation bar | ✅ Voltooid |
| **Consistente menu-/tabstructuur** | `_Layout.cshtml` met meertalig navigatiemenu en rol-afhankelijke acties | ✅ Voltooid |
| **Paginering waar zinvol** | Paginering (`CurrentPage`, `TotalPages`, `PageSize`) op `BoekenController` | ✅ Voltooid |
| **Data-annotaties voor validatie** | `[Required]`, `[MaxLength]`, `[DataType]`, `[Display]` op modellen/viewmodels | ✅ Voltooid |
| **Selectie- en sorteringsvelden** | Filters (categorie/zoektekst) & multi-kolom sortering op `Boeken/Index` | ✅ Voltooid |
| **Identity Framework (≥3 rollen)** | `Admin`, `Medewerker`, `Lid` rollen met `AppUser` custom velden | ✅ Voltooid |
| **Autorisatie** | `[Authorize]` en `[Authorize(Roles = "...")]` op controllers & Razor views | ✅ Voltooid |
| **Minstens 1x AJAX partial refresh** | Live AJAX search/sort/filter via `IndexPartial` & AJAX status-toggle `QuickToggleAvailability` | ✅ Voltooid |
| **Extra Middleware** | `CookieLoggingMiddleware` voor cookie-logging en cultuurafhandeling | ✅ Voltooid |
| **RESTful API methoden** | `AuthController`, `BoekenApiController`, `CategorieenApiController`, `UitleningenApiController` | ✅ Voltooid |
| **VOLLEDIGE meertaligheid (≥2 talen)** | `LanguageService` met Nederlands (NL) en Engels (EN) + taal-wisselaar cookie | ✅ Voltooid |
| **Foutloos werken & foutopvang** | Exception middleware, alert banners (`TempData["Success"]`/`TempData["Error"]`) | ✅ Voltooid |

### Deel 2: MAUI (.NET Project)
| Criterium / Requirement | Geïmplementeerd In Project | Status |
|---|---|---|
| **Realisatie in MAUI** | `MijnBibliotheekMAUI` project in .NET 9 | ✅ Voltooid |
| **API-databron** | Consumptie van `MijnBibliotheekWeb` REST API via `HttpClient` met cookie-container | ✅ Voltooid |
| **Lokale "lite" databank voor offline gebruik** | `LocalDbService` met SQLite (`sqlite-net-pcl`) voor offline opslag en automatisch herstel bij netwerkuitval | ✅ Voltooid |
| **Class Library hergebruik** | Modellen en DTO structuur gedeeld met `MijnBibliotheekModels` | ✅ Voltooid |
| **Analyse met User Stories & Pages** | Opgenomen in dit projectvoorstel | ✅ Voltooid |

---

## 6. Conclusie

Het project **Bibliotheekbeheer - Cultureel Centrum** voldoet aan alle voorwaarden en fundamentele eisen van zowel de **.NET Advanced** examenopdracht als de **.NET Project (MAUI)** opdracht. De combinatie van een robuuste EF Core Class Library, een meertalige en responsieve ASP.NET Core webapplicatie met AJAX en REST API, en een offline-capable MAUI mobiele app vormt een compleet en kwalitatief hoogstaand examenwerkstuk.
