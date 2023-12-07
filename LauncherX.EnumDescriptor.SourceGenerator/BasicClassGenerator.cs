using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace LauncherX.EnumDescriptor.SourceGenerator
{
    public static class BasicClassGenerator
    {
        public static CompilationUnitSyntax Get(
            string namespaceName,
            string className)
        {
            return CompilationUnit()
                .AddUsings(GetUsing())
                .AddMembers(NamespaceDeclaration(ParseName(namespaceName))
                    .AddMembers(GetClass(className)));
        }

        private static UsingDirectiveSyntax[] GetUsing()
        {
            return [
                UsingDirective(
                    QualifiedName(
                        QualifiedName(
                            QualifiedName(
                                IdentifierName("LauncherX"),
                                IdentifierName("Avalonia")),
                            IdentifierName("Class")),
                        IdentifierName("Helper")))
            ];
        }

        private static ClassDeclarationSyntax GetClass(string className)
        {
            return ClassDeclaration(className)
                .WithModifiers(
                    TokenList(
                    [
                        Token(SyntaxKind.PartialKeyword)
                    ]))
                .WithMembers(
                    SingletonList<MemberDeclarationSyntax>(
                        MethodDeclaration(
                                PredefinedType(
                                    Token(SyntaxKind.StringKeyword)),
                                Identifier("GetName"))
                            .WithModifiers(
                                TokenList(
                                [
                                    Token(SyntaxKind.ProtectedKeyword),
                                    Token(SyntaxKind.StaticKeyword)
                                ]))
                            .WithParameterList(
                                ParameterList(
                                    SingletonSeparatedList(
                                        Parameter(
                                                Identifier("resourceKey"))
                                            .WithType(
                                                PredefinedType(
                                                    Token(SyntaxKind.StringKeyword))))))
                            .WithBody(
                                Block(
                                    LocalDeclarationStatement(
                                        VariableDeclaration(
                                                IdentifierName(
                                                    Identifier(
                                                        TriviaList(),
                                                        SyntaxKind.VarKeyword,
                                                        "var",
                                                        "var",
                                                        TriviaList())))
                                            .WithVariables(
                                                SingletonSeparatedList(
                                                    VariableDeclarator(
                                                            Identifier("description"))
                                                        .WithInitializer(
                                                            EqualsValueClause(
                                                                InvocationExpression(
                                                                        MemberAccessExpression(
                                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                                            IdentifierName("LangHelper"),
                                                                            IdentifierName("GetStr")))
                                                                    .WithArgumentList(
                                                                        ArgumentList(
                                                                            SingletonSeparatedList(
                                                                                Argument(
                                                                                    IdentifierName(
                                                                                        "resourceKey")))))))))),
                                    ReturnStatement(
                                        ConditionalExpression(
                                            InvocationExpression(
                                                    MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        PredefinedType(
                                                            Token(SyntaxKind.StringKeyword)),
                                                        IdentifierName("IsNullOrWhiteSpace")))
                                                .WithArgumentList(
                                                    ArgumentList(
                                                        SingletonSeparatedList(
                                                            Argument(
                                                                IdentifierName("description"))))),
                                            InterpolatedStringExpression(
                                                    Token(SyntaxKind.InterpolatedStringStartToken))
                                                .WithContents(
                                                    List(
                                                        new InterpolatedStringContentSyntax[]
                                                        {
                                                            InterpolatedStringText()
                                                                .WithTextToken(
                                                                    Token(
                                                                        TriviaList(),
                                                                        SyntaxKind.InterpolatedStringTextToken,
                                                                        "[[",
                                                                        "[[",
                                                                        TriviaList())),
                                                            Interpolation(
                                                                IdentifierName("resourceKey")),
                                                            InterpolatedStringText()
                                                                .WithTextToken(
                                                                    Token(
                                                                        TriviaList(),
                                                                        SyntaxKind.InterpolatedStringTextToken,
                                                                        "]]",
                                                                        "]]",
                                                                        TriviaList()))
                                                        })),
                                            IdentifierName("description")))))))
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
}