namespace SPCoder.Context
{
    /// <summary>
    /// 
    /// </summary>
    /// <author>Damjan Tomic</author>
    public class ContextItem
    {
        public string Type { get; set; }
        public object Data { get; set; }
        public string Variable { get; set; }
        public string Name { get; set; }        

        public override string ToString()
        {
            return Variable + Name + " : (" + Data + ", " + Type + ")";
        }
    }
}