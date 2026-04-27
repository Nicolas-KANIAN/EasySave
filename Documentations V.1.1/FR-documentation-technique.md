# Documentation Support Technique - EasySave v1.0

Ce document contient les informations techniques nécessaires pour l'installation, la compréhension de l'architecture et le dépannage de la version 1.0 (Console) de notre logiciel de sauvegarde EasySave.

---

## 1. Informations Générales

- **Version** : 1.1 (Livrable 2 - Application Console)
- **Langage** : C#
- **Framework** : .NET 8.0
- **Système d'exploitation** : Windows (compatible cross-platform)

**Outils et Méthodes :**
- **IDE & Versionning** : Visual Studio / GitHub
- **Revue de code par IA** : **CodeRabbit** intégré au dépôt GitHub. À chaque Pull Request, l'IA est utilisée uniquement pour générer un résumé automatique des changements apportés. *(Source : [Configuration CodeRabbit](https://docs.coderabbit.ai/reference/configuration))*
- **Tests unitaires** : **xUnit** *(Source : [Documentation xUnit](https://xunit.net/?tabs=cs))*
- **Formatage du code** : `.editorconfig`

---

## 2. Gitflow

Afin d'organiser au mieux notre projet, nous avons décidé de suivre un gitflow précis. Nous avons créé une branche `develop` à partir de `main`, et des branches `features` pour chaque fonctionnalité du projet à partir de la branche `develop`.

<img width="700" alt="Schéma du Gitflow" src="./assets/gitflow.webp" />

---

## 3. Structure des Fichiers et Configuration

L'application est portable. Tous les fichiers de configuration et de logs sont stockés dans le dossier où est présent l'exécutable `EasySave.exe`.

### Configuration Générale (`config.json`)
- **Emplacement** : `<Dossier_Application>/EasyLogs/config.json`
- **Paramètres globaux** :
  - `LogFormat` : Format d'écriture des fichiers de logs (`.Json`).
  - `Language` : Langue de l'interface console (`en` ou `fr`).

### Configuration des Travaux (`jobs.json`)
- **Emplacement** : `<Dossier_Application>/EasyLogs/jobs.json`
- **Format** : Tableau JSON d'objets `BackupJob` (Name, Source, Target, Type).
- **Règle Métier** : Limité techniquement à 5 travaux maximum. Si ce fichier est supprimé, la liste sera vide au prochain lancement.

### Journaux d'Activité (`DailyLog_{date}`)
- **Emplacement** : `<Dossier_Application>/EasyLogs/DailyLog_{dd_MM_yyyy}.json`
- **Format** : Un fichier par jour généré par la DLL `EasyLog` en JSON ou XML.
- **Contenu** : Détail de chaque transfert (`Timestamp`, `Name`, `Source`, `Target`, `FileSize`, `TransferTimeMs`).

### Fichier d'État temps réel (`state.json`)
- **Emplacement** : `<Dossier_Application>/EasyLogs/state.json`
- **Contenu** : État d'avancement du travail en cours (`Progression`, `TotalFilesToCopy`, `NbFilesLeftToDo`, `RemainingFilesSize`, `State`).
- **Comportement** : Réécrit dynamiquement à chaque transfert de fichier.
- **Format** : JSON ou XML.

---

## 4. Architecture et Conception Technique

### Structuration des Dossiers
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

<img src="./assets/Usecase Diagram V.1.0.png" alt="Diagramme de Cas d'Utilisation" width="600" />

**2. Diagramme d'Activité**  
Détaille le flux d'exécution logique du moteur lorsqu'une sauvegarde est lancée.

![Diagramme d'Activité](<./assets/Activity Diagram V.1.0.png>)

**3. Diagramme de Séquence**  
Illustre les appels chronologiques entre les différents objets lors du cycle de vie d'une sauvegarde.

![Diagramme de Séquence](<./assets/Sequence Diagram V.1.0.png>)

**4. Diagramme de Classes**  
Représente l'architecture statique détaillée de l'application et de ses Design Patterns.

![Diagramme de Classes](<./assets/Class Diagram V.1.0.png>)


### Implémentation des Design Patterns

L'architecture a été pensée pour garantir un code robuste, évolutif et testable. Nous avons implémenté plusieurs Design Patterns pour résoudre des problématiques techniques précises et respecter les principes de la programmation orientée objet.

#### 1. Le Singleton : Sécuriser l'accès aux fichiers (Thread-Safety)
* **Composant concerné :** `Logger`
* **Problème technique :** Si deux travaux de sauvegarde se terminent à la même milliseconde et tentent d'écrire simultanément dans le même fichier journal (`DailyLog_xxx.json`), le système d'exploitation bloquera l'accès (File Lock), provoquant un crash de l'application.
* **Solution apportée :** Le Singleton garantit qu'il n'existe qu'une seule et unique instance du `Logger` en mémoire. Associé à un mécanisme de verrouillage (`lock`), il agit comme un goulot d'étranglement sécurisé : toutes les requêtes d'écriture sont traitées de manière séquentielle, évitant absolument tout conflit d'accès concurrent.

#### 2. L'Observer : Découpler le moteur de l'interface et des logs
* **Composants concernés :** `IBackupObserver`, `StateLoggerObserver`
* **Problème technique :** Le moteur de sauvegarde (`BackupEngine`) a une seule responsabilité : copier des fichiers de manière fiable. S'il devait formater du JSON pour mettre à jour le fichier d'état en temps réel (`state.json`), il serait surchargé et fortement couplé au système de logging.
* **Solution apportée :** Le moteur se contente de "diffuser" son avancement à chaque étape d'une copie (`NotifyObservers`). L'observateur (`StateLoggerObserver`) écoute ces notifications et se charge de l'écriture sur le disque. Le moteur reste léger, rapide, et ignore totalement comment ces informations sont traitées.

#### 3. La Strategy : Isoler la logique algorithmique (Le "Comment")
* **Composants concernés :** `IBackupStrategy`, `FullBackupStrategy`, `DifferentialBackupStrategy`
* **Problème technique :** Gérer les différences de logique entre une sauvegarde complète et différentielle directement dans le moteur entraînerait des conditions complexes (`if/switch`) et violerait le principe Ouvert/Fermé (Open/Closed) de SOLID.
* **Solution apportée :** Chaque algorithme de calcul des fichiers à copier est encapsulé dans sa propre classe. Le moteur demande simplement à la stratégie : *"Voici le dossier source, donne-moi la liste des fichiers à copier"*, sans se soucier des mathématiques internes. Ajouter un nouveau type de sauvegarde à l'avenir ne nécessitera aucune modification du moteur central.

#### 4. La Factory : Centraliser la création des objets (Le "Qui")
* **Composant concerné :** `BackupFactory`
* **Problème technique :** Bien que le moteur sache utiliser une `Strategy`, il ne sait pas comment instancier concrètement la bonne classe (Complète ou Différentielle) à partir de la configuration choisie par l'utilisateur (enum `BackupType`).
* **Solution apportée :** La Factory analyse le type de sauvegarde demandé et instancie dynamiquement l'outil adéquat. Le moteur confie la création de l'algorithme à la Factory, puis l'utilise via l'interface `IBackupStrategy`. Le `BackupEngine` devient ainsi totalement générique et aveugle à la complexité de création des objets.

#### 5. Le Bridge / (Inversion de Dépendance) : Rendre le système testable
* **Composants concernés :** `IFileSystem`, `LocalFileSystem`
* **Problème technique :** Les classes natives de .NET (`System.IO.File`, `Directory`) interagissent directement avec le disque dur. Cela rend les tests unitaires lents et dangereux, car ils nécessiteraient de créer et supprimer de vrais fichiers physiques.
* **Solution apportée :** Création d'une interface `IFileSystem` agissant comme une couche d'abstraction (Wrapper). En production, le moteur utilise `LocalFileSystem` (le vrai disque). En phase de test (xUnit), cette abstraction nous permet d'injecter des "Mocks" (un faux système de fichiers simulé en mémoire vive), garantissant des tests ultra-rapides, sûrs et isolés du système d'exploitation.

### Respect des Principes SOLID

L'architecture de l'application a été pensée pour garantir un code robuste, testable unitairement et prêt à évoluer vers des interfaces graphiques (pour le Livrable 2) sans réécriture du moteur central :

- **S - Responsabilité Unique** : Chaque fichier a un rôle strict. `JobManager` gère la liste, `BackupEngine` orchestre la copie, `Logger` gère les écritures.
- **O - Ouvert/Fermé** : Si nous devons ajouter une nouvelle sauvegarde (ex: Incrémentale), nous n'avons pas à modifier `BackupEngine`, mais simplement à créer une nouvelle classe implémentant `IBackupStrategy`.
- **L - Substitution de Liskov** : Le `BackupEngine` s'attend à recevoir une `IBackupStrategy`. Qu'il s'agisse d'une `FullBackupStrategy` ou d'une `DifferentialBackupStrategy`, le moteur l'utilise exactement de la même manière sans anomalie.
- **I - Ségrégation des Interfaces** : Nos interfaces sont petites et ciblées. `IBackupObserver` ne contient que la méthode `Update()`, `IBackupStrategy` ne contient que `GetFilesToCopy()`.
- **D - Inversion des Dépendances** : `BackupEngine` (haut niveau) ne dépend plus directement de `System.IO.File` (bas niveau), mais de l'abstraction `IFileSystem`.

---

## 5. Intégrité et Tests

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