using Microsoft.CodeAnalysis;
using Phazor.Reactive.Generators.Models;
using Phazor.Reactive.Generators.Models.Effects;
using Phazor.Reactive.Generators.Models.Entities;
using Phazor.Reactive.Generators.Models.Events;
using Phazor.Reactive.Generators.Receivers;
using Phazor.Reactive.Generators.Tools;
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

        foreach (var evt in receiver.Events)
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
        var typeSyntax = entity.ToSyntax();
        var namespaceIdentifier = IdentifierName(entity.InterfaceType.ContainingNamespace.ToDisplayString());
        var namespaceSyntax = NamespaceDeclaration(namespaceIdentifier).AddMembers(typeSyntax);

        var syntax = CompilationUnit().AddMembers(namespaceSyntax);
        var text = syntax.NormalizeWhitespace(eol: "\n").ToFullString();
        var fileName = $"{entity.FullyQualifiedName}.cs";

        context.AddSource(fileName, text);
    }

    private static void GenerateEntityFactory(ReactiveEntity entity, GeneratorExecutionContext context)
    {
        var factory = new EntityFactory(entity);

        var typeSyntax = factory.ToSyntax();
        var namespaceIdentifier = IdentifierName(entity.InterfaceType.ContainingNamespace.ToDisplayString());
        var namespaceSyntax = NamespaceDeclaration(namespaceIdentifier).AddMembers(typeSyntax);

        var syntax = CompilationUnit().AddMembers(namespaceSyntax);
        var text = syntax.NormalizeWhitespace(eol: "\n").ToFullString();
        var fileName = $"{factory.FullyQualifiedName}.cs";

        context.AddSource(fileName, text);
    }

    private static void GenerateEventHandler(
        ReactiveEvent evt,
        IEnumerable<ReactiveEntity> entities,
        GeneratorExecutionContext context)
    {
        var handler = new ReactiveEventHandler(evt);

        var typeSyntax = handler.ToSyntax(entities);
        var namespaceIdentifier = IdentifierName(evt.EventType.ContainingNamespace.ToDisplayString());
        var namespaceSyntax = NamespaceDeclaration(namespaceIdentifier).AddMembers(typeSyntax);

        var syntax = CompilationUnit().AddMembers(namespaceSyntax);
        var text = syntax.NormalizeWhitespace(eol: "\n").ToFullString();
        var fileName = $"{handler.FullyQualifiedName}.cs";

        context.AddSource(fileName, text);
    }

    private static void GenerateRegistration(RegistrationExtension registration, GeneratorExecutionContext context)
    {
        var typeSyntax = registration.ToSyntax();
        var namespaceIdentifier = IdentifierName(context.Compilation.Assembly.Name);
        var namespaceSyntax = NamespaceDeclaration(namespaceIdentifier).AddMembers(typeSyntax);

        var syntax = CompilationUnit().AddMembers(namespaceSyntax);
        var text = syntax.NormalizeWhitespace(eol: "\n").ToFullString();
        var fileName = $"{registration.FullyQualifiedName}.cs";

        context.AddSource(fileName, text);
    }
}