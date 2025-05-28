using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TRoaster.Log.LogHelper;
using TRoaster.Core;
using TRoaster.Entity;
using BeanLib.Entity.ParamVaild;
using TRoaster.Helper;

namespace TRoaster.Repository
{
    public class RecipeInfoRepository
    {
        public bool IsExistRecipe(String programName) {
            try
            {
                SqlSugarClient sqlClient = RTMDBConnection.GetDBConnection();

                var query = sqlClient.Queryable<TmsRecipeFormHis>()
                 .Where(t3 => t3.FORM_NAME_HIS == programName).Count() ;

                if (query > 0)
                {
                    return true;
                }
                else { 
                    return false;
                }  
            }
            catch (Exception ex)
            {
                Log4netHelper.Info("检查烘箱配方出现异常：" + ex.Message);
                return false;
            }
        }
        public List<RecipeModel> GetRecipeList(String programName)
        {
            try
            {
                SqlSugarClient sqlClient = RTMDBConnection.GetDBConnection();

                var query = sqlClient.Queryable<TmsRecipeItemList, TmsRecipeFormItemHis, TmsRecipeFormHis, TmsRecipeFormTypeHis>(
                     (t1, t2, t3, t4) => new object[]{
                        JoinType.Left,t1.ID == t2.FORM_ITEM_NAME_ID_HIS,
                        JoinType.Left,t2.FORM_NAME_ID_HIS == t3.ID,
                        JoinType.Left,t2.FORM_TYPE_NAME_ID_HIS == t4.FORM_TYPE_NAME_ID_HIS,
                        JoinType.Left,t4.FORM_ID_HIS == t3.ID
                     })
                 .Where((t1, t2, t3, t4) => t3.FORM_NAME_HIS == programName)
                 //.Where((t1, t2, t3, t4) => t3.POSTFIX == ".txt")
                 //.Where((t1, t2, t3, t4) => t3.FORM_TEMPLATE_HIS == "OVEN-TEMPLATE-125-04")
                 .Where((t1, t2, t3, t4) => t3.AUDIT_DATA == "1")
                 .Where((t1, t2, t3, t4) => t1.ITEM_ONOFF == "ON");
                var result = query.Select((t1, t2, t3, t4) => new RecipeModel
                {
                    RecipeName = t3.FORM_NAME_HIS,
                    UpLimit = t1.ITEM_MAX, 
                    DownLimit = t1.ITEM_MIN,
                    ItemName = t1.ITEM_NAME
                    
                }).ToList();

                return result;

            }
            catch (Exception ex)
            { 
                Log4netHelper.Info("拉取烘箱参数数据出现异常：" + ex.Message);
                return null;
            }
        }
    }
}
