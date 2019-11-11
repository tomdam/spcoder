namespace SPCoder.Context
{
    /// <summary>
    /// 
    /// </summary>
    /// <author>Damjan Tomic</author>
    public interface IContextItem
    {
        string Type { get; set; }
        object Data { get; set; }
    }
}