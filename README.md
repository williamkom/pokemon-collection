# Pokémon Collection App

Eine Full-Stack Webanwendung zum Durchsuchen und Sammeln von Pokémon basierend auf der offiziellen [PokéAPI](https://pokeapi.co/).

Das Projekt besteht aus:

- **Backend:** ASP.NET Core Web API (.NET 10) mit JWT-Authentifizierung und PokéAPI-Integration.
- **Datenbank:** PostgreSQL 18 mit Entity Framework Core (Code-First Migrationen).
- **Frontend:** Angular 22 Single Page Application (Standalone Components, Signals, Nginx Reverse-Proxy).

---

## Inhaltsverzeichnis

1. [Voraussetzungen](#voraussetzungen)
2. [Schnellstart mit Docker (Empfohlen)](#schnellstart-mit-docker-empfohlen)
3. [Lokale Entwicklung ohne Docker](#lokale-entwicklung-ohne-docker)
   - [1. Datenbank starten](#1-datenbank-starten)
   - [2. Backend starten](#2-backend-starten)
   - [3. Frontend starten](#3-frontend-starten)
4. [Demo-Zugangsdaten](#demo-zugangsdaten)
5. [Architektur & Endpunkte](#architektur--endpunkte)
6. [Tests ausführen](#tests-ausführen)
7. [Zeitaufwand](#️-zeitaufwand)

---

## Voraussetzungen

Je nachdem, wie du das Projekt ausführen möchtest:

- **Variante A (Docker - empfohlen):**
  - [Docker Desktop](https://www.docker.com/products/docker-desktop/) (inkl. Docker Compose)
- **Variante B (Lokale Entwicklung):**
  - [.NET SDK 10.0](https://dotnet.microsoft.com/download)
  - [Node.js (v20 oder v22)](https://nodejs.org/) & npm
  - [Angular CLI](https://angular.dev/tools/cli): `npm install -g @angular/cli`
  - [PostgreSQL](https://www.postgresql.org/) (oder Docker nur für PostgreSQL)

---

## Schnellstart mit Docker (Empfohlen)

Die gesamte Anwendung (PostgreSQL, Backend API, Frontend & Nginx Proxy) lässt sich reproduzierbar mit einem einzigen Befehl starten.

### 1. Repository klonen & in das Root-Verzeichnis wechseln

```bash
git clone https://github.com/williamkom/pokemon-collection.git
cd pokemon-collection
```

### 2. Container bauen und im Hintergrund starten

```bash
docker compose up -d --build
```

Docker startet nun automatisch in der richtigen Reihenfolge:

1. **PostgreSQL** (Port 5432) und wartet auf den erfolgreichen Healthcheck.
2. **Backend API** (Port 8080 intern), führt beim Start automatisch alle noch ausstehenden EF Core Datenbankmigrationen durch und legt die Demo-Trainer an (`SeedDemoData: true`).
3. **Frontend Nginx** (Port 8080 extern), serviert die kompilierte Angular-App und leitet API-Aufrufe (`/api/*`) direkt an das Backend weiter.

### 3. Anwendung öffnen

Öffne im Browser:
👉 **[http://localhost:8080](http://localhost:8080)**

Zum Stoppen der Container:

```bash
docker compose down
```

_(Mit `docker compose down -v` werden zusätzlich die PostgreSQL-Volumes gelöscht.)_

---

## Lokale Entwicklung ohne Docker

Falls du aktiv am Quellcode arbeiten und Hot-Reloading nutzen möchtest:

### 1. Datenbank starten

Starte PostgreSQL lokal oder nutze Docker nur für die Datenbank:

```bash
docker compose up -d postgres
```

_Standard-Verbindungsdaten aus `backend/Pokemon.Api/appsettings.json`:_

- **Host:** `localhost`
- **Port:** `5432`
- **Database:** `pokemon_collection`
- **Username:** `pokemon_app`
- **Password:** `pokemon_dev_password`

### 2. Backend starten

Navigiere in das Backend-Projekt und starte die API:

```bash
cd backend/Pokemon.Api
dotnet run --launch-profile http
```

- Das Backend startet auf **[http://localhost:5021](http://localhost:5021)**.
- Beim Start werden EF Core Migrationen automatisch ausgeführt und die Demo-Trainer angelegt.

### 3. Frontend starten

Öffne ein zweites Terminal und starte den Angular Development Server:

```bash
cd frontend
npm install
npm start
```

- `npm start` startet `ng serve --proxy-config proxy.conf.json`.
- Alle Anfragen an `/api/*` werden automatisch an `http://localhost:5021` weitergeleitet.
- Öffne im Browser: **[http://localhost:4200](http://localhost:4200)**

---

## Demo-Zugangsdaten

Beim Start der Anwendung werden automatisch folgende Demo-Trainer angelegt:

| E-Mail                 | Passwort      |
| :--------------------- | :------------ |
| `trainer1@example.com` | `Trainer123!` |
| `trainer2@example.com` | `Trainer123!` |

---

## Architektur & Endpunkte

### Backend API-Routen

- **Authentifizierung (`/api/auth`)**
  - `POST /api/auth/login` – Login mit E-Mail und Passwort, liefert JWT-Token zurück.
  - `GET /api/auth/me` – Ruft Profildaten des angemeldeten Trainers ab (erfordert `Bearer <Token>`).
  - `POST /api/auth/logout` – Beendet die Session (erfordert `Bearer <Token>`).
- **Pokémon-Daten (`/api/pokemon`)**
  - `GET /api/pokemon?limit=20&offset=0` – Liste von Pokémon mit Details und Artworks (Live-Abfrage über PokéAPI).
- **Sammlung (`/api/collection`)**
  - `GET /api/collection` – Ruft alle gefangenen Pokémon des angemeldeten Trainers ab.
  - `POST /api/collection/{pokemonId}` – Fügt ein Pokémon der Sammlung des Trainers hinzu.

### Frontend Funktionsweise

- **`authGuard`:** Schützt Routen wie `/home`. Prüft lokal auf das Vorhandensein des Tokens und validiert dieses asynchron gegen das Backend (`/api/auth/me`).
- **`authInterceptor`:** Hängt bei allen ausgehenden `/api/*`-Requests automatisch den `Authorization: Bearer <Token>` Header an.
- **Signals:** Reaktivität und Statusverwaltung basieren auf Angular Signals.

---

## Tests ausführen

### Backend-Tests

Im Backend-Verzeichnis ausführen:

```bash
dotnet test backend/Pokemon.Tests/Pokemon.Tests.csproj
```

---

## ⏱️ Zeitaufwand

Für die Umsetzung der Aufgabe wurden insgesamt ca. **[3,30] Stunden** benötigt:

| Bereich / Phase                | Aufgaben                                                                                                                                                                     | Geschätzte Zeit     |
| :----------------------------- | :--------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | :------------------ |
| **Architektur & Planung**      | Analyse der Anforderungen, Technologieauswahl (.NET, Angular, Postgres), Entwurf des Datenbank- und Sicherheitskonzepts                                                      | ca. [15] Min.       |
| **Backend-Entwicklung**        | ASP.NET Core API Setup, Entity Framework Core Models & Migrationen, JWT-Authentifizierung & Passwort-Hashing, PokéAPI-Integration, Collection-Management                     | ca. [1,30] Std.     |
| **Frontend-Entwicklung**       | Angular Projekt-Setup (Standalone Components, Signals), Authentifizierungs-Flow (Login, `authGuard`, `authInterceptor`), Pokémon-Übersicht & Detailkarten, Sammlungs-Ansicht | ca. [30] Std.       |
| **Containerisierung & DevOps** | Dockerfiles für Backend und Frontend (Multi-Stage Builds), Nginx Reverse-Proxy-Konfiguration, `docker-compose.yml` mit Healthchecks                                          | ca. [30] Min.       |
| **Testing & Bugfixing**        | Integrationstests im Backend, Manuelles E2E-Testing im Browser ca. [30] Min.                                                                                                 |
| **Dokumentation**              | Ausführliche `README.md` mit reproduzierbaren Startanleitungen (Docker & Lokal), API- und Architekturübersicht                                                               | ca. [15] Min.       |
| **Gesamtaufwand**              |                                                                                                                                                                              | **ca. [X] Stunden** |
