using System;

namespace LauncherX.SourceGenerator.Shared
{
    public class I18NClassForAttribute : Attribute
    {
        public string LangDocName { get; }

        public I18NClassForAttribute(string langDocName)
        {
            LangDocName = langDocName;
        }
    }
}