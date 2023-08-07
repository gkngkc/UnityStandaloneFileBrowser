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
    [SerializeField] private string fileExtentions = "csv";

    [Tooltip("Allowed selection multiple files")]
    [SerializeField] private bool multiSelect = false;

    public UnityEvent<string> onFilesSelected = new();

#if !UNITY_EDITOR && UNITY_WEBGL
    private string fileInputName;
    private FileInputIndexedDB javaScriptFileInputHandler;

    void Start()
    {
        javaScriptFileInputHandler = FindObjectOfType<FileInputIndexedDB>(true);
        if (javaScriptFileInputHandler == null)
        {
            GameObject go = new GameObject("UserFileUploads");
            javaScriptFileInputHandler = go.AddComponent<FileInputIndexedDB>();
        }

        // Set file input name with generated id to avoid html conflicts
        fileInputName += "_" + gameObject.GetInstanceID();
        name = fileInputName;

        DrawHTMLOverCanvas javascriptInput = gameObject.AddComponent<DrawHTMLOverCanvas>();
        javascriptInput.SetupInput(fileInputName, fileExtentions, multiSelect);
        javascriptInput.AlignObjectID(fileInputName);
    }

    public void ClickNativeButton()
    {
        javaScriptFileInputHandler.SetCallbackAddress(SendResults);
    }
#else
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OpenFile);
    }

    public void OpenFile()
    {
        string[] fileExtentionNames = fileExtentions.Split(',');
        ExtensionFilter[] extentionfilters = new ExtensionFilter[1];

        extentionfilters[0] = new ExtensionFilter(fileExtentionNames[0], fileExtentionNames);

        string[] filenames = SFB.StandaloneFileBrowser.OpenFilePanel("select file(s)", "", extentionfilters, multiSelect);
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
        onFilesSelected.Invoke(filePaths);
    }
}
