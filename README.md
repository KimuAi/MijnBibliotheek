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
