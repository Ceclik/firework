using System;

namespace Localization
{
    [Serializable]
    public class LocalizationData
    {
        public LocalizationItem[] items;
    }

    [Serializable]
    public class LocalizationItem //template class for writing into JSON file
    {
        public string key;
        public string value;
    }
}