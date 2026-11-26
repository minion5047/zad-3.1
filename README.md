# Minutnik (Zadanie 3.1)

Aplikacja desktopowa w **Avalonia UI (.NET 10)** odmierzająca czas pracy (domyślnie `25:00`) i przerwy (domyślnie `05:00`), z możliwością zmiany obu długości w oknie, jednym przyciskiem `Start` / `Pauza`, przyciskiem `Reset`, automatycznym startem przerwy po zakończeniu pracy, licznikiem skończonych cykli oraz listą historii cykli (`HistoryList`).

Cała logika okna znajduje się w jednym pliku [MainWindow.axaml.cs](MainWindow.axaml.cs) i korzysta z zegara `DispatcherTimer` bez dodatkowych bibliotek zewnętrznych.

## Uruchomienie

```bash
dotnet build
dotnet run
```

---

## Cztery liczby z lekcji

**Data sprawdzenia:** `2026-11-26`  
**Narzędzie do zliczania tokenów:** `The Tokenizer Playground` (`@xenova/transformers`, model tokenizera: `Xenova/gemma-tokenizer`, z porównaniem do `Xenova/gpt-4`)  
**Używany model agenta:** `Gemini 4 Hydrogen` (`gemini-4-hydrogen`) w Antigravity

1. **Pierwsza wiadomość do agenta po polsku** (461 znaków):
   > Zbuduj minutnik w tym projekcie Avalonia. Ma odmierzać 25 minut pracy, potem 5 minut przerwy, i liczyć skończone cykle. Obie długości mają być do zmiany w oknie. Jeden przycisk start/pauza, drugi reset. Czas widoczny jako mm:ss, licznik cykli widoczny obok. Po skończonej pracy przerwa ma ruszyć sama. Interfejs po polsku, nazwy w kodzie po angielsku, bez dodatkowych bibliotek. Cała logika wprost w pliku MainWindow.axaml.cs, bez osobnych klas pośredniczących.
   - `Xenova/gemma-tokenizer`: **141 tokenów** (142 z tokenem specjalnym `<bos>`)
   - `Xenova/gpt-4`: **163 tokeny**

2. **Ta sama wiadomość po angielsku** (495 znaków):
   > Build a timer in this Avalonia project. It should count down 25 minutes of work, then 5 minutes of break, and count completed cycles. Both durations should be editable in the window. One start/pause button, a second reset button. Time displayed as mm:ss, cycle counter visible next to it. After work finishes, the break should start automatically. Interface in Polish, names in code in English, no additional libraries. All logic directly in MainWindow.axaml.cs, without separate helper classes.
   - `Xenova/gemma-tokenizer`: **106 tokenów** (107 z tokenem specjalnym `<bos>`)
   - `Xenova/gpt-4`: **102 tokeny**

3. **Okno kontekstu modelu (`Gemini 4 Hydrogen`):**
   - **1 048 576 tokenów** (`1M tokens`)
   - Źródło: [Gemini 4 Hydrogen — Gemini API Docs](https://ai.google.dev/gemini-api/docs/models/gemini-4-hydrogen) (oraz [Gemini API Models](https://ai.google.dev/gemini-api/docs/models))

4. **Maksymalna długość odpowiedzi modelu (`Gemini 4 Hydrogen`):**
   - **65 536 tokenów** (`64K`, domyślne wywołanie synchroniczne) / do **1 048 576 tokenów** (`1M`, w trybie *long decode*)
   - Źródło: [Gemini 4 Hydrogen — Gemini API Docs](https://ai.google.dev/gemini-api/docs/models/gemini-4-hydrogen)

---

## Wynik próby wpisania `25,5` i `25.5`

Test wykonany na systemie Windows z polskimi ustawieniami regionalnymi (`CultureInfo.CurrentCulture` = `pl-PL`) przy użyciu `double.TryParse(..., NumberStyles.Float, CultureInfo.CurrentCulture, ...)`:

- **Wpisanie `25,5`** (przecinek):
  - Parsowanie kończy się sukcesem (`25.5` minuty).
  - Wyświetlacz czasu od razu przelicza wartość i pokazuje **`25:30`** (1530 sekund), komunikat błędu pozostaje ukryty, a przycisk `Start` uruchamia odliczanie od `25:30`.
- **Wpisanie `25.5`** (kropka):
  - Parsowanie kończy się niepowodzeniem, ponieważ w kulturze `pl-PL` separatorem dziesiętnym jest przecinek, a nie kropka.
  - Pod polami wejściowymi pojawia się czerwony komunikat błędu: `Podaj poprawną długość (liczba większa od 0, np. 25 lub 0,5).`
  - Wyświetlacz czasu nie zmienia się na 25 minut i 30 sekund, a kliknięcie `Start` nie uruchamia odliczania, dopóki użytkownik nie poprawi wartości.
