using CIYW.Const.Enums;
using CIYW.Resources.Forms.Enum;

namespace CIYW.Resources.Forms.Models;

public class FormItemModel
{
    public string Name { get; set; }
    public FormItemInputDateTypeEnum ValueType { get; set; }
    public FormItemInputType InputType { get; set; }
    public DictionaryTypeEnum SelectDataType { get; set; }
    public IList<FormItemValidatorModel> Validators { get; set; }
}