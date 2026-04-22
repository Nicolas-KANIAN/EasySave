# PGE A3 FISE INFO - Software Engineering : EasySave Project

🇬🇧 **[English Version]** | 🇫🇷 [Version Française](#-version-française)

[![.NET](https://img.shields.io/badge/.NET-8.0-blue)](#) [![Language](https://img.shields.io/badge/Language-C%23-purple)](#) [![CodeRabbit](https://img.shields.io/badge/CodeRabbit-AI_Review-FF6B6B)](#)

## 1. Academic & Business Context

This project is the core evaluation for the **Software Engineering** module. 

Our development team acts as a newly integrated software unit for the publisher **ProSoft**. Reporting to the CIO, we are tasked with the end-to-end development of **EasySave**, a professional backup management software. 
The software is integrated into the ProSoft Suite (Unit price: €200 excl. tax, with a 12% annual maintenance contract). The main business objective is to deliver a robust product while keeping future development and maintenance costs as low as possible.

## 2. Problematic & Challenges

Developing EasySave is an exercise in **software architecture and lifecycle management**. 
The project spans several weeks and requires the software to evolve drastically without breaking its core functionalities:

* **Scalability & Evolvability:** The software begins as a basic Console Application but must seamlessly transition to a Graphical User Interface (GUI) using the **MVVM pattern**.
* **Strict Code Quality:** International maintainability requires 100% English code, strict C# naming conventions, and absolute avoidance of code redundancy.
* **Traceability:** Industrial-grade real-time logging systems must be implemented to track the exact state and performance of file transfers down to the millisecond.

## 3. Project Roadmap & Deliverables

The project follows an iterative, accelerated development cycle divided into three major releases:

### Phase 1: The Foundation (Deliverable 1 - EasySave v1.0)
* **Goal:** Build the core business logic and backup engine.
* **Format:** C# Console Application.
* **Features:** Creation of up to 5 sequential backup jobs (Full or Differential).
  * Implementation of the `EasyLog.dll` library for real-time state tracking and daily JSON logs.
* **Engineering:** Setup of GitHub CI/CD, Unit Tests (`xUnit`), CodeRabbit AI integration, and initial UML diagrams.

### Phase 2: The Interface (Deliverable 2 - EasySave v2.0)
* **Goal:** Migrate from the console to a modern Graphical User Interface.
* **Architecture:** Strict implementation of the **MVVM (Model-View-ViewModel)** architectural pattern.
* **Features:** Visual representation of backup progress and interactive job management.

### Phase 3: The Optimization (Deliverable 3 - EasySave v3.0)
* **Goal:** Advanced performance and system optimization (Business software detection, Parallel execution, Encryption).

## 4. Technical Stack & CI/CD Ecosystem

To guarantee that the project can be inherited by other international ProSoft subsidiaries, our team strictly adheres to a robust DevOps pipeline:

* **Language & Framework:** C# / .NET 8.0 / Visual Studio
* **Version Control:** GitHub (with branch protections and mandatory PR reviews).
* **Code Formatting:** `.editorconfig` enforced via `dotnet format` to guarantee naming conventions.
* **Unit Testing:** Automated `xUnit` test suites running on every commit.
* **AI Code Review (CodeRabbit):** To enforce ProSoft's strict constraints, we integrated **CodeRabbit** into our GitHub repository. On every Pull Request, this AI-driven tool automatically scans the code to:
  * Detect and block any French variables or comments (enforcing the English-only rule).
  * Highlight code redundancy and suggest refactoring.
  * Prevent hardcoded paths (e.g., `C:\temp\`).
  * Generate automated, professional Release Notes based on the commits.

## 5. Getting Started
### Prerequisites
* [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* Visual Studio 2022 (or Rider / VS Code)

### How to run
1. Clone the repository: `git clone https://github.com/your-repo/EasySave.git`
2. Navigate to the project folder: `cd EasySave`
3. Run the application: `dotnet run --project EasySave`

---

## 🇫🇷 Version Française

## 1. Contexte Académique & Métier

Ce projet constitue l'évaluation principale du module d'**Ingénierie Logicielle**.

Notre équipe de développement agit en tant que nouvelle unité logicielle intégrée pour l'éditeur **ProSoft**. Sous la direction du DSI, nous sommes chargés du développement de A à Z d'**EasySave**, un logiciel de gestion de sauvegarde professionnel.
Le logiciel est intégré à la suite ProSoft (Prix unitaire : 200 € HT, avec un contrat de maintenance annuel de 12%). L'objectif métier principal est de livrer un produit robuste tout en maintenant les coûts de développement et de maintenance futurs aussi bas que possible.

## 2. Problématique & Défis

Le développement d'EasySave est un exercice d'**architecture logicielle et de gestion du cycle de vie**.
Le projet s'étale sur plusieurs semaines et nécessite que le logiciel évolue considérablement sans casser ses fonctionnalités de base :

* **Scalabilité & Évolutivité :** Le logiciel commence comme une application Console basique mais doit migrer de manière transparente vers une Interface Graphique (GUI) en utilisant le pattern **MVVM**.
* **Qualité de Code Stricte :** La maintenabilité à l'international exige un code 100% en anglais, des conventions de nommage C# strictes et l'évitement absolu de toute redondance de code.
* **Traçabilité :** Des systèmes de journalisation en temps réel de niveau industriel doivent être mis en place pour suivre l'état exact et les performances des transferts de fichiers à la milliseconde près.

## 3. Feuille de Route & Livrables

Le projet suit un cycle de développement itératif et accéléré divisé en trois versions majeures :

### Phase 1 : Les Fondations (Livrable 1 - EasySave v1.0)
* **Objectif :** Construire la logique métier de base et le moteur de sauvegarde.
* **Format :** Application Console C#.
* **Fonctionnalités :** Création de jusqu'à 5 travaux de sauvegarde séquentiels (Complets ou Différentiels).
  * Implémentation de la bibliothèque `EasyLog.dll` pour le suivi de l'état en temps réel et les journaux JSON quotidiens.
* **Ingénierie :** Mise en place de la CI/CD GitHub, Tests Unitaires (`xUnit`), intégration de l'IA CodeRabbit et premiers diagrammes UML.

### Phase 2 : L'Interface (Livrable 2 - EasySave v2.0)
* **Objectif :** Migrer de la console vers une Interface Graphique moderne.
* **Architecture :** Implémentation stricte du pattern architectural **MVVM (Modèle-Vue-VueModèle)**.
* **Fonctionnalités :** Représentation visuelle de la progression des sauvegardes et gestion interactive des travaux.

### Phase 3 : L'Optimisation (Livrable 3 - EasySave v3.0)
* **Objectif :** Optimisation avancée des performances et du système (Détection des logiciels métiers, Exécution parallèle, Chiffrement).

## 4. Stack Technique & Écosystème CI/CD

Pour garantir que le projet puisse être repris par d'autres filiales internationales de ProSoft, notre équipe adhère strictement à un pipeline DevOps robuste :

* **Langage & Framework :** C# / .NET 8.0 / Visual Studio
* **Contrôle de Version :** GitHub (avec protection des branches et revues de PR obligatoires).
* **Formatage du Code :** `.editorconfig` appliqué via `dotnet format` pour garantir le respect des conventions de nommage.
* **Tests Unitaires :** Suites de tests automatisées `xUnit` exécutées à chaque commit.
* **Revue de Code par l'IA (CodeRabbit) :** Pour faire respecter les contraintes strictes de ProSoft, nous avons intégré **CodeRabbit** à notre dépôt GitHub. À chaque Pull Request, cet outil basé sur l'IA analyse automatiquement le code pour :
  * Détecter et bloquer toute variable ou commentaire en français (pour faire respecter la règle du "tout-anglais").
  * Mettre en évidence la redondance du code et suggérer des refactorisations.
  * Empêcher les chemins codés en dur (ex. : `C:\temp\`).
  * Générer des notes de version automatisées et professionnelles basées sur les commits.

## 5. Pour Commencer
### Prérequis
* [SDK .NET 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
* Visual Studio 2022 (ou Rider / VS Code)

### Comment lancer l'application
1. Cloner le dépôt : `git clone https://github.com/your-repo/EasySave.git`
2. Naviguer vers le dossier du projet : `cd EasySave`
3. Lancer l'application : `dotnet run --project EasySave`