using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using AM_i18n.Scripts.Attribute;

namespace AM_i18n.Scripts.Core.Editor
{
    [CustomPropertyDrawer(typeof(TextKeySearchWindowAttribute))]
    public class TextKeySearchWindowAttributeDrawer : PropertyDrawer
    {
        private TextKeySearchProvider _textKeySearchProvider = null;
        private string _propertyValueName = null;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();

            EditorGUI.BeginProperty(position, label, property);

            // Field name position
            EditorGUI.LabelField(position, label);

            // Search window position
            Rect btnRect = position;
            btnRect.x = position.width / 2f;
            btnRect.width = (position.width / 2f) + 20f;

            UpdatePropertyValueNameIfNull(property);
            if (EditorGUI.DropdownButton(btnRect, new GUIContent(_propertyValueName), FocusType.Passive, EditorStyles.popup))
            {
                CreateSearchWindowForProperty(property);
            }

            EditorGUI.EndProperty();
            UpdatePropertyValueName(property);
        }

        private void CreateSearchWindowForProperty(SerializedProperty property)
        {
            Vector2 positionOffset = new Vector2(0f, 25f);
            Vector2 mousePosition = GUIUtility.GUIToScreenPoint(Event.current.mousePosition) + positionOffset;

            if (_textKeySearchProvider == null)
            {
                _textKeySearchProvider = ScriptableObject.CreateInstance<TextKeySearchProvider>();
                _textKeySearchProvider.RegisterCallback(tk =>
                {
                    Undo.RecordObject(property.serializedObject.targetObject, $"{property.displayName} change");
                    Undo.FlushUndoRecordObjects();

                    property.enumValueIndex = (int)tk;
                    bool hasChange = property.serializedObject.ApplyModifiedProperties();
                    if (hasChange)
                    {
                        UpdatePropertyValueName(property);
                        SceneView.RepaintAll();
                    }
                });
            }

            SearchWindow.Open(new SearchWindowContext(mousePosition), _textKeySearchProvider);
        }

        private void UpdatePropertyValueNameIfNull(SerializedProperty property)
        {
            if (string.IsNullOrEmpty(_propertyValueName) == false) { return; }
            UpdatePropertyValueName(property);
        }

        private void UpdatePropertyValueName(SerializedProperty property)
        {
            uint enumValueIndex = (uint)Mathf.Max(0, property.enumValueIndex);
            TextKey enumValue = (TextKey)enumValueIndex;
            _propertyValueName = enumValue.ToString();
        }
    }
}
