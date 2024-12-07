using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace PSPOS.ApiService.Data.ValueConverters;

public class UtcNullableDateTimeConverter : ValueConverter<DateTime?, DateTime?>
{
    public UtcNullableDateTimeConverter()
        : base(
            v => v.HasValue ? v.Value.ToUniversalTime() : v,
            v => v.HasValue ? v.Value.ToLocalTime() : v
        )
    {
    }
}