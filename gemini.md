# Project Specification: ZenithTime

An enterprise-compliant Windows 11 desktop application providing a top-docked digital clock and an integrated Pomodoro timer.

---

## 1. Security & Enterprise Constraints

The application will be executed on a corporate-managed workstation and must adhere to strict enterprise security standards:

* Zero External Dependencies: Built using native .NET (WPF / C#) with zero third-party NuGet packages. Rely exclusively on core System.* namespaces.
* No Dynamic Executions: No runtime downloading or execution of external scripts or binaries.
* Built-in Assets Only: Use system fonts (Segoe UI, Segoe UI Emoji) to avoid bundling external asset files.
* Permissions: Must run entirely within user space without administrative or elevated privileges.
* Network Isolation: The application is entirely local. No network calls, telemetry, or data transmission.

---

## 2. Window Behavior & Dynamic Layout

* Form Factor: A custom-shaped tab/capsule that "hangs" down from the absolute top-center of the primary Windows screen.
* Window Configuration: Borderless (WindowStyle=None), transparent background (AllowsTransparency=True), and always on top (Topmost=True).
* Dynamic Position: Subscribes to Microsoft.Win32.SystemEvents.DisplaySettingsChanged. Re-centers along the X-axis (Top = 0) upon resolution changes or multi-monitor events.

---

## 3. UI/UX & Visual Design (ASCII Art)

The window geometry uses a custom PathGeometry in XAML to create organic, outward-curving corners at the top edge and inward-curving corners at the bottom.

### 3.1 Compact Mode (Default State)
Displays the time in HH:mm. Clicking directly on the clock text toggles a persistent seconds mode (HH:mm:ss).

======================= SCREEN TOP EDGE (CENTERED) =======================
                  ___                             ___
                     \                           /
                      |          14:35          |   <-- Click to toggle seconds
                      \_________________________/

### 3.2 Expanded Mode (Hover / Active State)
Expands vertically downwards when the mouse hovers over the tab.

#### State A: Pomodoro Idle (Ready)
======================= SCREEN TOP EDGE (CENTERED) =======================
                  ___                             ___
                     \                           /
                      |          14:35          |
                      |                         |
                      |       [ ▶ Start ]       |   <-- Play icon (Green #22C55E)
                      \_________________________/

#### State B: Pomodoro Active (Counting Down)
======================= SCREEN TOP EDGE (CENTERED) =======================
                  ___                             ___
                     \                           /
                      |          14:42          |
                      |                         |
                      |    [ ⏸ 24:59 ]  [ ■ ]  |   <-- Pause icon (Orange/Red #EF4444)
                      \_________________________/       Stop button appears dynamically

---

## 4. Component Logic & Completion Signals

### 4.1 Clock Module
- Driven by a high-precision native timer (System.Windows.Threading.DispatcherTimer) ticking every 100-500ms.
- Evaluates a boolean flag _showSeconds.
- Updates the text block binding reactively based on the active format profile.

### 4.2 Pomodoro State Machine
To guarantee precision on enterprise machines without background thread drift, calculate a static target expiration time timestamp when starting/resuming, rather than naively decrementing a counter:
Target Time = DateTime.Now + Remaining Duration
Calculates a static target expiration timestamp when starting/resuming to avoid thread drift: Target Time = DateTime.Now + Remaining Duration.
The state cycle transitions as follows:
1. Idle: Displays ▶ Start (Green). Clicking transitions to Work.
2. Work (25 mins): Displays ⏸ MM:ss (Orange/Red). Clicking transitions to Paused. Reaching 00:00 triggers Break.
3. Break (5 mins): Displays ⏸ MM:ss (Blue). Reaching 00:00 resets back to Idle (or auto-cycles).
4. Paused: Displays ▶ MM:ss (Green, blinking softly). Clicking resumes the countdown toward a newly calculated target time.
5. Stop Action (■): Resets the state machine immediately back to Idle and hides the stop control.


### 4.3 Completion Signals (When Timer Hits 00:00)
1. Visual Pulse: Trigger a XAML Storyboard animation causing the background Path to softly pulse/glow (3-4 times) in Orange/Red (End of Work) or Blue (End of Break) to catch the user's eye in the peripheral vision.
2. Audio Cue: Play a discrete, native Windows notification sound using System.Media.SystemSounds.Asterisk.Play() to avoid external asset dependencies.

### 5. File Structure Guidance for Gemini CLI
When implementing, generate the project across exactly three core native files:
1. ZenithTime.csproj: Core SDK-style project file targeting .NET 10.0-windows with WinExe output types and UseWPF set to true. No NuGet packages.
2. MainWindow.xaml: Declarative UI layout containing the custom Vector Path geometry, Triggers for hover expansions, and bindings for the state-driven button layout.
3. MainWindow.xaml.cs: Code-behind (namespace ZenithTime) containing the event handlers for display resolution shifts, native mouse interactions, clock routines, and the atomic Pomodoro timer engine.

---

## 6. Development Roadmap & Checklist

- [x] 1. Setup Project Environment
  - [x] Create ZenithTime.csproj targeting .NET 10.0-windows with UseWPF set to true.
  - [x] Confirm project builds with zero NuGet dependencies.

- [x] 2. Implement Main Window Shell & Custom Geometry
  - [x] Configure Window properties (Topmost, AllowsTransparency, WindowStyle=None).
  - [x] Write XAML PathGeometry for the outward-curving top wings and rounded bottom.
  - [x] Implement Window_MouseEnter and Window_MouseLeave Storyboards for smooth vertical slide expansion.

- [x] 3. Dynamic Display & Resolution Centering
  - [x] Implement RecenterWindow calculation using SystemParameters.PrimaryScreenWidth.
  - [x] Wire up SystemEvents.DisplaySettingsChanged to fire RecenterWindow automatically.

- [x] 4. Core Clock Module
  - [x] Set up DispatcherTimer for the digital clock display.
  - [x] Implement click handler on TxtClock to toggle between HH:mm and HH:mm:ss formats.

- [ ] 5. Pomodoro Engine & Smart UI
  - [ ] Build the state machine logic (Idle, Work, Break, Paused).
  - [ ] Connect the main button to toggle dynamically between state states, text values, and colors.
  - [ ] Implement the contextual Stop button logic.

- [ ] 6. Alert Notifications
  - [ ] Create the XAML flashing/pulsing background animation.
  - [ ] Integrate SystemSounds alert on phase completion.

Implementerar ett steg i taget. När ett steg är klart markera i listan med [x] att uppgiften är klar. Stoppa och informaera om att punkten är klar. 
På det sättet kan git brancher skapas för varje punkt.