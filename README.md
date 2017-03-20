# Unity Standalone File Browser

A simple wrapper for native file dialogs on Windows/Mac.

- Works in editor and runtime.
- Open file/folder, save file dialogs supported.
- Multiple file selection.
- File extension filter.
- Windows version requires .NET 2.0 api compatibility level 
- Linux is not supported.
- Basic WebGL support.

Example usage:

```csharp
// Open file
var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", "", false);

// Open file with filter
var extensions = new [] {
    new ExtensionFilter("Image Files", "png", "jpg", "jpeg" ),
    new ExtensionFilter("Sound Files", "mp3", "wav" ),
    new ExtensionFilter("All Files", "*" ),
};
var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, true);

// Save file
var path = StandaloneFileBrowser.SaveFilePanel("Save File", "", "", "");

// Save file with filter
var extensionList = new [] {
    new ExtensionFilter("Binary", "bin"),
    new ExtensionFilter("Text", "txt"),
};
var path = StandaloneFileBrowser.SaveFilePanel("Save File", "", "MySaveFile", extensionList);
```
Look for Sample/BasicSampleScene.unity for more detailed examples.

Mac Screenshot
![Alt text](/Images/sfb_mac.jpg?raw=true "Mac")

Windows Screenshot
![Alt text](/Images/sfb_win.jpg?raw=true "Win")

WebGL:
 - Basic upload/download file support.
 - Not well tested, probably not much reliable.
 - Since browsers require more work to do file operations, webgl isn't directly implemented to Open/Save calls. You can check CanvasSampleScene.unity and canvas sample scripts for example usages.
 
 Live Demo: https://gkngkc.github.io/
