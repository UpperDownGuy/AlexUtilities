# AlexUtilities

A colorful and interactive C# console application that provides a mix of useful **system utilities** and a few fun extras. Built to be lightweight, easy to run, and beginner-friendly.

## ✨ Features

### 🔧 Utilities

* **System Information** – Detailed overview including:

  * Device name & user name
  * Operating system (with version & build)
  * Last boot time
  * CPU (name, cores, clock speed)
  * GPU(s) with VRAM
  * RAM usage (total & free)
  * Storage devices (model & size)
  * Disk partitions (format, label, space usage)
  * USB devices currently connected
* **Disk Partitions** – Displays detailed info for each mounted drive
* **USB Devices** – Quick list of detected USB devices

### 🎮 Fun

* **Guessing Game** – A simple number guessing game for quick breaks

## ⚙️ Requirements

* Windows (with .NET 6.0+)
* Administrator privileges (for full system info access)

## 🚀 How to Run

```bash
# Clone the repo
git clone https://github.com/yourusername/AlexUtilities.git
cd AlexUtilities

# Build and run
dotnet run
```

When launched, the app will prompt for admin privileges if required.

## 📂 Project Structure

```
AlexUtilities/
 ├── Program.cs   # Main entry point with utilities and menu system
 └── README.md    # Project description
```

## 📜 License

This project is licensed under the MIT License. Free to use and modify.
