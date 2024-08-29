using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Phazor.Reactive.Generators.Models;
using Phazor.Reactive.Generators.Models.Effects;
using Phazor.Reactive.Generators.Models.Entities;
using Phazor.Reactive.Generators.Models.Events;
using Phazor.Reactive.Generators.Receivers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Phazor.Reactive.Generators.Generators;

[Generator]
public class ReactiveEntityGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
        => context.RegisterForSyntaxNotifications(() => new ReactiveEntitySyntaxReceiver());

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxContextReceiver is not ReactiveEntitySyntaxReceiver receiver)
            return;

        foreach (ReactiveEntity entity in receiver.Entities)
        {
            if (context.CancellationToken.IsCancellationRequested)
                return;

            var factory = new EntityFactory(entity);

            GenerateEntity(entity, context);
            GenerateEntityFactory(factory, context);
            GenerateEntityFactoryAlias(factory, context);
        }

        foreach (ReactiveEvent evt in receiver.Events)
        {
            if (context.CancellationToken.IsCancellationRequested)
                return;

            GenerateEventHandler(evt, receiver.Entities, context);
        }

        var registration = new RegistrationExtension(
            context.Compilation.Assembly,
            Factories: receiver.Entities.Select(x => new EntityFactory(x)).ToArray(),
            EventHandlers: receiver.Events.Select(x => new ReactiveEventHandler(x)).ToArray());

        GenerateRegistration(registration, context);
    }

    private static void GenerateEntity(ReactiveEntity entity, GeneratorExecutionContext context)
    {
        TypeDeclarationSyntax typeSyntax = entity.ToSyntax(context);
        IdentifierNameSyntax namespaceIdentifier =
            IdentifierName(entity.InterfaceType.ContainingNamespace.ToDisplayString());
        NamespaceDeclarationSyntax namespaceSyntax = NamespaceDeclaration(namespaceIdentifier).AddMembers(typeSyntax);

        CompilationUnitSyntax syntax = CompilationUnit().AddMembers(namespaceSyntax);
        string text = syntax.NormalizeWhitespace(eol: "\n").ToFullString();
        string fileName = $"{entity.FullyQualifiedName}.cs";

        context.AddSource(fileName, text);
    }

    private static void GenerateEntityFactory(EntityFactory factory, GeneratorExecutionContext context)
    {
        ClassDeclarationSyntax typeSyntax = factory.ToFactorySyntax(context);

        IdentifierNameSyntax namespaceIdentifier = IdentifierName(
            factory.Entity.InterfaceType.ContainingNamespace.ToDisplayString());

        NamespaceDeclarationSyntax namespaceSyntax = NamespaceDeclaration(namespaceIdentifier).AddMembers(typeSyntax);

        CompilationUnitSyntax syntax = CompilationUnit().AddMembers(namespaceSyntax);
        string text = syntax.NormalizeWhitespace(eol: "\n").ToFullString();
        string fileName = $"{factory.FullyQualifiedName}.cs";

        context.AddSource(fileName, text);
    }

    private static void GenerateEntityFactoryAlias(EntityFactory factory, GeneratorExecutionContext context)
    {
        InterfaceDeclarationSyntax typeSyntax = factory.ToFactoryAliasSyntax();

        IdentifierNameSyntax namespaceIdentifier = IdentifierName(
            factory.Entity.InterfaceType.ContainingNamespace.ToDisplayString());

        NamespaceDeclarationSyntax namespaceSyntax = NamespaceDeclaration(namespaceIdentifier).AddMembers(typeSyntax);

        CompilationUnitSyntax syntax = CompilationUnit().AddMembers(namespaceSyntax);
        string text = syntax.NormalizeWhitespace(eol: "\n").ToFullString();
        string fileName = $"{factory.AliasFullyQualifiedName}.cs";

        context.AddSource(fileName, text);
    }

    private static void GenerateEventHandler(
        ReactiveEvent evt,
        IEnumerable<ReactiveEntity> entities,
        GeneratorExecutionContext context)
    {
        var handler = new ReactiveEventHandler(evt);

        ClassDeclarationSyntax typeSyntax = handler.ToSyntax(entities);
        IdentifierNameSyntax namespaceIdentifier = IdentifierName(evt.EventType.ContainingNamespace.ToDisplayString());
        NamespaceDeclarationSyntax namespaceSyntax = NamespaceDeclaration(namespaceIdentifier).AddMembers(typeSyntax);

        CompilationUnitSyntax syntax = CompilationUnit().AddMembers(namespaceSyntax);
        string text = syntax.NormalizeWhitespace(eol: "\n").ToFullString();
        string fileName = $"{handler.FullyQualifiedName}.cs";

        context.AddSource(fileName, text);
    }

    private static void GenerateRegistration(RegistrationExtension registration, GeneratorExecutionContext context)
    {
        if (registration.TryGetSyntax(out ClassDeclarationSyntax? typeSyntax) is false)
            return;

        IdentifierNameSyntax namespaceIdentifier = IdentifierName(context.Compilation.Assembly.Name);
        NamespaceDeclarationSyntax namespaceSyntax = NamespaceDeclaration(namespaceIdentifier).AddMembers(typeSyntax);

        CompilationUnitSyntax syntax = CompilationUnit().AddMembers(namespaceSyntax);
        string text = syntax.NormalizeWhitespace(eol: "\n").ToFullString();
        string fileName = $"{registration.FullyQualifiedName}.cs";

        context.AddSource(fileName, text);
    }
}