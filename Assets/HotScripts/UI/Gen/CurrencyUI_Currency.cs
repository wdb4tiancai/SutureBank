using UIFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI {

	public partial class CurrencyUI_Currency : MonoBehaviour {

		[SerializeField]
		private RectTransform_Image_Container m_icon;
		public RectTransform_Image_Container icon { get { return m_icon; } }

		[SerializeField]
		private RectTransform_Text_Container m_value;
		public RectTransform_Text_Container value { get { return m_value; } }

		[SerializeField]
		private RectTransform_Image_Container m_add;
		public RectTransform_Image_Container add { get { return m_add; } }

	}

}
