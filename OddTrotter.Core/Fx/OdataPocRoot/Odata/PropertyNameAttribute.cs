namespace Fx.OdataPocRoot.Odata
{
    using System;

    public sealed class PropertyNameAttribute : Attribute
    {
        public PropertyNameAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }

        public string PropertyName { get; }
    }
}
