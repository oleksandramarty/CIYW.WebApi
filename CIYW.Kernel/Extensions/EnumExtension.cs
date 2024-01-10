using CIYW.Models.Responses.Dictionary;

namespace CIYW.Kernel.Extensions;

public static class EnumExtension
{
    public static DictionaryResponse<string> ConvertEnumToDictionary<TEnum>()
    {
        List<string> enumNames = Enum.GetNames(typeof(TEnum)).ToList();
        List<DictionaryItemResponse<string>> items = new List<DictionaryItemResponse<string>>();

        foreach (var name in enumNames)
        {
            TEnum enumValue = (TEnum)Enum.Parse(typeof(TEnum), name);
            
            items.Add(new DictionaryItemResponse<string>
            {
                Id = $"{Convert.ToInt32(enumValue)}",
                Name = name,
                Hint = name
            });
        }

        return new DictionaryResponse<string>(items);
    }
}