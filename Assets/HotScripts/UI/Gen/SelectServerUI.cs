using UIFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI {

	public partial class SelectServerUI : BaseUi {

		[SerializeField]
		private RectTransform_Button_Image_Container m_EnterGame;
		public RectTransform_Button_Image_Container EnterGame { get { return m_EnterGame; } }

		[SerializeField]
		private RectTransform_Button_Image_Container m_Select;
		public RectTransform_Button_Image_Container Select { get { return m_Select; } }

		[SerializeField]
		private RectTransform_Text_Container m_ServerName;
		public RectTransform_Text_Container ServerName { get { return m_ServerName; } }

		[SerializeField]
		private RectTransform_Image_Container m_ServerState;
		public RectTransform_Image_Container ServerState { get { return m_ServerState; } }

		protected void ReleaseList() {
		}

	}

}
