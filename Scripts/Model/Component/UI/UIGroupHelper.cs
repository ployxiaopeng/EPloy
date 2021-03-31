//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2019 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using GameFramework.UI;
using UnityEngine;
using UnityEngine.UI;

namespace ETModel
{
    /// <summary>
    /// 默认界面组辅助器。
    /// </summary>
    public class UIGroupHelper : IUIGroupHelper
    {
        private int m_Depth = 0;
        private Canvas CachedCanvas = null;
        public GameObject Group { get; protected set; }
        /// <summary>
        /// 设置界面组深度。
        /// </summary>
        /// <param name="depth">界面组深度。</param>
        public void SetDepth(int depth)
        {
            m_Depth = depth;
            Group.layer = LayerMask.NameToLayer("UI");
            CachedCanvas.overrideSorting = true;
            CachedCanvas.sortingLayerName = "UI";
            CachedCanvas.sortingOrder = depth;
            SetRectTransform(Group.transform as RectTransform);
        }

        public UIGroupHelper(string name, Transform parent)
        {
            Group = new GameObject(name);
            Group.transform.SetParent(parent,false);
            Group.AddComponent<RectTransform>();
            CachedCanvas = Group.AddComponent<Canvas>();
            Group.AddComponent<GraphicRaycaster>().ignoreReversedGraphics = false;

        }

        private void SetRectTransform(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMax = Vector3.zero;
            rect.offsetMin = Vector3.zero;
        }
    }
}
