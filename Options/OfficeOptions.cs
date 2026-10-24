namespace OfficeProductionApi.Options;

public sealed class OfficeOptions
{
    public const string SectionName = "Office";

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public int ReportRetentionDays { get; set; }

    public int MaximumReportTitleLength { get; set; }
}