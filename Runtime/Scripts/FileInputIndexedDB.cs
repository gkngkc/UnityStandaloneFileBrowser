using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using UnityEngine.Events;

/// <summary>
/// This system handles the user file uploads.
/// They are moved into the IndexedDB so they can be streamread from Unity.
/// This avoids having to load the (large) amount of data in the Unity heap memory
/// </summary>
public class FileInputIndexedDB : MonoBehaviour
{
    [SerializeField] private string sendMessageObjectName = "UserFileUploads";

    public UnityEvent<string> filesImportedEvent;

    [DllImport("__Internal")]
    private static extern void InitializeIndexedDB(string dataPath);
    
    [DllImport("__Internal")]
    private static extern void SyncFilesFromIndexedDB(string callbackObject,string callbackMethod);

    [DllImport("__Internal")]
    private static extern void SyncFilesToIndexedDB(string callbackObject, string callbackMethod);

    private Action<string> callbackAddress;
    private List<string> filenames = new List<string>();
    private int numberOfFilesToLoad = 0;
    private int fileCount = 0;

    private void Awake()
    {
        //This name is required so .SendMessage can send messages back to this object from FileUploads.jslib
        this.gameObject.name = sendMessageObjectName;

#if !UNITY_EDITOR && UNITY_WEBGL
        InitializeIndexedDB(Application.persistentDataPath);
#endif
    }

    public void SetCallbackAddress(Action<string> callback)
    {
        Debug.Log("Callback set for FileInputIndexedDB");
        callbackAddress = callback;
    }

    // Called from javascript, the total number of files that are being loaded.
    public void FileCount(int count)
    {
        numberOfFilesToLoad = count;
        fileCount = 0;
        filenames = new List<string>();
        Debug.Log("expecting " + count + " files");

        StartCoroutine(WaitForFilesToBeLoaded());
    }

    //called from javascript
    public void LoadFile(string filename)
    {
        filenames.Add(filename);
        fileCount++;
        Debug.Log("received: " + filename);
    }

    // called from javascript
    public void LoadFileError(string name)
    {
        fileCount++;
        //LoadingScreen.Instance.Hide();
        Debug.Log("unable to load " + name);
    }

    // runs while javascript is busy saving files to indexedDB.
    IEnumerator WaitForFilesToBeLoaded()
    {
        while (fileCount < numberOfFilesToLoad)
        {
            yield return null;
        }
        numberOfFilesToLoad = 0;
        fileCount = 0;
        ProcessFiles();
    }

    public void ProcessFiles()
    {
        // start js-function to update the contents of application.persistentdatapath to match the contents of indexedDB.
        SyncFilesFromIndexedDB(this.gameObject.name, "IndexedDBUpdated");
    }

    public void IndexedDBUpdated() // called from SyncFilesFromIndexedDB
    {
        ProcessAllFiles();
    }

    void ProcessAllFiles()
    {
        var files = string.Join(",", filenames);
        if (callbackAddress == null)
        {
            Debug.Log("FileInputIndexedDB: No callback set. Using default file import event.");
            filesImportedEvent.Invoke(files);
        }
        else
        {
            callbackAddress(files);
            callbackAddress = null;
        }
    }

    public void IndexedDBSyncCompleted()
    {
        Debug.Log("Synced Unity file changes back to IndexedDB");
    }

    public void ClearDatabase(bool succes)
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        filenames.Clear();
        if (succes)
        {
            SyncFilesToIndexedDB(this.gameObject.name,"IndexedDBSyncCompleted");
        }
#endif
    }
}
