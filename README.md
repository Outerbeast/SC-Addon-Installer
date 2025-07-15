![alt text](https://github.com/Outerbeast/SC-Addon-Installer/blob/main/preview.png?raw=true)
# Sven Co-op Addon Installer

A simple addon installer for Sven Co-op, supporting various archive formats and direct downloads.

## Features

- Supports installation of Sven Co-op addons from archives and map files.
- Handles the following archive formats:
  - `.zip`, `.7z`, `.rar`, `.tar`, `.gz`, `.bz2`, `.xz`, `.zst`
- Handles the following map file types:
  - `.bsp`, `.cfg`, `.res`, `.wad`, `.fgd`
- Drag-and-drop support for adding files.
- Download and install addons directly from URLs.
- Simple, user-friendly Windows Forms interface.

## Installation
- Download the application from the [Releases](https://github.com/Outerbeast/SC-Addon-Installer/releases) section
- Run the exe for initial setup, this will search for your Sven Co-op game install

## Usage

1. **Add Addons:**
   - Drag and drop addon files or archives into the list, or use the "Add" button.
   - You can also add download URLs or a list file containing URLs via the "Add download" option.

2. **Set Install Path:**
   - Click the install path label to select your Sven Co-op Addon Directory.

3. **Install Addons:**
   - Click "Install" to extract/copy all listed addons to the selected directory.
   - The installer will overwrite existing files if necessary.

4. **Manage List:**
   - Use "Remove" to delete selected addons from the list.
   - Use "Clear" to remove all addons from the list.

## Building from Source

### Prerequisites

- [Download .NET SDK 9.0+](https://dotnet.microsoft.com/en-us/download)  
  Confirm it's installed by running:

```cmd
dotnet --version
```

### Build Instructions
1. [Download](https://github.com/Outerbeast/SC-Addon-Installer/archive/refs/heads/main.zip) or clone the repository:

```cmd
git clone https://github.com/Outerbeast/SC-Addon-Installer.git
cd SC-Addon-Installer
```
2. Run the build script:
- Double-click `build.cmd` or run it manually:
```
build.cmd
```

The executable will be generated in the current directory.

## License

See [LICENSE](LICENSE) for details.

## Feedback & Issues

If you have feedback or encounter issues, please open an issue on [GitHub Issues](https://github.com/Outerbeast/SC-Addon-Installer/issues).

---

Thank you for using Sven Co-op Addon Installer!
You are now ready to head on over to [SCMapDB](http://scmapdb.wikidot.com/) and start playing your favorite campaigns today.

### Credits
- **Outerbeast** - Author
- **Garompa** - Testing and feedback
- - **SharpCompress** - Archive handling library (MIT License)