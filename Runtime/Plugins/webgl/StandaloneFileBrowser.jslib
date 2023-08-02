var StandaloneFileBrowserWebGLPlugin = {

	 InitializeIndexedDB: function (str) {
        window.databaseName = UTF8ToString(str);

        console.log("Database name: " + window.databaseName);

        window.selectedFiles = [];
        window.filesToSave = 0;
        window.counter = 0;
        window.databaseConnection = null;

        window.indexedDB = window.indexedDB || window.webkitIndexedDB || window.mozIndexedDB || window.OIndexedDB || window.msIndexedDB;
        window.IDBTransaction = window.IDBTransaction || window.webkitIDBTransaction || window.OIDBTransaction || window.msIDBTransaction;
        window.dbVersion = 21;

        
		
		window.FileSaved = function FileSaved() {
			filesToSave = filesToSave - 1;
			if (filesToSave == 0) {
				window.databaseConnection.close();
			}
		};

       

        window.ReadFiles = function ReadFiles(SelectedFiles) {
            if (window.File && window.FileReader && window.FileList && window.Blob) {
                window.ConnectToDatabaseAndReadFiles(SelectedFiles);
                SendMessage('UserFileUploads', 'FileCount', SelectedFiles.length);
            } else {
                alert("Bestanden inladen wordt helaas niet ondersteund door deze browser.");
            };
        };

        window.ConnectToDatabaseAndReadFiles = function ConnectToDatabase(SelectedFiles) {
            //Connect to database
			window.filesToSave = SelectedFiles.length;
			
            var dbConnectionRequest = window.indexedDB.open("/idbfs", window.dbVersion);
            dbConnectionRequest.onsuccess = function () {
                console.log("connected to database");
                window.databaseConnection = dbConnectionRequest.result;
                for (var i = 0; i < SelectedFiles.length; i++) {
                    window.ReadFile(SelectedFiles[i])
                };
            }
            dbConnectionRequest.onerror = function () {
                alert("Kan geen verbinding maken met de indexedDatabase");
            }
        };

        window.ReadFile = function ReadFile(file) {
            window.filereader = new FileReader();
            window.filereader.onload = function (e) {

            const uint8Array = new Uint8Array(e.target.result);
            window.SaveData(uint8Array, file.name);
            window.counter = counter + 1;
            };
            window.filereader.readAsArrayBuffer(file);
        };
        
        window.SaveData = function SaveData(uint8Array, filename) {

            var data = {
                timestamp: new Date(),
                mode: 33206,
                contents: uint8Array
            };

            var transaction = window.databaseConnection.transaction(["FILE_DATA"], "readwrite");
            var newIndexedFilePath = window.databaseName + "/" + filename;
            var dbRequest = transaction.objectStore("FILE_DATA").put(data, newIndexedFilePath);
            
            console.log("Saving file: " + newIndexedFilePath);
            dbRequest.onsuccess = function () {
                SendMessage('UserFileUploads', 'LoadFile', filename);
                console.log("File saved: " + newIndexedFilePath);
                window.FileSaved();
            };
            dbRequest.onerror = function () {
                SendMessage('UserFileUploads', 'LoadFileError', filename);
                alert("Could not save: " + newIndexedFilePath);
                window.FileSaved();
            };
        };
    },

 SyncFilesFromIndexedDB: function (callbackObject, callbackMethod) {
		var callbackObjectString = UTF8ToString(callbackObject);
		var callbackMethodString = UTF8ToString(callbackMethod);	
		console.log("Set callback object to " + callbackObjectString);
		console.log("Set callback method to " + callbackMethodString);
		
        FS.syncfs(true, function (err) {
            if(err != null){
				console.log(err);
			}
            SendMessage(callbackObjectString, callbackMethodString);
        });
    },
    SyncFilesToIndexedDB: function (callbackObject, callbackMethod) {
		var callbackObjectString = UTF8ToString(callbackObject);
		var callbackMethodString = UTF8ToString(callbackMethod);	
		console.log("Set callback object to " + callbackObjectString);
		console.log("Set callback method to " + callbackMethodString);
	
        FS.syncfs(false, function (err) {
			if(err != null){
				console.log(err);
			}
            SendMessage(callbackObjectString, callbackMethodString);
        });
    },


    // Open file.
    // gameObjectNamePtr: Unique GameObject name. Required for calling back unity with SendMessage.
    // methodNamePtr: Callback method name on given GameObject.
    // filter: Filter files. Example filters:
    //     Match all image files: "image/*"
    //     Match all video files: "video/*"
    //     Match all audio files: "audio/*"
    //     Custom: ".plist, .xml, .yaml"
    // multiselect: Allows multiple file selection
    UploadFile: function(gameObjectNamePtr, methodNamePtr, filterPtr, multiselect) {
        gameObjectName = Pointer_stringify(gameObjectNamePtr);
        methodName = Pointer_stringify(methodNamePtr);
        filter = Pointer_stringify(filterPtr);

        // Delete if element exist
        var fileInput = document.getElementById("fileselect")
        if (fileInput) {
            document.body.removeChild(fileInput);
        }

        fileInput = document.createElement('input');
        fileInput.setAttribute('id', "fileselect");
        fileInput.setAttribute('type', 'file');
        fileInput.setAttribute('style','visibility:hidden;');
        if (multiselect) {
            fileInput.setAttribute('multiple', '');
        }
        if (filter) {
            fileInput.setAttribute('accept', filter);
        }
        fileInput.onclick = function (event) {
            // File dialog opened
            this.value = null;
        };
        fileInput.onchange = function (event) {
            // multiselect works
            var urls = [];
		ReadFiles(urls);
            

            // Remove after file selected
           // document.body.removeChild(fileInput);
        }
        document.body.appendChild(fileInput);

        document.onmouseup = function() {

            fileInput.click();

            document.onmouseup = null;
        }
    },

    // Save file
    // DownloadFile method does not open SaveFileDialog like standalone builds, its just allows user to download file
    // gameObjectNamePtr: Unique GameObject name. Required for calling back unity with SendMessage.
    // methodNamePtr: Callback method name on given GameObject.
    // filenamePtr: Filename with extension
    // byteArray: byte[]
    // byteArraySize: byte[].Length
    DownloadFile: function(gameObjectNamePtr, methodNamePtr, filenamePtr, byteArray, byteArraySize) {
        gameObjectName = Pointer_stringify(gameObjectNamePtr);
        methodName = Pointer_stringify(methodNamePtr);
        filename = Pointer_stringify(filenamePtr);

        var bytes = new Uint8Array(byteArraySize);
        for (var i = 0; i < byteArraySize; i++) {
            bytes[i] = HEAPU8[byteArray + i];
        }

        var downloader = window.document.createElement('a');
        downloader.setAttribute('id', gameObjectName);
        downloader.href = window.URL.createObjectURL(new Blob([bytes], { type: 'application/octet-stream' }));
        downloader.download = filename;
        document.body.appendChild(downloader);

        document.onmouseup = function() {
            downloader.click();
            document.body.removeChild(downloader);
        	document.onmouseup = null;

            SendMessage(gameObjectName, methodName);
        }
    }
};

mergeInto(LibraryManager.library, StandaloneFileBrowserWebGLPlugin);