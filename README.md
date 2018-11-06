# Unity Standalone File Browser

A simple wrapper for native file dialogs on Windows/Mac/Linux.

- Works in editor and runtime.
- Open file/folder, save file dialogs supported.
- Multiple file selection.
- File extension filter.
- Mono/IL2CPP backends supported.
- Linux support by [Ricardo Rodrigues](https://github.com/RicardoEPRodrigues).
- Basic WebGL support.

[Download Package](https://github.com/gkngkc/UnityStandaloneFileBrowser/releases/download/1.2/StandaloneFileBrowser.unitypackage)

Example usage:

```csharp
// Open file
var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", "", false);

// Open file async
StandaloneFileBrowser.OpenFilePanelAsync("Open File", "", "", false, (string[] paths) => {  });

// Open file with filter
var extensions = new [] {
    new ExtensionFilter("Image Files", "png", "jpg", "jpeg" ),
    new ExtensionFilter("Sound Files", "mp3", "wav" ),
    new ExtensionFilter("All Files", "*" ),
};
var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, true);

// Save file
var path = StandaloneFileBrowser.SaveFilePanel("Save File", "", "", "");

// Save file async
StandaloneFileBrowser.SaveFilePanelAsync("Save File", "", "", "", (string path) => {  });

// Save file with filter
var extensionList = new [] {
    new ExtensionFilter("Binary", "bin"),
    new ExtensionFilter("Text", "txt"),
};
var path = StandaloneFileBrowser.SaveFilePanel("Save File", "", "MySaveFile", extensionList);
```
See Sample/BasicSampleScene.unity for more detailed examples.

Mac Screenshot
![Alt text](/Images/sfb_mac.jpg?raw=true "Mac")

Windows Screenshot
![Alt text](/Images/sfb_win.jpg?raw=true "Win")

Linux Screenshot
![Alt text](/Images/sfb_linux.jpg?raw=true "Win")

Notes:
- Windows
    * Requires .NET 2.0 api compatibility level 
    * Async dialog opening not implemented, ..Async methods simply calls regular sync methods.
    * Plugin import settings should be like this;
    
    ![Alt text](/Images/win_import_1.jpg?raw=true "Plugin Import Ookii") ![Alt text](/Images/win_import_2.jpg?raw=true "Plugin Import System.Forms")
    
- Mac
    * Sync calls are throws an exception at development build after native panel loses and gains focus. Use async calls to avoid this.

WebGL:
 - Basic upload/download file support.
 - File filter support.
 - Not well tested, probably not much reliable.
 - Since browsers require more work to do file operations, webgl isn't directly implemented to Open/Save calls. You can check CanvasSampleScene.unity and canvas sample scripts for example usages.
 
 Live Demo: https://gkngkc.github.io/
