using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace LauncherX.EnumDescriptor.SourceGenerator
{
    [Generator]
    public class Main : ISourceGenerator
    {
        public const string DescriptionAttribute = "DescriptionAttribute";
        public const string LocalizedDescriptionAttribute = "LocalizedDescriptionAttribute";
        public const string GenerateAttribute = "LauncherX.SourceGenerator.Shared.GenerateEnumDescriptorAttribute";

        public void Initialize(GeneratorInitializationContext context)
        {
#if DEBUG
            // Debugger.Launch();
#endif

            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is not SyntaxReceiver syntaxReceiver)
                return;

            var compilation = context.Compilation;

            foreach (var classDeclaration in syntaxReceiver.CandidateClasses)
            {
                var semanticModel = compilation.GetSemanticModel(classDeclaration.SyntaxTree);
                var classSymbol = ModelExtensions.GetDeclaredSymbol(semanticModel, classDeclaration);
                
                var attributes =
                    classSymbol
                        .GetAttributes()
                        .Where(ad => ad.AttributeClass?.ToDisplayString() == GenerateAttribute);

                var classNamespace = classSymbol.ContainingNamespace.ToDisplayString();
                var className = classSymbol.Name;

                foreach (var attribute in attributes)
                {
                    var enumType = attribute.ConstructorArguments.FirstOrDefault().Value as INamedTypeSymbol;
                    if (enumType == null || !enumType.EnumUnderlyingType!.SpecialType.Equals(SpecialType.System_Int32))
                        continue;
                    
                    var enumElements = 
                        enumType
                            .GetMembers()
                            .OfType<IFieldSymbol>()
                            .Where(field => field.HasConstantValue && field.IsStatic && field.IsConst);

                    var enumDescriptions = new List<ExpressionSyntax>();

                    foreach (var element in enumElements)
                    {
                        var descriptionAttribute = element.GetAttributes()
                            .FirstOrDefault(attr =>
                                attr.AttributeClass?.Name is DescriptionAttribute or LocalizedDescriptionAttribute);

                        if (descriptionAttribute != null)
                        {
                            if (descriptionAttribute.AttributeClass?.Name == LocalizedDescriptionAttribute)
                            {
                                var resourceKey = descriptionAttribute.ConstructorArguments.FirstOrDefault().Value?.ToString();

                                if (!string.IsNullOrEmpty(resourceKey))
                                {
                                    enumDescriptions.Add(GetLocalizedExpressionSyntax(resourceKey));
                                }
                            }
                            else
                            {
                                var content =
                                    descriptionAttribute.ConstructorArguments.FirstOrDefault().Value?.ToString() ??
                                    element.Name;
                                enumDescriptions.Add(GetDescriptionSyntax(content));
                            }
                        }
                        else
                        {
                            enumDescriptions.Add(GetDescriptionSyntax(element.Name));
                        }
                    }

                    var enumTypeName = enumType.Name;
                    var enumClassName = $"{enumTypeName}BindingExtension";
                    var codeStr = EnumDescriptionMarkupGenerator
                        .Get(classNamespace, className, enumClassName, enumDescriptions)
                        .NormalizeWhitespace()
                        .ToFullString();

                    context.AddSource($"{enumClassName}.EnumDescriptor.g.cs", SourceText.From(codeStr, System.Text.Encoding.UTF8));
                }

                var codeString = BasicClassGenerator.Get(classNamespace, className).NormalizeWhitespace().ToFullString();
                context.AddSource($"{className}.EnumDescriptor.g.cs", SourceText.From(codeString, System.Text.Encoding.UTF8));
            }
        }

        private static ExpressionSyntax GetDescriptionSyntax(string content)
        {
            return LiteralExpression(
                SyntaxKind.StringLiteralExpression,
                Literal(content));
        }

        private static ExpressionSyntax GetLocalizedExpressionSyntax(string key)
        {
            return InvocationExpression(
                    IdentifierName("GetName"))
                .WithArgumentList(
                    ArgumentList(
                        SingletonSeparatedList(
                            Argument(
                                LiteralExpression(
                                    SyntaxKind.StringLiteralExpression,
                                    Literal(key))))));
        }

        internal class SyntaxReceiver : ISyntaxReceiver
        {
            public List<ClassDeclarationSyntax> CandidateClasses { get; } = new ();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is not ClassDeclarationSyntax classDeclarationSyntax) return;
                if (classDeclarationSyntax.AttributeLists.Count <= 0) return;

                var hasAttr = classDeclarationSyntax.AttributeLists
                    .SelectMany(attributeList => attributeList.Attributes)
                    .Any(attribute => attribute.Name.ToString() == "GenerateEnumDescriptor");

                if (!hasAttr) return;

                CandidateClasses.Add(classDeclarationSyntax);
            }
        }
    }
}