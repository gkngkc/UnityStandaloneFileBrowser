using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Netherlands3D.JavascriptConnection
{
	public class DrawHTMLOverCanvas : MonoBehaviour
	{
		[DllImport("__Internal")]
		private static extern void DisplayDOMObjectWithID(string id = "htmlID", string display = "none", float x = 0, float y = 0, float width = 0, float height = 0, float offsetX = 0, float offsetY = 0);


		[SerializeField]
		private string htmlObjectID = "";

		private Image image;

		[SerializeField]
		private bool alignEveryUpdate = false;

		private Rect screenSpaceRectangle;

		private void Awake()
		{
			image = GetComponent<Image>();
		}


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

		/// <summary>
		/// Tell JavaScript to make a DOM object with htmlObjectID to align with the Image component
		/// </summary>
		private void AlignHTMLOverlay()
		{
			var screenSpaceRectangle = GetScreenSpaceRectangle();

			DisplayDOMObjectWithID(htmlObjectID, "inline",
				screenSpaceRectangle.x / Screen.width,
				screenSpaceRectangle.y / Screen.height,
				screenSpaceRectangle.width / Screen.width,
				screenSpaceRectangle.height / Screen.height
			);
		}

		/// <summary>
		/// Get the Image its rectangle in screenspace
		/// </summary>
		/// <returns>Rectangle with screenspace position, width and height</returns>
		private Rect GetScreenSpaceRectangle()
		{
			var size = Vector2.Scale(image.rectTransform.rect.size, image.rectTransform.lossyScale);
			screenSpaceRectangle = new Rect(image.rectTransform.position.x, image.rectTransform.position.y, size.x, size.y);
			screenSpaceRectangle.x -= (image.rectTransform.pivot.x * size.x);
			screenSpaceRectangle.y -= (image.rectTransform.pivot.y * size.y) + size.y;
			return screenSpaceRectangle;
		}
	}
}
