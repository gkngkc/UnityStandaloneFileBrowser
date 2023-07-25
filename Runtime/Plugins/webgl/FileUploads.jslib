mergeInto(LibraryManager.library, {
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

        //Inject our required html input fields
        window.InjectHiddenFileInput = function InjectHiddenFileInput(inputFieldName, acceptedExtentions, multiFileSelect) { 
			
			//Make sure file extentions start with a dot ([.jpg,.png] instead of [jpg,png] etc)
			var acceptedExtentionsArray = acceptedExtentions.split(",");
			for(var i = 0; i<acceptedExtentionsArray.length;i++)
			{
				acceptedExtentionsArray[i] = "." + acceptedExtentionsArray[i].replace(".","");
			}
			
			var newInput = document.createElement("input");
            newInput.id = inputFieldName;
            newInput.type = 'file';
            newInput.accept = acceptedExtentionsArray.toString();
			newInput.multiple = multiFileSelect;
			newInput.onclick = function() {
				unityInstance.SendMessage(inputFieldName, 'ClickNativeButton');
			};
            newInput.onchange = function () {
                window.ReadFiles(this.files);
            };
            newInput.style.cssText = 'display:none; cursor:pointer; opacity: 0; position: fixed; bottom: 0; left: 0; z-index: 2; width: 0px; height: 0px;';
            document.body.appendChild(newInput);
        };

        window.InjectHiddenFileInput('geojson', '.json,.geojson', false);

        //Support for dragging dropping files on browser window
        document.addEventListener("dragover", function (event) {
            event.preventDefault();
        });

        document.addEventListener("drop", function (event) {
            console.log("File dropped");
            event.stopPropagation();
            event.preventDefault();
            // tell Unity how many files to expect
            window.ReadFiles(event.dataTransfer.files);
        });
		
		window.FileSaved = function FileSaved() {
			filesToSave = filesToSave - 1;
			if (filesToSave == 0) {
				window.databaseConnection.close();
			}
		};

        window.ClearInputs = function ClearInputs() {
            var inputs = document.getElementsByTagName('input');
            for (i = 0; i < inputs.length; ++i) {
                inputs[i].value = '';
            }
        };

        window.ReadFiles = function ReadFiles(SelectedFiles) {
            if (window.File && window.FileReader && window.FileList && window.Blob) {
                window.ConnectToDatabaseAndReadFiles(SelectedFiles);
                unityInstance.SendMessage('UserFileUploads', 'FileCount', SelectedFiles.length);
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
                unityInstance.SendMessage('UserFileUploads', 'LoadFile', filename);
                console.log("File saved: " + newIndexedFilePath);
                window.FileSaved();
            };
            dbRequest.onerror = function () {
                unityInstance.SendMessage('UserFileUploads', 'LoadFileError', filename);
                alert("Could not save: " + newIndexedFilePath);
                window.FileSaved();
            };
        };
    },
    UploadFromIndexedDB: function (filePath, targetURL, callbackObject, callbackMethodSuccess, callbackMethodFailed) {
		var callbackObjectString = UTF8ToString(callbackObject);	
		var callbackMethodSuccessString = UTF8ToString(callbackMethodSuccess);	
		var callbackMethodFailedString = UTF8ToString(callbackMethodFailed);	
		
		console.log("Set callback object to " + callbackObjectString);
		console.log("Set callback succeeded method to " + callbackMethodSuccessString);
		console.log("Set callback failed method to " + callbackMethodFailedString);
		
        var fileName = UTF8ToString(filePath);
        var url = UTF8ToString(targetURL);

        var dbConnectionRequest = window.indexedDB.open("/idbfs", window.dbVersion);
        dbConnectionRequest.onsuccess = function () {
            console.log("Connected to database");
            window.databaseConnection = dbConnectionRequest.result;

            var transaction = window.databaseConnection.transaction(["FILE_DATA"], "readonly");
            var indexedFilePath = window.databaseName + "/" + fileName;
            console.log("Uploading from IndexedDB file: " + indexedFilePath);

            var dbRequest = transaction.objectStore("FILE_DATA").get(indexedFilePath);
            dbRequest.onsuccess = function (e) {
                var record = e.target.result;
                var xhr = new XMLHttpRequest;
                xhr.open("PUT", url, false);
                xhr.send(record.contents);
                window.databaseConnection.close();
                unityInstance.SendMessage(callbackObjectString, callbackMethodSuccessString);
            };
            dbRequest.onerror = function () {
                window.databaseConnection.close();
                unityInstance.SendMessage(callbackObjectString, callbackMethodFailedString, filename);
            };
        }
        dbConnectionRequest.onerror = function () {
            alert("Kan geen verbinding maken met de indexedDatabase");
        }
    },
	DownloadFromIndexedDB: function (filePath, callbackObject, callbackMethod) {
        var fileNameString = UTF8ToString(filePath);	
		var callbackObjectString = UTF8ToString(callbackObject);	
		var callbackMethodString = UTF8ToString(callbackMethod);	
		
		console.log("Set callback object to " + callbackObjectString);
		console.log("Set callback method to " + callbackMethodString);
		
        var dbConnectionRequest = window.indexedDB.open("/idbfs", window.dbVersion);
        dbConnectionRequest.onsuccess = function () {
            console.log("Connected to database");
            window.databaseConnection = dbConnectionRequest.result;

            var transaction = window.databaseConnection.transaction(["FILE_DATA"], "readonly");
            var indexedFilePath = window.databaseName + "/" + fileNameString;
            console.log("Downloading from IndexedDB file: " + indexedFilePath);

            var dbRequest = transaction.objectStore("FILE_DATA").get(indexedFilePath);
            dbRequest.onsuccess = function (e) {                
                var blob = new Blob([e.target.result], { type: 'application/octetstream' });
				var url = window.URL.createObjectURL(blob);
				var onlyFileName = fileNameString.replace(/^.*[\\\/]/, '');
				const a = document.createElement("a");
			    a.href = url;
			    a.setAttribute("download", onlyFileName);
			    document.body.appendChild(a);
			    a.click();
				window.setTimeout(function() { 
				  window.URL.revokeObjectURL(url);
				  document.body.removeChild(a);
				  unityInstance.SendMessage(callbackObjectString, callbackMethodString, fileNameString);
				}, 0);
                window.databaseConnection.close();
            };
            dbRequest.onerror = function () {
                window.databaseConnection.close();
            };
        }
        dbConnectionRequest.onerror = function () {
            alert("Kan geen verbinding maken met de indexedDatabase");
        }
    },
	AddFileInput: function (inputName,fileExtentions,multiSelect) {
		var inputNameID = UTF8ToString(inputName);
        var allowedFileExtentions = UTF8ToString(fileExtentions);
		
		if (typeof window.InjectHiddenFileInput !== "undefined") { 
			window.InjectHiddenFileInput(inputNameID, allowedFileExtentions, multiSelect);
		}
		else{
			console.log("Cant create file inputfield. You need to initialize the IndexedDB connection first using InitializeIndexedDB(str)");
		}
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
    ClearFileInputFields: function () {
        window.ClearInputs();
    }
});