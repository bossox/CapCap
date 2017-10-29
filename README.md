> **Boss Ox** / 2017.10.29  / Beijing @RUC

# CapCap

Capture full screen and save the image automatically by pressing hotkey, silently in background with no popup window or message box.

---

Latest Version: 1.3

System Requirement: Windows NT, .NET 4.5.2 and above, 2 MB HDD

---

[TOC]

---

## Quick Start

- Launch the application.
- Press `HotKey` (`Alt + W` by default) to capture screen. It doesn't matter if the window is closed or minimized.
- Click `Menu` / `Open folder` menu or press `Ctrl + O` in application to open up the *Autosave* folder. You can see the results there.
- Press `Menu` / `Exit`  menu to exit the application.

## Instruction

### Screen capturing with default settings

Use this mode when you need an emergent screenshot, no settings required on program launch. Simply press the hotkey will do. Screenshots will be saved automatically in a temporary folder named *Autosave* at the same place where CapCap.exe exists. Image files will be named by time and order. Files in history will not be deleted.

For detailed procedures, please refer to **Quick Start** section.

### Screen capturing with custom settings

This mode fits most of the circumstances. You can customize several settings before actually capturing screen, including screenshot content, notification method, naming method and archive folder, etc.

1. Launch the application.

2. Setup folder and name

   1. Click `Save to (Autosave)...` button and choose a folder. You may also create a new folder in the dialog box. 

      > **Notice**
      >
      > If the text on the button is red, it means the folder is somewhat unavailable, and you need to choose another one.

   2. Input **Name Pattern** in the long textbox. This is how the screenshot files are named by.

      > **Notice**
      >
      > If the textbox turns red, it means the text you input is invalid as a file name. You need to try fix the error.
      >
      > Details about name pattern, please refer to **Name Pattern** section.

   3. Input **Next Number** in the short textbox. This indicates the next value of `<#>` variable in **Name Pattern**.

      > **Notice**
      >
      > if the textbox turns red, it means the text you input is invalid as a number. You need to input a non-negative integer in it.

3. Setup other settings

   1. Check `Settings` / `Cursor` menu to capture cursor in the screenshot image.

   2. Check `Settings` / `Notification` menu to send system notification after capture.

   3. Check `Settings` / `Sound` menu to play informing tone after capture.

   4. Check `Settings` / `BMP` menu (or `GIF`, `JPEG`, `PNG`, `TIFF`) to choose which image format to save.

   5. Check `Settings` / `Overwrite` menu to force overwrite file that already exists. If you don't want to do this, check `Settings` / `Rename` menu to save file with an additional number attached to the original filename, such as `CapImg_01 (2).JPEG`, where "(2)" is a automatically incremented number if the filename still exists, till the world ends.

   6. Click `Alt + W` button to customize hotkey. Supported key combinations are listed below.

      Control Keys: Control, Shift, Alt.

      Letter Keys: A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z.

      Number Keys: 1, 2, 3, 4, 5, 6, 7, 8, 9, 0.

      Function Keys: F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12.

      Rules: Only one key from LetteryKey, NumberKey and FunctionKey must exists. Any combinations of ControlKey can be added.

      Example: W, Control + W, Shift + W, Alt + W, Control + Shift + W, Control + Alt + W, Control + Shift + Alt + W.

      > **Notice**
      >
      > If the text turns red. It means the key combination you are pressing is unavailable. Maybe it's because the hotkey is already registered by another application, or the hotkey is preserved by the operation system. You then need to press another key combination.
      >
      > Press `Escape` key or `Enter` key to cancel the customization process and return.
      >
      > Any other keys will be ignored.

4. Press `HotKey` to capture screen.

5. Exit the application.

### Name Pattern

Name Pattern a line of text describing how a file is named when the screenshot is being saved. By default, Windows does not allow using following characters in file name: `\`, `/`, `|`, `<`, `>`, `?`, `:`, `*`, `"`.

In version Beta 2, we introduce **Name Variable** for an advanced level of naming logic. A Name Variable is a specific predefined string enclosed by a pair of angle brackets (`<`, `>`). The software will replace the string with the real value indicated by that string when a new file name is generated.

Supported variables are listed in the following table.

|  Variable  | Meaning                              | Example |
| :--------: | ------------------------------------ | :-----: |
|  \<Year\>  | Number of current year               |  2017   |
| \<Month\>  | Number of current month              |   10    |
|  \<Day\>   | Number of current day                |    3    |
|  \<Week\>  | Current day of week                  | Tuesday |
|  \<Hour\>  | Number of current hour               |   21    |
| \<Minute\> | Number of current minute             |   47    |
| \<Second\> | Number of current second             |   32    |
|   \<#\>    | Number showing in the second textbox |    1    |

