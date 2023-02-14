using System;
using System.Diagnostics.CodeAnalysis;

namespace Project.Cad.Data.Attribute
{
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CollectionAttribute : System.Attribute
    {
        public string Name { get; set; }
        public CollectionAttribute(string name)
        {
            Name = name;
        }
    }
}
