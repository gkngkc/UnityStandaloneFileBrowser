/* Copyright(C)  X Gemeente
                 X Amsterdam
                 X Economic Services Departments
Licensed under the EUPL, Version 1.2 or later (the "License");
You may not use this work except in compliance with the License. You may obtain a copy of the License at:
https://joinup.ec.europa.eu/software/page/eupl
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
implied. See the License for the specific language governing permissions and limitations under the License.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using UnityEngine.Events;
//using Netherlands3D.ModelParsing;
//using Netherlands3D.Traffic.VISSIM;
//using Netherlands3D.Interface;
//using Netherlands3D.Events;


/// <summary>
/// This system handles the user file uploads.
/// They are moved into the IndexedDB so they can be streamread from Unity.
/// This avoids having to load the (large) amount of data in the Unity heap memory
/// </summary>
public class FileInputIndexedDB : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void InitializeIndexedDB(string dataPath);
    [DllImport("__Internal")]
    private static extern void SyncFilesFromIndexedDB(string callbackObject,string callbackMethod);
    [DllImport("__Internal")]
    private static extern void SyncFilesToIndexedDB(string callbackObject, string callbackMethod);

    

    //[DllImport("__Internal")]
    //private static extern void ClearFileInputFields();
    Action<string> callbackAdress;
    private List<string> filenames = new List<string>();
    private int numberOfFilesToLoad = 0;
    private int fileCount = 0;

    //[SerializeField]
    private UnityEvent<string> filesImportedEvent;

    [SerializeField]
    //private BoolEvent clearDataBaseEvent;

    private string sendMessageObjectName = "UserFileUploads";

    private void Awake()
    {
        //This name is required so .SendMessage can send messages back to this object from FileUploads.jslib
        this.gameObject.name = sendMessageObjectName;

        //if (clearDataBaseEvent)
        //    clearDataBaseEvent.AddListenerStarted(ClearDatabase);

#if !UNITY_EDITOR && UNITY_WEBGL
        InitializeIndexedDB(Application.persistentDataPath);
#endif
    }

    public void SetCallbackAdress(Action<string> callback)
    {
        Debug.Log("Callback set for FileInputIndexedDB");
        callbackAdress = callback;
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
        //ClearFileInputFields();
        ProcessAllFiles();
    }

    void ProcessAllFiles()
    {
        //LoadingScreen.Instance.Hide();

        var files = string.Join(",", filenames);
        if (callbackAdress == null)
        {
            Debug.Log("FileInputIndexedDB: No callback set. Using default file import event.");
            filesImportedEvent.Invoke(files);
        }
        else
        {
            callbackAdress(files);
            callbackAdress = null;
        }
    }

    public void IndexedDBSyncCompleted()
    {
        Debug.Log("Synced Unity file changes back to IndexedDB");
    }

    public void ClearDatabase(bool succes)
    {
#if !UNITY_EDITOR && UNITY_WEBGL
       // ClearFileInputFields();
        filenames.Clear();
        if (succes)
        {
            SyncFilesToIndexedDB(this.gameObject.name,"IndexedDBSyncCompleted");
        }
#endif
    }
}
