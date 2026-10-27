// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EnsembleWeather.Serialization;

/// <summary>
/// Reads and writes an extensible enumeration as its wire string, keeping values this SDK does not know.
/// </summary>
/// <typeparam name="T">The extensible enumeration.</typeparam>
internal abstract class WireStringConverter<T> : JsonConverter<T>
    where T : struct
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected a string for {typeof(T).Name}, got {reader.TokenType}.");
        }

        return Create(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        => writer.WriteStringValue(GetValue(value));

    protected abstract T Create(string value);

    protected abstract string GetValue(T value);
}
