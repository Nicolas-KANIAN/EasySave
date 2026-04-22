# User Manual

Welcome to EasySave, a modern, reliable, and high-performance backup solution developed by our team.

In this manual, you will find an overview of the application’s main features and how to use them.

## Quick Start

To launch the application, double-click `EasySave.exe`.

## Main Menu

When the application starts, choose the language you want to use to continue navigating through the application. To do this, enter the number corresponding to the desired language, then press **Enter** to confirm.

You will then access the main menu, which displays all available options:

- **1. Create a backup job**
- **2. List backup jobs**
- **3. Run a backup job**
- **4. Delete a job**
- **5. Exit**

Enter the number of the option you want, then press **Enter** to continue.

### 1. Create a backup job

This option lets you define a new backup job. Please note that it will not run automatically after creation.

You will need to provide:

- **Job name**: A unique name for the backup.
- **Source directory (e.g., C:\Folder)**: The full path of the folder to back up.
- **Target directory (e.g., D:\Backup)**: The full path where the files will be copied.
- **Type (0 = Full, 1 = Differential)**:
  - *0 = Full*: Copies all files every time.
  - *1 = Differential*: Copies only the files modified since the last full backup.

A message confirming that the job has been created will appear, and the EasySave menu will be displayed again.

> **Note**: You can create up to 5 backup jobs at the same time. If this limit is reached, you must delete an existing job before creating a new one.

### 2. List backup jobs

This option displays a numbered list of all backup jobs, in chronological order, with their name, type, source directory, and target directory.

### 3. Run a backup job

This option runs one or more backup jobs. All files in the source folder will be copied to the target folder according to the selected backup type. Activity logs will be updated in real time.

You can enter the index of the job or jobs you want to run, or simply type **All** to run them all.

### 4. Delete a job

This option lets you remove a job from the list. Simply select the job to delete using its index and confirm your choice. You can also cancel by pressing **q**.

### 5. Exit

This option lets you exit the backup software.