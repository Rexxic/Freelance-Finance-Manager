## **Stufe 1: Konzeption & Planung**

---

### **1. Projektidee**

**Titel:** Freelance Finance Manager  
**Ziel:** Entwicklung einer Desktop-/Web-Anwendung (C# + MySQL), die Freelancer bei der Verwaltung ihrer Finanzen unterstützt – mit Fokus auf variable Einkommen, Ausgaben, Steuerplanung und finanzielle Prognosen.

---

### **2. Zielgruppenanalyse & Problemdefinition**

**Zielgruppe:**

- Freelancer
    
- Gig-Worker
    
- Selbstständige mit unregelmäßigen Einnahmen
    

**Herausforderungen der Zielgruppe:**

- Schwankende Einkommensströme
    
- Fehlende Transparenz über monatliche Ausgaben
    
- Keine klare Struktur zur Steuer- oder Rücklagenplanung
    
- Keine Tools, die auf die Bedürfnisse von Freelancern zugeschnitten sind
    

**Lösungsansatz:**

- Einfache, aber leistungsfähige App mit Modulen zur Einnahmen-/Ausgabenverwaltung, Budgetprognose und Steuerplanung.
    

---

### **3. Entwurf eines ER-Diagramms (mit Erklärung)**

**Datenbankmodell – zentraler Überblick (vereinfacht):**

**Entitäten:**

- `User` (Nutzer der App)
    
- `Income` (Einnahmen)
    
- `Expense` (Ausgaben)
    
- `Category` (Kategorien für Einnahmen/Ausgaben)
    
- `TaxEstimate` (automatisch berechnete Steuerschätzung)
    
- `BudgetForecast` (geplante Einnahmen & Ausgaben)
    

```plaintext
User
- UserID (PK)
- Name
- Email
- PasswordHash

Income
- IncomeID (PK)
- UserID (FK)
- Amount
- Date
- CategoryID (FK)
- Description

Expense
- ExpenseID (PK)
- UserID (FK)
- Amount
- Date
- CategoryID (FK)
- Description

Category
- CategoryID (PK)
- Name
- Type (Income/Expense)

TaxEstimate
- TaxID (PK)
- UserID (FK)
- Year
- EstimatedTaxAmount

BudgetForecast
- ForecastID (PK)
- UserID (FK)
- Month
- PlannedIncome
- PlannedExpense
```

---

### **4. Klassendiagramm (Basisstruktur in C#)**

```mermaid
classDiagram

%% ======== Abstract Base Repository ========
class BaseRepository~T~ {
  +Add(T entity)
  +GetById(int id)
  +GetAll() List~T~
  +Update(T entity)
  +Delete(int id)
}

%% ======== User and Repo ========
class User {
  +int UserID
  +string Name
  +string Email
  +string PasswordHash
}

class UserRepository {
  +Add(User user)
  +GetById(int id)
  +GetAll() List~User~
  +Update(User user)
  +Delete(int id)
}
UserRepository --|> BaseRepository~User~

%% ======== Income and Repo ========
class Income {
  +int IncomeID
  +int UserID
  +decimal Amount
  +DateTime Date
  +int CategoryID
  +string Description
}

class IncomeRepository {
  +Add(Income income)
  +GetById(int id)
  +GetAll() List~Income~
  +Update(Income income)
  +Delete(int id)
}
IncomeRepository --|> BaseRepository~Income~

%% ======== Expense and Repo ========
class Expense {
  +int ExpenseID
  +int UserID
  +decimal Amount
  +DateTime Date
  +int CategoryID
  +string Description
}

class ExpenseRepository {
  +Add(Expense expense)
  +GetById(int id)
  +GetAll() List~Expense~
  +Update(Expense expense)
  +Delete(int id)
}
ExpenseRepository --|> BaseRepository~Expense~

%% ======== Category and Repo ========
class Category {
  +int CategoryID
  +string Name
  +CategoryType Type
}
class CategoryRepository {
  +Add(Category category)
  +GetById(int id)
  +GetAll() List~Category~
  +Update(Category category)
  +Delete(int id)
  +GetByNameAndType(string, CategoryType)
}
CategoryRepository --|> BaseRepository~Category~

%% ======== TaxEstimate and Repo ========
class TaxEstimate {
  +int TaxID
  +int UserID
  +int Year
  +decimal EstimatedTaxAmount
}

class TaxEstimateRepository {
  +Add(TaxEstimate tax)
  +GetById(int id)
  +GetAll() List~TaxEstimate~
  +Update(TaxEstimate tax)
  +Delete(int id)
}
TaxEstimateRepository --|> BaseRepository~TaxEstimate~

%% ======== BudgetForecast and Repo ========
class BudgetForecast {
  +int ForecastID
  +int UserID
  +int Month
  +decimal PlannedIncome
  +decimal PlannedExpense
}

class BudgetForecastRepository {
  +Add(BudgetForecast forecast)
  +GetById(int id)
  +GetAll() List~BudgetForecast~
  +Update(BudgetForecast forecast)
  +Delete(int id)
}
BudgetForecastRepository --|> BaseRepository~BudgetForecast~

%% ======== Enums and Utilities ========
class CategoryType {
  <<enum>>
  +Income
  +Expense
}

class DatabaseManager {
  <<static>>
  +MySqlConnection GetConnection()
  +void InitializeDatabase()
  +void AddUser(string name, string email, string password)
}

%% ======== Associations ========
User --> "*" Income : owns >
User --> "*" Expense : owns >
User --> "*" TaxEstimate : estimates >
User --> "*" BudgetForecast : plans >
Income --> Category
Expense --> Category
Category --> CategoryType

```

---

### **5. Planung des User Interfaces (GUI)**

**Technologie:** C# mit WinForms, WPF oder ASP.NET (je nach Plattform)  
**Designprinzipien:**

- Einfach, modern, responsive
    
- Finanzdashboard als Startansicht
    
- Navigation zu Modulen: Einnahmen, Ausgaben, Budget, Steuer, Einstellungen
    

**Geplante GUI-Komponenten:**

- **Dashboard:** Monatsübersicht, Saldo, Graphen
    
- **Einnahmen-/Ausgabenmaske:** + / - Button, Kategorie, Beschreibung, Betrag
    
- **Steuerschätzung:** Einfaches Tool mit %-Sätzen
    
- **Berichte & Prognosen:** Line-Charts, Balkendiagramme
    
- **Kategorieverwaltung:** Eigene Kategorien erstellen
    
![](Wireframe%20Entwurf.png)