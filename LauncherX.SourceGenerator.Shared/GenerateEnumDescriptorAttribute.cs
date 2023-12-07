using System;

namespace LauncherX.SourceGenerator.Shared
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class GenerateEnumDescriptorAttribute : Attribute
    {
        public Type EnumType { get; }

        public GenerateEnumDescriptorAttribute(Type enumType)
        {
            EnumType = enumType;
        }
    }
}