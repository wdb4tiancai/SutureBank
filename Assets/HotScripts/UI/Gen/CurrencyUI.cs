using System.Collections.Generic;
using UIFramework;
using UnityEngine;

namespace Game.UI {

	public partial class CurrencyUI : BaseUi {

		[SerializeField]
		private RectTransform_Container m_CurrencyBar;
		public RectTransform_Container CurrencyBar { get { return m_CurrencyBar; } }

		[SerializeField]
		private RectTransform_CurrencyUI_Currency_Container m_Currency;
		public RectTransform_CurrencyUI_Currency_Container Currency { get { return m_Currency; } }

		[System.Serializable]
		public class RectTransform_CurrencyUI_Currency_Container {

			[SerializeField]
			private GameObject m_GameObject;
			public GameObject gameObject { get { return m_GameObject; } }

			[SerializeField]
			private RectTransform m_rectTransform;
			public RectTransform rectTransform { get { return m_rectTransform; } }

			[SerializeField]
			private CurrencyUI_Currency m_Currency;
			public CurrencyUI_Currency Currency { get { return m_Currency; } }

			private Queue<CurrencyUI_Currency> mCachedInstances;
			public CurrencyUI_Currency GetInstance(bool ignoreSibling = false) {
				CurrencyUI_Currency instance = null;
				if (mCachedInstances != null) {
					while ((instance == null || instance.Equals(null)) && mCachedInstances.Count > 0) {
						instance = mCachedInstances.Dequeue();
					}
				}
				if (instance == null || instance.Equals(null)) {
					instance = Instantiate<CurrencyUI_Currency>(m_Currency);
				}
				Transform t0 = m_Currency.transform;
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
			public bool CacheInstance(CurrencyUI_Currency instance) {
				if (instance == null || instance.Equals(null)) { return false; }
				if (mCachedInstances == null) { mCachedInstances = new Queue<CurrencyUI_Currency>(); }
				if (mCachedInstances.Contains(instance)) { return false; }
				instance.gameObject.SetActive(false);
				mCachedInstances.Enqueue(instance);
				return true;
			}

			public void CacheInstanceList(List<CurrencyUI_Currency> instanceList) {
				gameObject.SetActive(false);
				foreach (var instance in instanceList){
					CacheInstance(instance);
				}
				instanceList.Clear();
			}

		}

	}

}
