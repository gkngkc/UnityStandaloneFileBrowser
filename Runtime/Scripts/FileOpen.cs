using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using SFB;
using UnityEngine.Events;

#if !UNITY_EDITOR && UNITY_WEBGL
using Netherlands3D.JavascriptConnection;
#endif
public class FileOpen : MonoBehaviour
{

    [Tooltip("Allowed file input selections")]
    [SerializeField]
    private string fileExtentions = ".csv";

    [Tooltip("Allowed selection multiple files")]
    [SerializeField]
    private bool multiSelect = false;

    [SerializeField] private UnityEvent<string> OnFilesSelected;

#if !UNITY_EDITOR && UNITY_WEBGL
    private string fileInputName;
    FileInputIndexedDB javaScriptFileInputHandler;

    void Start()
    {
            javaScriptFileInputHandler = FindObjectOfType<FileInputIndexedDB>(true);
            if (javaScriptFileInputHandler == null)
            {
                GameObject go = new GameObject("UserFileUploads");
                javaScriptFileInputHandler = go.AddComponent<FileInputIndexedDB>();
            }
        // Set file input name with generated id to avoid html conflictions
        fileInputName += "_" + gameObject.GetInstanceID();
        name = fileInputName;

        DrawHTMLOverCanvas javascriptInput = gameObject.AddComponent<DrawHTMLOverCanvas>();
        javascriptInput.SetupInput(fileInputName, fileExtentions, multiSelect);
        javascriptInput.AlignObjectID(fileInputName);
    }


    public void ClickNativeButton()
    {
        javaScriptFileInputHandler.SetCallbackAdress(SendResults);
    }
#else
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(openfile);
    }

    public void openfile()
    {
        string[] filenames = SFB.StandaloneFileBrowser.OpenFilePanel("select file(s)", "", fileExtentions, multiSelect);
        string resultingFiles = "";
        for (int i = 0; i < filenames.Length; i++)
        {
            System.IO.File.Copy(filenames[i], System.IO.Path.Combine(Application.persistentDataPath, System.IO.Path.GetFileName(filenames[i])),true);
            resultingFiles += System.IO.Path.GetFileName(filenames[i])+ ",";
        }
        SendResults(resultingFiles);
    }
#endif
    public void SendResults(string filePaths)
    {
        Debug.Log("button received: " + filePaths);
        OnFilesSelected.Invoke(filePaths);
    }
}
