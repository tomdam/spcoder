
namespace SPCoder.Describer
{
    /// <summary>
    /// 
    /// </summary>
    /// <author>Damjan Tomic</author>
    public class DescriberPropertiesData
    {
        public bool IsEditable { get; set; }
        public int MaxDisplayedSize { get; set; }
        public object DisplayedObject { get; set; }
        public string MsdnLinkFormat { get; set; }
        public bool WordWrap { get; set; }

        public DescriberPropertiesData()
        {
            IsEditable = false;
            MaxDisplayedSize = 2048;
        }
    }
}