using System.ComponentModel;
using CIYW.Models.Responses.Dictionary;

namespace CIYW.Kernel.Extensions;

public static class EnumExtension
{
    public static DictionaryResponse<string> ConvertEnumToDictionary<TEnum>() where TEnum: Enum
    {
        List<string> enumNames = Enum.GetNames(typeof(TEnum)).ToList();
        List<DictionaryItemResponse<string>> items = new List<DictionaryItemResponse<string>>();

        foreach (var name in enumNames)
        {
            TEnum enumValue = (TEnum)Enum.Parse(typeof(TEnum), name);

            items.Add(new DictionaryItemResponse<string>
            {
                Id = name,
                Name = enumValue.GetDescription(),
                Hint = enumValue.GetDescription()
            });
        }

        return new DictionaryResponse<string>(items);
    }
    
    public static string GetDescription(this Enum en)
    {
        if (en == null)
        {
            return "";
        }

        var type = en.GetType();
        var memInfo = type.GetMember(en.ToString());

        if (memInfo.Length <= 0)
        {
            return en.ToString();
        }

        var attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

        return attrs.Length > 0 ? ((DescriptionAttribute)attrs[0]).Description : en.ToString();
    }
}