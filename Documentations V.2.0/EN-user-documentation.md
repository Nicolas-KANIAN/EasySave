# User Manual

Welcome to EasySave, a modern, reliable, and high-performance backup solution developed by our team. 

In this manual, you will find an overview of the application's main features and instructions on how to use them.

Version 2.0 marks a major evolution with an intuitive Graphical User Interface (WPF) and advanced security features.

---

## Getting Started

To launch the application, double-click the `EasySave.exe` executable. 
The interface is divided into three main areas:
1. **Backup Jobs (Left)**: The list of your jobs and action buttons (Run/Delete).
2. **Create/Update a Job (Top Right)**: The form used to configure your backups.
3. **Settings & Activity (Bottom)**: Global configuration and the real-time event log.

---

## Language Selection
No more typing numbers! Simply click on the **flags (French or English)** located at the top right of the window to instantly change the interface language.

---

## Managing Backup Jobs

Unlike the previous version, you can now create an **unlimited number** of backup jobs.

### 1. Create a Job
In the **"Create a job"** section:
- **Name**: A unique name to identify your backup.
- **Source directory**: Click in the field and enter the path of the folder to be backed up.
- **Target directory**: Enter the path where the files will be copied.
- **Backup type**: Select **Full** or **Differential** from the dropdown menu.
- Click **Create**.

### 2. Update a Job (New in V.2.0)
To modify an existing configuration:
1. **Select** the desired job from the list on the left.
2. The information will automatically appear in the form on the right.
3. Modify the necessary fields (Name, Source, Target, or Type).
4. Click the **Update** button.

### 3. Delete a Job
Select one or more jobs from the list, then click the **Delete selected** button located below the list. 

---

## Running Backups

You can start your backups in two ways:
- **Run selected**: Select the jobs you wish to launch and click this button.
- **Run all**: Launches all jobs in the list one after the other.

### Security: Business Software
EasySave 2.0 monitors if a specific professional software (e.g., `calculator.exe`) is open. 
- If the software is detected at startup, the backup is blocked to prevent file conflicts.
- If it is opened during a backup, EasySave immediately puts the process on **pause**.

---

## Settings 

The **Settings** section at the bottom right allows you to configure the application's global behavior:
- **Log format**: Choose between **JSON** or **XML** for your reports.
- **Business software process**: Enter the name of the process to monitor (e.g., `calculator.exe`).
- **Extensions to encrypt**: Enter the file extensions to be encrypted (e.g., `.txt;.pdf`).
- **Crypto key**: Define your secret key for encryption via **CryptoSoft**.

*Don't forget to click **Save settings** to apply your changes.*

---

## Tracking and Logs

EasySave generates two types of tracking files in the application folder:
1. **Real-time tracking (`state.json`)**: This file updates in real-time and allows you to track the exact progress of the current transfer.
2. **Daily reports (`DailyLog_date.json`)**: Keeps a detailed history, transfer time, and size of each copied file on a day-to-day basis. In V.2.0, these logs now include the **encryption time (in ms)** for files secured via CryptoSoft.

---

## Activity Logs
The **Activity** zone at the bottom left of the screen informs you live of every action: successful creation, directory error, business software detection, or backup completion.