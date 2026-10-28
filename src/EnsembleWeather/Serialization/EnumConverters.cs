// SPDX-License-Identifier: BSD-2-Clause

namespace EnsembleWeather.Serialization;

internal sealed class AstronomicalSeasonConverter : WireStringConverter<AstronomicalSeason>
{
    protected override AstronomicalSeason Create(string value) => new(value);

    protected override string GetValue(AstronomicalSeason value) => value.Value;
}

internal sealed class AttributionSourceKindConverter : WireStringConverter<AttributionSourceKind>
{
    protected override AttributionSourceKind Create(string value) => new(value);

    protected override string GetValue(AttributionSourceKind value) => value.Value;
}

internal sealed class ConditionIconConverter : WireStringConverter<ConditionIcon>
{
    protected override ConditionIcon Create(string value) => new(value);

    protected override string GetValue(ConditionIcon value) => value.Value;
}

internal sealed class CurrentConditionsSourceConverter : WireStringConverter<CurrentConditionsSource>
{
    protected override CurrentConditionsSource Create(string value) => new(value);

    protected override string GetValue(CurrentConditionsSource value) => value.Value;
}

internal sealed class ElevationSourceConverter : WireStringConverter<ElevationSource>
{
    protected override ElevationSource Create(string value) => new(value);

    protected override string GetValue(ElevationSource value) => value.Value;
}

internal sealed class ForecastVariableConverter : WireStringConverter<ForecastVariable>
{
    protected override ForecastVariable Create(string value) => new(value);

    protected override string GetValue(ForecastVariable value) => value.Value;
}

internal sealed class GapFlowStatusConverter : WireStringConverter<GapFlowStatus>
{
    protected override GapFlowStatus Create(string value) => new(value);

    protected override string GetValue(GapFlowStatus value) => value.Value;
}

internal sealed class InversionKindConverter : WireStringConverter<InversionKind>
{
    protected override InversionKind Create(string value) => new(value);

    protected override string GetValue(InversionKind value) => value.Value;
}

internal sealed class LocationTypeConverter : WireStringConverter<LocationType>
{
    protected override LocationType Create(string value) => new(value);

    protected override string GetValue(LocationType value) => value.Value;
}

internal sealed class MembersViewConverter : WireStringConverter<MembersView>
{
    protected override MembersView Create(string value) => new(value);

    protected override string GetValue(MembersView value) => value.Value;
}

internal sealed class MoonPhaseConverter : WireStringConverter<MoonPhase>
{
    protected override MoonPhase Create(string value) => new(value);

    protected override string GetValue(MoonPhase value) => value.Value;
}

internal sealed class NowcastSourceConverter : WireStringConverter<NowcastSource>
{
    protected override NowcastSource Create(string value) => new(value);

    protected override string GetValue(NowcastSource value) => value.Value;
}

internal sealed class PrecipitationKindConverter : WireStringConverter<PrecipitationKind>
{
    protected override PrecipitationKind Create(string value) => new(value);

    protected override string GetValue(PrecipitationKind value) => value.Value;
}

internal sealed class ProbabilityAggregationConverter : WireStringConverter<ProbabilityAggregation>
{
    protected override ProbabilityAggregation Create(string value) => new(value);

    protected override string GetValue(ProbabilityAggregation value) => value.Value;
}

internal sealed class ProbabilityOperatorConverter : WireStringConverter<ProbabilityOperator>
{
    protected override ProbabilityOperator Create(string value) => new(value);

    protected override string GetValue(ProbabilityOperator value) => value.Value;
}

internal sealed class StormOrientationConverter : WireStringConverter<StormOrientation>
{
    protected override StormOrientation Create(string value) => new(value);

    protected override string GetValue(StormOrientation value) => value.Value;
}

internal sealed class UnitSystemConverter : WireStringConverter<UnitSystem>
{
    protected override UnitSystem Create(string value) => new(value);

    protected override string GetValue(UnitSystem value) => value.Value;
}

internal sealed class WeatherConditionConverter : WireStringConverter<WeatherCondition>
{
    protected override WeatherCondition Create(string value) => new(value);

    protected override string GetValue(WeatherCondition value) => value.Value;
}

internal sealed class WindStatusConverter : WireStringConverter<WindStatus>
{
    protected override WindStatus Create(string value) => new(value);

    protected override string GetValue(WindStatus value) => value.Value;
}

internal sealed class NullReasonConverter : WireStringConverter<NullReason>
{
    protected override NullReason Create(string value) => new(value);

    protected override string GetValue(NullReason value) => value.Value;
}

internal sealed class WindowTotalAggregationConverter : WireStringConverter<WindowTotalAggregation>
{
    protected override WindowTotalAggregation Create(string value) => new(value);

    protected override string GetValue(WindowTotalAggregation value) => value.Value;
}
