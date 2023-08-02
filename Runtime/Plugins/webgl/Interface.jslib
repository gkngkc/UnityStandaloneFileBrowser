mergeInto(LibraryManager.library, {
    /*
    This method draws a hidden div, at the same location of our Unity canvas button.
    This way we can,for example, directly trigger the file upload dialog.
    Other indirect ways are blocked because of browser security.
    Canvas clicks caught by Unity are 'too late' for the browser to detect as a legitimate user action.
     */
    DisplayDOMObjectWithID: function (id, display, x, y, width, height, offsetX, offsetY) {
        var idString = UTF8ToString(id);
        var targetDomObject = document.getElementById(idString);
		var viewCanvas = document.getElementById("unity-canvas");
		
		var viewcanvasRect = viewCanvas.getBoundingClientRect();
		var left = viewcanvasRect.left;
		var bottom = viewcanvasRect.bottom;
		var canvasHeight = viewcanvasRect.height;
		var canvasWidth = viewcanvasRect.width

        if (targetDomObject != null) {
            targetDomObject.style.display = UTF8ToString(display);
            
            	targetDomObject.style.left=left+(x*canvasWidth)+"px";
		targetDomObject.style.top=bottom-((y+height)*canvasHeight)+"px";
		targetDomObject.style.width = (width)*canvasWidth + "px";
            targetDomObject.style.height = (height)*canvasHeight + "px";
        } else {
            console.log("Interface.jslib->DisplayDOMObjectWithID: Cant find DOM object with id: " + idString + ".");
        }
    },
    /*
    If the user changed the UI DPI in Unity, this method can be used to scale HTML elements with it.
     */
    ChangeInterfaceScale: function (scale) {
        //For example the fontsize of myDivname:
        //document.getElementById("myDivName").style.fontSize = scale + "em";
    }
});
