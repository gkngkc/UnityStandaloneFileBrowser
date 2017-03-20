namespace SFB {
    public interface IStandaloneFileBrowser {
        string[] OpenFilePanel(string title, string directory, ExtensionFilter[] extensions, bool multiselect);
        string[] OpenFolderPanel(string title, string directory, bool multiselect);
        string SaveFilePanel(string title, string directory, string defaultName, ExtensionFilter[] extensions);
    }
}
