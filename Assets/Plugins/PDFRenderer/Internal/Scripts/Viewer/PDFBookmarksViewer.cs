using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Paroxe.PdfRenderer.Internal.Viewer
{
    public class PDFBookmarksViewer : UIBehaviour
    {
        [SerializeField]
        private RectTransform m_BooksmarksContainer;
        [SerializeField]
        private PDFBookmarkListItem m_ItemPrefab;
        [SerializeField]
        private Image m_LastHighlightedImage;

#if !UNITY_WEBGL
        private CanvasGroup m_CanvasGroup;
        private bool m_Initialized = false;
        private RectTransform m_LeftPanel;
        private bool m_Loaded = false;
        private PDFDocument m_Document;
        private PDFViewer m_Viewer;
        private RectTransform m_RectTransform;
        private List<RectTransform> m_TopLevelItems;
#endif

#if !UNITY_WEBGL
        private PDFBookmark m_RootBookmark;
#endif

	    public Image LastHighlightedImage
	    {
		    get { return m_LastHighlightedImage; }
		    set { m_LastHighlightedImage = value; }
	    }

#if !UNITY_WEBGL
        public PDFBookmark RootBookmark
	    {
		    get { return m_RootBookmark; }
	    }

	    public PDFViewer Viewer
	    {
		    get { return m_Viewer; }
	    }
#endif

        public void DoUpdate()
        {
#if !UNITY_WEBGL
            if (m_Initialized)
            {
                float containerHeight = 0.0f;

                foreach (RectTransform child in m_TopLevelItems)
	                containerHeight += child.sizeDelta.y;
            }

            if (m_RectTransform != null && m_LeftPanel != null &&
                Math.Abs(m_RectTransform.sizeDelta.x - (m_LeftPanel.sizeDelta.x - 24.0f)) > 0.01f)
            {
                m_RectTransform.sizeDelta = new Vector2(m_LeftPanel.sizeDelta.x - 24.0f, m_RectTransform.sizeDelta.y);
            }
#endif
        }

        private void Cleanup()
        {
#if !UNITY_WEBGL
            if (m_Loaded)
            {
                m_Loaded = false;
                m_Initialized = false;
                m_TopLevelItems = null;
                m_Document = null;
                m_RootBookmark = null;

                bool isNotFirst = false;
                foreach (Transform child in m_BooksmarksContainer.transform)
                {
                    if (isNotFirst)
                        Destroy(child.gameObject);
                    else
                        isNotFirst = true;
                }

                m_ItemPrefab.gameObject.SetActive(false);
                m_CanvasGroup.alpha = 0.0f;
            }
#endif
        }

        public void OnDocumentLoaded(PDFDocument document)
        {
#if !UNITY_WEBGL
	        if (m_Loaded || !gameObject.activeInHierarchy) 
		        return;

	        m_Loaded = true;
            m_Document = document;

            m_TopLevelItems = new List<RectTransform>();

            m_RectTransform = transform as RectTransform;
            m_LeftPanel = transform.parent as RectTransform;

            m_RootBookmark = m_Document.GetRootBookmark();

            if (m_RootBookmark == null) 
	            return;

            m_ItemPrefab.gameObject.SetActive(true);

            foreach (PDFBookmark child in m_RootBookmark.EnumerateChildrenBookmarks())
            {
	            PDFBookmarkListItem item = null;

	            item = Instantiate(m_ItemPrefab.gameObject).GetComponent<PDFBookmarkListItem>();
	            RectTransform itemTransform = item.transform as RectTransform;
	            itemTransform.SetParent(m_BooksmarksContainer, false);
	            itemTransform.localScale = Vector3.one;
	            itemTransform.anchorMin = new Vector2(0.0f, 1.0f);
	            itemTransform.anchorMax = new Vector2(0.0f, 1.0f);
	            itemTransform.offsetMin = Vector2.zero;
	            itemTransform.offsetMax = Vector2.zero;

	            m_TopLevelItems.Add(item.transform as RectTransform);

	            item.Initialize(child, 0, false);
            }

            m_ItemPrefab.gameObject.SetActive(false);

            m_Initialized = true;

            if (GetComponentInParent<PDFViewerLeftPanel>().Thumbnails.gameObject.GetComponent<CanvasGroup>().alpha == 0.0f)
	            StartCoroutine(DelayedShow());
#endif
        }

#if !UNITY_WEBGL
        IEnumerator DelayedShow()
        {
            yield return null;
            yield return null;
            yield return null;
            m_CanvasGroup.alpha = 1.0f;
        }
#endif

        public void OnDocumentUnloaded()
        {
#if !UNITY_WEBGL
            Cleanup();
#endif
        }

#if !UNITY_WEBGL
        protected override void OnDisable()
        {
            base.OnDisable();

            if (m_Loaded)
            {
                Cleanup();
            }
        }
#endif

#if !UNITY_WEBGL
        protected override void OnEnable()
        {
            base.OnEnable();

            DoOnEnable();
        }
#endif

        public void DoOnEnable()
        {
#if !UNITY_WEBGL
            if (m_Viewer == null)
                m_Viewer = GetComponentInParent<PDFViewer>();
            if (m_CanvasGroup == null)
                m_CanvasGroup = GetComponent<CanvasGroup>();
            if (m_RectTransform == null)
                m_RectTransform = transform as RectTransform;

            m_ItemPrefab.gameObject.SetActive(false);
            m_CanvasGroup.alpha = 0.0f;

            if (!m_Loaded && m_Viewer.Document != null && m_Viewer.Document.IsValid)
                OnDocumentLoaded(m_Viewer.Document);
#endif
        }
    }
}