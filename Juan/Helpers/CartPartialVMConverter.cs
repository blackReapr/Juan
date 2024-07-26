using Juan.ViewModels;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Juan.Helpers;

public class CartPartialVMConverter : JsonConverter<CartPartialVM>
{
    public override CartPartialVM Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var cartPartialVM = new CartPartialVM();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                string propertyName = reader.GetString();
                reader.Read();

                switch (propertyName)
                {
                    case "Coupon":
                        cartPartialVM.Coupon = reader.GetString();
                        break;
                    case "DiscountRate":
                        cartPartialVM.DiscountRate = reader.GetDecimal();
                        break;
                    case "Items":
                        var items = JsonSerializer.Deserialize<List<CartVM>>(ref reader, options);
                        cartPartialVM.AddRange(items);
                        break;
                }
            }
        }

        return cartPartialVM;
    }

    public override void Write(Utf8JsonWriter writer, CartPartialVM value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("Coupon");
        JsonSerializer.Serialize(writer, value.Coupon, options);

        writer.WritePropertyName("DiscountRate");
        JsonSerializer.Serialize(writer, value.DiscountRate, options);

        writer.WritePropertyName("Items");
        JsonSerializer.Serialize(writer, (List<CartVM>)value, options);

        writer.WriteEndObject();
    }

    public static string Serialize(CartPartialVM value)
    {
        var options = new JsonSerializerOptions
        {
            Converters = { new CartPartialVMConverter() },
            WriteIndented = true
        };
        return JsonSerializer.Serialize(value, options);
    }

    public static CartPartialVM? Deserialize(string? json)
    {
        if (json == null) return null;
        var options = new JsonSerializerOptions
        {
            Converters = { new CartPartialVMConverter() }
        };
        return JsonSerializer.Deserialize<CartPartialVM>(json, options);
    }
}
