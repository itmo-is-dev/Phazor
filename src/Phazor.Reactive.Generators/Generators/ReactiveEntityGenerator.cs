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

            GenerateEntity(entity, context);
            GenerateEntityFactory(entity, context);
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
        TypeDeclarationSyntax typeSyntax = entity.ToSyntax();
        IdentifierNameSyntax namespaceIdentifier = IdentifierName(entity.InterfaceType.ContainingNamespace.ToDisplayString());
        NamespaceDeclarationSyntax namespaceSyntax = NamespaceDeclaration(namespaceIdentifier).AddMembers(typeSyntax);

        CompilationUnitSyntax syntax = CompilationUnit().AddMembers(namespaceSyntax);
        string text = syntax.NormalizeWhitespace(eol: "\n").ToFullString();
        string fileName = $"{entity.FullyQualifiedName}.cs";

        context.AddSource(fileName, text);
    }

    private static void GenerateEntityFactory(ReactiveEntity entity, GeneratorExecutionContext context)
    {
        var factory = new EntityFactory(entity);

        ClassDeclarationSyntax typeSyntax = factory.ToSyntax();
        IdentifierNameSyntax namespaceIdentifier = IdentifierName(entity.InterfaceType.ContainingNamespace.ToDisplayString());
        NamespaceDeclarationSyntax namespaceSyntax = NamespaceDeclaration(namespaceIdentifier).AddMembers(typeSyntax);

        CompilationUnitSyntax syntax = CompilationUnit().AddMembers(namespaceSyntax);
        string text = syntax.NormalizeWhitespace(eol: "\n").ToFullString();
        string fileName = $"{factory.FullyQualifiedName}.cs";

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
        ClassDeclarationSyntax typeSyntax = registration.ToSyntax();
        IdentifierNameSyntax namespaceIdentifier = IdentifierName(context.Compilation.Assembly.Name);
        NamespaceDeclarationSyntax namespaceSyntax = NamespaceDeclaration(namespaceIdentifier).AddMembers(typeSyntax);

        CompilationUnitSyntax syntax = CompilationUnit().AddMembers(namespaceSyntax);
        string text = syntax.NormalizeWhitespace(eol: "\n").ToFullString();
        string fileName = $"{registration.FullyQualifiedName}.cs";

        context.AddSource(fileName, text);
    }
}