using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SFB;

[RequireComponent(typeof(Button))]
public class CanvasSampleSaveFileText : MonoBehaviour, IPointerDownHandler {
    public string Title = "";
    public string Directory = "";
    public string FileName = "";
    public string Extension = "";

    // Sample text data
    private string _data = "Example text created by StandaloneFileBrowser";

#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //
    [DllImport("__Internal")]
    private static extern void DownloadFile(string id, string filename, byte[] byteArray, int byteArraySize);

    // Broser plugin should be called in OnPointerDown.
    public void OnPointerDown(PointerEventData eventData) {
        var bytes = Encoding.UTF8.GetBytes(_data);
        DownloadFile(gameObject.name, FileName + "." + Extension, bytes, bytes.Length);
    }

    // Called from browser
    public void OnFileDownloaded() {
        //
    }
#else
    //
    // Standalone platforms & editor
    //
    public void OnPointerDown(PointerEventData eventData) { }

    // Listen OnClick event in standlone builds
    void Start() {
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    public void OnClick() {
        var path = StandaloneFileBrowser.SaveFilePanel(Title, Directory, FileName, Extension);
        if (!string.IsNullOrEmpty(path)) {
            File.WriteAllText(path, _data);
        }
    }
#endif
}