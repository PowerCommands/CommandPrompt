# CommandPrompt

CommandPrompt är ett modulärt .NET-ramverk för att bygga konsolapplikationer med en färdig grundfunktionalitet. Projektet består av två delar med tydliga och separata roller.

---

## Vad är CommandPrompt egentligen?

**CommandPrompt är inte en enda app.** Det är ett ramverk plus ett verktyg för att skapa nya appar baserade på ramverket.

De två delarna är:

- **`PainKiller.CommandPrompt.CoreLib`** — den återanvändbara kärnan som alla CommandPrompt-baserade klienter bygger på
- **`PainKiller.PromptKit`** — en fungerande klient som dels används för att testa modulerna, dels används för att skapa upp nya CommandPrompt-projekt

Det tredje projektet i lösningen är **`PainKiller.ReadLine`**, ett eget readline-bibliotek med autocomplete-stöd som CoreLib använder för inmatning.

---

## PainKiller.CommandPrompt.CoreLib

CoreLib är den stabila plattformen. Den innehåller allt som varje CommandPrompt-baserad klient behöver:

| Område | Innehåll |
|---|---|
| **Kommandoloop** | `CommandLoop`, `CommandRuntime`, `CommandExecutor` — hanterar inmatning, parsning och körning |
| **Kommandodiscovery** | `CommandDiscoveryService` hittar alla kommandon via reflektion |
| **Basklasser** | `ConsoleCommandBase<TConfig>` — alla kommandon ärver från denna |
| **Konfiguration** | Typad YAML-konfiguration via `ConfigurationService`, modellerad i `ApplicationConfiguration`, `CoreConfiguration`, `LogConfiguration` |
| **Logging** | Serilog-integration konfigurerad via YAML, tillgänglig via `LoggerProvider` |
| **EventBus** | `EventBusService` — decouplar runtime från moduler via publish/subscribe |
| **Presentation** | `DialogService`, `ConsoleService`, `ConsoleWriter` — gemensamma UI-primitiver |
| **Autocomplete** | Byggs upp vid start från konfigurationens suggestions och alla kommandonamn |
| **Inbyggda kommandon** | `help`, `describe`, `cls`, `log` — alltid tillgängliga i alla klienter |

### Startup-sekvensen

Varje CommandPrompt-klient startar via en `Startup.Build()`-metod som gör följande i ordning:

1. Läser YAML-konfiguration → `ConfigurationService.Service.Get<TConfig>()`
2. Konfigurerar Serilog-logging baserat på konfigurationen
3. Visar logo om `ShowLogo` är aktiverat
4. Prenumererar på `SetupRequiredEventArgs` på eventbussen
5. Kör kommandodiscovery → `CommandDiscoveryService.DiscoverCommands(config)`
6. Anropar `OnInitialized()` på varje kommando
7. Bygger autocomplete från suggestions + kommandonamn
8. Publicerar `WorkingDirectoryChangedEventArgs`
9. Returnerar `CommandLoop` redo att köras

### Konfigurationsmodellen

YAML-filen har följande huvudstruktur:

```yaml
configuration:
  core:
    name:           # Appens namn och fönsterrubriken
    version:        # Versionstext
    prompt:         # Prompttext, t.ex. "cp>"
    defaultCommand: # Kommando som körs utan argument
    showLogo:       # true/false
    logoColor:      # Konsolfrg för logon
    suggestions:    # Extra autocomplete-förslag
    roamingDirectory: # AppData-undermapp för appen
    modules:        # Modulspecifika konfigurationsnoder
  log:
    fileName:
    filePath:
    rollingIntervall:
    restrictedToMinimumLevel:
```

Varje modul lägger sin konfiguration under `configuration.core.modules`.

---

## Modulerna

Modulerna är valbara feature-paket som finns under `CoreLib/Modules/`. De är opt-in — varje modul kopieras bara in i ett nytt projekt om användaren väljer den vid skapandet. Varje modul har en `readme.md` som beskriver den och som PromptKit visar vid modulval.

| Modul | Vad den ger |
|---|---|
| **SecurityModule** | AES-kryptering, hantering av krypterade hemligheter i konfigurationen |
| **ShellModule** | `cd`, `dir`, `file` — navigering och grundläggande filoperationer i terminalen |
| **TextModule** | Hantering av textinmatning, encode/decode av strängar, clipboard-stöd |
| **StorageModule** | Enkel objekt-persistens i fil |
| **DbStorageModule** | Dapper-baserad objekt-persistens i databas |
| **InfoPanelModule** | Dynamisk informationspanel som reserverar översta raden i konsolen |
| **OllamaModule** | Chat mot lokal AI-modell via Ollama-servern, streaminsstöd |
| **ChatGptModule** | Sökning via ChatGPT i webbläsaren (kräver ShellModule) |
| **GitModule** | Grundläggande Git-integration direkt i CommandPrompt |
| **PgpModule** | PGP-kryptografiska operationer |

---

## PainKiller.PromptKit

PromptKit fyller två syften simultaneously:

1. **Testplats** — en körbar klient där alla moduler kan testas och utvärderas under aktiv utveckling
2. **Projektgenerator** — verktyget som skapar nya CommandPrompt-baserade projekt

### Kommandon i PromptKit

| Kommando | Vad det gör |
|---|---|
| `new` | Skapar ett nytt CommandPrompt-projekt |
| `demo` | Demonstrerar funktionalitet |
| `information` | Visar information om tillgängliga moduler |
| `update` | Uppdaterar ett befintligt projekt |
| `publish` | Publicerar projektet |

### Hur `new`-kommandot fungerar

1. Användaren anger projektnamn (t.ex. `Company.MagicPrompts`)
2. Användaren anger output-katalog
3. Kommandot byter till `Modules`-katalogen
4. `InstallManager` tar vid och kör hela genereringsprocessen:
   - Identifierar tillgängliga moduler från källträdet
   - Visar en multi-select dialog för modulval
   - Kopierar CoreLib-strukturen till output
   - Kopierar bara valda moduler
   - Genererar en YAML-konfiguration med bara valda modulers sektioner

### Viktig detalj om generatorn

Generatorn arbetar mot det **levande källträdet** — inte mot paketerade templates. Det innebär att förbättringar i CoreLib och moduler direkt slår igenom i nästa `new`-körning. Det är med avsikt: CommandPrompt är tänkt att vara ett system där ramverket och generatorn utvecklas i tandem.

Den genererade lösningsstrukturen ser ut så här:

```
{outputPath}/
  {ProjectName}/
    PainKiller.CommandPrompt.CoreLib/   # Kopia av core
    {ProjectName}Client/                 # Den nya klientappen
```

---

## Arkitekturprinciper

**Kommandon ska vara tunna.** De orchestrerar, frågar användaren och delegerar. Affärslogik hör hemma i managers och tjänster.

**Ny generell funktionalitet hör hemma i CoreLib.** Om alla klienter kan tänkas behöva det — lägg det i core.

**Ny domänspecifik funktionalitet hör hemma som modul.** Om det är opt-in — skapa en ny modul med egen katalog, readme och konfigurationsnod.

**EventBus är decouplingmekanismen.** Moduler och core kommunicerar via publish/subscribe på `EventBusService`, inte via direkta beroenden.

**Konfiguration är central.** Varje feature — även logging och UX-detaljer som prompt-text och logofärg — styrs via YAML. Nya features bör följa samma mönster med en tydlig konfigurationsnod.

---

## Projektet i ett mening

> CoreLib är grunden, modulerna är valbara tillbehör, PromptKit är både testmiljön och verktyget som paketerar ihop allt till ett nytt projekt.
