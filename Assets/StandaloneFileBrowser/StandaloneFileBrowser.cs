using System;
using UnityEngine;

namespace SFB {
    public struct ExtensionFilter {
        public string Name;
        public string[] Extensions;

        public ExtensionFilter(string filterName, params string[] filterExtensions) {
            Name = filterName;
            Extensions = filterExtensions;
        }
    }

    public class StandaloneFileBrowser {
        private static IStandaloneFileBrowser _platformWrapper = null;

        static StandaloneFileBrowser() {
#if UNITY_STANDALONE_OSX
            _platformWrapper = new StandaloneFileBrowserMac();
#elif UNITY_STANDALONE_WIN
            _platformWrapper = new StandaloneFileBrowserWindows();
#elif UNITY_STANDALONE_LINUX
            _platformWrapper = new StandaloneFileBrowserLinux();
#elif UNITY_EDITOR
            _platformWrapper = new StandaloneFileBrowserEditor();
#endif
        }

        /// <summary>
        /// Native open file dialog
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="directory">Root directory</param>
        /// <param name="extension">Allowed extension</param>
        /// <param name="multiselect">Allow multiple file selection</param>
        /// <returns>Returns array of chosen paths. Zero length array when cancelled</returns>
        public static string[] OpenFilePanel(string title, string directory, string extension, bool multiselect) {
            SetWindowedFullscreenIfExclusive();
            
            var extensions = string.IsNullOrEmpty(extension) ? null : new [] { new ExtensionFilter("", extension) };
            var res = OpenFilePanel(title, directory, extensions, multiselect);

            SetExclusiveBackIfChanged();
            return res;
        }

        /// <summary>
        /// Native open file dialog
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="directory">Root directory</param>
        /// <param name="extensions">List of extension filters. Filter Example: new ExtensionFilter("Image Files", "jpg", "png")</param>
        /// <param name="multiselect">Allow multiple file selection</param>
        /// <returns>Returns array of chosen paths. Zero length array when cancelled</returns>
        public static string[] OpenFilePanel(string title, string directory, ExtensionFilter[] extensions, bool multiselect) {
            SetWindowedFullscreenIfExclusive();
            
            var res = _platformWrapper.OpenFilePanel(title, directory, extensions, multiselect);

            SetExclusiveBackIfChanged();
            return res;
        }

        /// <summary>
        /// Native open file dialog async
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="directory">Root directory</param>
        /// <param name="extension">Allowed extension</param>
        /// <param name="multiselect">Allow multiple file selection</param>
        /// <param name="cb">Callback")</param>
        public static void OpenFilePanelAsync(string title, string directory, string extension, bool multiselect, Action<string[]> originalCb) {
            SetWindowedFullscreenIfExclusive();
            Action<string[]> cb = (string[] strs) =>
            {
                SetExclusiveBackIfChanged();
                originalCb.Invoke(strs);
            };

            var extensions = string.IsNullOrEmpty(extension) ? null : new [] { new ExtensionFilter("", extension) };
            OpenFilePanelAsync(title, directory, extensions, multiselect, cb);
        }

        /// <summary>
        /// Native open file dialog async
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="directory">Root directory</param>
        /// <param name="extensions">List of extension filters. Filter Example: new ExtensionFilter("Image Files", "jpg", "png")</param>
        /// <param name="multiselect">Allow multiple file selection</param>
        /// <param name="cb">Callback")</param>
        public static void OpenFilePanelAsync(string title, string directory, ExtensionFilter[] extensions, bool multiselect, Action<string[]> originalCb) {
            SetWindowedFullscreenIfExclusive();
            Action<string[]> cb = (string[] strs) =>
            {
                SetExclusiveBackIfChanged();
                originalCb.Invoke(strs);
            };

            _platformWrapper.OpenFilePanelAsync(title, directory, extensions, multiselect, cb);
        }

        /// <summary>
        /// Native open folder dialog
        /// NOTE: Multiple folder selection doesn't supported on Windows
        /// </summary>
        /// <param name="title"></param>
        /// <param name="directory">Root directory</param>
        /// <param name="multiselect"></param>
        /// <returns>Returns array of chosen paths. Zero length array when cancelled</returns>
        public static string[] OpenFolderPanel(string title, string directory, bool multiselect) {
            SetWindowedFullscreenIfExclusive();

            var res = _platformWrapper.OpenFolderPanel(title, directory, multiselect);

            SetExclusiveBackIfChanged();
            return res;
        }

