# Oveger Architecture

## Overview
Oveger is a WPF-based overlay application for managing file shortcuts and groups.

## Components

### Infrastructure (Scripts)
- **ConfigManager**: Handles JSON serialization/deserialization of user preferences and group assignments.
- **IconManager**: Manages icon extraction and caching.

### UI (Interfaces)
- **MainWindow**: The main overlay window that displays items and groups.
- **groupsWindow**: UI for creating and managing groups.
- **RightButtonClick**: Context menu for item actions.

## Data Flow
1. User adds a file shortcut.
2. `ConfigManager` saves the path to `config.json`.
3. `MainWindow` loads paths and queries `ConfigManager` for group assignments.
4. Cards are instantiated and placed in the corresponding `Expander` or `WrapPanel`.
