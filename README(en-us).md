> **Boss Ox** / 2017.09.29  / Beijing @RUC

# CapCap

Capture full screen and save the image automatically by pressing hotkey, silently in background with no popup window or message box.

## Instruction

### Screen capturing with default settings

Use this mode when you need an emergent screenshot, no settings required on program launch. Simply press the hotkey will do. Screenshots will be saved automatically in a temporary folder named *Autosave* at the same place where CapCap.exe exists. Image files will be named by time and order. Files in history will not be deleted.

1. Launch the application.
2. Press the `Hotkey` to capture screen.
3. Exit the application.

### Screen capturing with custom settings

This mode fits most of the circumstances. You can customize several settings before actually capturing screen, including screenshot content, notification method, naming method and archive folder, etc.

1. Launch the application.
2. Check `Cursor` to capture cursor in the screenshot image.
3. Check `Notification` to send system notification after capturing.
4. Check `Sound` to play a informing tone after capturing.
5. Input the prefix of the auto-saved files' name in the `First TextBox`.
6. Input the number of the next auto-saved file's name in the `Second TextBox`.
7. Click `Autosave` button to choose archive folder for auto-saving.
8. Press the `Hotkey` to capture screen.
9. Exit the application.

### Other features

- Double click a screenshot record to open the image in software registered as system default handler, if the file still exists.
- Click menu `Menu` / `Open folder` to open the archive folder in Windows Explorer.

## Notice

- For now, hotkey customization is not supported. The default hotkey is `Alt + W`.
- Existing files will be overwritten when saving images automatically. Please choose archive folder carefully. Backup in advance if needed.
- No exception handling coded. Use with caution.

## Future design

- Functionality
  - Custom Hotkey
  - Custom naming method
  - Batch screenshot management
  - Archive folder management
  - Email feedback
- Improvement
  - Source code splitting and refactoring
  - Asynchronous processing
  - Exception handling
  - Garbage collecting
- Other
  - What about a hidden Easter egg?

## Update log

#### 2017.09.29 / Beta

- Initial release。