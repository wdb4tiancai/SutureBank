using UIFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{

    public partial class MainUi : BaseScreen
    {

        [SerializeField]
        private RectTransform_Button_Image_Container m_Button1;
        public RectTransform_Button_Image_Container Button1 { get { return m_Button1; } }

        [SerializeField]
        private RectTransform_Button_Image_Container m_Button2;
        public RectTransform_Button_Image_Container Button2 { get { return m_Button2; } }

    }

}
