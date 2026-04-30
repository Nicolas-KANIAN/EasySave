# Documentation Support Technique - EasySave v2.0

Ce document contient les informations techniques nécessaires pour l'installation, la compréhension de l'architecture de la version 2.0 (Interface Graphique) de notre logiciel de sauvegarde EasySave.

---

## 1. Informations Générales

- **Version** : 2.0 (Livrable 2 - Application Graphique)
- **Langage** : C#
- **Framework** : .NET 8.0
- **UI Framework** : **Avalonia UI** (Interface cross-platform compatible Windows, macOS et Linux)
- **Architecture Logique** : **MVVM** (Model-View-ViewModel)

**Outils et Méthodes :**
- **IDE & Versionning** : Visual Studio / GitHub
- **Revue de code par IA** : **CodeRabbit** intégré au dépôt GitHub. À chaque Pull Request, l'IA génère automatiquement un résumé des changements et vérifie le respect des bonnes pratiques de code *(Source : [Configuration CodeRabbit](https://docs.coderabbit.ai/reference/configuration))*
- **Tests unitaires** : **xUnit** *(Source : [Documentation xUnit](https://xunit.net/?tabs=cs))*
- **Formatage du code** : `.editorconfig`

---

## 2. Gitflow

Afin d'organiser au mieux notre projet, nous avons décidé de suivre un gitflow précis. Nous avons créé une branche `develop` à partir de `main`, et des branches `features` pour chaque fonctionnalité du projet à partir de la branche `develop`.

<img width="700" alt="Schéma du Gitflow" src="./assets/gitflow.webp" />

---

## 3. Structure des Fichiers et Configuration

L'application est portable. Tous les fichiers de configuration et de logs sont stockés dans le dossier où est présent l'exécutable `EasySaveApp.exe`.

### Configuration Générale (`config.json`)
- **Emplacement** : `<Dossier_Application>/config.json`
- **Paramètres globaux (Évolution V2.0)** :
  - `LogFormat` : Format d'écriture des fichiers de logs (`.Json` ou `.Xml`).
  - `Language` : Langue de l'interface (`en` ou `fr`).
  - **`BusinessSoftware`** : Nom de l'exécutable métier (ex: `calculator.exe`) interdisant l'exécution des sauvegardes.
  - **`ExtensionsToEncrypt`** : Liste des extensions de fichiers (ex: `.txt; .pdf`) nécessitant un chiffrement via le logiciel tiers.
  - **`CryptoKey`** : Clé de chiffrement utilisée par CryptoSoft.

### Configuration des Travaux (`jobs.json`)
- **Emplacement** : `<Dossier_Application>/jobs.json`
- **Format** : Tableau JSON d'objets `BackupJob` (Name, Source, Target, Type).
- **Règle Métier (Évolution V2.0)** : La limite technique historique de 5 travaux a été **levée**. L'utilisateur peut désormais créer et gérer un nombre illimité de travaux.

### Journaux d'Activité (`DailyLog_{date}`)
- **Emplacement** : `<Dossier_Application>/EasyLogs/DailyLog_{dd_MM_yyyy}.json`
- **Format** : Un fichier par jour généré par la DLL `EasyLog` en JSON ou XML.
- **Contenu (Évolution V2.0)** : Détail de chaque transfert (`Timestamp`, `Name`, `Source`, `Target`, `FileSize`, `TransferTimeMs`, et désormais **`EncryptionTimeMs`**).

### Fichier d'État temps réel (`state.json`)
- **Emplacement** : `<Dossier_Application>/EasyLogs/state.json`
- **Contenu** : État d'avancement du travail en cours (`Progression`, `TotalFilesToCopy`, `NbFilesLeftToDo`, `RemainingFilesSize`, `State`).
- **Comportement** : Réécrit dynamiquement à chaque transfert de fichier.

---

## 4. Architecture et Conception Technique

La version 2.0 marque la transition d'une architecture séquentielle (Console) vers une architecture événementielle et graphique, structurée autour du pattern MVVM.

### Structuration MVVM et Dossiers
L'application est divisée pour garantir une séparation stricte entre l'interface visuelle et la logique métier :

* **`Models`** : Structures de données (`BackupJob`). Ils intègrent désormais `ObservableObject` pour notifier l'interface en cas de modification (ex: mise à jour de la barre de progression).
* **`Views`** : Fichiers `.axaml` gérant uniquement l'affichage (Avalonia UI).
* **`ViewModels`** : Chefs d'orchestre (ex: `MainWindowViewModel`) liant les vues aux services via le *DataBinding* et les commandes (`ICommand`).
* **`Services`** : Logique métier de base (`JobManager`, `BackupEngine`, `EncryptionService`).
* **`Patterns`** : `Strategy`, `Factory`, `Bridge`, `Observer`, `Singleton` (Gèrent l'architecture logicielle sous-jacente).

### Nouvelles Fonctionnalités Métiers (v2.0)

**1. Interruption via Logiciel Métier**
Avant de démarrer et *pendant* la boucle de copie de chaque fichier, le `BackupEngine` vérifie si le processus défini dans `BusinessSoftware` est en cours d'exécution. Si c'est le cas, la sauvegarde s'interrompt immédiatement (état `INTERRUPTED`) et trace l'anomalie dans les logs.

**2. Chiffrement à la volée (CryptoSoft)**
Lorsqu'un fichier est copié, le moteur compare son extension avec la liste `ExtensionsToEncrypt`. En cas de correspondance, il fait appel à l'`EncryptionService` pour chiffrer le fichier cible en mesurant le temps d'exécution (`EncryptionTime`), qui est ensuite inscrit dans les logs journaliers.

### Modélisation UML

Afin de documenter la conception technique et fonctionnelle de l'application, nous avons modélisé le système à travers les vues fondamentales UML :

**1. Diagramme de Cas d'Utilisation (Use Case)**  
Définit les interactions possibles entre l'utilisateur et le système (Création, Modification, Lancement, Suppression de travaux).
<img src="./assets/Usecase Diagram V.2.0.png" alt="Diagramme de Cas d'Utilisation" width="600" />

**2. Diagramme d'Activité**  
Détaille le flux d'exécution logique du moteur, incluant désormais les conditions de vérification du logiciel métier et du chiffrement.
![Diagramme d'Activité](<./assets/Activity Diagram V.2.0.png>)

**3. Diagramme de Séquence**  
Illustre les appels chronologiques entre l'UI (MVVM), le Moteur, et les services annexes (Chiffrement, Logs) lors d'une sauvegarde.
![Diagramme de Séquence](<./assets/Sequence Diagram V.2.0.png>)

**4. Diagramme de Classes**  
Représente l'architecture statique détaillée de l'application et de ses Design Patterns.
![Diagramme de Classes](<./assets/Class Diagram V.2.0.png>)


### Implémentation des Design Patterns

L'architecture a été pensée pour garantir un code robuste, évolutif et testable. Les patterns de la v1.0 ont été conservés et adaptés au nouvel environnement graphique.

#### 1. Le Singleton : Sécuriser l'accès aux fichiers (Thread-Safety)
* **Problème technique :** Gérer les écritures concurrentes (logs et états), un aspect encore plus critique avec une UI asynchrone.
* **Solution apportée :** Le Singleton (`Logger`), associé à un mécanisme de verrouillage (`lock`), agit comme un goulot d'étranglement sécurisé : toutes les requêtes d'écriture sont traitées de manière séquentielle.

#### 2. L'Observer : Découpler le moteur de l'interface et des logs
* **Évolution V2.0 :** Le moteur se contente toujours de "diffuser" son avancement. La grande nouveauté est que cette notification met à jour la propriété `Progress` de l'objet `BackupJob`. Grâce au DataBinding du MVVM, l'interface graphique (barre de progression) se rafraîchit automatiquement, le moteur restant totalement aveugle à l'existence de l'UI.

#### 3. La Strategy : Isoler la logique algorithmique
* **Solution apportée :** Chaque algorithme de calcul (Complet/Différentiel) est encapsulé. Le moteur demande simplement à la stratégie la liste des fichiers à copier, sans se soucier des mathématiques internes (Principe Ouvert/Fermé).

#### 4. La Factory : Centraliser la création des objets
* **Solution apportée :** La Factory analyse le type de sauvegarde demandé par l'utilisateur depuis l'interface et instancie dynamiquement l'outil adéquat (`Full` ou `Differential`).

#### 5. Le Bridge / (Inversion de Dépendance) : Rendre le système testable
* **Solution apportée :** Création d'une interface `IFileSystem` agissant comme une abstraction. En production, le moteur utilise le vrai disque. En phase de test (xUnit), nous injectons des "Mocks" en mémoire vive, garantissant des tests sûrs et rapides sans toucher à l'OS.

### Respect des Principes SOLID

- **S - Responsabilité Unique** : `JobManager` gère la liste, `BackupEngine` orchestre la copie, `EncryptionService` chiffre.
- **O - Ouvert/Fermé** : Ajout possible de nouvelles sauvegardes (ex: Incrémentale) sans modifier `BackupEngine`.
- **L - Substitution de Liskov** : Le `BackupEngine` manipule les stratégies de la même manière sans anomalie.
- **I - Ségrégation des Interfaces** : Interfaces petites et ciblées (`IBackupObserver`, `IBackupStrategy`).
- **D - Inversion des Dépendances** : `BackupEngine` dépend de `IFileSystem` (abstraction) et non de `System.IO.File`.

---

## 5. Intégrité et Tests

La couverture de la solution **`EasySaveTest`** (xUnit) a été adaptée et étendue pour valider les nouvelles fondations techniques de la V2.0 :

- **Tests des Modèles (`BackupJobTests`)** : Vérification de la persistance de l'état graphique (`Progress` et `ShowProgress`).
- **Tests des Règles Métiers (`JobManagerTests`)** :
  - **Évolution :** Retrait de l'ancien test limitant le système à 5 travaux.
  - Ajout d'un test sur la méthode `UpdateJob()` garantissant que les modifications d'un travail existant s'enregistrent correctement.
- **Tests de Résilience (`EngineFaultToleranceTests`)** : S'assure que le moteur gère proprement les erreurs de saisie utilisateur depuis l'interface (ex: chemin source introuvable) en capturant l'exception (`Record.Exception`) au lieu de provoquer un crash applicatif.
- **Tests d'Interruption** : Validation de la détection d'un processus métier interdisant ou interrompant une sauvegarde à la volée.
- **Tests de l'Architecture (`LoggerTests` & `BackupFactoryTests`)** : Maintien strict de la validation du Singleton et de la Factory.