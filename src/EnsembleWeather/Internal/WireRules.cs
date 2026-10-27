// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Globalization;
using System.Text;

namespace EnsembleWeather;

/// <summary>
/// The wire rules of BEHAVIOR.md sections 3.1, 4 and 7: threshold conversion and rounding, number formatting and
/// RFC 3986 percent-encoding.
/// </summary>
internal static class WireRules
{
    public static bool IsAmount(ForecastVariable variable)
        => variable == ForecastVariable.Precipitation || variable == ForecastVariable.Snowfall;

    public static void RequireKnownVariable(ForecastVariable variable, string parameterName)
    {
        if (!variable.IsKnown)
        {
            throw new ArgumentException(
                $"Unknown variable '{variable}'; probability terms take temperature, wind_speed, snow_level, precipitation or snowfall.",
                parameterName);
        }
    }

    public static void RequireUnitFits(ForecastVariable variable, Threshold threshold, string parameterName)
    {
        if (threshold.Unit is not ThresholdUnit unit)
        {
            return;
        }

        bool fits = unit switch
        {
            ThresholdUnit.Millimeters or ThresholdUnit.Inches => IsAmount(variable),
            ThresholdUnit.Celsius or ThresholdUnit.Fahrenheit or ThresholdUnit.Kelvin => variable == ForecastVariable.Temperature,
            ThresholdUnit.Meters or ThresholdUnit.Feet => variable == ForecastVariable.SnowLevel,
            ThresholdUnit.MetersPerSecond or ThresholdUnit.MilesPerHour => variable == ForecastVariable.WindSpeed,
            _ => false,
        };
        if (!fits)
        {
            throw new ArgumentException($"A threshold in {unit} does not measure '{variable}'.", parameterName);
        }
    }

    /// <summary>The served precision of a variable, in decimals (BEHAVIOR.md section 4).</summary>
    public static int Precision(ForecastVariable variable, UnitSystem units)
    {
        if (IsAmount(variable))
        {
            return units == UnitSystem.Imperial ? 3 : 2;
        }

        return variable == ForecastVariable.SnowLevel ? 0 : 1;
    }

    /// <summary>Converts the caller's threshold to the request's units with the exact factors of section 7.</summary>
    public static double ToRequestUnits(Threshold threshold, ForecastVariable variable, UnitSystem units)
    {
        if (threshold.Unit is not ThresholdUnit unit)
        {
            return threshold.Value;
        }

        bool imperial = units == UnitSystem.Imperial;
        double v = threshold.Value;
        switch (unit)
        {
            case ThresholdUnit.Millimeters:
                return imperial ? v / 25.4 : v;
            case ThresholdUnit.Inches:
                return imperial ? v : v * 25.4;
            case ThresholdUnit.Meters:
                return imperial ? v / 0.3048 : v;
            case ThresholdUnit.Feet:
                return imperial ? v : v * 0.3048;
            case ThresholdUnit.MetersPerSecond:
                return imperial ? v / 0.44704 : v;
            case ThresholdUnit.MilesPerHour:
                return imperial ? v : v * 0.44704;
            default:
                double celsius = unit switch
                {
                    ThresholdUnit.Fahrenheit => (v - 32) * 5 / 9,
                    ThresholdUnit.Kelvin => v - 273.15,
                    _ => v,
                };
                if (imperial)
                {
                    return unit == ThresholdUnit.Fahrenheit ? v : (celsius * 9 / 5) + 32;
                }

                if (units == UnitSystem.Kelvin)
                {
                    return unit == ThresholdUnit.Kelvin ? v : celsius + 273.15;
                }

                return celsius;
        }
    }

    /// <summary>
    /// Rounds as the server does: <c>round_half_away_from_zero(v × 10^d) / 10^d</c> in doubles (so 1.005 at d = 2 is
    /// 1.00, because 1.005 × 100 is 100.4999…), with <c>-0</c> made <c>0</c>. Never banker's rounding.
    /// </summary>
    public static double Round(double value, int decimals)
    {
        double scale = Math.Pow(10, decimals);
        double r = Math.Round(value * scale, MidpointRounding.AwayFromZero) / scale;
        return r == 0 ? 0 : r;
    }

    /// <summary>A rounded threshold printed with exactly <paramref name="decimals"/> decimals.</summary>
    public static string FormatFixed(double rounded, int decimals)
        => rounded.ToString("F" + decimals.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);

    /// <summary>The shortest decimal that round-trips, never with an exponent (coordinates, radii).</summary>
    public static string FormatNumber(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Numbers sent to the API must be finite.");
        }

        if (value == 0)
        {
            return "0";
        }

        string r = value.ToString("R", CultureInfo.InvariantCulture);
        int e = r.IndexOfAny(['E', 'e']);
        if (e < 0)
        {
            return r;
        }

        // Expand d.dddE±x into plain digits.
        int exponent = int.Parse(r.Substring(e + 1), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture);
        string mantissa = r.Substring(0, e);
        bool negative = mantissa[0] == '-';
        if (negative)
        {
            mantissa = mantissa.Substring(1);
        }

        int dot = mantissa.IndexOf('.');
        string digits = dot < 0 ? mantissa : mantissa.Remove(dot, 1);
        int point = (dot < 0 ? mantissa.Length : dot) + exponent;
        string plain;
        if (point <= 0)
        {
            plain = "0." + new string('0', -point) + digits;
        }
        else if (point >= digits.Length)
        {
            plain = digits + new string('0', point - digits.Length);
        }
        else
        {
            plain = digits.Substring(0, point) + "." + digits.Substring(point);
        }

        return negative ? "-" + plain : plain;
    }

    /// <summary>
    /// RFC 3986 percent-encoding: unreserved characters (<c>A-Z a-z 0-9 - . _ ~</c>) as they are, every other UTF-8
    /// byte as <c>%XX</c> in uppercase hex (space is <c>%20</c>, never <c>+</c>). <paramref name="keep"/> lists extra
    /// characters sent literally (<c>:</c> and <c>,</c> in <c>prob</c> and <c>totals</c>).
    /// </summary>
    public static string PercentEncode(string value, string keep = "")
    {
        var builder = new StringBuilder(value.Length + 8);
        foreach (byte b in Encoding.UTF8.GetBytes(value))
        {
            char c = (char)b;
            bool unreserved = (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9')
                || c == '-' || c == '.' || c == '_' || c == '~';
            if (unreserved || (b < 0x80 && keep.IndexOf(c) >= 0))
            {
                builder.Append(c);
            }
            else
            {
                builder.Append('%').Append(b.ToString("X2", CultureInfo.InvariantCulture));
            }
        }

        return builder.ToString();
    }

    public static string FormatBool(bool value) => value ? "true" : "false";

    public static string FormatInt(long value) => value.ToString(CultureInfo.InvariantCulture);
}
