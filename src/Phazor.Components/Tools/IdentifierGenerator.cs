using System.Buffers;

namespace Phazor.Components.Tools;

internal static class IdentifierGenerator
{
    private const string Encode32Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUV";
    private const int IdLength = 13;

    private static readonly SpanAction<char, long> GenerateImplDelegate = GenerateImpl;
    private static long _lastId = DateTime.UtcNow.Ticks;

    private static void GenerateImpl(Span<char> buffer, long id)
    {
        // Accessing the last item in the beginning elides range checks for all the subsequent items.
        buffer[12] = Encode32Chars[(int)id & 31];
        buffer[0] = Encode32Chars[(int)(id >> 60) & 31];
        buffer[1] = Encode32Chars[(int)(id >> 55) & 31];
        buffer[2] = Encode32Chars[(int)(id >> 50) & 31];
        buffer[3] = Encode32Chars[(int)(id >> 45) & 31];
        buffer[4] = Encode32Chars[(int)(id >> 40) & 31];
        buffer[5] = Encode32Chars[(int)(id >> 35) & 31];
        buffer[6] = Encode32Chars[(int)(id >> 30) & 31];
        buffer[7] = Encode32Chars[(int)(id >> 25) & 31];
        buffer[8] = Encode32Chars[(int)(id >> 20) & 31];
        buffer[9] = Encode32Chars[(int)(id >> 15) & 31];
        buffer[10] = Encode32Chars[(int)(id >> 10) & 31];
        buffer[11] = Encode32Chars[(int)(id >> 5) & 31];
    }

    public static string Next()
    {
        long id = Interlocked.Increment(ref _lastId);
        return string.Create(IdLength, id, GenerateImplDelegate);
    }
}
