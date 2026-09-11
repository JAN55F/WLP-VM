using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using HalconDotNet;

namespace VMPro
{
    /// <summary>
    /// HALCON 对象序列化护栏。
    ///
    /// 背景：部分工具类与 ToolIO.value 会持有"空 HALCON 对象"——例如形状/模板匹配工具
    /// 构造函数里 GenEmptyObj 出来的空区域，或工具尚未运行时缓存的空输出对象。
    /// 它们的 object-ID 为 NULL，BinaryFormatter 直接序列化时会调用 HALCON 的
    /// serialize_object 并抛出 "HALCON error #4056: object-ID is NULL"，
    /// 导致保存项目（Project.SaveProjectToPath）、保存流程（*.job）直接崩溃。
    ///
    /// 方案：为 HObject / HTuple 注册 ISerializationSurrogate——
    /// 空对象只写一个标记（完全不触碰 HALCON 序列化），非空对象委托 HALCON
    /// 自身的 ISerializable 实现照常入库（图像/区域/元组内容与体积不变）；
    /// 反序列化时空标记还原为空实例，非空数据走 HALCON 原生构造。
    /// </summary>
    internal static class HalconSerializationGuard
    {
        private const string EmptyMarker = "__WLP_HalconEmpty";

        /// <summary>
        /// 创建带 HALCON 空对象护栏的 BinaryFormatter。
        /// 所有序列化/反序列化 工程对象图（Project/Engine/Job/ToolInfo/ToolBase）的
        /// 调用点都应使用本方法，而不是直接 new BinaryFormatter()。
        /// </summary>
        internal static IFormatter CreateFormatter()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            SurrogateSelector selector = new SurrogateSelector();
            StreamingContextStates states = StreamingContextStates.All;
            selector.AddSurrogate(typeof(HObject), new StreamingContext(states), new HalconObjectSurrogate());
            selector.AddSurrogate(typeof(HTuple), new StreamingContext(states), new HalconTupleSurrogate());
            formatter.SurrogateSelector = selector;
            return formatter;
        }

        /// <summary>
        /// HObject 护栏：空对象（object-ID 为 NULL 或 CountObj 为 0）写标记，
        /// 非空对象走 HALCON 原生 ISerializable。
        /// 注意：SurrogateSelector 按精确类型匹配，HImage/HRegion 等子类不会被拦截，
        /// 仍走原生序列化，避免反序列化时类型不匹配。
        /// </summary>
        private sealed class HalconObjectSurrogate : ISerializationSurrogate
        {
            void ISerializationSurrogate.GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                HObject h = (HObject)obj;
                bool isEmpty;
                try
                {
                    isEmpty = h.CountObj() == 0;
                }
                catch
                {
                    // object-ID 为 NULL 的未初始化对象连 CountObj 都会抛 #4056，按空处理
                    isEmpty = true;
                }
                if (isEmpty)
                {
                    info.AddValue(EmptyMarker, true);
                    return;
                }
                ((ISerializable)h).GetObjectData(info, context);
            }

            object ISerializationSurrogate.SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                bool isEmpty = false;
                try
                {
                    isEmpty = info.GetBoolean(EmptyMarker);
                }
                catch
                {
                    isEmpty = false;
                }
                if (isEmpty)
                {
                    // 读回为“空但有效”的对象（GenEmptyObj，CountObj==0 可安全调用），
                    // 与运行期空状态一致；HALCON 运行时不可用时降级为未初始化实例。
                    try
                    {
                        HObject empty;
                        HOperatorSet.GenEmptyObj(out empty);
                        return empty;
                    }
                    catch
                    {
                        return new HObject();
                    }
                }
                return new HObject(info, context);
            }
        }

        /// <summary>
        /// HTuple 护栏：空元组写标记，非空元组委托 HALCON 原生 ISerializable；
        /// HALCON 运行时 DLL 暂不可用时同样降级为标记（读回为空元组），保证保存不崩溃。
        /// </summary>
        private sealed class HalconTupleSurrogate : ISerializationSurrogate
        {
            void ISerializationSurrogate.GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                HTuple t = (HTuple)obj;
                bool isEmpty;
                try
                {
                    isEmpty = t.Length == 0;
                    if (!isEmpty)
                        ((ISerializable)t).GetObjectData(info, context);
                }
                catch
                {
                    isEmpty = true;
                }
                if (isEmpty)
                    info.AddValue(EmptyMarker, true);
            }

            object ISerializationSurrogate.SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                bool isEmpty = false;
                try
                {
                    isEmpty = info.GetBoolean(EmptyMarker);
                }
                catch
                {
                    isEmpty = false;
                }
                if (isEmpty)
                    return new HTuple();
                return new HTuple(info, context);
            }
        }
    }
}
