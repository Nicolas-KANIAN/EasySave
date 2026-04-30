# Manuel d'Utilisation - EasySave V2.0

Bienvenue dans **EasySave**, une solution de sauvegarde moderne, fiable et performante développée par notre équipe. 

Vous trouverez dans ce manuel une présentation des principales fonctionnalités de l’application ainsi que la manière de les utiliser.

Cette version 2.0 marque une évolution majeure avec une interface graphique intuitive (WPF) organisée par onglets pour une gestion simplifiée de vos données.

---

## Choix de la Langue

Vous pouvez changer instantanément la langue de l'application en cliquant sur les **icônes de drapeaux (Français ou Anglais)** situées en haut à droite de la fenêtre.

---

## Démarrage

Pour lancer l'application, double-cliquez sur l'exécutable `EasySave.exe`. 

L'interface se divise en trois onglets :
1.  **Tâches** 
2.  **Paramètres** 
3.  **Journaux** 

---

## 1. Onglet Tâches
C'est l'écran principal permettant de gérer et d'exécuter vos travaux de sauvegarde.

### Travaux de sauvegarde (Gauche)
- **Liste des travaux** : Affiche tous les travaux créés.
- **Lancer sélection** : Exécute uniquement les travaux cochés ou sélectionnés dans la liste.
- **Tout lancer** : Lance l'intégralité des travaux de la liste séquentiellement.
- **Supprimer** : Retire définitivement le travail sélectionné.
- **Activite & Logs en temps réel** : Situés en bas, ces encadrés affichent les événements système et la progression en direct des transferts de fichiers.

### Formulaire Travail (Droite)
Pour configurer une sauvegarde :
1. **Nom** : Saisissez un nom unique pour identifier le travail.
2. **Repertoire source** : Indiquez le chemin du dossier à sauvegarder.
3. **Repertoire cible** : Indiquez le chemin de destination.
4. **Type de sauvegarde** : Choisissez entre **Complet** (tous les fichiers) ou **Différentiel** (uniquement les fichiers modifiés).
5. **Actions** : 
   - Cliquez sur **Créer** pour ajouter un nouveau travail.
   - Sélectionnez un travail existant dans la liste pour activer le bouton **Modifier**.
   - Utilisez **Effacer** pour vider les champs du formulaire.

---

## 2. Onglet Paramètres
Cet onglet permet de configurer les règles globales de l'application.

### Logs
- **Format des logs** : Choisissez entre **Json** ou **Xml** pour la génération de vos rapports d'activité quotidiens.

### Cryptage (CryptoSoft)
- **Extensions a chiffrer** : Définissez les types de fichiers à sécuriser (ex: `.txt; .pdf; .docx`). 
- **Clé CryptoSoft** : Définissez la clé secrète utilisée par le moteur de chiffrement CryptoSoft.

### Logiciel metier
- **Processus logiciel metier** : Indiquez le nom du logiciel prioritaire (ex: `calculator.exe`). EasySave mettra automatiquement en **pause** toute sauvegarde si ce processus est détecté en cours d'exécution.

> **Note** : N'oubliez pas de cliquer sur le bouton **Enregistrer** en bas pour appliquer vos modifications.

---

## 3. Onglet Journaux
Cet onglet est dédié à la consultation de votre historique de sauvegarde.

- **Sélecteur de Date** : Choisissez une date précise pour consulter les logs correspondants.
- **Ouvrir les logs** : Charge et affiche le contenu du fichier de log pour la date sélectionnée dans la visionneuse.
- **Logs du jour** : Raccourci pour afficher instantanément l'activité enregistrée pour la journée en cours.
- **Visionneuse de Logs** : Affiche les détails techniques (heure, taille des fichiers, temps de transfert et temps de chiffrement).