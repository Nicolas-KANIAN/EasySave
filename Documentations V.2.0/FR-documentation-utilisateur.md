# Manuel d'Utilisation

Bienvenue dans EasySave, une solution de sauvegarde moderne, fiable et performante développée par notre équipe. 

Vous trouverez dans ce manuel une présentation des principales fonctionnalités de l’application ainsi que la manière de les utiliser.

Cette version 2.0 marque une évolution majeure avec une interface graphique intuitive (WPF) et des fonctionnalités de sécurité avancées.

---

## Démarrage

Pour lancer l'application, double-cliquez sur l'exécutable `EasySave.exe`. 
L'interface se divise en trois zones principales :
1.  **Travaux de sauvegarde (Gauche)** : La liste de vos travaux et les boutons d'action (Lancer/Supprimer).
2.  **Créer/Modifier un travail (Droite Haut)** : Le formulaire pour configurer vos sauvegardes.
3.  **Paramètres & Activité (Bas)** : La configuration globale et le journal des événements en temps réel.

---

## Choix de la Langue
Plus besoin de saisir de numéro ! Cliquez simplement sur les **drapeaux (Français ou Anglais)** situés en haut à droite de la fenêtre pour changer instantanément la langue de l'interface.

---

## Gestion des Travaux de Sauvegarde

Contrairement à la version précédente, vous pouvez désormais créer un **nombre illimité** de travaux de sauvegarde.

### 1. Créer un travail
Dans la section **"Créer un travail"** :
- **Nom** : Un nom unique pour identifier votre sauvegarde.
- **Répertoire source** : Cliquez dans le champ et saisissez le chemin du dossier à sauvegarder.
- **Répertoire cible** : Saisissez le chemin où les fichiers seront copiés.
- **Type de sauvegarde** : Sélectionnez **Complet** ou **Différentiel** dans le menu déroulant.
- Cliquez sur **Créer**.

### 2. Modifier un travail (Nouveauté V.2.0)
Pour modifier une configuration existante :
1.  **Sélectionnez** le travail souhaité dans la liste à gauche.
2.  Les informations s'affichent automatiquement dans le formulaire de droite.
3.  Modifiez les champs nécessaires (Nom, Source, Cible ou Type).
4.  Cliquez sur le bouton **Modifier**.

### 3. Supprimer un travail
Sélectionnez un ou plusieurs travaux dans la liste, puis cliquez sur le bouton **Supprimer sélection** situé sous la liste. 

---

## Exécution des Sauvegardes

Vous pouvez lancer vos sauvegardes de deux manières :
- **Lancer sélection** : Sélectionnez les travaux que vous souhaitez lancer et cliquez sur ce bouton.
- **Tout lancer** : Lance tous les travaux de la liste les uns après les autres.

### Sécurité : Logiciel Métier
EasySave 2.0 surveille si un logiciel professionnel (ex: `calculator.exe`) est ouvert. 
- Si le logiciel est détecté au lancement, la sauvegarde est bloquée pour éviter les conflits de fichiers.
- S'il est ouvert pendant une sauvegarde, EasySave met immédiatement le processus en **pause**.

---

## Paramètres 

La section **Paramètres** en bas à droite permet de configurer le comportement global de l'application :
- **Format des logs** : Choisissez entre **JSON** ou **XML** pour vos rapports.
- **Logiciel métier** : Indiquez le nom du processus à surveiller (ex: `calculator.exe`).
- **Extensions à chiffrer** : Saisissez les extensions de fichiers à chiffrer (ex: `.txt;.pdf`).
- **Crypto key** : Définissez votre clé secrète pour le chiffrement via **CryptoSoft**.

*N'oubliez pas de cliquer sur **Sauvegarder** pour appliquer vos changements.*

---

## Suivi et Logs

EasySave génère deux types de fichiers de suivi dans le dossier de l'application :
1.  **Suivi en direct (`state.json`)** : Ce fichier se met à jour en temps réel et vous permet de suivre la progression exacte du transfert en cours.
2.  **Rapports journaliers (`DailyLog_date.json`)** : Contient l'historique détaillé, le temps de transfert et la taille de chaque fichier copié au jour le jour. En V.2.0, ces logs incluent désormais le **temps de chiffrement (en ms)** pour les fichiers sécurisés via CryptoSoft.

---

## Logs d'Activité
La zone **Activité** en bas à gauche de l'écran vous informe en direct de chaque action : création réussie, erreur de répertoire, détection du logiciel métier ou fin de sauvegarde.