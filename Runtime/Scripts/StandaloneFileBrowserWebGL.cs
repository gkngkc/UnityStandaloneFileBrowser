using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SFB
{
    public class StandaloneFileBrowserWebGL : IStandaloneFileBrowser
    {
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void UploadFile(string gameobjectname, string methodname, string filter, bool multiselect);

        FileInputIndexedDB indexedDBConnection;

        

        public string[] OpenFilePanel(string title, string directory, ExtensionFilter[] extensions, bool multiselect)
        {
            throw new NotImplementedException();
        }

        public void OpenFilePanelAsync(string title, string directory, ExtensionFilter[] extensions, bool multiselect, Action<string[]> cb)
        {
            throw new NotImplementedException();
        }

        public string[] OpenFolderPanel(string title, string directory, bool multiselect)
        {
            throw new NotImplementedException();
        }

        public void OpenFolderPanelAsync(string title, string directory, bool multiselect, Action<string[]> cb)
        {
            throw new NotImplementedException();
        }

        public string SaveFilePanel(string title, string directory, string defaultName, ExtensionFilter[] extensions)
        {
            throw new NotImplementedException();
        }

        public void SaveFilePanelAsync(string title, string directory, string defaultName, ExtensionFilter[] extensions, Action<string> cb)
        {
            throw new NotImplementedException();
        }
    }
}
