
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeanLib.Entity.ParamVaild
{
    [SqlSugar.SugarTable("TMS_RECIPE_FORM_LIST")]
    public class TmsRecipeFormList
    {
        public string ID { get; set; }
        public string FORM_NAME{ get; set; }
}
}
