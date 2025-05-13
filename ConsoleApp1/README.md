# Veeam FolderSync

## One-Way Folder Synchronization(C#)

Veeam FolderSync is a C# console application that performs one-way synchronization from a _Source_ folder to a _Replica_ folder.  
It ensures the Replica folder always contains an exact copy of the Source folder — syncing additions, deletions, and modifications.

---

## Features

- Periodic synchronization (interval defined by user)
- One-way sync: Source → Replica
- Logs all operations (copy, delete) to both console and log file
- Avoids unnecessary copying when files haven't changed
- Runs via command-line with custom paths and interval
- No hardcoded folder paths or third-party sync libraries

---

## Usage

## Command-Line Syntax

FolderSync.exe "<source_path>" "<replica_path>" <interval_in_seconds> "<log_file_path>"

___

# Example
FolderSync.exe "C:\Users\shubh\Desktop\Test\Source" "C:\Users\shubh\Desktop\Test\Replica" 5 "C:\Users\shubh\Desktop\Test\log.txt"

What will this do?
- Sync every 5 seconds
- Keep Replica exactly matched with Source
- Log all actions to both console and log.txt

Each log entry includes a timestamp and operation type:

___

# Log example:
2025-05-13 17:45:20 [COPY] file.txt -> file.txt
2025-05-13 17:46:10 [DELETE] old_file.txt

___

# How to Test

- Build the solution in Visual Studio or using dotnet build
- Navigate to the output directory (e.g. /bin/Debug/net8.0/)
- Run the executable using your desired arguments
- Modify the Source folder: Add, remove, or edit files
- Wait for the sync interval
- Observe logs and confirm changes in Replica