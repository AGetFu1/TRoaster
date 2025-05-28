
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeanLib.Entity.ParamVaild
{
    [SqlSugar.SugarTable("TMS_RECIPE_FORM_TYPE_HIS")]
    public class TmsRecipeFormTypeHis
    {
        public string FORM_TYPE_NAME_ID_HIS { get; set; }
        public string FORM_TYPE_NAME_HIS { get; set; }
        public string FORM_ID_HIS { get; set; }
    }
}