For example, if a **Name Pattern** text is

```html
CapImage_<Year>-<Month>-<Day>_<#>
```

and today is October 3rd, 2017. **Next Number** (#) is 15. Then the next screenshot image file will be named as `CapImage_2017-10-3_15`, and its extensions.

> **Notice**
>
> Variables are case insensitive. It doesn't matter if you input `<Year>` or `<year>` or `<yEaR>`. They all mean the same.

In version 1.0, we've added a drop-down menu to help you insert supported variables quickly. Simply click the triangle button beside the long textbox, and choose a menu item. The variable will be inserted to the name pattern textbox automatically.

In version 1.2, we've added an advanced syntax for `<#>`. Now you can input a number before `#` to set zero-padding format. For example, variable `<3#>` will be translated into `001`, when `#` is 1. Negative number is not allowed.

### Other features

- Double click on a screenshot record to open the image with software registered as system default handler, if it still exists.
- Click `Menu` / `Open folder` menu or press `Ctrl + O` in application to open up the folder in Windows Explorer. You can see the results there.
- If you do not want to see the RUC80 anniversary logo on startup, uncheck `Menu` / `RUC80 welcome` menu.
- You can change the UI language by selecting different language in `Settings` / `Language` menu. Currently, EN-US and ZH-CN are supported.
- You can clear the history list by clicking `Menu` / `Clear history` menu. Actual image files will not be affected.
- All settings will be saved and loaded completely automatically.

## Notice

- By default, the `HotKey`  is `Alt + W`. You can customize it by clicking `Alt + W` button and pressing new hotkey combinations.

## Future design

- Functionality
  - New screen capture method: when mouse moves to a specific area. ★
  - Batch screenshot management.
  - Archive folder management.
- Improvement
  - Exception handling.
- Fix
  - Localization of ToolTips, and history list.
  - Cannot overwrite file when occupied.

## Update log

#### 2017.10.29 / 1.3

- **Add:** Beautiful icons all around the software.
- **Improve:** Compression quality of JPEG file raised to 96%.
- **Improve:** ZH-CN as default language.
- **Improve:** More options in popup menu from tray.
- **Fix:** Unavailable shortcut key written in popup menu from tray.
- **Fix:** Inaccuracy information in About panel.

#### 2017.10.12 / 1.2

- **Add:** A menu item to clear the history list.
- **Add:** Localization support, EN-US and ZH-CN.
- **Fix:** Width inaccuracy of column *Time*.

#### 2017.10.12 / 1.1

- **Add:** Rename and Overwrite option when filename already exists.
- **Improve:** Zero Padding for numbers (#).

#### 2017.10.08 / 1.0

- **Improve:** several UI details.
- **Add:** hotkey customization.
- **Add:** variable insertion menu.
- **Fix:** hotkey conflict on startup.

#### 2017.10.03 / Beta

- **Improve:** multithread screen capture.
- **Improve:** code refactoring.
- **Improve:** user experience optimization (UI, logic).
- **Improve:** use different audio feedbacks to distinguish success from fail.
- **Improve:** automatically save and load settings.
- **Improve:** record exception detail.
- **Add:** multiple keyboard shortcuts.
- **Add:** naming variable for custom name pattern.
- **Add:** several new image formats for saving.
- **Add:** 80th anniversary logo of Renmin University of China.
- **Add:** instruction document (help).
- **Add:** author's Weibo link ([@BO-SoftwareService](http://weibo.com/bosoftwareservice))

#### 2017.09.29 / Beta

- Initial release。

# Author

I'm now a graduate student at Renmin University of China, master of Webonomics. I love programming as well as making useful tools for myself and friends. This is what I do to kill time.

I'm no professional programmer, so there may exist many bugs waiting to be discovered and fixed in my software. Hopefully, you'll be okay with that.

Feel free to contact me by

- Weibo.com: [@BO-SoftwareService](Http://weibo.com/bosoftwareservice)
- E-mail: [BO_SoftwareService@yeah.net](mailto://bo_softwareservice@yeah.net)
- GitHub Repository: [CapCap](http://github.com/bossox/capcap)
- GitHub: [BossOx](http://github.com/bossox)

> By the way, the RUC80 theme is added as a ceremony of the 80th anniversary of my mother school, on 2017.10.03.

Thanks.

---

Boss Ox