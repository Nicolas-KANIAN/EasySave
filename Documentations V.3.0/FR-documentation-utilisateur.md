# Manuel d'Utilisation - EasySave V3.0

Bienvenue dans **EasySave**, une solution de sauvegarde moderne, fiable et performante développée par notre équipe. 

Vous trouverez dans ce manuel une présentation des principales fonctionnalités de l’application ainsi que la manière de les installer et de les utiliser.

La **version 2.0** marquait une évolution majeure avec une interface graphique intuitive et multi-plateformes organisée par onglets. La **version 3.0** va plus loin en introduisant l'exécution en **multi-threading** (parallélisme), un contrôle total sur l'exécution (Pause/Stop), et une gestion réseau avancée avec Docker pour centraliser vos journaux de transfert.

---

## 1. Installation et Déploiement

Avant de lancer EasySave, vous devez récupérer l'application et, si vous le souhaitez, configurer le serveur de centralisation des logs.

### Récupération de l'application (Release)
1. Rendez-vous sur la page GitHub du projet dans la section **Releases**.
2. Téléchargez l'archive `.zip` correspondant à la dernière version (V3.0).
3. Extrayez le contenu de l'archive dans le dossier de votre choix sur votre ordinateur.
4. L'application est portable : aucune installation classique n'est requise. Le fichier `EasySaveApp.exe` est prêt à être utilisé.

### Déploiement du serveur de logs (Docker)
Si vous souhaitez utiliser la fonctionnalité de centralisation des logs, vous devez démarrer le serveur Docker fourni avec la solution. Assurez-vous que Docker Desktop est installé et en cours d'exécution sur votre machine ou votre serveur.
1. Ouvrez un terminal (Invite de commandes ou PowerShell).
2. Naviguez vers le dossier contenant le fichier `Dockerfile` du serveur de logs (dossier `EasySaveLogServer`).
3. Construisez l'image Docker avec la commande : `docker build -t easysave-log-server .`
4. Démarrez le conteneur en exposant le port réseau avec la commande : `docker run -d -p 12345:12345 --name easysave-logger easysave-log-server`
5. Le serveur est maintenant à l'écoute. Vous pourrez configurer l'adresse IP et le port dans les paramètres de l'application EasySave.

---

## 2. Démarrage et Navigation

Pour lancer l'application, double-cliquez sur l'exécutable `EasySaveApp.exe`. 

**Choix de la Langue :** Vous pouvez changer instantanément la langue de l'application en cliquant sur les icônes de drapeaux (Français ou Anglais) situées en haut à droite de la fenêtre.

L'interface se divise en trois onglets principaux : **Tâches**, **Paramètres**, et **Journaux**.

---

## 3. Onglet Tâches

C'est l'écran principal permettant de créer, configurer et exécuter vos travaux de sauvegarde.

**Créer ou Modifier une sauvegarde (Panneau de droite)**
* **Nom** : Saisissez un nom unique pour identifier le travail (ex: "Sauvegarde Comptabilité").
* **Type de sauvegarde** : Choisissez **Complet** (copie tout) ou **Différentiel** (copie uniquement les fichiers modifiés).
* **Répertoires** : Utilisez les boutons `...` pour sélectionner vos dossiers Source et Cible.
* **Actions** : Utilisez les boutons Créer, Mettre à jour, ou Effacer pour gérer le formulaire.

**Gérer et Exécuter vos sauvegardes (Panneau de gauche)**
* **Sélection** : Cochez les cases à gauche de chaque nom pour sélectionner plusieurs travaux.
* **Exécution** : Utilisez "Lancer la sélection" ou "Tout lancer" pour démarrer les copies en parallèle.
* **Suppression** : Retire définitivement le travail sélectionné de la liste.
* **Pause** : Permet de mettre à l'arrêt les jobs en cours.
* **Reprendre** : Permet de relancer les jobs à l'arrêt.
* **Arrêter** : Permet l'arrêt complet des jobs en cours.

**Contrôle et Suivi en temps réel**
* **Commandes en direct** : Pendant une sauvegarde, vous pouvez utiliser les boutons Pause, Reprendre ou Arrêter pour contrôler le flux.
* **Suivi** : L'état global du système et la progression détaillée s'affichent en bas de l'écran.

---

## 4. Onglet Paramètres

Cet onglet permet de configurer les règles globales du moteur EasySave. N'oubliez pas de cliquer sur **Enregistrer** en bas de page pour appliquer vos modifications.

**Logs et Routage réseau**
* **Format et Destination** : Choisissez le format local (JSON/XML) et la destination d'écriture (Local, Centralisé via Docker, ou Les deux).

**Cryptage (CryptoSoft)**
* **Fichiers sécurisés** : Listez les extensions (ex: `.txt; .pdf`) à chiffrer à la volée avec votre Clé CryptoSoft. Le moteur garantit une protection "Single-Instance" pour éviter les conflits d'accès.

**Protection et Optimisation (Nouveauté V3.0)**
* **Logiciel métier** : Indiquez un exécutable critique (ex: `calculatorapp.exe`). S'il est ouvert, toutes les sauvegardes seront automatiquement mises en pause.
* **Extensions prioritaires** : Listez les extensions (ex: `.txt`) devant être transférées en urgence au début de la tâche.
* **Taille Max pour exécution simultanée** : Fixez une limite (en Ko). Les fichiers dépassant ce seuil seront transférés un par un pour éviter la saturation matérielle de votre réseau.

---

## 5. Onglet Journaux

Cet onglet est dédié à la consultation de votre historique d'activité. Saisissez une date (au format A-M-J) ou cliquez sur "Logs du jour" pour charger le rapport. La visionneuse affichera alors les détails techniques : taille des fichiers, temps de transfert et temps de chiffrement.

---

## 6. Utilisation Avancée (Ligne de commande - CLI)

EasySave peut être exécuté silencieusement via un terminal (Invite de commandes ou PowerShell). Naviguez vers le dossier d'EasySave et lancez l'application suivie des numéros des travaux à exécuter.

* **Un seul travail** : `EasySaveApp.exe 2`
* **Liste précise** : `EasySaveApp.exe 1;3;5`
* **Séquence** : `EasySaveApp.exe 1-4`

L'exécution en ligne de commande bénéficie du même moteur multi-threading et des mêmes règles de sécurité que l'interface graphique.