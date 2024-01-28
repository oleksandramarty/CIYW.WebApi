using CIYW.Resources.Forms.Enum;

namespace CIYW.Resources.Forms.Models;

public class FormModel
{
    public FormTypeEnum Type { get; set; }
    public FormRequestTypeEnum ModelName { get; set; }
    public IList<FormItemModel> Controls { get; set; }
}