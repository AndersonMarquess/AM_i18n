using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

namespace AM_i18n.Scripts.Core
{
    public class TextKeySearchProvider : ScriptableObject, ISearchWindowProvider
    {
        private event Action<TextKey> _onClickOverItemCallback = null;
        private List<SearchTreeEntry> _searchTreeEntries = null;

        public void RegisterCallback(Action<TextKey> clickOverItemCallback)
        {
            _onClickOverItemCallback = clickOverItemCallback;
            FillSearchTreeEntries();
        }

        private void FillSearchTreeEntries()
        {
            _searchTreeEntries ??= new List<SearchTreeEntry>();
            _searchTreeEntries.Clear();

            TextKey[] enumEntries = (TextKey[])Enum.GetValues(typeof(TextKey));

            for (int i = 0; i < enumEntries.Length; i++)
            {
                GUIContent itemLabel = new GUIContent(enumEntries[i].ToString());

                if (i == 0)
                {
                    _searchTreeEntries.Add(new SearchTreeGroupEntry(itemLabel, 0));
                }

                SearchTreeEntry newTreeEntry = new SearchTreeEntry(itemLabel);
                newTreeEntry.level = 1;
                newTreeEntry.userData = enumEntries[i];
                _searchTreeEntries.Add(newTreeEntry);
            }
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            if (_searchTreeEntries == null || _searchTreeEntries.Count <= 0)
            {
                FillSearchTreeEntries();
            }

            return _searchTreeEntries;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            _onClickOverItemCallback?.Invoke((TextKey)SearchTreeEntry.userData);
            return true;
        }
    }
}
