# 🚀 Oveger

A modern, high-performance file launcher and organizer for Windows. Oveger streamlines your workflow by providing instant access to your most-used applications, documents, and media through a sleek, customizable interface.

---

## 📺 Demo

https://github.com/ChickChuck2/Oveger/assets/48648882/4140bc81-2b3c-419f-b24d-147118cce3db

---

## ✨ Key Features

- ⚡ **Global Access**: Summon your launcher from anywhere with a customizable hotkey (Default: `Ctrl + Alt + S`).
- 📁 **Smart Organization**: Categorize your shortcuts into expandable groups for a clutter-free desktop.
- 🎬 **Rich Media Support**: Automatic thumbnail generation for videos (powered by FFmpeg) and instant image previews.
- 🛠️ **Full Customization**: Easily rename labels, update paths, and manage entries directly within the application.
- 🚀 **OS Integration**: Native system tray support, "Start with Windows" option, and quick access to file properties.
- 🎨 **Modern UI**: A clean, translucent interface with smooth animations and high-quality icon extraction.

---

## 🛠️ Tech Stack

Built with modern technologies to ensure performance and reliability:

- **Language**: C#
- **Framework**: WPF (.NET)
- **Data Persistence**: JSON (via Newtonsoft.Json)
- **Media Processing**: FFmpeg (via NReco.VideoConverter)
- **APIs**: Windows Win32 API for global hotkeys and shell integration.

---

## 🚀 Getting Started

1. **Launch**: Start `Oveger.exe`.
2. **Summon**: Use **Ctrl + Alt + S** to show or hide the launcher.
3. **Add**: Click the **"Adicionar Arquivo"** button to add your first shortcut.
4. **Organize**: Right-click any item to rename it, move it to a group, or change its path.
5. **Auto-Run**: Enable "Iniciar com Windows" from the system tray menu to have Oveger always ready.

---

## 📂 Project Structure

- `MainWindow.xaml.cs`: Core application logic and UI handling.
- `Scripts/`: Modular logic for configurations, icon management, and specialized windows.
- `Resources/`: Project assets including icons and UI elements.

---

Developed with ❤️ by **CyWoodsDev**.
