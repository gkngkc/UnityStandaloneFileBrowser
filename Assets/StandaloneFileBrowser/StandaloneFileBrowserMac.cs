#if UNITY_STANDALONE_OSX

using System;
using System.Runtime.InteropServices;

namespace SFB {
    public class StandaloneFileBrowserMac : IStandaloneFileBrowser {
        [DllImport("StandaloneFileBrowser")]
        private static extern IntPtr DialogOpenFilePanel(string title, string directory, string extension, bool multiselect);
        [DllImport("StandaloneFileBrowser")]
        private static extern IntPtr DialogOpenFolderPanel(string title, string directory, bool multiselect);
        [DllImport("StandaloneFileBrowser")]
        private static extern IntPtr DialogSaveFilePanel(string title, string directory, string defaultName, string extension);

        public string[] OpenFilePanel(string title, string directory, ExtensionFilter[] extensions, bool multiselect) {
            var paths = Marshal.PtrToStringAnsi(DialogOpenFilePanel(title, directory, GetFilterFromFileExtensionList(extensions), multiselect));
            return paths.Split((char)28);
        }

        public string[] OpenFolderPanel(string title, string directory, bool multiselect) {
            var paths = Marshal.PtrToStringAnsi(DialogOpenFolderPanel(title, directory, multiselect));
            return paths.Split((char)28);
        }

        public string SaveFilePanel(string title, string directory, string defaultName, ExtensionFilter[] extensions) {
            return Marshal.PtrToStringAnsi(DialogSaveFilePanel(title, directory, defaultName, GetFilterFromFileExtensionList(extensions)));
        }

        private static string GetFilterFromFileExtensionList(ExtensionFilter[] extensions) {
            if (extensions == null) {
                return "";
            }

            var filterString = "";
            foreach (var filter in extensions) {
                filterString += filter.Name + ";";

                foreach (var ext in filter.Extensions) {
                    filterString += ext + ",";
                }

                filterString = filterString.Remove(filterString.Length - 1);
                filterString += "|";
            }
            filterString = filterString.Remove(filterString.Length - 1);
            return filterString;
        }
    }
}

#endif