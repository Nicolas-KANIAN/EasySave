# Documentation Support Technique - EasySave v1.0

Ce document contient les informations techniques nécessaires pour l'installation, la compréhension de l'architecture et le dépannage de la version 1.0 (Console) de notre logiciel de sauvegarde EasySave.

---

## 1. Informations Générales

- **Version** : 1.0 (Livrable 1 - Application Console)
- **Langage** : C#
- **Framework** : .NET 8.0
- **Système d'exploitation** : Windows (compatible cross-platform)

**Outils et Méthodes :**
- **IDE & Versionning** : Visual Studio / GitHub
- **Revue de code par IA** : **CodeRabbit** intégré au dépôt GitHub. À chaque Pull Request, l'IA vérifie le respect des standards (100 % anglais, absence de chemins codés en dur), détecte les redondances et propose des refactorisations. *(Source : [Configuration CodeRabbit](https://docs.coderabbit.ai/reference/configuration))*
- **Tests unitaires** : **xUnit** *(Source : [Documentation xUnit](https://xunit.net/?tabs=cs))*
- **Formatage du code** : `.editorconfig`

---

## 2. Gitflow

Afin d'organiser au mieux notre projet, nous avons décidé de suivre un gitflow précis. Nous avons créé une branche `develop` à partir de `main`, et des branches `features` pour chaque fonctionnalité du projet à partir de la branche `develop`.

<img width="700" alt="Schéma du Gitflow" src="./Documentations/assets/gitflow.webp" />

---

## 3. Structure des Fichiers et Configuration

L'application est portable. Tous les fichiers de configuration et de logs sont stockés dans le dossier où est présent l'exécutable `EasySave.exe`.

### Configuration Générale (`config.json`)
- **Emplacement** : `<Dossier_Application>/EasyLogs/config.json`
- **Paramètres globaux** :
  - `LogFormat` : Format d'écriture des fichiers de logs (`Json` ou `Xml`).
  - `Language` : Langue de l'interface console (`en` ou `fr`).

### Configuration des Travaux (`jobs.json`)
- **Emplacement** : `<Dossier_Application>/EasyLogs/jobs.json`
- **Format** : Tableau JSON d'objets `BackupJob` (Name, Source, Target, Type).
- **Règle Métier** : Limité techniquement à 5 travaux maximum. Si ce fichier est supprimé, la liste sera vide au prochain lancement.

### Journaux d'Activité (`DailyLog_{date}`)
- **Emplacement** : `<Dossier_Application>/EasyLogs/DailyLog_{dd_MM_yyyy}.json` (ou `.xml`)
- **Format** : Un fichier par jour généré par la DLL `EasyLog`.
- **Contenu** : Détail de chaque transfert (`Timestamp`, `Name`, `Source`, `Target`, `FileSize`, `TransferTimeMs`).

### Fichier d'État temps réel (`state.json`)
- **Emplacement** : `<Dossier_Application>/EasyLogs/state.json` (ou `.xml`)
- **Contenu** : État d'avancement du travail en cours (`Progression`, `TotalFilesToCopy`, `NbFilesLeftToDo`, `RemainingFilesSize`, `State`).
- **Comportement** : Réécrit dynamiquement à chaque transfert de fichier.

---

## 4. Architecture et Conception Technique

### Structuration des Dossiers (Architecture Console)
L'application est divisée en espaces de noms (namespaces) garantissant un couplage faible :

* **`EasySave.Models`** : Structures de données (`BackupJob`, `BackupType`).
* **`EasySave.Services`** : Logique métier et orchestration (`JobManager`, `BackupEngine`).
* **`EasySave.Patterns.Strategy`** : Algorithmes de sauvegarde (`FullBackupStrategy`, `DifferentialBackupStrategy`).
* **`EasySave.Patterns.Factory`** : Usines de création logicielle (`BackupFactory`).
* **`EasySave.Patterns.Bridge`** : Abstraction du système de fichiers OS (`IFileSystem`, `LocalFileSystem`).
* **`EasySave.Patterns.Observer`** : Écoute du suivi temps réel (`IBackupObserver`, `StateLoggerObserver`).
* **`EasyLog` (DLL)** : Bibliothèque externe de journalisation (Gère les fichiers de logs journaliers et l'état en direct).

### Modélisation UML

Afin de documenter la conception technique et fonctionnelle de l'application, nous avons modélisé le système à travers les quatre vues fondamentales UML :

**1. Diagramme de Cas d'Utilisation (Use Case)**  
Définit les interactions possibles entre l'utilisateur et le système (Création, Lancement, Suppression de travaux).

![Diagramme de Cas d'Utilisation](./Documentations/assets/Usecase%20Diagram%20V.1.0.png)

**2. Diagramme d'Activité**  
Détaille le flux d'exécution logique du moteur lorsqu'une sauvegarde est lancée.

![Diagramme d'Activité](./Documentations/assets/Activity%20Diagram%20V.1.0.png)

**3. Diagramme de Séquence**  
Illustre les appels chronologiques entre les différents objets lors du cycle de vie d'une sauvegarde.

![Diagramme de Séquence](./Documentations/assets/Sequence%20Diagram%20V.1.0.png)

**4. Diagramme de Classes**  
Représente l'architecture statique détaillée de l'application et de ses Design Patterns.

![Diagramme de Classes](./Documentations/assets/Class%20Diagram%20V.1.0.png)


### Implémentation des Design Patterns

| Pattern | Problème technique résolu | Solution apportée dans l'architecture |
| :--- | :--- | :--- |
| **Strategy** | Éviter de polluer le moteur de sauvegarde avec des algorithmes mathématiques complexes. | Encapsulation de chaque algorithme (`Full`, `Differential`) dans sa propre classe via l'interface `IBackupStrategy`. |
| **Observer** | Mettre à jour `state.json` en temps réel sans rendre le moteur dépendant du système de logs. | Le moteur "diffuse" son état (`NotifyObservers`). `StateLoggerObserver` écoute et écrit sans impacter le moteur. |
| **Singleton** | Éviter les crashs d'accès concurrents (File Lock) lors de l'écriture des fichiers de logs. | La classe `Logger` garantit une instance unique sécurisée par un mécanisme de verrouillage (`lock`). |
| **Factory** | Simplifier l'instanciation des stratégies sans multiplier les `if/switch` dans le moteur. | `BackupFactory` génère et retourne dynamiquement le bon algorithme en fonction du type demandé par l'utilisateur. |
| **Bridge** | Permettre les tests unitaires sans créer de vrais fichiers physiques sur le disque dur. | Création d'une interface `IFileSystem` agissant comme un pont, permettant d'injecter des "Mocks" en phase de test. |

### Respect des Principes SOLID

L'architecture de l'application a été pensée pour garantir un code robuste, testable unitairement et prêt à évoluer vers des interfaces graphiques (pour le Livrable 2) sans réécriture du moteur central :

- **S - Responsabilité Unique** : Chaque fichier a un rôle strict. `JobManager` gère la liste, `BackupEngine` orchestre la copie, `Logger` gère les écritures.
- **O - Ouvert/Fermé** : Si nous devons ajouter une nouvelle sauvegarde (ex: Incrémentale), nous n'avons pas à modifier `BackupEngine`, mais simplement à créer une nouvelle classe implémentant `IBackupStrategy`.
- **L - Substitution de Liskov** : Le `BackupEngine` s'attend à recevoir une `IBackupStrategy`. Qu'il s'agisse d'une `FullBackupStrategy` ou d'une `DifferentialBackupStrategy`, le moteur l'utilise exactement de la même manière sans anomalie.
- **I - Ségrégation des Interfaces** : Nos interfaces sont petites et ciblées. `IBackupObserver` ne contient que la méthode `Update()`, `IBackupStrategy` ne contient que `GetFilesToCopy()`.
- **D - Inversion des Dépendances** : `BackupEngine` (haut niveau) ne dépend plus directement de `System.IO.File` (bas niveau), mais de l'abstraction `IFileSystem`.

---

## 5. Intégrité et Tests (Assurance Qualité)

Nous avons accordé une grande importance à la qualité du code. La solution **`EasySaveTest`** (xUnit) couvre les aspects critiques du moteur v1.0 :

- **Tests des Modèles (`BackupJobTests`)** : Vérifie que l'instanciation d'un travail affecte correctement le nom, les répertoires et le type en mémoire.
- **Tests de l'Architecture (`BackupFactoryTests`)** : S'assure que la fabrique logicielle retourne le bon objet métier (`Full` ou `Differential`) selon le paramètre fourni.
- **Tests de Concurrence (`LoggerTests`)** : Prouve que le pattern Singleton retourne strictement la même instance mémoire à chaque appel pour éviter tout conflit d'écriture.
- **Tests des Règles Métiers (`JobManagerTests`)** :
  - Validation de l'ajout/suppression de travaux via index.
  - Création de six travaux consécutifs pour prouver le blocage technique au-delà de la **limite stricte de cinq travaux**.
  - Maintien de la stabilité (anti-crash) lors d'une tentative de suppression avec un index hors limites.
- **Tests de Résilience (`BackupEngineTests`)** : Vérifie que le moteur s'arrête proprement et de manière sécurisée (sans erreur fatale) si le répertoire source fourni est physiquement introuvable.

L'objectif de cette couverture est de valider les fondations techniques (v1.0) avant d'aborder la migration vers une interface graphique.