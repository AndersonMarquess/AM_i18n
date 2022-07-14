using UnityEngine;
using UnityEditor;

namespace AM_i18n.Scripts.Core
{
    public class TextManagerWindow : EditorWindow
    {
        [MenuItem("Tools/Text Manager", false, 30002)]
        private static void OpenWindow()
        {
            TextManagerWindow window = GetWindow<TextManagerWindow>("Text Manager", true);
            window.Show();
        }

        [Header("New Entry Details")]
        [SerializeField]
        private Language _entryLanguage = Language.en_US;
        [SerializeField]
        private TextKey _textEnumKey = TextKey.NONE_0;
        [SerializeField]
        private string _textKey = string.Empty;
        [SerializeField, TextArea(10, 10)]
        private string _textValue = string.Empty;
        private TextDataIOUtility _textDataIOUtility = null;
        private SerializedObject _serializedObject = null;
        private SerializedProperty _serializedPropertyGameLanguage = null;
        private SerializedProperty _serializedPropertyTextEnumKey = null;
        private SerializedProperty _serializedPropertyTextKey = null;
        private SerializedProperty _serializedPropertyTextValue = null;

        private void OnEnable()
        {
            _textDataIOUtility ??= new TextDataIOUtility(_entryLanguage);
            _serializedObject ??= new SerializedObject(this);

            _serializedPropertyGameLanguage = _serializedObject.FindProperty("_entryLanguage");
            _serializedPropertyTextEnumKey = _serializedObject.FindProperty("_textEnumKey");
            _serializedPropertyTextKey = _serializedObject.FindProperty("_textKey");
            _serializedPropertyTextValue = _serializedObject.FindProperty("_textValue");
        }

        private void OnGUI()
        {
            DrawSerializedPropreties();

            GUI.enabled = CanCreateJsonEntry();
            GUILayout.Space(10f);
            if (GUILayout.Button("Create"))
            {
                SaveDataToJSON();
            }
            GUI.enabled = true;
        }

        private void DrawSerializedPropreties()
        {
            _serializedObject.Update();
            EditorGUILayout.PropertyField(_serializedPropertyGameLanguage);

            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(_serializedPropertyTextEnumKey);
            if (_textEnumKey != default && GUILayout.Button("Print Key Value"))
            {
                PrintContent();
            }
            GUILayout.EndHorizontal();

            if (_textEnumKey == default)
            {
                EditorGUILayout.PropertyField(_serializedPropertyTextKey);
            }
            EditorGUILayout.PropertyField(_serializedPropertyTextValue);

            bool hasChanged = _serializedObject.ApplyModifiedProperties();
            if (hasChanged)
            {
                SceneView.RepaintAll();
            }
        }

        private bool CanCreateJsonEntry()
        {
            bool hasKey = string.IsNullOrEmpty(_textKey) == false || _textEnumKey != default;
            return hasKey && string.IsNullOrEmpty(_textValue) == false;
        }

        private void SaveDataToJSON()
        {
            uint lastKeyID = (uint)_textEnumKey;
            if (_textEnumKey == TextKey.NONE_0)
            {
                lastKeyID = _textDataIOUtility.WriteKeyToEnum(_textKey);
            }

            _textDataIOUtility.SaveDataToJSON(_entryLanguage, lastKeyID, _textValue);

            GUI.FocusControl(null);

            _textKey = string.Empty;
            _textValue = string.Empty;
            _textEnumKey = TextKey.NONE_0;

            AssetDatabase.Refresh();
        }

        private void PrintContent()
        {
            EntryDataCollection entryDataCollection = _textDataIOUtility.LoadEntryDataCollect(_entryLanguage);
            for (int i = 0; i < entryDataCollection.EntriesDatas.Count; i++)
            {
                EntryData entryData = entryDataCollection.EntriesDatas[i];
                if (entryData.KeyID == (uint)_textEnumKey)
                {
                    string[] values = entryData.Values;
                    for (int j = 0; j < values.Length; j++)
                    {
                        Debug.Log(values[j]);
                    }
                }
            }
        }
    }
}
