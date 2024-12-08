namespace PSPOS.ApiService.Data.ValueConverters;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class UtcDateTimeConverter : ValueConverter<DateTime, DateTime>
{
    public UtcDateTimeConverter()
        : base(
            v => v.ToUniversalTime(),
            v => v.ToLocalTime()
        )
    {
    }
}