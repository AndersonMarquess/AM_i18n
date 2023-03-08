using System.Collections.Generic;

namespace AM_i18n.Scripts.Core
{
    [System.Serializable]
    public class EntryDataCollection
    {
        public List<EntryData> EntriesDatas = null;

        public EntryDataCollection()
        {
            EntriesDatas ??= new List<EntryData>();
        }

        public bool AddTextToExistingEntryData(uint keyID, string textValue)
        {
            for (int i = 0; i < EntriesDatas.Count; i++)
            {
                EntryData entryData = EntriesDatas[i];
                if (entryData.KeyID == keyID)
                {
                    string[] entryValues = entryData.Values;
                    entryData.Values = new string[entryValues.Length + 1];
                    for (int j = 0; j < entryValues.Length; j++)
                    {
                        entryData.Values[j] = entryValues[j];
                    }
                    entryData.Values[entryValues.Length] = textValue;
                    return true;
                }
            }
            return false;
        }
    }
}
