using UIFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI {

	public partial class LoginUi : BaseUi {

		[SerializeField]
		private RectTransform_Button_Image_Container m_Button;
		public RectTransform_Button_Image_Container Button { get { return m_Button; } }

	}

}
