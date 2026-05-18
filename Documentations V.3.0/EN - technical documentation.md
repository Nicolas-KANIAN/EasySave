# Technical Support Documentation - EasySave v3.0

This document contains the technical information necessary to understand the architecture of version 3.0 (Parallelism, Network & CLI) of our EasySave backup software.

---

## 1. General Information

* **Version**: 3.0 (Deliverable 3 - Optimization, Network and Parallel Execution)
* **Language**: C#
* **Framework**: .NET 8.0
* **UI Framework**: **Avalonia UI** (Cross-platform interface compatible with Windows, macOS, and Linux)
* **Log server**: **Docker** Container (Linux deployment)
* **Logical Architecture**: **MVVM** (Model-View-ViewModel)
* **Tools**: Visual Studio, GitHub, CodeRabbit (AI Review), xUnit (Tests).

---

## 2. Gitflow

To best organize our project, we followed a specific gitflow with a `develop` branch stemming from `main`, and `features` branches for each functionality.

<img width="700" alt="Gitflow Diagram" src="./assets/gitflow.webp" />

---

## 3. File Structure and Configuration

The graphical application and the command-line application share the same business core. The configuration files are stored in the execution directory.

**Global Configuration (`config.json`)**
The file embeds the log format, the language, the blocking business software, the encryption rules, as well as the new V3.0 metrics: priority extensions, the simultaneous transfer size limit, and the TCP coordinates of the Docker server.

**Logs and Real-Time State (`state.json`)**
Daily logs are now sent dynamically to the Docker server via a TCP socket. The state file now manages multiple jobs in parallel. For the graphical interface, progress extraction has been delegated to the `LogReaderService`. This service uses `JsonDocument` and `XDocument` to dynamically parse the data tree and accurately target the current job, thereby avoiding mixing the progress percentages of different parallel tasks.

---

## 4. Architecture and Technical Design

Version 3.0 introduces massive parallelism (`Parallel.ForEach`) and an absolute separation of responsibilities.

**Parallel Execution and Prioritization**
Files are copied in parallel depending on the available processor cores. The function first isolates files corresponding to priority extensions to execute them in a first batch, then processes the rest.

**Hardware Security (Large file locking)**
To prevent network collapse, a file exceeding the size limit claims an exclusive token (`SemaphoreSlim`). No other large file can transit until this one is finished.

**Flow Control and Single-Instance**
The architecture uses a `ManualResetEvent` to instantly freeze parallel threads upon detection of the business software, and a `CancellationToken` for emergency stops. Furthermore, the `CryptoSoft` software is now protected by a global `Mutex` guaranteeing its single-instance execution on the operating system.

**CLI Launch**
`Program.cs` bypasses the Avalonia UI when it detects arguments (e.g., `1-3;5`), injects dependencies, and autonomously launches the `BackupEngine` instances in parallel.

### UML Modeling

**1. Use Case Diagram**
Integrates the new control commands (Pause/Stop) and the CLI mode.
<img src="./assets/Usecase Diagram V.3.0.png" alt="Use Case Diagram" width="600" />

**2. Activity Diagram**
Details the parallel flow, the semaphore wait for large files, and the priority loop.
<img src="./assets/Activity Diagram V.3.0.png" alt="Activity Diagram" width="600" />

**3. Sequence Diagram**
Illustrates the asynchronous emission of logs to the TCP server (Docker) and the hardware pause.
<img src="./assets/Sequence Diagram V.3.0.png" alt="Sequence Diagram" width="600" />

**4. Class Diagram**
Displays the complete dependency injection and the new services.
<img src="./assets/Class Diagram V.3.0.svg" alt="Class Diagram" width="600" />

### Compliance with SOLID Principles

* **S - Single Responsibility**: Extraction of the XML/JSON file reading into an exclusive third-party service (`LogReaderService`).
* **O - Open/Closed**: The *Strategy Pattern* (`IBackupStrategy`) remained intact despite the integration of multithreading.
* **L - Liskov Substitution**: The engine manipulates the strategies (Full/Diff) interchangeably.
* **I - Interface Segregation**: Maintenance of strict and targeted contracts (`IBackupObserver`).
* **D - Dependency Inversion**: The `BackupEngine` no longer instantiates the file system or the encryption service. It requires them via its constructor, guaranteeing total decoupling.

---

## 5. Integrity and Tests

The **`EasySaveTest`** suite has been overhauled to validate dependency injection (DI):

* **Mock Refactoring**: Dependency injection allows testing the engine by providing it with in-memory file systems, thereby securing automated tests.
* **Fault Tolerance**: Tests ensure that a forced stop via `CancellationToken` does not cause an unexpected application crash.
* **Concurrent Access**: The "Silent Failure" writing method on `state.json` formally protects parallel threads against untimely lockouts caused by the graphical interface reading it.