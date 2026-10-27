// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EnsembleWeather.Serialization;

/// <summary>
/// Reads and writes an OpenAPI <c>format: date</c> (<c>yyyy-MM-dd</c>) as a <see cref="DateTime"/> with
/// <see cref="DateTimeKind.Unspecified"/>: a local calendar date, not an instant.
/// </summary>
internal sealed class CalendarDateConverter : JsonConverter<DateTime>
{
    private const string Format = "yyyy-MM-dd";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? text = reader.TokenType == JsonTokenType.String ? reader.GetString() : null;
        if (text is null
            || !DateTime.TryParseExact(text, Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
        {
            throw new JsonException("Expected a calendar date (yyyy-MM-dd).");
        }

        return DateTime.SpecifyKind(date, DateTimeKind.Unspecified);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(Format, CultureInfo.InvariantCulture));
}
