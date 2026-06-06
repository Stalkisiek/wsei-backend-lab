# WSEI Backend Lab

## Projekt: Dziekanat

### Autor
- Mateusz Migdał

### Opis Funkcjonalności

#### Zadanie 11

Ten projekt rozszerza aplikację o kompleksowe API dostępne dla wykładowców i pracowników dziekanatu. System umożliwia:

1. **Przeglądanie Studentów**
   - Wykładowca może przeglądać listę studentów zapisanych na kursy prowadzone przez tego prowadzącego
   - Informacje zawierają: ID studenta, imię, nazwisko, email, PESEL, rok studiów, kierunek

2. **Zarządzanie Ocenami**
   - Dodawanie ocen dla studentów w kursach prowadzonych przez wykładowcę
   - Edycja istniejących ocen
   - Każda zmiana jest notowana w historii (kto i kiedy zmienił)
   - Ocenę może wstawić prowadzący kurs lub pracownik dziekanatu

3. **PESEL ValueObject**
   - Klasa `Pesel` zaimplementowana jako ValueObject
   - Walidacja formatu (11 cyfr) i sumy kontrolnej algorytmu PESEL
   - Metody do ekstrakcji daty urodzenia i płci

### Kluczowe Komponenty Task 11

#### 1. PESEL ValueObject

```csharp
public sealed class Pesel : ValueObject
{
    public static Pesel From(string value) { }
    public DateTime GetBirthDate() { }
    public char GetGender() { }
    public override string ToString() { }
}
```

**Cechy:**
- Walidacja 11-cyfrowa liczba
- Weryfikacja sumy kontrolnej (algorytm PESEL)
- Konwersja do/z string w bazie danych

#### 2. Grade Model z Historią Zmian

```csharp
public class Grade : EntityBase
{
    public GradeValue GradeValue { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public List<GradeChangeHistory> ChangeHistory { get; set; }
}

public class GradeChangeHistory : EntityBase
{
    public GradeValue? PreviousValue { get; set; }
    public GradeValue NewValue { get; set; }
    public string ChangedBy { get; set; }
    public DateTime ChangedAt { get; set; }
}
```

#### 3. Serwis Wykładowcy

```csharp
public interface ILecturerService
{
    Task<IEnumerable<LecturerCourseDto>> GetCoursesByLecturerAsync(Guid lecturerId);
    Task<IEnumerable<LecturerStudentDto>> GetStudentsByCourseAsync(Guid lecturerId, Guid courseId);
    Task<IEnumerable<GradeWithHistoryDto>> GetStudentGradesAsync(Guid lecturerId, Guid studentId, Guid courseId);
    Task<GradeWithHistoryDto> AddGradeAsync(Guid lecturerId, Guid studentId, Guid courseId, LecturerGradeUpdateDto dto, string changedBy);
    Task<GradeWithHistoryDto> UpdateGradeAsync(Guid lecturerId, Guid gradeId, LecturerGradeUpdateDto dto, string changedBy);
}
```

#### 4. Polityki Autoryzacji

```csharp
public enum AppPolicies
{
    Administrator,
    Lecturer,
    DeanOffice,
    LecturerOrDeanOffice
}
```

### Endpointy API

#### Kursy Wykładowcy

```http
GET /api/lecturers/{lecturerId}/courses
Authorization: Bearer <token>
```

Wymagane role: Lecturer lub DeanOffice

**Response (200):**
```json
[
  {
    "id": "guid",
    "code": "INF-101",
    "name": "Programowanie 1",
    "ectsCredits": 5,
    "completionType": "Examination",
    "enrolledStudentsCount": 25
  }
]
```

#### Studenci w Kursie

```http
GET /api/lecturers/{lecturerId}/courses/{courseId}/students
Authorization: Bearer <token>
```

**Response (200):**
```json
[
  {
    "id": "guid",
    "studentId": "ALB-2025-0001",
    "firstName": "Piotr",
    "lastName": "Zieliński",
    "email": "piotr.zielinski@wsei.edu.pl",
    "pesel": "01010122349",
    "yearOfStudy": 1,
    "programName": "INF-BSC"
  }
]
```

#### Oceny Studenta

```http
GET /api/lecturers/{lecturerId}/students/{studentId}/courses/{courseId}/grades
Authorization: Bearer <token>
```

