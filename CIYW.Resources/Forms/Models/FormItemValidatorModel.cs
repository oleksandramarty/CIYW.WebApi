using System.Runtime.Serialization;
using CIYW.Resources.Forms.Enum;

namespace CIYW.Resources.Forms.Models;

[DataContract]
public class FormItemValidatorModel
{
    public FormItemValidatorModel(FormItemValidatorTypeEnum type)
    {
        ValidatorType = type;
    }

    public FormItemValidatorModel(FormItemValidatorTypeEnum type, string value)
    : this(type)
    {
        Value = value;
    }
    [DataMember]
    public FormItemValidatorTypeEnum ValidatorType { get; set; }

    [DataMember]
    public string Value { get; set; }
}
