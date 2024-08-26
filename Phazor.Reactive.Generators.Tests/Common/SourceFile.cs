using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace Phazor.Reactive.Generators.Tests.Common;

public readonly record struct SourceFile(string Name, string Content, Encoding? Encoding = null)
{
    public string FilePath { get; init; } = Name;

    public static async Task<SourceFile> LoadAsync(string path)
    {
        string name = Path.GetFileName(path);
        string content = await File.ReadAllTextAsync(path);

        return new SourceFile(name, content) { FilePath = path };
    }

    public static implicit operator (string, SourceText)(SourceFile sourceFile)
        => (sourceFile.Name, SourceText.From(sourceFile.Content, sourceFile.Encoding ?? Encoding.UTF8));
}