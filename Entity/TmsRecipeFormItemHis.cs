
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeanLib.Entity.ParamVaild
{
    [SqlSugar.SugarTable( "TMS_RECIPE_FORM_ITEM_HIS")]
    public class TmsRecipeFormItemHis
    {
       public string FORM_ITEM_NAME_ID_HIS { get; set; }
       public string FORM_NAME_ID_HIS { get; set; }
       public string FORM_TYPE_NAME_ID_HIS { get; set; }
    }
}
