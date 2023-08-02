
using System.Runtime.InteropServices;

using UnityEngine;


namespace Netherlands3D.JavascriptConnection
{
	public class DrawHTMLOverCanvas : MonoBehaviour
	{
		[DllImport("__Internal")]
		private static extern void AddFileInput(string inputName, string fileExtentions, bool multiSelect);
		[DllImport("__Internal")]
		private static extern void DisplayDOMObjectWithID(string id = "htmlID", string display = "none", float x = 0, float y = 0, float width = 0, float height = 0, float offsetX = 0, float offsetY = 0);

		
		[SerializeField]
		private string htmlObjectID = "";

		

		[SerializeField]
		private bool alignEveryUpdate = true;

		

		


#if !UNITY_EDITOR && UNITY_WEBGL
		private void Update()
		{
			if (alignEveryUpdate)
				AlignHTMLOverlay();
		}
		private void OnEnable()
        {
            AlignHTMLOverlay();
        }
        private void OnDisable()
        {
            DisplayDOMObjectWithID(htmlObjectID,"none");
        }
#endif
		/// <summary>
		/// Sets the target html DOM id to follow.
		/// </summary>
		/// <param name="id">The ID (without #)</param>
		public void AlignObjectID(string id, bool alignEveryUpdate = true){
			htmlObjectID = id;
			this.alignEveryUpdate = alignEveryUpdate;
		}
		public void SetupInput(string fileInputName, string fileExtentions,bool multiSelect)
		{
			AddFileInput(fileInputName, fileExtentions, multiSelect);
		}

		/// <summary>
		/// Tell JavaScript to make a DOM object with htmlObjectID to align with the Image component
		/// </summary>
		private void AlignHTMLOverlay()
		{

			Canvas canvas = GetComponent<RectTransform>().root.GetComponent<Canvas>();
			float canvasheight = canvas.GetComponentInParent<RectTransform>().rect.height;
			float canvaswidth = canvas.GetComponentInParent<RectTransform>().rect.width;
			Vector3[] corners = new Vector3[4];
			GetComponent<RectTransform>().GetWorldCorners(corners);
			
			DisplayDOMObjectWithID(htmlObjectID, "inline",
				corners[0].x / canvaswidth,
				corners[0].y / canvasheight,
				(corners[2].x - corners[0].x) / canvaswidth,
				(corners[2].y - corners[0].y) / canvasheight
			); ; ; ;
			
		}

		
	}
}
