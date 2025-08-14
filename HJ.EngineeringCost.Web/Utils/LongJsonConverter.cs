using System.Text.Json;
using System.Text.Json.Serialization;

namespace HJ.EngineeringCost.Web.Utils;

/// <summary>
/// Long JSON序列化转换器
/// </summary>
public class LongJsonConverter : JsonConverter<long>
{
    /// <summary>
    /// 从JSON读取long值
    /// </summary>
    /// <param name="reader">JSON读取器</param>
    /// <param name="typeToConvert">目标类型</param>
    /// <param name="options">JSON序列化选项</param>
    /// <returns>long值</returns>
    public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // 处理字符串类型的JSON值
        if (reader.TokenType == JsonTokenType.String)
        {
            string stringValue = reader.GetString();

            // 处理空字符串，返回默认值0或根据业务需求处理
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return 0;
            }

            // 尝试将字符串转换为long
            if (long.TryParse(stringValue, out long result))
            {
                return result;
            }

            // 无法转换时抛出异常或返回默认值
            throw new JsonException($"无法将字符串 '{stringValue}' 转换为 long 类型。");
        }

        // 处理原始数字类型的JSON值
        if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetInt64();
        }

        // 处理其他类型的JSON值
        throw new JsonException($"不支持的JSON令牌类型: {reader.TokenType}");
    }

    /// <summary>
    /// 将long写入JSON
    /// </summary>
    /// <param name="writer">JSON写入器</param>
    /// <param name="value">long值</param>
    /// <param name="options">JSON序列化选项</param>
    public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}