        /// <summary>
        /// Native open folder dialog async
        /// NOTE: Multiple folder selection doesn't supported on Windows
        /// </summary>
        /// <param name="title"></param>
        /// <param name="directory">Root directory</param>
        /// <param name="multiselect"></param>
        /// <param name="cb">Callback")</param>
        public static void OpenFolderPanelAsync(string title, string directory, bool multiselect, Action<string[]> originalCb) {
            SetWindowedFullscreenIfExclusive();
            Action<string[]> cb = (string[] strs) =>
            {
                SetExclusiveBackIfChanged();
                originalCb.Invoke(strs);
            };

            _platformWrapper.OpenFolderPanelAsync(title, directory, multiselect, cb);
        }

        /// <summary>
        /// Native save file dialog
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="directory">Root directory</param>
        /// <param name="defaultName">Default file name</param>
        /// <param name="extension">File extension</param>
        /// <returns>Returns chosen path. Empty string when cancelled</returns>
        public static string SaveFilePanel(string title, string directory, string defaultName , string extension) {
            SetWindowedFullscreenIfExclusive();

            var extensions = string.IsNullOrEmpty(extension) ? null : new [] { new ExtensionFilter("", extension) };
            var res = SaveFilePanel(title, directory, defaultName, extensions);

            SetExclusiveBackIfChanged();
            return res;
        }

        /// <summary>
        /// Native save file dialog
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="directory">Root directory</param>
        /// <param name="defaultName">Default file name</param>
        /// <param name="extensions">List of extension filters. Filter Example: new ExtensionFilter("Image Files", "jpg", "png")</param>
        /// <returns>Returns chosen path. Empty string when cancelled</returns>
        public static string SaveFilePanel(string title, string directory, string defaultName, ExtensionFilter[] extensions) {
            SetWindowedFullscreenIfExclusive();

            var res = _platformWrapper.SaveFilePanel(title, directory, defaultName, extensions);

            SetExclusiveBackIfChanged();
            return res;
        }

        /// <summary>
        /// Native save file dialog async
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="directory">Root directory</param>
        /// <param name="defaultName">Default file name</param>
        /// <param name="extension">File extension</param>
        /// <param name="cb">Callback")</param>
        public static void SaveFilePanelAsync(string title, string directory, string defaultName , string extension, Action<string> originalCb) {
            SetWindowedFullscreenIfExclusive();
            Action<string> cb = (string str) =>
            {
                SetExclusiveBackIfChanged();
                originalCb.Invoke(str);
            };

            var extensions = string.IsNullOrEmpty(extension) ? null : new [] { new ExtensionFilter("", extension) };
            SaveFilePanelAsync(title, directory, defaultName, extensions, cb);
        }

        /// <summary>
        /// Native save file dialog async
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="directory">Root directory</param>
        /// <param name="defaultName">Default file name</param>
        /// <param name="extensions">List of extension filters. Filter Example: new ExtensionFilter("Image Files", "jpg", "png")</param>
        /// <param name="cb">Callback")</param>
        public static void SaveFilePanelAsync(string title, string directory, string defaultName, ExtensionFilter[] extensions, Action<string> originalCb) {
            SetWindowedFullscreenIfExclusive();
            Action<string> cb = (string str) =>
            {
                SetExclusiveBackIfChanged();
                originalCb.Invoke(str);
            };

            _platformWrapper.SaveFilePanelAsync(title, directory, defaultName, extensions, cb);
        }

        private static bool didChangeFullscreenMode = false;
        private static void SetWindowedFullscreenIfExclusive()
        {
            if (Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen)
            {
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                didChangeFullscreenMode = true;
            }
            else
            {
                didChangeFullscreenMode = false;
            }
        }
        private static void SetExclusiveBackIfChanged()
        {
            if (didChangeFullscreenMode)
            {
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                didChangeFullscreenMode = false;
            }
        }
    }
}
