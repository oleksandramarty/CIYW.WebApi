using CIYW.Const.Enums;
using CIYW.Kernel.Extensions;
using CIYW.Resources.Forms.Enum;
using CIYW.Resources.Forms.Models;

namespace CIYW.Resources.Forms;

public class FormItemModelBuilder
{
    private readonly FormItemModel _formItem;

    public FormItemModelBuilder(string name, string placeholder = null)
    {
        _formItem = new FormItemModel
        {
            Name = name,
            Placeholder = placeholder,
            ValueType = FormItemInputDateTypeEnum.String,
            InputType = FormItemInputType.Text,
            InputTypeStr = FormItemInputType.Text.GetDescription(),
            WrapperType = FormItemWrapperTypeEnum.Full,
            Validators = new List<FormItemValidatorModel>()
        };
    }
    
    public FormItemModelBuilder WithPlaceholder(string placeholder)
    {
        _formItem.Placeholder = placeholder;
        return this;
    }
    
    public FormItemModelBuilder WithInputType(FormItemInputType inputType)
    {
        _formItem.InputType = inputType;
        _formItem.InputTypeStr = inputType.GetDescription();
        return this;
    }
    
    public FormItemModelBuilder WithDateType(FormItemInputDateTypeEnum valueType)
    {
        _formItem.ValueType = valueType;
        return this;
    }
    
    public FormItemModelBuilder WithWrapper(FormItemWrapperTypeEnum wrapperType)
    {
        _formItem.WrapperType = wrapperType;
        return this;
    }

    public FormItemModelBuilder WithSelectDataType(DictionaryTypeEnum selectDataType)
    {
        _formItem.SelectDataType = selectDataType;
        return this;
    }

    public FormItemModelBuilder WithLabel(string label)
    {
        _formItem.Label = label;
        return this;
    }

    public FormItemModelBuilder WithDefaultValue(object defaultValue)
    {
        _formItem.DefaultValue = defaultValue;
        return this;
    }

    public FormItemModelBuilder WithInline(bool inline)
    {
        _formItem.Inline = inline;
        return this;
    }

    public FormItemModelBuilder WithOkHint(FormItemOkHintModel okHint)
    {
        _formItem.OkHint = okHint;
        return this;
    }

    public FormItemModelBuilder WithValidator(FormItemValidatorModel validator)
    {
        _formItem.Validators.Add(validator);
        return this;
    }
    public FormItemModelBuilder WithValidators(IList<FormItemValidatorModel> validators)
    {
        _formItem.Validators = validators;
        return this;
    }

    public FormItemModel Build()
    {
        return _formItem;
    }
}