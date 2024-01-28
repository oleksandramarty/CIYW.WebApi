using CIYW.Resources.Forms.Enum;
using CIYW.Resources.Forms.Models;

namespace CIYW.Resources.Forms;

public class FormItemValidatorModelBuilder
{
    private readonly IList<FormItemValidatorModel> _validators;

    public FormItemValidatorModelBuilder()
    {
        _validators = new List<FormItemValidatorModel>();
    }
    
    public FormItemValidatorModelBuilder WithMinLen(string value)
    {
        _validators.Add(new FormItemValidatorModel(FormItemValidatorTypeEnum.Min, value));
        return this;
    }
    
    public FormItemValidatorModelBuilder WithMaxLen(string value)
    {
        _validators.Add(new FormItemValidatorModel(FormItemValidatorTypeEnum.MaxLen, value));
        return this;
    }
    
    public FormItemValidatorModelBuilder WithRequired()
    {
        _validators.Add(new FormItemValidatorModel(FormItemValidatorTypeEnum.Required));
        return this;
    }
    
    public FormItemValidatorModelBuilder WithMin(string value)
    {
        _validators.Add(new FormItemValidatorModel(FormItemValidatorTypeEnum.Min, value));
        return this;
    }
    
    public FormItemValidatorModelBuilder WithMax(string value)
    {
        _validators.Add(new FormItemValidatorModel(FormItemValidatorTypeEnum.Max, value));
        return this;
    }

    public FormItemValidatorModelBuilder WithEmail()
    {
        _validators.Add(new FormItemValidatorModel(FormItemValidatorTypeEnum.Email));
        return this;
    }
    
    public FormItemValidatorModelBuilder WithAtLeastOne(string value)
    {
        _validators.Add(new FormItemValidatorModel(FormItemValidatorTypeEnum.AtLeastOne, value));
        return this;
    }
    
    public FormItemValidatorModelBuilder WithPattern(string value)
    {
        _validators.Add(new FormItemValidatorModel(FormItemValidatorTypeEnum.Pattern, value));
        return this;
    }

    public IList<FormItemValidatorModel> Build()
    {
        return _validators;
    }
}