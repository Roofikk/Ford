using Newtonsoft.Json;
using System;

public class DateConverter : JsonConverter<DateTime?>
{
    public override DateTime? ReadJson(JsonReader reader, Type objectType, DateTime? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.Value == null)
        {
            return null;
        }

        reader.DateFormatString = "yyyy-MM-dd";
        serializer.DateFormatString = "yyyy-MM-dd";
        return serializer.Deserialize<DateTime>(reader);
    }

    public override void WriteJson(JsonWriter writer, DateTime? value, JsonSerializer serializer)
    {
        string dateTimeStr = null;

        if (value != null)
        {
            dateTimeStr = value.Value.ToString("yyyy-MM-dd");
        }

        writer.WriteValue(dateTimeStr);
    }
}
