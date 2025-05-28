
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeanLib.Entity.ParamVaild
{
    [SqlSugar.SugarTable("TMS_RECIPE_FORM_HIS")]
    public class TmsRecipeFormHis
    {
       public string ID            {get;set;}
       public string FORM_NAME_HIS {get;set;}
        public string FORM_TEMPLATE_HIS { get; set; }
        public string POSTFIX { get; set; }
        public string AUDIT_DATA { get; set; }

    }
}
