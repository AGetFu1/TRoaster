using System.Text;
using TouchSocket.Core;
using TouchSocket.Sockets;
using TRoaster.Entity;
using TRoaster.Interface;
using TRoaster.Repository;
using TRoaster.Service;
using static TRoaster.Log.LogHelper;

namespace TRoaster.Core.Plugin
{
    public class ReadWorkCardPlugin : PluginBase, ITcpReceivedPlugin<TcpClient>
    {

        public async Task OnTcpReceived(TcpClient client, ReceivedDataEventArgs e)
        {
            ByteBlock byteBlock = e.ByteBlock;

            string msg = Encoding.UTF8.GetString(byteBlock.Buffer, 0, byteBlock.Len);

            bool isMatch = DataParser.IsMatch(msg, OperationCommands.BakeOrder);
            if (isMatch)
            {
                string deviceName = DataParser.GetDeviceName(msg);
                string workerData = DataParser.GetData(msg);
                String replyData = "";
                string proName="";
                string stepCode = "";
                string stepName = "";
                ProgramStepModel programStep;
                Log4netHelper.Info("获取到工单信息");
                Log4netHelper.Info("deviceName："+ deviceName);
                Log4netHelper.Info("工单信息:"+ workerData);
                Task.Run(() => {
                    //调用接口获取程序
                    try
                    { 
                        Log4netHelper.Info("调用接口获取程序/工序名称");
                        programStep = /*new ProgramStepModel { roastcode = "10-011-110", stepcode = "16", stepname = "16" ,stepcategorycode="14", stepcategory ="16"};*/ new MESInterface().GetRoasterProgram(deviceName, workerData);
                        if (programStep != null) {
                                proName = programStep.roastcode;
                                stepCode = programStep.stepcategorycode;
                                stepName = programStep.stepcategory;
                                Log4netHelper.Info("proName:"+ proName+"\t"+ "stepCode:"+ stepCode+"\t"+ "stepName:"+ stepName);
                        }
                       
                    }
                    catch (Exception ex)
                    {
                        replyData = deviceName+",recipe,error;";
                    }
                    if (string.IsNullOrEmpty(proName))
                    {
                        replyData = deviceName + ",recipe,error;";
                        Log4netHelper.Info("获取程序名称为空");
                        
                    }
                    else
                    {
                        Log4netHelper.Info("program:" + proName);
                        LotManager lotManager = new LotManager();
                        lotManager.AddLotInfo(deviceName, workerData, proName, stepCode,stepName);
                        LocalSaveLot(deviceName, workerData, proName, lotManager, stepCode, stepName);
                        
                        bool isExist = IsExistRecipe(proName);
                        if (isExist)
                        {
                            
                            replyData = deviceName + ",recipe," + proName + ";";
                        }
                        else {
                            Log4netHelper.Info("配方未在RTM维护，请检查！");
                            replyData = deviceName + ",recipe,error;";
                        }
                        FetchRecipeList(deviceName, proName);
                    }
                    client.Send(replyData);
                    Log4netHelper.Info("工单录入消息回复，返回配方：" + replyData);
                    
                });
                e.Handled = true;
            }
            await e.InvokeNext();
        }
        public bool IsExistRecipe(String programName) {
            RecipeInfoRepository recipeInfoRepository = new RecipeInfoRepository();
            return recipeInfoRepository.IsExistRecipe(programName);
        }
        public void  FetchRecipeList(String deviceName, String programName) {
            RecipeInfoRepository recipeInfoRepository =  new RecipeInfoRepository();
            LotManager lotManager = new LotManager();
            var repis = lotManager.GetRecipeList(deviceName);
            Task.Run(() => { 
                if (repis == null) {
                    List<RecipeModel> recipeModels = recipeInfoRepository.GetRecipeList(programName);

                    if (recipeModels != null && recipeModels.Count > 0)
                    {
                        lotManager.AddRecipes(deviceName, recipeModels);
                    }
                } 
            }); 
        }
        private static void LocalSaveLot(string deviceName, string workerData, string proName, LotManager lotManager,string stepCode,string stepName)
        {
            String workcardloc = "";
            String WorkCards = lotManager.IniReadValue("Lot", "WorkCards");
            if (string.IsNullOrEmpty(WorkCards))
            {
                workcardloc = workerData+"&"+stepCode+"&"+stepName+";";
            }
            else {
                workcardloc = WorkCards + workerData + "&" + stepCode + "&" + stepName+";";
            }
            lotManager.IniWriteValue("Lot", "DeviceName", deviceName);
            lotManager.IniWriteValue("Lot", "WorkCards", workcardloc);
            lotManager.IniWriteValue("Lot", "programName", proName);
            //lotManager.IniWriteValue("Lot", "stepCode", stepCode);
            //lotManager.IniWriteValue("Lot", "stepName", stepName);
        }
    }
}
