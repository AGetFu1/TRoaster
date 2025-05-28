using TRoaster.Core;
using TRoaster.Entity;
using TRoaster.Helper;
using TRoaster.Repository;
using static TRoaster.Log.LogHelper;

namespace TRoaster.Service
{
    public class LotManager
    {
        private String _deviceName { get; }
        RoasterClient _client;
        LotInfoModel lotInfoModel;
        string path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        INIFile iniFile;
        public LotManager()
        {
            iniFile = new INIFile(path + "lotinfo.ini");
        }
       
        //添加工卡至当前批次中
        public bool AddLotInfo(string deviceName, String workOrder,String programName,string stepcode,string stepName)
        {
            if (!GlobleCache.lotinfoDic.ContainsKey(deviceName))
            {
                lotInfoModel = new LotInfoModel();
                 
                List<CardModel> cardModels = new List<CardModel>();
                lotInfoModel.DeviceName = deviceName;
                lotInfoModel.programName = programName; 
                 
                 
                CardModel cardModel = new CardModel();
                cardModel.WordCard = workOrder;
                cardModel.stepcode = stepcode ;
                cardModel.stepname = stepName ;
                cardModels.Add(cardModel);
                lotInfoModel.Cards = cardModels;
                GlobleCache.lotinfoDic.Add(deviceName, lotInfoModel);
                return true;
            }
            else {
                lotInfoModel = GetLotInfo(deviceName);
                if (lotInfoModel == null)
                {
                    List<string> workcards = new List<string>();
                    List<CardModel> cardModels = new List<CardModel>();
                    lotInfoModel = new LotInfoModel();
                    lotInfoModel.DeviceName = deviceName;
                    lotInfoModel.programName = programName;
                    
                    workcards.Add(workOrder);
                     
                    CardModel cardModel = new CardModel();
                    cardModel.stepcode = stepcode;
                    cardModel.stepname = stepName;
                    cardModel.WordCard = workOrder;
                    cardModels.Add(cardModel);
                    if (lotInfoModel.Cards != null)
                    {
                        lotInfoModel.Cards = cardModels;
                    }
                    else {
                        lotInfoModel.Cards.AddRange(cardModels);
                    }
                    
                    GlobleCache.lotinfoDic.Add(deviceName, lotInfoModel);
                    return true;
                }
                else { 
                    lotInfoModel.DeviceName = deviceName;
                    lotInfoModel.programName = programName;
                    List<CardModel> cardModels = new List<CardModel>();
                    CardModel cardModel = new CardModel();
                    cardModel.stepcode = stepcode;
                    cardModel.stepname = stepName; 
                    cardModel.WordCard = workOrder;
                    if (lotInfoModel.Cards == null)
                    {
                        cardModels.Add(cardModel);
                        lotInfoModel.Cards = cardModels;
                    }
                    else
                    {
                        lotInfoModel.Cards.Add(cardModel);
                    } 
                    return true;
                }
            }
            return false;
        }
        // 查询设备
        public LotInfoModel GetLotInfo(string key)
        {
            if (GlobleCache.lotinfoDic.ContainsKey(key))
            {
                return GlobleCache.lotinfoDic[key];
            }
            return null;
        }
        public void SetIds(string deviceName,List<CardModel> ids) {
            if (GlobleCache.lotinfoDic.ContainsKey(deviceName))
            {
                GlobleCache.lotinfoDic[deviceName].Cards = ids;
            }
        }
        public List<CardModel> GetIds(string deviceName)
        {
            if (GlobleCache.lotinfoDic.ContainsKey(deviceName))
            {
                return GlobleCache.lotinfoDic[deviceName].Cards;
            }
            return null ;
        }
        // 删除设备
        public bool RemoveLotInfo(string key)
        {
            return GlobleCache.lotinfoDic.Remove(key) && GlobleCache.RecipeListDic.Remove(key);
        }
        public bool RemoveCurrentWorkCard(string key,string workCard)
        {
            if (GlobleCache.lotinfoDic.ContainsKey(key))
            {
                if (GlobleCache.lotinfoDic.TryGetValue(key, out LotInfoModel lotInfo))
                {
                    lotInfo.Cards.RemoveAll(f=>f.WordCard == workCard);
                    return true;
                }
            }
            return false;
        }
        public void SetBakeBeginDate(String DeviceName) {
            if (GlobleCache.lotinfoDic.ContainsKey(DeviceName)) {
                GlobleCache.lotinfoDic[DeviceName].BakeBeginTime = DateTime.Now;
            }
        }
        public void SetConstTempBeginDate(String DeviceName)
        {
            if (GlobleCache.lotinfoDic.ContainsKey(DeviceName))
            {
                GlobleCache.lotinfoDic[DeviceName].ConstTempBeginTime = DateTime.Now;
            }
        }
        public void SetBakeEndDate(String DeviceName)
        {
            if (GlobleCache.lotinfoDic.ContainsKey(DeviceName))
            {
                GlobleCache.lotinfoDic[DeviceName].BakeEndTime = DateTime.Now;
            }
        }
        public void SetConstTempEndDate(String DeviceName)
        {
            if (GlobleCache.lotinfoDic.ContainsKey(DeviceName))
            {
                GlobleCache.lotinfoDic[DeviceName].ConstTempEndTime = DateTime.Now;
            }
        }
        public void SetResult(String DeviceName, String Reason) {
            if (GlobleCache.lotinfoDic.ContainsKey(DeviceName))
            {
                String tempRes = GlobleCache.lotinfoDic[DeviceName].Result;
                if (!string.IsNullOrEmpty(tempRes))
                {
                    GlobleCache.lotinfoDic[DeviceName].Result = tempRes + ";" + Reason;
                }
                else {
                    GlobleCache.lotinfoDic[DeviceName].Result = Reason;
                }
            }
        }
        public void SetStage(String DeviceName, String Stage)
        {
            Log4netHelper.Info("设备号："+ DeviceName+", 阶段："+ Stage);
            if (GlobleCache.lotinfoDic.ContainsKey(DeviceName))
            {
                GlobleCache.lotinfoDic[DeviceName].Stage = Stage;
            }
            else {
                Log4netHelper.Info("不存在批次信息，请检查是否开批成功");
            }
        }
        public String GetStage(String DeviceName)
        {
            if (GlobleCache.lotinfoDic.ContainsKey(DeviceName))
            {
                String Stage = GlobleCache.lotinfoDic[DeviceName].Stage;

                return Stage;
            }
            return "";
        }
        public String GetResult(String DeviceName)
        {
            if (GlobleCache.lotinfoDic.ContainsKey(DeviceName))
            {
                String tempRes = GlobleCache.lotinfoDic[DeviceName].Result;
                
                return tempRes;
            }
            return "";
        }
        public List<RecipeModel> GetRecipeList(String DeviceName) {
            if (GlobleCache.lotinfoDic.ContainsKey(DeviceName))
            {
                if (GlobleCache.RecipeListDic.ContainsKey(DeviceName))
                {
                    return GlobleCache.RecipeListDic[DeviceName];
                }
                else {
                    String programName = "";
                    RecipeInfoRepository recipeInfoRepository = new RecipeInfoRepository();
                    LotInfoModel lotInfoModel = GlobleCache.lotinfoDic[DeviceName];
                    if (lotInfoModel != null) {
                        programName = lotInfoModel.programName;
                        LotManager lotManager = new LotManager();

                        List<RecipeModel> recipeModels = recipeInfoRepository.GetRecipeList(programName);

                        if (recipeModels != null && recipeModels.Count > 0)
                        {
                            lotManager.AddRecipes(DeviceName, recipeModels);
                        }
                    } 
                }
            }
            else {
                Log4netHelper.Info("不存在批次信息："+DeviceName);
            }
            return null;
        }
        public void AddRecipes(String DeviceName, List<RecipeModel> recipes)
        {
            GlobleCache.RecipeListDic[DeviceName] = recipes;
        }
        public string IniReadValue(string Section, string Key)
        {
            string temp = iniFile.IniReadValue(Section, Key);
            return temp.ToString();
        }
        public void IniWriteValue(string Section, string Key, string Value)
        {
            iniFile.IniWriteValue(Section, Key, Value);
        }

        internal void RemoveRecipeList(string deviceName)
        {
            if (GlobleCache.lotinfoDic.ContainsKey(deviceName))
            {
                if (GlobleCache.RecipeListDic.ContainsKey(deviceName))
                {
                     GlobleCache.RecipeListDic.Remove(deviceName);
                } 
            } 
        }
    }
}
