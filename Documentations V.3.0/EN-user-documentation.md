# User Manual - EasySave V3.0

Welcome to **EasySave**, a modern, reliable, and high-performance backup solution developed by our team. 

You will find in this manual an overview of the application's main features and how to install and use them.

**Version 2.0** marked a major evolution with an intuitive, cross-platform graphical user interface organized by tabs. **Version 3.0** goes further by introducing **multi-threading** (parallel execution), full control over execution (Pause/Stop), and advanced network management with Docker to centralize your transfer logs.

---

## 1. Installation and Deployment

Before launching EasySave, you must retrieve the application and, if desired, configure the log centralization server.

### Retrieving the application (Release)
1. Go to the project's GitHub page in the **Releases** section.
2. Download the `.zip` archive corresponding to the latest version (V3.0).
3. Extract the contents of the archive to a folder of your choice on your computer.
4. The application is portable: no standard installation is required. The `EasySaveApp.exe` file is ready to use.

### Deploying the log server (Docker)
If you want to use the log centralization feature, you must start the Docker server provided with the solution. Make sure Docker Desktop is installed and running on your machine or server.
1. Open a terminal (Command Prompt or PowerShell).
2. Navigate to the folder containing the log server's `Dockerfile` (`EasySaveLogServer` folder).
3. Build the Docker image with the command: `docker build -t easysave-log-server .`
4. Start the container and expose the network port with the command: `docker run -d -p 12345:12345 --name easysave-logger easysave-log-server`
5. The server is now listening. You can configure the IP address and port in the EasySave application settings.

---

## 2. Startup and Navigation

To launch the application, double-click the `EasySaveApp.exe` executable. 

**Language Choice:** You can instantly change the application's language by clicking the flag icons (French or English) located at the top right of the window.

The interface is divided into three main tabs: **Tasks**, **Settings**, and **Logs**.

---

## 3. Tasks Tab

This is the main screen for creating, configuring, and executing your backup jobs.

**Create or Edit a backup (Right panel)**
* **Name**: Enter a unique name to identify the job (e.g., "Accounting Backup").
* **Backup type**: Choose **Full** (copies everything) or **Differential** (copies only modified files).
* **Directories**: Use the `...` buttons to select your Source and Target folders.
* **Actions**: Use the Create, Update, or Clear buttons to manage the form.

**Manage and Execute your backups (Left panel)**
* **Selection**: Check the boxes to the left of each name to select multiple jobs.
* **Execution**: Use "Run selected" or "Run all" to start parallel copies.
* **Delete**: Permanently removes the selected job from the list.
* **Pause**: Puts the running jobs on hold.
* **Resume**: Restarts jobs that are on hold.
* **Stop**: Completely stops the currently running jobs.

**Real-time Control and Tracking**
* **Live commands**: During a backup, you can use the Pause, Resume, or Stop buttons to control the flow.
* **Tracking**: The overall system state and detailed progress are displayed at the bottom of the screen.

---

## 4. Settings Tab

This tab allows you to configure the global rules of the EasySave engine. Do not forget to click **Save** at the bottom of the page to apply your changes.

**Logs and Network Routing**
* **Format and Destination**: Choose the local format (JSON/XML) and the writing destination (Local, Centralized via Docker, or Both).

**Encryption (CryptoSoft)**
* **Secured files**: List the extensions (e.g., `.txt; .pdf`) to be encrypted on the fly with your CryptoSoft Key. The engine guarantees "Single-Instance" protection to avoid access conflicts.

**Protection and Optimization (New in V3.0)**
* **Business software**: Specify a critical executable (e.g., `calculatorapp.exe`). If it is open, all backups will automatically be paused.
* **Priority extensions**: List the extensions (e.g., `.txt`) that must be transferred urgently at the beginning of the task.
* **Max size for simultaneous execution**: Set a limit (in KB). Files exceeding this threshold will be transferred one by one to prevent hardware saturation of your network.

---

## 5. Logs Tab

This tab is dedicated to viewing your activity history. Enter a date (in YYYY-MM-DD format) or click "Today's logs" to load the report. The viewer will then display the technical details: file sizes, transfer times, and encryption times.

---

## 6. Advanced CLI Usage (Command Line)

EasySave can be executed silently via a terminal (Command Prompt or PowerShell). Navigate to the EasySave folder and launch the application followed by the numbers of the jobs to execute.

* **A single job**: `EasySaveApp.exe 2`
* **Specific list**: `EasySaveApp.exe 1;3;5`
* **Sequence**: `EasySaveApp.exe 1-4`

Command line execution benefits from the same multi-threading engine and security rules as the graphical interface.