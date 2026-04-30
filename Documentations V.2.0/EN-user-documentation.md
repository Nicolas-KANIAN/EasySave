# User Manual - EasySave V2.0

Welcome to **EasySave**, a modern, reliable, and high-performance backup solution developed by our team.

In this manual, you will find an overview of the application's main features and how to use them.

Version 2.0 marks a major evolution with an intuitive Graphical User Interface (WPF) organized by tabs for simplified data management.

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

### Logs
* **Log format**: Choose between **Json** or **Xml** for generating your daily activity reports.

### Encryption (CryptoSoft)
* **Extensions to encrypt**: Define the file types to be secured (e.g., `.txt; .pdf; .docx`).
* **Crypto Key**: Define the secret key used by the CryptoSoft encryption engine.

### Business Software
* **Business software process**: Enter the name of the priority software (e.g., `calculator.exe`). EasySave will automatically **pause** any backup if this process is detected running.

> **Note**: Don't forget to click the **Save** button at the bottom to apply your changes.

---

## 3. Logs Tab
This tab is dedicated to viewing your backup history.

* **Date Picker**: Choose a specific date to view the corresponding logs.
* **Open logs**: Loads and displays the content of the log file for the selected date in the viewer.
* **Today logs**: Shortcut to instantly view the activity recorded for the current day.
* **Log Viewer**: Displays technical details (time, file size, transfer time, and encryption time).