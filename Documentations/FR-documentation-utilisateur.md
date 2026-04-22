# Manuel d'Utilisation

Bienvenue dans EasySave, une solution de sauvegarde moderne, fiable et performante développée par notre équipe. 

Vous trouverez dans ce manuel une présentation des principales fonctionnalités de l’application ainsi que la manière de les utiliser.

## Démarrage rapide

Pour lancer l'application, double-cliquez sur `EasySave.exe`.

## Menu Principal

À l'ouverture, chosissez la langue que vous souhaitez utiliser pour poursuivre la navigation dans l’application. Pour ce faire, entrez le numéro correspondant à la langue souhaitée, puis appuyez sur la touche **Entrée** pour valider.

Vous accédez ensuite au menu principal, qui affiche toutes les options disponibles:

- **1. Créer un travail de sauvegarde** 
- **2. Afficher les travaux** 
- **3. Lancer une sauvegarde** 
- **4. Supprimer un travail** 
- **5. Quitter** 

Entrez le numéro de l'option souhaitez, puis appuyez sur la touche **Entrée** pour continuer.

### 1. Créer un travail de sauvegarde

Permet de définir un nouveau travail de sauvegarde. Attention, celui-ci ne s'exécute pas automatiquement après sa création ! 

Vous devrez fournir :

- **Nom du travail** : Un nom unique pour la sauvegarde.
- **Répertoire Source (ex: C:\Dossier)** : Le chemin complet du dossier à sauvegarder.
- **Répertoire Cible (ex: D:\Backup)** : Le chemin complet où les fichiers seront copiés.
- **Type (0 = Complet, 1 = Différentiel)** :
    - *0 = Complet* : Copie tous les fichiers à chaque fois.
    - *1 = Différentiel* : Copie uniquement les fichiers modifiés depuis la dernière sauvegarde complète.

Un message confirmant la réussite de la création du travail s'affichera, puis le menu EsaySave réapparaîtra.

> **Note** : Vous pouvez créer jusqu'à 5 sauvegardes simultanément. Si cette limite est atteinte, vous devrez supprimer un travail existant avant d’en créer un nouveau.

### 2. Afficher les travaux

Affiche une liste numérotée de tous les travaux de sauvegarde, dans l'ordre chronologique, avec leur nom, leur type, leur répertoire source et leur répertoire cible.

### 3. Lancer une sauvegarde

Lance un ou plusieurs travaux de sauvegarde. L'ensemble des fichiers du dossier source sera copié vers le dossier cible selon le type de sauvegarde défini. Les logs d'activité seront mis à jour en temps réel.

Vous pouvez saisir l’index du ou des travaux que vous souhaitez exécuter, ou simplement écrire **All** pour les lancer tous.

### 4. Supprimer un travail

Permet de retirer un travail de la liste. Sélectionnez simplement le travail à supprimer à l'aide de son index et confirmez votre choix. Vous pouvez aussi annuler en appuyant sur la touche **q**.

### 5. Quitter

Permet de quitter le logiciel de sauvegarde.