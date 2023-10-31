using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UIFramework
{

    [System.Serializable]
    public class Transform_Container
    {

        [SerializeField]
        private GameObject m_GameObject;
        public GameObject gameObject { get { return m_GameObject; } }

        [SerializeField]
        private Transform m_transform;
        public Transform transform { get { return m_transform; } }

        public void SetActive(bool val)
        {
            m_GameObject.SetActive(val);
        }
    }
    [System.Serializable]
    public class RectTransform_Container
    {

        [SerializeField]
        private GameObject m_GameObject;
        public GameObject gameObject { get { return m_GameObject; } }

        [SerializeField]
        private RectTransform m_rectTransform;
        public RectTransform rectTransform { get { return m_rectTransform; } }

        public void SetActive(bool val)
        {
            m_GameObject.SetActive(val);
        }
    }

    [System.Serializable]
    public class RectTransform_Image_Container
    {

        [SerializeField]
        private GameObject m_GameObject;
        public GameObject gameObject { get { return m_GameObject; } }

        [SerializeField]
        private RectTransform m_rectTransform;
        public RectTransform rectTransform { get { return m_rectTransform; } }

        [SerializeField]
        private Image m_image;
        public Image image { get { return m_image; } }

        public void SetSprite(Sprite sprite, bool withActive = false)
        {
        }

        public void SetSpriteLookActive(Sprite sprite)
        {
        }

        public void SetActive(bool val)
        {
            m_GameObject.SetActive(val);
        }

        public void SetFillAmount(double val, double max)
        {
        }

        public void SetAlphaA(float a)
        {
            var color = m_image.color;
            color.a = a;
            m_image.color = color;
        }

        public void SetColor(Color col)
        {
            m_image.color = col;
        }
    }

    [System.Serializable]
    public class RectTransform_RawImage_Container
    {

        [SerializeField]
        private GameObject m_GameObject;
        public GameObject gameObject { get { return m_GameObject; } }

        [SerializeField]
        private RectTransform m_rectTransform;
        public RectTransform rectTransform { get { return m_rectTransform; } }

        [SerializeField]
        private RawImage m_rawImage;
        public RawImage rawImage { get { return m_rawImage; } }


        public void SetActive(bool val)
        {
            m_GameObject.SetActive(val);
        }
    }

    [System.Serializable]
    public class RectTransform_Button_Image_Container
    {

        [SerializeField]
        private GameObject m_GameObject;
        public GameObject gameObject { get { return m_GameObject; } }

        [SerializeField]
        private RectTransform m_rectTransform;
        public RectTransform rectTransform { get { return m_rectTransform; } }

        [SerializeField]
        private Button m_button;
        public Button button { get { return m_button; } }

        [SerializeField]
        private Image m_image;
        public Image image { get { return m_image; } }
        public void RemoveOnClick()
        {
            m_button.onClick.RemoveAllListeners();
        }

        public void SetOnClick(UnityAction action)
        {
        }

        public void SetActive(bool val)
        {
            m_GameObject.SetActive(val);
        }

        public void SetSprite(Sprite sprite, bool withActive = false)
        {
        }

        public void SetAlphaA(float a)
        {
            var color = m_image.color;
            color.a = a;
            m_image.color = color;
        }
    }
    [System.Serializable]
    public class RectTransform_Text_Button_Container
    {
        [SerializeField]
        private GameObject m_GameObject;
        public GameObject gameObject { get { return m_GameObject; } }

        [SerializeField]
        private RectTransform m_rectTransform;
        public RectTransform rectTransform { get { return m_rectTransform; } }

        [SerializeField]
        private Text m_text;
        public Text text { get { return m_text; } }

        [SerializeField]
        private Button m_button;
        public Button button { get { return m_button; } }

    }
    [System.Serializable]
    public class RectTransform_Text_Container
    {

        [SerializeField]
        private GameObject m_GameObject;
        public GameObject gameObject { get { return m_GameObject; } }

        [SerializeField]
        private RectTransform m_rectTransform;
        public RectTransform rectTransform { get { return m_rectTransform; } }

        [SerializeField]
        private Text m_text;
        public Text text { get { return m_text; } }

        public void SetActive(bool val)
        {
            m_GameObject.SetActive(val);
        }

        public void SetText(string val)
        {
        }

        public void SetTextFormat(double val)
        {
        }

        public void SetText(int val)
        {
        }

        public void SetTips(int tipsId, params object[] args)
        {
        }

        public void SetTextColor(Color col)
        {
            m_text.color = col;
        }

        public void ClearText()
        {
        }
    }

    [System.Serializable]
    public class RectTransform_InputField_Image_Container
    {

        [SerializeField] private GameObject m_GameObject;

        public GameObject gameObject
        {
            get { return m_GameObject; }
        }

        [SerializeField] private RectTransform m_rectTransform;

        public RectTransform rectTransform
        {
            get { return m_rectTransform; }
        }

        [SerializeField] private InputField m_inputField;

        public InputField inputField
        {
            get { return m_inputField; }
        }

        [SerializeField] private Image m_image;

        public Image image
        {
            get { return m_image; }
        }
    }
    [System.Serializable]
    public class RectTransform_Button_Container
    {

        [SerializeField]
        private GameObject m_GameObject;
        public GameObject gameObject { get { return m_GameObject; } }

        [SerializeField]
        private RectTransform m_rectTransform;
        public RectTransform rectTransform { get { return m_rectTransform; } }

        [SerializeField]
        private Button m_button;
        public Button button { get { return m_button; } }

        public void SetOnClick(UnityAction action)
        {
        }

        public void SetActive(bool val)
        {
            m_GameObject.SetActive(val);
        }
    }
    [System.Serializable]
    public class RectTransform_Toggle_Container
    {

        [SerializeField]
        private GameObject m_GameObject;
        public GameObject gameObject { get { return m_GameObject; } }

        [SerializeField]
        private RectTransform m_rectTransform;
        public RectTransform rectTransform { get { return m_rectTransform; } }

        [SerializeField]
        private Toggle m_toggle;
        public Toggle toggle { get { return m_toggle; } }

    }

    [System.Serializable]
    public class RectTransform_Slider_Container
    {

        [SerializeField]
        private GameObject m_GameObject;
        public GameObject gameObject { get { return m_GameObject; } }

        [SerializeField]
        private RectTransform m_rectTransform;
        public RectTransform rectTransform { get { return m_rectTransform; } }

        [SerializeField]
        private Slider m_slider;
        public Slider slider { get { return m_slider; } }

    }
}