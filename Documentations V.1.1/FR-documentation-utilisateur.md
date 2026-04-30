# Manuel d'Utilisation

Bienvenue dans EasySave, une solution de sauvegarde moderne, fiable et performante développée par notre équipe. 

Vous trouverez dans ce manuel une présentation des principales fonctionnalités de l’application ainsi que la manière de les utiliser.

## Démarrage rapide

Pour lancer l'application, double-cliquez sur l'exécutable `EasySave.exe`. 
Vous pouvez également lancer l'application en ligne de commande en y ajoutant directement les index des travaux à exécuter. Deux méthodes sont possibles :
- **Une plage de travaux** : Utilisez un tiret (ex: `EasySave.exe 1-3` pour exécuter automatiquement les travaux 1, 2 et 3).
- **Des travaux spécifiques** : Utilisez un point-virgule (ex: `EasySave.exe 1;3` pour exécuter automatiquement uniquement les travaux 1 et 3).

## Choix de la langue

À l'ouverture, choisissez la langue que vous souhaitez utiliser pour la navigation. Entrez le numéro correspondant (1 pour English, 2 pour Français), puis appuyez sur la touche **Entrée** pour valider.

---

## Menu Principal

Une fois la langue sélectionnée, vous accédez au menu principal. Entrez le numéro de l'option souhaitée, puis appuyez sur la touche **Entrée** pour continuer.

### 1. Créer un travail de sauvegarde

Permet de définir un nouveau travail de sauvegarde. **Attention**, celui-ci ne s'exécute pas automatiquement après sa création, il est simplement sauvegardé dans votre liste ! 

Vous devrez fournir :
- **Nom du travail** : Un nom unique pour identifier la sauvegarde.
- **Répertoire Source (ex: C:\Dossier)** : Le chemin complet du dossier que vous souhaitez sauvegarder.
- **Répertoire Cible (ex: D:\Backup)** : Le chemin complet où les fichiers seront copiés.
- **Type (0 = Complet, 1 = Différentiel)** :
    - *0 = Complet* : Copie l'intégralité des fichiers de la source vers la cible à chaque exécution.
    - *1 = Différentiel* : Copie uniquement les fichiers nouveaux ou modifiés depuis la dernière sauvegarde.

Un message confirmant la réussite de la création s'affichera, puis le menu EasySave réapparaîtra.

> **Note** : Vous pouvez créer jusqu'à 5 sauvegardes. Si cette limite est atteinte, vous devrez supprimer un travail existant avant d’en créer un nouveau.

### 2. Afficher les travaux

Affiche la liste numérotée de tous vos travaux de sauvegarde enregistrés, dans l'ordre chronologique, avec leur nom, leur type, et leurs répertoires source/cible.

### 3. Lancer une sauvegarde

Lance l'exécution de vos travaux de sauvegarde. Tous les fichiers du dossier source seront copiés vers le dossier cible selon le type de sauvegarde défini.

Lorsqu'on vous le demande, vous avez plusieurs possibilités de saisie :
- **Un seul travail** : Tapez simplement son numéro (ex: `2`).
- **Plusieurs travaux précis** : Séparez les numéros par un point-virgule (ex: `1;3` lancera le travail 1 puis le 3).
- **Une plage de travaux** : Utilisez un tiret (ex: `1-3` lancera les travaux 1, 2 et 3).
- **Tous les travaux** : Tapez le mot `all` pour exécuter l'intégralité de votre liste de manière séquentielle.

### 4. Supprimer un travail

Permet de retirer un travail de votre liste. Entrez simplement l'index (le numéro) du travail à supprimer. Si vous changez d'avis, vous pouvez annuler l'opération en appuyant sur la touche **q**.

### 5. Paramètres (Format des logs)

Permet de changer le format du fichier de logs. Entrez simplement le numéro du format que vous souhaitez (1 pour JSON ou 2 pour XML). Si déjà renseigné, le format actuel des logs apparaîtra. Si vous changez d'avis, vous pouvez sortir de cette opération en appuyant sur la touche **q**.

### 6. Quitter

Ferme proprement l'application EasySave.

---

## Fichiers de suivi (Logs)

Pendant et après vos sauvegardes, EasySave génère automatiquement des rapports dans le dossier `EasyLogs` (situé au même endroit que votre application) :
- **Suivi en direct (`state.json` / `state.xml`)** : Ce fichier se met à jour en temps réel et vous permet de suivre la progression exacte du transfert en cours.
- **Rapports journaliers (`DailyLog_{date}.json` / `DailyLog_{date}.xml`)** : Conserve l'historique détaillé, le temps de transfert et la taille de chaque fichier copié au jour le jour.