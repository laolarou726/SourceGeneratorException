using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using System.Collections.Generic;

namespace LauncherX.EnumDescriptor.SourceGenerator;

public static class EnumDescriptionMarkupGenerator
{
    public static CompilationUnitSyntax Get(
        string namespaceName,
        string baseClassName,
        string className,
        List<ExpressionSyntax> values)
    {
        return CompilationUnit()
            .AddUsings(GetUsing())
            .AddMembers(NamespaceDeclaration(ParseName(namespaceName))
                .AddMembers(GetClass(baseClassName, className, values)));
    }

    private static UsingDirectiveSyntax[] GetUsing()
    {
        return
        [
            UsingDirective(
                IdentifierName("System"))
        ];
    }

    private static ClassDeclarationSyntax GetClass(
        string baseClassName,
        string className,
        List<ExpressionSyntax> values)
    {
        return ClassDeclaration(className)
            .WithModifiers(
                TokenList(
                    Token(SyntaxKind.PublicKeyword),
                    Token(SyntaxKind.SealedKeyword)))
            .WithBaseList(
                BaseList(
                    SingletonSeparatedList<BaseTypeSyntax>(
                        SimpleBaseType(
                            IdentifierName(baseClassName)))))
            .WithMembers(
                List<MemberDeclarationSyntax>([
                    MethodDeclaration(
                            PredefinedType(
                                Token(SyntaxKind.ObjectKeyword)),
                            Identifier("ProvideValue"))
                        .WithModifiers(
                            TokenList(
                            [
                                Token(SyntaxKind.PublicKeyword),
                                Token(SyntaxKind.OverrideKeyword)
                            ]))
                        .WithParameterList(
                            ParameterList(
                                SingletonSeparatedList(
                                    Parameter(
                                            Identifier("serviceProvider"))
                                        .WithType(
                                            IdentifierName("IServiceProvider")))))
                        .WithBody(
                            Block(
                                SingletonList<StatementSyntax>(
                                    ReturnStatement(
                                        ImplicitArrayCreationExpression(
                                            InitializerExpression(
                                                SyntaxKind.ArrayInitializerExpression,
                                                SeparatedList(values)))))))
                    ]))
            .AddAttributeLists(AttributeList(SingletonSeparatedList(
                Attribute(ParseName("System.CodeDom.Compiler.GeneratedCodeAttribute"))
                    .WithArgumentList(AttributeArgumentList(
                        SeparatedList(new[]
                        {
                            AttributeArgument(
                                LiteralExpression(SyntaxKind.StringLiteralExpression,
                                    Literal("EnumDescriptionGenerator"))
                            ),
                            AttributeArgument(
                                LiteralExpression(SyntaxKind.StringLiteralExpression,
                                    Literal("1.0"))
                            )
                        })
                    ))
            )));
        }
}