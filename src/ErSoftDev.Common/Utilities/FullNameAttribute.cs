namespace ErSoftDev.Common.Utilities
{
    public class FullNameAttribute(string name) : Attribute
    {
        public string Name { get; set; } = name;
    }
}
