using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace CIYW.Kernel.Errors;

public class ErrorMessage
{
    public ErrorMessage() { }

    public ErrorMessage(string message, int statuscode)
    {
        Message = message;
        StatusCode = statuscode;
    }

    public ErrorMessage(string message, int statuscode, IReadOnlyCollection<InvalidFieldInfo> invalidFields)
    {
        Message = message;
        StatusCode = statuscode;
        InvalidFields = invalidFields;
    }

    public IReadOnlyCollection<InvalidFieldInfo> InvalidFields { get; set; }
    public string Message { get; set; }
    public int StatusCode { get; set; }

    public string ToJson()
    {
        var jopt = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
            IgnoreNullValues = true,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        return JsonSerializer.Serialize(this, jopt);
    }

    public static ErrorMessage FromJson(string data)
    {
        return JsonSerializer.Deserialize<ErrorMessage>(data);
    }
}