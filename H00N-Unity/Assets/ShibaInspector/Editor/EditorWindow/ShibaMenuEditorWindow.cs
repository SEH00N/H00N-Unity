using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ShibaInspector.EidtorWindow
{
    public class ShibaMenuEditorWindow : ShibaEditorWindow
    {
        [System.Serializable]
        public class ShibaMenu
        {
            public string menuName;
            public ShibaEditorContentWrapper contentWrapper;

            public ShibaMenu(string menuName, object content)
            {
                this.menuName = menuName;
                contentWrapper = ScriptableObject.CreateInstance<ShibaEditorContentWrapper>();
                contentWrapper.content = content;
            }
        }

        private List<ShibaMenu> menus;
        private int _selectedMenu = 0;
        private GUIStyle _centeredLabelStyle;
        private float sidebarWidth = 116;
        private float separatorWidth = 2;

        protected virtual void OnEnable()
        {
            _centeredLabelStyle = new GUIStyle(EditorStyles.whiteLabel)
            {
                alignment = TextAnchor.MiddleLeft
            };

            menus = new List<ShibaMenu>();
        }

        protected virtual void OnGUI()
        {
            // draw menu
            // 1) 배경색과 구분선 그리기
            var fullRect = new Rect(0, 0, position.width, position.height);
            var leftRect = new Rect(0, 0, sidebarWidth, fullRect.height);
            var sepRect  = new Rect(sidebarWidth, 0, separatorWidth, fullRect.height);
            var rightRect= new Rect(sidebarWidth + separatorWidth, 0,
                                    fullRect.width - sidebarWidth - separatorWidth,
                                    fullRect.height);

            // 원하는 색상으로 칠하기 (예: 왼쪽은 짙은 그레이, 오른쪽은 조금 밝은 그레이)
            EditorGUI.DrawRect(leftRect,  new Color(0.2f,  0.2f,  0.2f));
            EditorGUI.DrawRect(rightRect, new Color(0.15f, 0.15f, 0.15f));
            // 사이 구분선
            EditorGUI.DrawRect(sepRect,   new Color(0.1f,  0.1f,  0.1f));

            // 2) 영역별 GUI 렌더링
            GUILayout.BeginArea(leftRect);
            EditorGUILayout.BeginVertical(GUILayout.Width(sidebarWidth));
            for (int i = 0; i < menus.Count; i++)
            {
                // 라벨 영역 계산
                Rect r = GUILayoutUtility.GetRect(
                    new GUIContent(menus[i].menuName),
                    _centeredLabelStyle,          // <-- 여기 스타일 바꿔서
                    GUILayout.Height(30),
                    GUILayout.ExpandWidth(true)
                );
                // 선택된 메뉴 배경
                if (_selectedMenu == i)
                    EditorGUI.DrawRect(r, new Color(0.24f, 0.48f, 0.80f));

                // 라벨 그리기
                GUI.Label(r, menus[i].menuName, _centeredLabelStyle);

                // 클릭 처리
                if (Event.current.type == EventType.MouseDown && r.Contains(Event.current.mousePosition))
                {
                    _selectedMenu = i;
                    
                    Event.current.Use();
                }

                // 5) 구분선 그리기 (마지막 아이템 뒤에는 생략)
                if (i < menus.Count - 1)
                {
                    // 라벨 바로 아래에 1px 높이의 rect
                    Rect lineRect = new Rect(
                        r.x,
                        r.yMax,          // 라벨 영역 바로 아래
                        r.width,
                        1                // 두께 1px
                    );
                    EditorGUI.DrawRect(lineRect, new Color(0.1f,  0.1f,  0.1f));
                }
            }
            EditorGUILayout.EndVertical();
            GUILayout.EndArea();
        
            //draw content
            GUILayout.BeginArea(rightRect);
            if(menus.Count > _selectedMenu && menus[_selectedMenu] != null)
            {
                OnGUIContent(menus[_selectedMenu].contentWrapper);
            }
            GUILayout.EndArea();
        }

        protected virtual void OnDisable()
        {
            if (menus != null)
            {
                foreach (var menu in menus)
                {
                    if (menu.contentWrapper != null)
                    {
                    // This will immediately destroy the ScriptableObject
                    DestroyImmediate(menu.contentWrapper);
                    }
                }

                menus.Clear();
            }   
        }

        public void AddMenu(string name, object content)
        {
            if(string.IsNullOrEmpty(name))
                return;
            if(content == null)
                return;
            
            menus.Add(new ShibaMenu(name, content));
        }
    }
}