# PGE A3 FISE INFO - Software Engineering : EasySave Project 💾

🇬🇧 **[English Version]** | 🇫🇷 [Version Française](#-version-française)

[![.NET](https://img.shields.io/badge/.NET-8.0-blue)](#) [![Language](https://img.shields.io/badge/Language-C%23-purple)](#) [![CodeRabbit](https://img.shields.io/badge/CodeRabbit-AI_Review-FF6B6B)](#)

## 🏢 1. Academic & Business Context

This project is the core evaluation for the **Software Engineering (Génie Logiciel)** module. 

Our development team acts as a newly integrated software unit for the publisher **ProSoft**. Reporting to the CIO, we are tasked with the end-to-end development of **EasySave**, a professional backup management software. 
The software is integrated into the ProSoft Suite (Unit price: €200 excl. tax, with a 12% annual maintenance contract). The main business objective is to deliver a robust product while keeping future development and maintenance costs as low as possible.

## ❓ 2. Problematic & Challenges

Developing EasySave is an exercise in **software architecture and lifecycle management**. 
The project spans several weeks and requires the software to evolve drastically without breaking its core functionalities:

* **Scalability & Evolvability:** The software begins as a basic Console Application but must seamlessly transition to a Graphical User Interface (GUI) using the **MVVM pattern**.
* **Strict Code Quality:** International maintainability requires 100% English code, strict C# naming conventions, and absolute avoidance of code redundancy.
* **Traceability:** Industrial-grade real-time logging systems must be implemented to track the exact state and performance of file transfers down to the millisecond.

## 🗺️ 3. Project Roadmap & Deliverables

The project follows an iterative, accelerated development cycle divided into three major releases:

### 📍 Phase 1: The Foundation (Deliverable 1 - EasySave v1.0)
* **Goal:** Build the core business logic and backup engine.
* **Format:** C# Console Application.
* **Features:** * Creation of up to 5 sequential backup jobs (Full or Differential).
  * Implementation of the `EasyLog.dll` library for real-time state tracking and daily JSON logs.
* **Engineering:** Setup of GitHub CI/CD, Unit Tests (`xUnit`), CodeRabbit AI integration, and initial UML diagrams (ArgoUML).

### 📍 Phase 2: The Interface (Deliverable 2 - EasySave v2.0)
* **Goal:** Migrate from the console to a modern Graphical User Interface.
* **Architecture:** Strict implementation of the **MVVM (Model-View-ViewModel)** architectural pattern.
* **Features:** Visual representation of backup progress and interactive job management.

### 📍 Phase 3: The Optimization (Deliverable 3 - EasySave v3.0)
* **Goal:** Advanced performance and system optimization (Business software detection, Parallel execution, Encryption).

## ⚙️ 4. Technical Stack & CI/CD Ecosystem

To guarantee that the project can be inherited by other international ProSoft subsidiaries, our team strictly adheres to a robust DevOps pipeline:

* **Language & Framework:** C# / .NET 8.0 / Visual Studio 2022
* **Version Control:** GitHub (with branch protections and mandatory PR reviews).
* **Code Formatting:** `.editorconfig` enforced via `dotnet format` to guarantee naming conventions.
* **Unit Testing:** Automated `xUnit` test suites running on every commit.
* **🤖 AI Code Review (CodeRabbit):** To enforce ProSoft's strict constraints, we integrated **CodeRabbit** into our GitHub repository. On every Pull Request, this AI-driven tool automatically scans the code to:
  * Detect and block any French variables or comments (enforcing the English-only rule).
  * Highlight code redundancy and suggest refactoring.
  * Prevent hardcoded paths (e.g., `C:\temp\`).
  * Generate automated, professional Release Notes based on the commits.

---
---

<a id="-version-française"></a>
# 🇫🇷 Version Française

## 🏢 1. Contexte Académique et Métier

Ce projet constitue l'évaluation principale du module de **Génie Logiciel**. 

Notre équipe de développement agit en tant que nouvelle unité logicielle pour l'éditeur **ProSoft**. Sous la direction du DSI, nous sommes chargés du développement complet de **EasySave**, un logiciel de sauvegarde professionnel.
Le logiciel est intégré à la suite ProSoft (Prix de vente : 200 € HT, avec un contrat de maintenance annuel de 12 %). L'objectif métier est de livrer un produit robuste tout en minimisant les coûts de développement futurs.

## ❓ 2. Problématiques et Enjeux

Développer EasySave est un exercice poussé **d'architecture logicielle et de gestion du cycle de vie**.
Le projet s'étale sur plusieurs semaines et le logiciel doit évoluer sans casser le cœur de l'application :

* **Évolutivité :** Le logiciel commence comme une application Console mais devra migrer vers une interface graphique (GUI) en utilisant l'architecture **MVVM**.
* **Qualité de code stricte :** La maintenabilité internationale exige un code 100% en anglais, le respect strict des conventions de nommage C#, et l'absence totale de redondance (pas de copier-coller).
* **Traçabilité :** Un système de logs en temps réel doit être implémenté pour suivre l'état exact des transferts de fichiers à la milliseconde près.

## 🗺️ 3. Feuille de Route et Livrables

Le projet suit un cycle de développement itératif divisé en trois versions majeures :

### 📍 Phase 1 : Les Fondations (Livrable 1 - EasySave v1.0)
* **Objectif :** Construire la logique métier et le moteur de sauvegarde.
* **Format :** Application Console C#.
* **Fonctionnalités :** * Création de 5 travaux de sauvegarde séquentiels (Complet ou Différentiel).
  * Implémentation de la bibliothèque `EasyLog.dll` pour le suivi en temps réel et les logs journaliers JSON.
* **Ingénierie :** Mise en place de la CI/CD GitHub, Tests Unitaires (`xUnit`), intégration de CodeRabbit, et diagrammes UML (ArgoUML).

### 📍 Phase 2 : L'Interface (Livrable 2 - EasySave v2.0)
* **Objectif :** Migration vers une interface graphique moderne.
* **Architecture :** Implémentation stricte du patron de conception **MVVM (Model-View-ViewModel)**.
* **Fonctionnalités :** Représentation visuelle de la progression (barres de chargement) et gestion interactive des travaux.

### 📍 Phase 3 : L'Optimisation (Livrable 3 - EasySave v3.0)
* **Objectif :** Optimisation des performances (Détection de logiciels métiers, exécution parallèle, chiffrement).

## ⚙️ 4. Stack Technique et Écosystème CI/CD

Pour garantir que le projet puisse être repris par d'autres filiales internationales de ProSoft, notre équipe respecte un pipeline DevOps robuste :

* **Langage & Framework :** C# / .NET 8.0 / Visual Studio 2022
* **Versionning :** GitHub (avec protection des branches et revues de PR obligatoires).
* **Formatage du Code :** Règles `.editorconfig` appliquées via `dotnet format`.
* **Tests Unitaires :** Suites de tests `xUnit` automatisées.
* **🤖 Revue de Code par IA (CodeRabbit) :** Afin de respecter les contraintes strictes de ProSoft, nous avons intégré **CodeRabbit** à notre dépôt GitHub. À chaque Pull Request, cette IA analyse automatiquement le code pour :
  * Détecter et bloquer les variables ou commentaires en français (règle du 100% anglais).
  * Signaler les redondances de code et suggérer des refactorisations.
  * Interdire l'utilisation de chemins codés en dur (ex: `C:\temp\`).
  * Générer automatiquement des notes de version (Release Notes) professionnelles.
