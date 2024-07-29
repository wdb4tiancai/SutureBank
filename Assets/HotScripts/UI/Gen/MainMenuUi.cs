using UIFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI {

	public partial class MainMenuUi : BaseUi {

		[SerializeField]
		private RectTransform_Image_Container m_menu;
		public RectTransform_Image_Container menu { get { return m_menu; } }

		[SerializeField]
		private RectTransform_Container m_shop;
		public RectTransform_Container shop { get { return m_shop; } }

		[SerializeField]
		private RectTransform_Container m_weapon;
		public RectTransform_Container weapon { get { return m_weapon; } }

		[SerializeField]
		private RectTransform_Container m_combat;
		public RectTransform_Container combat { get { return m_combat; } }

		[SerializeField]
		private RectTransform_Container m_skill;
		public RectTransform_Container skill { get { return m_skill; } }

		[SerializeField]
		private RectTransform_Container m_home;
		public RectTransform_Container home { get { return m_home; } }

	}

}
