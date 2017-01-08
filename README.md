# UnityStandaloneFileBrowser

A simple wrapper for native file dialogs.

Features:

- Open file/folder, save file dialogs supported.
- Multiple file selection.
- File type filter

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