**Response (200):**
```json
[
  {
    "id": "guid",
    "value": 4.5,
    "type": "Final",
    "date": "2026-06-05T10:30:00Z",
    "lecturerName": "Jan Kowal",
    "createdBy": "lecturer1",
    "createdAt": "2026-06-05T10:30:00Z",
    "modifiedBy": "deanoffice1",
    "modifiedAt": "2026-06-05T12:00:00Z",
    "changeHistory": [
      {
        "id": "guid",
        "previousValue": 4.0,
        "newValue": 4.5,
        "changedBy": "deanoffice1",
        "changedAt": "2026-06-05T12:00:00Z"
      }
    ]
  }
]
```

#### Dodaj Ocenę

```http
POST /api/lecturers/{lecturerId}/students/{studentId}/courses/{courseId}/grades
Content-Type: application/json
Authorization: Bearer <token>

{
  "gradeValue": 4.5,
  "gradeType": "Final",
  "date": "2026-06-05T10:30:00Z"
}
```

**Response (201):**
```json
{
  "id": "guid",
  "value": 4.5,
  "type": "Final",
  "date": "2026-06-05T10:30:00Z",
  "lecturerName": "Jan Kowal",
  "createdBy": "lecturer1",
  "createdAt": "2026-06-05T10:30:00Z",
  "modifiedBy": null,
  "modifiedAt": null,
  "changeHistory": [
    {
      "id": "guid",
      "previousValue": null,
      "newValue": 4.5,
      "changedBy": "lecturer1",
      "changedAt": "2026-06-05T10:30:00Z"
    }
  ]
}
```

#### Edytuj Ocenę

```http
PUT /api/lecturers/{lecturerId}/grades/{gradeId}
Content-Type: application/json
Authorization: Bearer <token>

{
  "gradeValue": 5.0,
  "gradeType": "Final",
  "date": "2026-06-05T10:30:00Z"
}
```

### Baza Danych

#### Nowa Tabela GradeChangeHistory

| Kolumna | Typ | Opis |
|---------|-----|------|
| Id | GUID | Klucz główny |
| GradeId | GUID | Referencja do Grade |
| PreviousValue | integer (nullable) | Poprzednia wartość oceny |
| NewValue | integer | Nowa wartość oceny |
| ChangedBy | string | ID użytkownika, który dokonał zmiany |
| ChangedAt | DateTime | Data i godzina zmiany |

#### Zmiana w Tabelach

- **Students**: `NationalId` zamieniony na `Pesel` 
- **Lecturers**: `NationalId` zamieniony na `Pesel`
- **Courses**: Dodana kolumna `LecturerId` (Optional) - relacja do Lecturer

### Dane Testowe

Seeder zawiera:

**Użytkownicy:**
- `admin1` / `admin2` (Administrator) - pełne dostępy
- `lecturer1` (Lecturer) - dostęp do swoich kursów i ocen
- `deanoffice1` (DeanOffice) - dostęp do wszystkich ocen

**Prowadzący:**
- Jan Kowal (Dr) - PESEL: 90010122349
- Anna Nowak (Prof) - PESEL: 92020252348

**Studenci:**
- Piotr Zieliński (1 rok) - PESEL: 01010122349
- Alicja Maj (2 rok) - PESEL: 02020252348

### Testowanie

#### Unit testy pod zadanie 11

**PeselTests.cs** - Kompleksowe testy walidacji i funkcjonalności PESEL ValueObject:
- Walidacja formatu (11 cyfr, tylko cyfry)
- Sprawdzenie sumy kontrolnej algorytmu PESEL
- Ekstrakcja daty urodzenia z PESEL (obsługa różnych stuleci: 1800, 1900, 2000)
- Określanie płci na podstawie cyfry PESEL
- Testy ValueObject (równość, hash code, konwersja typów)
- Testy metod TryFrom i FromOrNull dla obsługi błędów

**GradeChangeHistoryTests.cs** - Testy śledzenia zmian ocen:
- Inicjalizacja historii zmian i automatyczne przypisywanie timestampów
- Śledzenie danych modyfikacji (ModifiedBy, ModifiedAt)
- Zapisywanie poprzedniej i nowej wartości oceny
- Identyfikacja użytkownika dokonującego zmian
- Chronologiczne porządkowanie zmian
- Obsługa casos dla nowych ocen (PreviousValue = null)


