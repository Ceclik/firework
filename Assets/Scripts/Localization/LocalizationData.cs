namespace Localization
{
    [System.Serializable]
    public class LocalizationData
    {
        public LocalizationItem[] items;
    }

    [System.Serializable]
    public class LocalizationItem  //template class for writing into JSON file
    {
        public string key;
        public string value;
    }
}
