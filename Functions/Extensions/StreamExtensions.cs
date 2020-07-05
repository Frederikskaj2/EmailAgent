using System.IO;
using Newtonsoft.Json;

namespace Frederikskaj2.EmailAgent.Functions
{
    internal static class StreamExtensions
    {
        private static readonly JsonSerializer serializer = new JsonSerializer();

        public static void Serialize<T>(this Stream stream, T value)
        {
            using var writer = new StreamWriter(stream, leaveOpen: true);
            using var jsonWriter = new JsonTextWriter(writer);
            serializer.Serialize(jsonWriter, value);
            jsonWriter.Flush();
        }

        public static T Deserialize<T>(this Stream stream)
        {
            using var reader = new StreamReader(stream);
            using var jsonReader = new JsonTextReader(reader);
            return serializer.Deserialize<T>(jsonReader);
        }
    }
}
