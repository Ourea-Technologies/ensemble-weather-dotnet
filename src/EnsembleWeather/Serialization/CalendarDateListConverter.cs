// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EnsembleWeather.Serialization;

/// <summary>
/// Reads and writes an array of OpenAPI <c>format: date</c> strings (<c>yyyy-MM-dd</c>) as calendar dates, as
/// <see cref="CalendarDateConverter"/> does one.
/// </summary>
internal sealed class CalendarDateListConverter : JsonConverter<IReadOnlyList<DateTime>>
{
    private static readonly CalendarDateConverter Date = new();

    public override IReadOnlyList<DateTime> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException("Expected an array of calendar dates.");
        }

        var dates = new List<DateTime>();
        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            dates.Add(Date.Read(ref reader, typeof(DateTime), options));
        }

        return dates;
    }

    public override void Write(Utf8JsonWriter writer, IReadOnlyList<DateTime> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (DateTime date in value)
        {
            Date.Write(writer, date, options);
        }

        writer.WriteEndArray();
    }
}
