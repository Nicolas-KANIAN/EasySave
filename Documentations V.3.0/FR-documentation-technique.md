# Documentation Support Technique - EasySave v3.0

Ce document contient les informations techniques nécessaires pour la compréhension de l'architecture de la version 3.0 (Parallélisme, Réseau & CLI) de notre logiciel de sauvegarde EasySave.

---

## 1. Informations Générales

* **Version** : 3.0 (Livrable 3 - Optimisation, Réseau et Exécution Parallèle)
* **Langage** : C#
* **Framework** : .NET 8.0
* **UI Framework** : **Avalonia UI** (Interface cross-platform compatible Windows, macOS et Linux)
* **Serveur de logs** : Conteneur **Docker** (Déploiement Linux)
* **Architecture Logique** : **MVVM** (Model-View-ViewModel)
* **Outils** : Visual Studio, GitHub, CodeRabbit (Revue IA), xUnit (Tests).

---

## 2. Gitflow

Afin d'organiser au mieux notre projet, nous avons suivi un gitflow précis avec une branche `develop` issue de `main`, et des branches `features` pour chaque fonctionnalité.

<img width="700" alt="Schéma du Gitflow" src="./assets/gitflow.webp" />

---

## 3. Structure des Fichiers et Configuration

L'application graphique et l'application en ligne de commande partagent le même noyau métier. Les fichiers de configuration sont stockés dans le répertoire d'exécution.

**Configuration Globale (`config.json`)**
Le fichier embarque le format des logs, la langue, le logiciel métier bloquant, les règles de chiffrement, ainsi que les nouvelles métriques V3.0 : les extensions prioritaires, la taille limite de transfert simultané, et les coordonnées TCP du serveur Docker.

**Journaux et État Temps Réel (`state.json`)**
Les logs journaliers sont désormais expédiés dynamiquement vers le serveur Docker via un socket TCP. Le fichier d'état gère désormais plusieurs travaux en parallèle. Pour l'interface graphique, l'extraction de la progression a été déléguée au `LogReaderService`. Ce service exploite `JsonDocument` et `XDocument` pour parcourir dynamiquement l'arbre de données et cibler avec précision le travail en cours, évitant ainsi de mélanger les pourcentages d'avancement des différentes tâches parallèles.

---

## 4. Architecture et Conception Technique

La version 3.0 introduit le parallélisme massif (`Parallel.ForEach`) et une séparation absolue des responsabilités.

**Exécution Parallèle et Priorisation**
Les fichiers sont copiés en parallèle selon les cœurs disponibles du processeur. La fonction isole d'abord les fichiers correspondant aux extensions prioritaires pour les exécuter dans un premier lot, puis traite le reste.

**Sécurité Matérielle (Verrouillage des gros fichiers)**
Pour éviter l'effondrement du réseau, un fichier dépassant la limite de taille réclame un jeton exclusif (`SemaphoreSlim`). Aucun autre gros fichier ne peut transiter tant que celui-ci n'est pas terminé.

**Contrôle du Flux et Single-Instance**
L'architecture utilise un `ManualResetEvent` pour geler instantanément les threads parallèles en cas de détection du logiciel métier, et un `CancellationToken` pour l'arrêt d'urgence. De plus, le logiciel `CryptoSoft` est désormais protégé par un `Mutex` global garantissant son unicité d'exécution sur le système d'exploitation.

**Lancement CLI**
Le `Program.cs` court-circuite l'UI Avalonia lorsqu'il détecte des arguments (ex: `1-3;5`), injecte les dépendances, et lance les `BackupEngine` en parallèle de manière autonome.

### Modélisation UML

**1. Diagramme de Cas d'Utilisation (Use Case)**
Intègre les nouvelles commandes de contrôle (Pause/Stop) et le mode CLI.
<img src="./assets/Usecase Diagram V.3.0.png" alt="Diagramme de Cas d'Utilisation" width="600" />

**2. Diagramme d'Activité**
Détaille le flux parallèle, l'attente du sémaphore pour les gros fichiers, et la boucle prioritaire.
<img src="./assets/Activity Diagram V.3.0.png" alt="Diagramme d'Activité" width="600" />

**3. Diagramme de Séquence**
Illustre l'émission asynchrone des logs vers le serveur TCP (Docker) et la mise en pause matérielle.
<img src="./assets/Sequence Diagram V.3.0.png" alt="Diagramme de Séquence" width="600" />

**4. Diagramme de Classes**
Affiche l'injection complète des dépendances et les nouveaux services.
<img src="./assets/Class Diagram V.3.0.svg" alt="Diagramme de Classes" width="600" />

### Respect des Principes SOLID

* **S - Responsabilité Unique** : Extraction de la lecture des fichiers XML/JSON dans un service tiers exclusif (`LogReaderService`).
* **O - Ouvert/Fermé** : Le *Strategy Pattern* (`IBackupStrategy`) est resté intact malgré l'intégration du multithreading.
* **L - Substitution de Liskov** : Le moteur manipule les stratégies (Full/Diff) indifféremment.
* **I - Ségrégation des Interfaces** : Maintien de contrats stricts et ciblés (`IBackupObserver`).
* **D - Inversion des Dépendances** : Le `BackupEngine` n'instancie plus le système de fichier ni le service de chiffrement. Il les exige via son constructeur, garantissant un découplage total.

---

## 5. Intégrité et Tests

La suite **`EasySaveTest`** a été refondue pour valider l'injection de dépendances (DI) :

* **Refactoring des Mocks** : L'injection de dépendances permet de tester le moteur en lui fournissant des systèmes de fichiers en mémoire vive, sécurisant ainsi les tests automatisés.
* **Tolérance aux pannes** : Les tests garantissent que l'arrêt forcé via `CancellationToken` n'engendre pas de crash inopiné de l'application.
* **Accès concurrents** : La méthode d'écriture "Silent Failure" sur le `state.json` protège formellement les threads parallèles contre les verrouillages intempestifs causés par la lecture de l'interface graphique.