# User Manual - EasySave V3.0

Welcome to **EasySave**, a modern, reliable, and high-performance backup solution developed by our team.

In this manual, you will find an overview of the application's main features and how to use them.

Version 3.0 marks a major evolution towards **multi-threading** and precise management of your data transfers.

---

## Language Selection

You can instantly change the application's language by clicking the **flag icons (French or English)** located in the top-right corner of the window.

---

## Getting Started

To launch the application, double-click the `EasySave.exe` executable.

The interface is divided into three tabs:
1. **Jobs**
2. **Settings**
3. **Logs**

---

## 1. Jobs Tab
This is the main screen for managing and executing your backup tasks.

### Backup Jobs (Left)
* **Job List**: Displays all created jobs.
* **Run selected**: Executes only the jobs checked or selected in the list.
* **Run all**: Launches all jobs in the list sequentially.
* **Delete**: Permanently removes the selected job.
- **Pause**: Suspend the execution of the backup.
- **Resume**: Restart the backup from where it was paused.
- **Stop**: Completely terminate the backup process.
* **Activity & Real-time Logs**: Located at the bottom, these boxes display system events and live file transfer progress.

### Job Form (Right)
To configure a backup:
1. **Name**: Enter a unique name to identify the job.
2. **Source directory**: Enter the path of the folder to be backed up.
3. **Target directory**: Enter the destination path.
4. **Backup type**: Choose between **Full** (all files) or **Differential** (only modified files).
5. **Actions**:
    * Click **Create** to add a new job.
    * Select an existing job from the list to enable the **Update** button.
    * Use **Clear** to empty the form fields.

---

## 2. Settings Tab
This tab allows you to configure global application rules.

### Logs and Centralization
- **Log Format**: Choose between **JSON** or **XML** for the generation of your daily activity reports.
- **Docker Centralization**: Enable real-time log transmission to a remote **Docker** instance for centralized supervision of your backup fleet.

### Encryption (CryptoSoft)
- **Extensions to Encrypt**: Define the file types to be secured (e.g., `.txt; .pdf; .docx`).
- **CryptoSoft Key**: Define the secret key used by the CryptoSoft encryption engine.
- **Single-Instance Management**: The engine automatically manages an exclusive **lock** on CryptoSoft to avoid access conflicts during parallel backups.

### Business Software
- **Business Software Process**: Enter the name of the priority software (e.g., `calculator.exe`). EasySave will automatically **pause** any backup if this process is detected running.

### Optimization & Priorities (v3.0)
- **n KB Threshold (Parallel Size)**: Set a size limit. Files exceeding this threshold will be transferred one by one to avoid bandwidth saturation, while smaller files continue in parallel.
- **Priority Extensions**: List the files (e.g., `.db; .sql`) that should be transferred at the beginning of the task, before other data.

> **Note**: Don't forget to click the **Save** button at the bottom to apply your changes.

---

## 3. Logs Tab
This tab is dedicated to viewing your backup history.

* **Date Picker**: Choose a specific date to view the corresponding logs.
* **Open logs**: Loads and displays the content of the log file for the selected date in the viewer.
* **Today logs**: Shortcut to instantly view the activity recorded for the current day.
* **Log Viewer**: Displays technical details (time, file size, transfer time, and encryption time).