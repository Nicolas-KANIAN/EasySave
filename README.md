# EasySave Project - ProSoft Suite 💾

[![Build Status](https://img.shields.io/badge/build-passing-brightgreen)](#) [![.NET](https://img.shields.io/badge/.NET-8.0-blue)](#) [![Language](https://img.shields.io/badge/Language-C%23-purple)](#)

## 🏢 1. Project Context & Introduction

Our development team has joined the software publisher **ProSoft**. Reporting directly to the CIO, we are responsible for the end-to-end management and development of the **EasySave** backup software. 

EasySave is a premium solution integrated into the ProSoft Suite pricing policy:
* **Unit Price:** €200 (excluding tax)
* **Annual Maintenance Contract:** 12% of the purchase price (includes 5/7 8am-5pm support and updates, tacitly renewed and indexed).

Our team is tasked with the full lifecycle of the software: development, major/minor release management, customer support documentation, and strict version control to minimize future development costs.

---

## 📅 2. Project Calendar & Deliverables

The project follows an accelerated development cycle divided into three major milestones:

### Deliverable 1: EasySave Version 1.0 (Current Version)
* **Day 1:** Project launch and Specifications receipt.
* **Day 3:** Work environment setup and tutor access granting.
* **D-1:** Delivery of UML architecture diagrams (ArgoUML).
* **Delivery Day:** Official release of EasySave v1.0 (Console Application) and associated documentation.

### Deliverable 2: EasySave Versions 2.0 & 1.1 (Upcoming)
* **Post-D1:** Provision of specifications for the GUI version (MVVM architecture).
* **D-1:** Delivery of updated UML diagrams.
* **Delivery Day:** Receipt of Deliverable 2.

### Deliverable 3: EasySave Version 3.0 (Final)
* **Post-D2:** Specifications for Version 3.
* **D-2:** Final UML diagrams.
* **Presentation Day:** Project defense and final delivery.

---

## 🎯 3. Deliverable 1 Specifications (v1.0)

Version 1.0 is a robust, bilingual (English/French) **Console Application** built on .NET 8.0.

### ⚙️ Core Features
* **Job Management:** Users can configure up to **5 simultaneous backup jobs**.
* **Job Definition:** Each job consists of a Name, Source Directory, Target Directory, and Backup Type.
* **Backup Types Supported:**
  * **Full Backup:** Copies all files from the source to the target.
  * **Differential Backup:** Copies only files that are new or have been modified since the last backup.
* **Execution:** Jobs can be executed individually or sequentially. Parallel execution is not supported in this version.
* **Storage Compatibility:** Supports local disks, external drives, and network drives (UNC paths).

### 📝 Tracking & Logging System
To ensure strict monitoring, logging features are isolated in a custom Dynamic Link Library (**`EasyLog.dll`**). Logs are safely stored in the application directory, strictly avoiding temporary OS folders (e.g., `C:\temp\`).

1. **Daily Log File (e.g., `YYYY-MM-DD.json`):**
   Records every file transfer in real-time. Includes Timestamp, Backup Name, Source Path, Target Path, File Size, and Transfer Time (in ms).
2. **Real-Time Status File (`state.json`):**
   A single file updated in real-time tracking the exact progression of the current job (Active/Inactive state, total files, remaining files, file currently being copied, and percentage of completion). 

*Note: All logs are formatted in JSON with proper line breaks and indentation for quick reading via text editors.*

---

## 🛠️ 4. Technical Constraints & Environment

To guarantee that the project can be taken over by other international ProSoft teams, strict management constraints are enforced:

* **Framework:** C# Language, .NET 8.0 Library.
* **IDE:** Visual Studio 2022 (or higher).
* **Version Control:** GitHub (with CI/CD checks for code quality and test validation).
* **UML Design:** ArgoUML.
* **Clean Code Standards:** * Strict English usage for all variables, methods, and comments.
  * Strict compliance with C# naming conventions (enforced via `.editorconfig`).
  * Absolute minimization of code redundancy (no copy-pasting).
  * High readability and maintainable architecture (decoupled UI and Business Logic to prepare for v2.0 MVVM).

---

## 🚀 5. User Manual (Quick Start)

### Interactive Mode
Launch the application by running the executable directly. You will be prompted to select your language (English or French) and presented with an interactive menu to Create, List, or Run backup jobs.

```bash
./EasySave.exe
