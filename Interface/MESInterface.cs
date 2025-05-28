using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using static RTM.ProductionServiceClient;
using static TRoaster.Log.LogHelper;
using ServiceReference1;
using TRoaster.Entity;
using TRoaster.Helper;

namespace TRoaster.Interface
{
    public class MESInterface
    {
 
         
        WebService_For_RtmMesSoapClient _service = new WebService_For_RtmMesSoapClient(WebService_For_RtmMesSoapClient.EndpointConfiguration.WebService_For_RtmMesSoap);
        TestProgramServiceClient testProgramServiceClient = new TestProgramServiceClient(TestProgramServiceClient.EndpointConfiguration.BasicHttpBinding_ITestProgramService);
        public GetLotInfoModeEntity[] GetLotInfo(String workCard) {
            //TA210420106-03
            return _service.GetLotInfoStringAsync(workCard).Result;
        }
        public TestprogramEntity[] GetProgramInfo(string LotID, string Equipmenttype, string Storeequipment, string ProcessStepCode)
        {
            TestprogramEntity[] testprograms = _service.GetTestprogramAsync(LotID, Equipmenttype, Storeequipment, ProcessStepCode).Result;
            return testprograms;
        }
        public String HoldLot(string LotID, string Storeequipment, string ProcessStepCode,string BakeBeginDate,string BakeEndDate,String tempBeginDate,String Result,String msg,String tempEndDate)
        {
            Task<String> resultTask= testProgramServiceClient.RoastCardSetMsgAsync(LotID, Storeequipment, ProcessStepCode, BakeBeginDate, tempBeginDate, BakeEndDate, Result, msg, tempEndDate);
            String result = resultTask.Result;
            return result;
        }

        public ProgramStepModel GetRoasterProgram(String eqpId,String cardId) {
            Task<ResultEntity>  resultEntityTask = _service.RoastProgramAsync(eqpId,cardId);
            ResultEntity resultEntity = resultEntityTask.Result;
            if (resultEntity != null)
            {
                if (resultEntity.ErrorCode != "0")
                {
                    Log4netHelper.Info("查询PWS接口数据报错：" + resultEntity.ErrorMessage);
                     
                }
                else {
                    return ConvertJson.DeserializeJsonToObject<ProgramStepModel>(resultEntity.ErrorMessage);
                }
            }
            else {
                Log4netHelper.Info("PWS接口返回数据为 NULL");
            }
            return null;
        }
        
    }
}
