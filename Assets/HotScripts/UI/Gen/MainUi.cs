using System.Collections.Generic;
using UIFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI {

	public partial class MainUi : BaseScreen {

		[SerializeField]
		private RectTransform_Image_Container m_Image;
		public RectTransform_Image_Container Image { get { return m_Image; } }

		[SerializeField]
		private RectTransform_MainUi_aaa_Container m_aaa;
		public RectTransform_MainUi_aaa_Container aaa { get { return m_aaa; } }

		[System.Serializable]
		public class RectTransform_MainUi_aaa_Container {

			[SerializeField]
			private GameObject m_GameObject;
			public GameObject gameObject { get { return m_GameObject; } }

			[SerializeField]
			private RectTransform m_rectTransform;
			public RectTransform rectTransform { get { return m_rectTransform; } }

			[SerializeField]
			private MainUi_aaa m_aaa;
			public MainUi_aaa aaa { get { return m_aaa; } }

			private Queue<MainUi_aaa> mCachedInstances;
			public MainUi_aaa GetInstance(bool ignoreSibling = false) {
				MainUi_aaa instance = null;
				if (mCachedInstances != null) {
					while ((instance == null || instance.Equals(null)) && mCachedInstances.Count > 0) {
						instance = mCachedInstances.Dequeue();
					}
				}
				if (instance == null || instance.Equals(null)) {
					instance = Instantiate<MainUi_aaa>(m_aaa);
				}
				Transform t0 = m_aaa.transform;
				Transform t1 = instance.transform;
				t1.SetParent(t0.parent);
				t1.localPosition = t0.localPosition;
				t1.localRotation = t0.localRotation;
				t1.localScale = t0.localScale;
				if (!ignoreSibling) {
					t1.SetSiblingIndex(t0.GetSiblingIndex() + 1);
				}else{
					t1.SetAsLastSibling();
				}
				return instance;
			}
			public bool CacheInstance(MainUi_aaa instance) {
				if (instance == null || instance.Equals(null)) { return false; }
				if (mCachedInstances == null) { mCachedInstances = new Queue<MainUi_aaa>(); }
				if (mCachedInstances.Contains(instance)) { return false; }
				instance.gameObject.SetActive(false);
				mCachedInstances.Enqueue(instance);
				return true;
			}

			public void CacheInstanceList(List<MainUi_aaa> instanceList) {
				gameObject.SetActive(false);
				foreach (var instance in instanceList){
					CacheInstance(instance);
				}
				instanceList.Clear();
			}

		}

	}

}
