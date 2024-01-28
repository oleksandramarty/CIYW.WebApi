using CIYW.Const.Enums;
using CIYW.Resources.Forms.Enum;

namespace CIYW.Resources.Forms.Models;

public class FormItemModel
{
    public string Name { get; set; }
    public object DefaultValue { get; set; }
    public FormItemInputDateTypeEnum ValueType { get; set; }
    public FormItemInputType InputType { get; set; }
    public string InputTypeStr { get; set; }
    public DictionaryTypeEnum? SelectDataType { get; set; }
    public IList<FormItemValidatorModel> Validators { get; set; }
    public string? Label { get; set; }
    public string? Placeholder { get; set; }
    public bool? Inline { get; set; }
    public FormItemOkHintModel? OkHint { get; set; }
    public FormItemWrapperTypeEnum WrapperType { get; set; }
}
