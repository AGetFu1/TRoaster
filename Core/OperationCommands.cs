using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRoaster.Core
{
    /// <summary>
    /// 定义一个枚举类型来表示不同的操作命令。
    /// </summary>
    public enum OperationCommands
    {
        /// <summary>
        /// 心跳信号
        /// </summary>
        Heartbeat = 1,

        /// <summary>
        /// 实时温度读取
        /// </summary>
        RealTimeTemperature = 2,

        /// <summary>
        /// 烘烤工单创建
        /// </summary>
        BakeOrder = 3,

        /// <summary>
        /// 开始录入工单
        /// </summary>
        StartWorkOrderEntry = 4,

        /// <summary>
        /// 录入工单完成
        /// </summary>
        WorkOrderEntryComplete = 5,

        /// <summary>
        /// 删除当前工单
        /// </summary>
        DeleteCurrentWorkOrder = 6,

        /// <summary>
        /// 删除全部工单
        /// </summary>
        DeleteAllWorkOrders = 7,

        /// <summary>
        /// 烘烤开始
        /// </summary>
        BakeStart = 8,

        /// <summary>
        /// 烘烤结束
        /// </summary>
        BakeEnd = 9,

        /// <summary>
        /// 恒温开始
        /// </summary>
        ConstanTempStart =10,

        /// <summary>
        /// 恒温结束
        /// </summary>
        ConstanTempEnd =11
    }
}
