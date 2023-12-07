using Avalonia.Markup.Xaml;
using LauncherX.SourceGenerator.Shared;

namespace SourceGeneratorException;

enum TestEnum
{
    Test1,
    Test2,
    Test3,
    Test4,
    Test5,
    Test6,
    Test7,
    Test8
}

[GenerateEnumDescriptor(typeof(TestEnum))]
public abstract partial class EnumBindingBase : MarkupExtension;