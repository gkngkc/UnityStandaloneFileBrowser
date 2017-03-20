using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SFB;

[RequireComponent(typeof(Button))]
public class CanvasSampleSaveFileImage : MonoBehaviour, IPointerDownHandler {
    public string Title = "";
    public string Directory = "";
    public string FileName = "";
    public string Extension = "";

    private byte[] _textureBytes;

    void Awake() {
        // Create red texture
        var width = 100;
        var height = 100;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                tex.SetPixel(i, j, Color.red);
            }
        }
        tex.Apply();
        _textureBytes = tex.EncodeToPNG();
        UnityEngine.Object.Destroy(tex);
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //
    [DllImport("__Internal")]
    private static extern void DownloadFile(string id, string filename, byte[] byteArray, int byteArraySize);

    // Broser plugin should be called in OnPointerDown.
    public void OnPointerDown(PointerEventData eventData) {
        DownloadFile(gameObject.name, FileName + "." + Extension, _textureBytes, _textureBytes.Length);
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
            File.WriteAllBytes(path, _textureBytes);
        }
    }
#endif
}