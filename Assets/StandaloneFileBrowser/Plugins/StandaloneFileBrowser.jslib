var StandaloneFileBrowserWebGLPlugin = {
    // Open file.
    // gameObjectNamePtr: GameObject name required for calling back unity side with SendMessage. And it should be unique
    // filter(disabled): Filter files. Example filters:
    //     Match all image files: "image/*"
    //     Match all video files: "video/*"
    //     Match all audio files: "audio/*"
    //     Custom: ".plist,.xml,.yaml"
    // multiselect(disabled): Allows multiple file selection
    UploadFile: function(gameObjectNamePtr/*, filter, multiselect*/) {
        gameObjectName = Pointer_stringify(gameObjectNamePtr);

        // Delete if element exist
        var fileInput = document.getElementById(gameObjectName)
        if (fileInput) {
            document.body.removeChild(fileInput);
        }

        fileInput = document.createElement('input');
        fileInput.setAttribute('id', gameObjectName);
        fileInput.setAttribute('type', 'file');
        fileInput.setAttribute('style','display:none;');
        fileInput.setAttribute('style','visibility:hidden;');
        // if (multiselect) {
        //     fileInput.setAttribute('multiple', multiselect);
        // }
        // if (filter) {
        //     fileInput.setAttribute('accept', filter);
        // }
        fileInput.onclick = function (event) {
            // File dialog opened
            this.value = null;
        };
        fileInput.onchange = function (event) {
            // multiselect works
            // for (var i = 0; i < event.target.files.length; i++) {
            //     console.log(URL.createObjectURL(event.target.files[i]));
            // }
            // File selected
            SendMessage(gameObjectName, 'OnFileUploaded', URL.createObjectURL(event.target.files[0]));

            // Remove after file selected
            document.body.removeChild(fileInput);
        }
        document.body.appendChild(fileInput);

        document.onmouseup = function() {
            fileInput.click();
            document.onmouseup = null;
        }
    },

    // Open folder. - NOT IMPLEMENTED
    UploadFolder: function(gameObjectNamePtr) {
        gameObjectName = Pointer_stringify(gameObjectNamePtr);
        SendMessage(gameObjectName, 'OnFolderUploaded', '');
    },

    // Save file
    // DownloadFile method does not open SaveFileDialog like standalone builds, its just allows user to download file
    // gameObjectNamePtr: GameObject name required for calling back unity side with SendMessage. And it should be unique
    //     DownloadFile does not return any info, just calls 'OnFileDownloaded' without any parameter
    // filenamePtr: Filename with extension
    // byteArray: byte[]
    // byteArraySize: byte[].Length
    DownloadFile: function(gameObjectNamePtr, filenamePtr, byteArray, byteArraySize) {
        gameObjectName = Pointer_stringify(gameObjectNamePtr);
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

            SendMessage(gameObjectName, 'OnFileDownloaded');
        }
    }
};

mergeInto(LibraryManager.library, StandaloneFileBrowserWebGLPlugin);