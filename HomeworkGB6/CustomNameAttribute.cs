namespace HomeworkGB6
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class CustomNameAttribute : Attribute
    {
        public string PropertyName { get; set; }
        public CustomNameAttribute() { PropertyName = "SomeProperty"; } 
        public CustomNameAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}
