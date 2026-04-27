# User Manual

Welcome to EasySave, a modern, reliable, and high-performance backup solution developed by our team. 

In this manual, you will find an overview of the application's main features and how to use them.

## Quick Start

To launch the application, double-click the `EasySave.exe` executable. 
You can also launch the application via the command line by directly adding the indexes of the jobs to be executed. Two methods are available:
- **A range of jobs**: Use a hyphen (e.g., `EasySave.exe 1-3` to automatically run jobs 1, 2, and 3).
- **Specific jobs**: Use a semicolon (e.g., `EasySave.exe 1;3` to automatically run only jobs 1 and 3).

## Language Selection

Upon opening, choose the language you wish to use for navigation. Enter the corresponding number (1 for English, 2 for Français), then press **Enter** to confirm.

---

## Main Menu

Once the language is selected, you will access the main menu. Enter the number of your desired option, then press **Enter** to continue.

### 1. Create a backup job

Allows you to define a new backup job. **Warning**: It does not run automatically after creation; it is simply saved in your list! 

You will need to provide:
- **Job Name**: A unique name to identify the backup.
- **Source Directory (e.g., C:\Folder)**: The full path of the folder you want to back up.
- **Target Directory (e.g., D:\Backup)**: The full path where the files will be copied.
- **Type (0 = Full, 1 = Differential)**:
    - *0 = Full*: Copies all files from the source to the target upon each execution.
    - *1 = Differential*: Copies only new or modified files since the last backup.

A message confirming the successful creation will appear, then the EasySave menu will return.

> **Note**: You can create up to 5 backups. If this limit is reached, you must delete an existing job before creating a new one.

### 2. List backup jobs

Displays a numbered list of all your saved backup jobs in chronological order, with their name, type, and source/target directories.

### 3. Run a backup job

Starts the execution of your backup jobs. All files in the source folder will be copied to the target folder according to the defined backup type.

When prompted, you have several input options:
- **A single job**: Simply type its number (e.g., `2`).
- **Several specific jobs**: Separate the numbers with a semicolon (e.g., `1;3` will run job 1 then job 3).
- **A range of jobs**: Use a hyphen (e.g., `1-3` will run jobs 1, 2, and 3).
- **All jobs**: Type the word `all` to execute your entire list sequentially.

### 4. Delete a backup job

Allows you to remove a job from your list. Simply enter the index (number) of the job to delete. If you change your mind, you can cancel the operation by pressing the **q** key.

### 5. Settings (Log Format)

Allows you to change the log file format. Simply enter the number corresponding to the format you want (1 for JSON or 2 for XML). If it has already been entered, the current log format will be displayed. If you change your mind, you can cancel this operation by pressing the **q** key.

### 6. Exit

Safely closes the EasySave application.

---

## Tracking Files (Logs)

During and after your backups, EasySave automatically generates reports in the `EasyLogs` folder (located in the same place as your application):
- **Real-time tracking (`state.json`)**: This file updates in real-time and allows you to track the exact progress of the ongoing transfer.
- **Daily reports (`DailyLog_date.json`)**: Keeps a detailed history, transfer time, and size of each copied file on a day-to-day basis.