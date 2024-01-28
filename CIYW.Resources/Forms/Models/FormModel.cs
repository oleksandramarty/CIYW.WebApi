using CIYW.Resources.Forms.Enum;

namespace CIYW.Resources.Forms.Models;

public class FormModel
{
    public FormTypeEnum Type { get; set; }
    public IList<FormItemModel> Controls { get; set; }
    public string Title { get; set; }
    public FormButtonModel SubmitButton { get; set; }
    public FormButtonModel? CancelButton { get; set; }
}