> **Boss Ox** / 2017.10.03  / Beijing @RUC

# CapCap

Capture full screen and save the image automatically by pressing hotkey, silently in background with no popup window or message box.



---

[TOC]

---

## Quick Start

- Launch the application.
- Press `Alt + W` to capture screen. It doesn't matter if the window is closed or minimized.
- Click `Menu` / `Open folder` menu or press `Ctrl + O` in application to open up the *Autosave* folder. You can see the results there.
- Press `Menu` / `Exit`  menu or press `Alt + F4` to exit the application.

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

4. Press `Alt + W` to capture screen.

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

and today is October 3rd, 2017. **Next Number** is 15. Then the next screenshot image file will be named as `CapImage_2017-10-3_15`, and its extensions.

> **Notice**
>
> Variables are case insensitive. It doesn't matter if you input `<Year>` or `<year>` or `<yEaR>`. They all mean the same.

### Other features

- Double click on a screenshot record to open the image with software registered as system default handler, if it still exists.
- Click `Menu` / `Open folder` menu or press `Ctrl + O` in application to open up the folder in Windows Explorer. You can see the results there.

## Notice

- For now, hotkey customization is not supported. The default hotkey is `Alt + W`.
- Existing files will be overwritten when saving images automatically. Please choose archive folder carefully. Backup in advance if needed.
- No exception handling coded. Use with caution.

## Future design

- Functionality
  - Custom Hotkey
  - Batch screenshot management
  - Archive folder management
  - Email feedback
- Improvement
  - Exception handling

## Update log

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