using System;
using System.Collections.Generic;

namespace VMPro
{
    /// <summary>
    /// PLC设备配置；运行期的通讯对象不参与序列化。
    /// </summary>
    [Serializable]
    internal class PLCDevice
    {
        internal PLCDevice(string name)
        {
            Name = name;
        }

        internal string Name       = string.Empty;
        internal string IpAddress  = "192.168.0.1";
        internal int    Port       = 44818;
        internal byte   Slot       = 0;
        // 旧项目没有保存品牌，反序列化旧流程时保持 AB/CIP 的原有行为。
        internal PLCBrand Brand    = PLCBrand.AB;
        // 汇川系列：AM、H3U、H5U、Easy。旧项目默认 H3U，以保持此前汇川接入的行为。
        internal string InovanceSeries = "H3U";
        internal bool   AutoConnectAfterStart    = false;
        internal bool   AutoDisconnectBeforeClose = true;

        [NonSerialized]
        private object _syncRoot = new object();

        internal object SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                    _syncRoot = new object();
                return _syncRoot;
            }
        }

        /// <summary>
        /// 静态字典：设备名对应运行期通讯实例。
        /// </summary>
        [NonSerialized]
        internal static Dictionary<string, CipCommunication> L_cipComm =
            new Dictionary<string, CipCommunication>();

        internal static void EnsureRuntimeStore()
        {
            if (L_cipComm == null)
                L_cipComm = new Dictionary<string, CipCommunication>();
        }

        internal static void ResetRuntimeStore()
        {
            EnsureRuntimeStore();
            foreach (CipCommunication comm in L_cipComm.Values)
            {
                try
                {
                    if (comm != null)
                        comm.Dispose();
                }
                catch { }
            }
            L_cipComm.Clear();
        }

        internal void EnsureRuntime()
        {
            EnsureRuntimeStore();
            if (_syncRoot == null)
                _syncRoot = new object();
        }

        internal static PLCDevice FindByName(string name)
        {
            if (Project.Instance.L_PLCDevice == null || string.IsNullOrEmpty(name))
                return null;

            for (int i = 0; i < Project.Instance.L_PLCDevice.Count; i++)
            {
                PLCDevice device = Project.Instance.L_PLCDevice[i];
                if (device != null && device.Name == name)
                    return device;
            }
            return null;
        }

        internal CipCommunication Comm
        {
            get
            {
                EnsureRuntimeStore();
                CipCommunication c;
                return L_cipComm.TryGetValue(Name, out c) ? c : null;
            }
        }

        internal bool IsConnected
        {
            get
            {
                var comm = Comm;
                return comm != null && comm.IsConnected;
            }
        }

        /// <summary>
        /// 用当前 IP、端口、品牌和系列重建通讯实例。
        /// </summary>
        internal void Rebuild()
        {
            EnsureRuntimeStore();
            CipCommunication old;
            if (L_cipComm.TryGetValue(Name, out old))
            {
                try { old.Dispose(); } catch { }
            }
            int effectivePort = Port;
            if (effectivePort <= 0)
                effectivePort = Brand == PLCBrand.Inovance ? 502 : 44818;
            var comm = new CipCommunication(IpAddress, effectivePort, Brand, InovanceSeries);
            comm.SetSlot(Slot);
            L_cipComm[Name] = comm;
        }

        /// <summary>
        /// 连接 PLC，返回是否成功。
        /// </summary>
        internal bool Connect(out string errorMsg)
        {
            try
            {
                lock (SyncRoot)
                {
                    Rebuild();
                    var result = Comm.Connect();
                    errorMsg = result.Message;
                    return result.IsSuccess;
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                Log.SaveError(ex);
                return false;
            }
        }

        /// <summary>
        /// 断开 PLC 连接。
        /// </summary>
        internal void Disconnect()
        {
            try
            {
                lock (SyncRoot)
                {
                    var comm = Comm;
                    if (comm != null)
                        comm.Disconnect();
                }
            }
            catch { }
        }

        /// <summary>
        /// 断开、释放资源，并从运行期字典中移除。
        /// </summary>
        internal void Close()
        {
            Disconnect();
            EnsureRuntimeStore();
            CipCommunication c;
            if (L_cipComm.TryGetValue(Name, out c))
            {
                try { c.Dispose(); } catch { }
                L_cipComm.Remove(Name);
            }
        }
    }
}
