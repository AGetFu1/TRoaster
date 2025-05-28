
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeanLib.Entity.ParamVaild
{
    [SqlSugar.SugarTable("TMS_RECIPE_ITEM_LIST")]
    public class TmsRecipeItemList
    {
       public string ID         {get;set;}
       public string ITEM_NAME  {get;set;}
       public string ITEM_MAX   {get;set;}
       public string ITEM_MIN   {get;set;}
       public string ITEM_ONOFF { get; set; }
       public string ITEM_TYPE { get; set; }
    }
}
