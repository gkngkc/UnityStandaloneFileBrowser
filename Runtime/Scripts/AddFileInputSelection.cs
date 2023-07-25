using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
#if UNITY_WEBGL
#endif
//using Netherlands3D.Events;
#if UNITY_STANDALONE || UNITY_EDITOR
using SFB;
#endif

namespace Netherlands3D.JavascriptConnection
{
    public class AddFileInputSelection : MonoBehaviour
    {
        [DllImport("__Internal")]
        private static extern void AddFileInput(string inputName, string fileExtentions, bool multiSelect);

        [Tooltip("HTML DOM ID")]
        [SerializeField]
        private string fileInputName = "fileInput";

        [Tooltip("Allowed file input selections")]
        [SerializeField]
        private string fileExtentions = ".csv";

        [Tooltip("Allowed selection multiple files")]
        [SerializeField]
        private bool multiSelect = false;
        private FileInputIndexedDB javaScriptFileInputHandler;
        private Button button;
        [SerializeField] private UnityEvent<string> eventFileLoaderFileImported;
        void Start()
        {
            button = GetComponent<Button>();

#if UNITY_WEBGL && !UNITY_EDITOR
            javaScriptFileInputHandler = FindObjectOfType<FileInputIndexedDB>(true);
            if (javaScriptFileInputHandler == null)
            {
                GameObject go = new GameObject("UserFileUploads");
                javaScriptFileInputHandler = go.AddComponent<FileInputIndexedDB>();
            }
            
            // Set file input name with generated id to avoid html conflictions
            fileInputName += "_" + gameObject.GetInstanceID();
            name = fileInputName;
            
            AddFileInput(fileInputName, fileExtentions, multiSelect);
            gameObject.AddComponent<DrawHTMLOverCanvas>().AlignObjectID(fileInputName);
#endif

            // Execute setup based on platform
            // Standalone
#if UNITY_STANDALONE && !UNITY_EDITOR
            button.onClick.AddListener(OnButtonClickUnityStandalone);
#endif

#if UNITY_EDITOR
            button.onClick.AddListener(OnButtonClickUnityEditor);
#endif
        }



#if UNITY_WEBGL && !UNITY_EDITOR

        /// <summary>
        /// If the click is registerd from the HTML overlay side, this method triggers the onClick events on the button
        /// </summary>
        public void ClickNativeButton()
        {
            if(button != null)
            {
                javaScriptFileInputHandler.SetCallbackAdress(SendResults);
                Debug.Log("Invoked native Unity button click event on " + this.gameObject.name);
                button.onClick.Invoke();
            }
        }
#endif

        // Standalone
#if UNITY_STANDALONE && !UNITY_EDITOR

        /// <summary>
        /// When the user clicks the button in standalone mode
        /// </summary>
        private void OnButtonClickUnityStandalone()
        {
            string[] result = StandaloneFileBrowser.OpenFilePanel("Select File", "", fileExtentions, multiSelect);
            if(result.Length != 0)
            {
                for (int i = 0; i < result.Length; i++)
                {
                    if (result[i]=="") continue;
                    string filename = System.IO.Path.GetFileName(result[i]);
                    string newFilepath = System.IO.Path.Combine(Application.persistentDataPath, filename);
                    result[i] = filename;
                    System.IO.File.Copy(result[i], newFilepath,true);
                }
                // Invoke the event with joined string values
                eventFileLoaderFileImported.Invoke(string.Join(",", result));
            }
        }
#endif

        // Unity Editor
#if UNITY_EDITOR

        /// <summary>
        /// When the user clicks the button in editor mode
        /// </summary>
        private void OnButtonClickUnityEditor()
        {
            string filePath = UnityEditor.EditorUtility.OpenFilePanel("Select File", "", fileExtentions);
            if (filePath.Length != 0)
            {
                string filename = System.IO.Path.GetFileName(filePath);
                string newFilePath = System.IO.Path.Combine(Application.persistentDataPath, filename);
                System.IO.File.Copy(filePath, newFilePath, true);
                UnityEngine.Debug.Log("[File Importer-unityEditor] Import file from file path: " + filename);
                eventFileLoaderFileImported.Invoke(newFilePath);
            }
        }
#endif

        // WebGL
        public void SendResults(string filePaths)
        {
            Debug.Log("button received: " + filePaths);
            eventFileLoaderFileImported.Invoke(filePaths);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Check if user didn't put a . in the file extention
            if (fileExtentions.Contains(".")) fileExtentions = fileExtentions.Replace(".", "");
        }
#endif
    }
}