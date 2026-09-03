using System;
using System.Threading.Tasks;
using HslCommunication;
using HslCommunication.Profinet.AllenBradley;
using HslCommunication.Profinet.Inovance;
using HslCommunication.Profinet.Melsec;
using HslCommunication.Profinet.Omron;

namespace VMPro
{

/// <summary>
/// EtherNet/IP (CIP) communication wrapper for Allen-Bradley and Omron PLCs,
/// plus Modbus TCP communication for Inovance PLCs and MC 3E Binary TCP for Mitsubishi PLCs.
/// </summary>
public class CipCommunication : IDisposable
{
    private static OperateResult<byte> ConvertByteResult(OperateResult<byte[]> result)
    {
        if (!result.IsSuccess)
            return new OperateResult<byte>(result.ErrorCode, result.Message);
        if (result.Content == null || result.Content.Length == 0)
            return new OperateResult<byte>(-1, "PLC返回空数据");
        return OperateResult.CreateSuccessResult(result.Content[0]);
    }

    private readonly AllenBradleyNet _plc;
    private readonly InovanceTcpNet _inovancePlc;
    private readonly OmronCipNet _omronPlc;
    private readonly MelsecMcNet _mitsubishiPlc;
    private bool _disposed;
    private bool _isConnected;
    private readonly PLCBrand _brand;

    public bool IsConnected
    {
        get { return _isConnected; }
    }
    public string IpAddress { get; }
    public int Port { get; }

    /// <summary>
    /// Creates a CIP communication client using unconnected (UCMM) messaging.
    /// </summary>
    /// <param name="ipAddress">PLC IP address, e.g. "192.168.0.100"</param>
    /// <param name="port">PLC port, default 44818 for EtherNet/IP</param>
    public CipCommunication(string ipAddress, int port = 44818, PLCBrand brand = PLCBrand.AB, string inovanceSeries = "H3U")
    {
        if (ipAddress == null)
            throw new ArgumentNullException("ipAddress");

        IpAddress = ipAddress;
        Port = port;
        _brand = brand;
        if (brand == PLCBrand.Inovance)
            _inovancePlc = new InovanceTcpNet(ParseInovanceSeries(inovanceSeries), ipAddress, port, 1);
        else if (brand == PLCBrand.Omron)
            _omronPlc = new OmronCipNet(ipAddress, port);
        else if (brand == PLCBrand.Mitsubishi)
            _mitsubishiPlc = new MelsecMcNet(ipAddress, port);
        else
            _plc = new AllenBradleyNet(ipAddress, port);
    }

    private static InovanceSeries ParseInovanceSeries(string series)
    {
        if (string.Equals(series, "AM", StringComparison.OrdinalIgnoreCase))
            return InovanceSeries.AM;
        if (string.Equals(series, "H5U", StringComparison.OrdinalIgnoreCase))
            return InovanceSeries.H5U;
        if (string.Equals(series, "Easy", StringComparison.OrdinalIgnoreCase))
            return InovanceSeries.Easy;
        return InovanceSeries.H3U;
    }

    /// <summary>
    /// Optional: set a custom message router path.
    /// Call before Connect() if needed. Example: "1.15.2.18.1.12"
    /// </summary>
    public void SetMessageRouter(string routerPath)
    {
        if (string.IsNullOrEmpty(routerPath))
            throw new ArgumentNullException("routerPath");
        if (_brand != PLCBrand.AB)
            throw new InvalidOperationException("仅 AB PLC 支持 CIP 路由路径设置。");
        _plc.MessageRouter = new HslCommunication.Profinet.AllenBradley.MessageRouter(routerPath);
    }

    /// <summary>
    /// Set the CPU slot number for routing. Call before Connect() if needed.
    /// </summary>
    public void SetSlot(byte slot)
    {
        if (_brand == PLCBrand.AB)
            _plc.Slot = slot;
        else if (_brand == PLCBrand.Omron)
            _omronPlc.Slot = slot;
    }

    /// <summary>
    /// Connect to the PLC. Returns success status and error message.
    /// </summary>
    public OperateResult Connect()
    {
        if (_disposed)
            return new OperateResult("Object has been disposed.");

        OperateResult result = _brand == PLCBrand.Inovance ? _inovancePlc.ConnectServer() : (_brand == PLCBrand.Omron ? _omronPlc.ConnectServer() : (_brand == PLCBrand.Mitsubishi ? _mitsubishiPlc.ConnectServer() : _plc.ConnectServer()));
        if (result.IsSuccess)
            _isConnected = true;
        return result;
    }

    /// <summary>
    /// Connect to the PLC asynchronously.
    /// </summary>
    public async Task<OperateResult> ConnectAsync()
    {
        if (_disposed)
            return new OperateResult("Object has been disposed.");

        OperateResult result;
        if (_brand == PLCBrand.Inovance)
            result = await _inovancePlc.ConnectServerAsync();
        else if (_brand == PLCBrand.Omron)
            result = await _omronPlc.ConnectServerAsync();
        else if (_brand == PLCBrand.Mitsubishi)
            result = await _mitsubishiPlc.ConnectServerAsync();
        else
            result = await _plc.ConnectServerAsync();
        if (result.IsSuccess)
            _isConnected = true;
        return result;
    }

    /// <summary>
    /// Disconnect from the PLC.
    /// </summary>
    public OperateResult Disconnect()
    {
        if (_disposed || !IsConnected)
            return new OperateResult();

        OperateResult result = _brand == PLCBrand.Inovance ? _inovancePlc.ConnectClose() : (_brand == PLCBrand.Omron ? _omronPlc.ConnectClose() : (_brand == PLCBrand.Mitsubishi ? _mitsubishiPlc.ConnectClose() : _plc.ConnectClose()));
        _isConnected = false;
        return result;
    }

    // -- Bool --------------------------------------------

    public OperateResult<bool> ReadBool(string address)
    {
        EnsureConnected();
        return _brand == PLCBrand.Inovance ? _inovancePlc.ReadBool(address) : (_brand == PLCBrand.Omron ? _omronPlc.ReadBool(address) : (_brand == PLCBrand.Mitsubishi ? _mitsubishiPlc.ReadBool(address) : _plc.ReadBool(address)));
    }

    public OperateResult WriteBool(string address, bool value)
    {
        EnsureConnected();
        return _brand == PLCBrand.Inovance ? _inovancePlc.Write(address, value) : (_brand == PLCBrand.Omron ? _omronPlc.Write(address, value) : (_brand == PLCBrand.Mitsubishi ? _mitsubishiPlc.Write(address, value) : _plc.Write(address, value)));
    }

    public OperateResult<bool[]> ReadBoolArray(string address, ushort length)
    {
        EnsureConnected();
        return _brand == PLCBrand.Mitsubishi ? _mitsubishiPlc.ReadBool(address, length) : (_brand == PLCBrand.Omron ? _omronPlc.ReadBool(address, length) : _plc.ReadBool(address, length));
    }

    public OperateResult WriteBoolArray(string address, bool[] values)
    {
        EnsureConnected();
        return _brand == PLCBrand.Mitsubishi ? _mitsubishiPlc.Write(address, values) : (_brand == PLCBrand.Omron ? _omronPlc.Write(address, values) : _plc.Write(address, values));
    }

    // -- Byte --------------------------------------------

    public OperateResult<byte> ReadByte(string address)
    {
        EnsureConnected();
        return _brand == PLCBrand.Mitsubishi ? ConvertByteResult(_mitsubishiPlc.Read(address, 1)) : (_brand == PLCBrand.Omron ? _omronPlc.ReadByte(address) : _plc.ReadByte(address));
    }

    public OperateResult WriteByte(string address, byte value)
    {
        EnsureConnected();
        return _brand == PLCBrand.Mitsubishi ? _mitsubishiPlc.Write(address, value) : (_brand == PLCBrand.Omron ? _omronPlc.Write(address, value) : _plc.Write(address, value));
    }

    public OperateResult<byte[]> ReadByteArray(string address, ushort length)
    {
        EnsureConnected();
        return _brand == PLCBrand.Mitsubishi ? _mitsubishiPlc.Read(address, length) : (_brand == PLCBrand.Omron ? _omronPlc.Read(address, length) : _plc.Read(address, length));
    }

    public OperateResult WriteByteArray(string address, byte[] values)
    {
        EnsureConnected();
        return _brand == PLCBrand.Mitsubishi ? _mitsubishiPlc.Write(address, values) : (_brand == PLCBrand.Omron ? _omronPlc.Write(address, values) : _plc.Write(address, values));
    }

    // -- Int16 -------------------------------------------

    public OperateResult<short> ReadInt16(string address)
    {
        EnsureConnected();
        return _brand == PLCBrand.Inovance ? _inovancePlc.ReadInt16(address) : (_brand == PLCBrand.Omron ? _omronPlc.ReadInt16(address) : (_brand == PLCBrand.Mitsubishi ? _mitsubishiPlc.ReadInt16(address) : _plc.ReadInt16(address)));
    }

    public OperateResult WriteInt16(string address, short value)
    {
        EnsureConnected();
        return _brand == PLCBrand.Inovance ? _inovancePlc.Write(address, value) : (_brand == PLCBrand.Omron ? _omronPlc.Write(address, value) : (_brand == PLCBrand.Mitsubishi ? _mitsubishiPlc.Write(address, value) : _plc.Write(address, value)));
    }

    public OperateResult<short[]> ReadInt16Array(string address, ushort length)
    {
        EnsureConnected();
        if (_brand == PLCBrand.Mitsubishi) { var r = _mitsubishiPlc.ReadInt16(address, length); return r.IsSuccess ? OperateResult.CreateSuccessResult(r.Content) : new OperateResult<short[]>(r.ErrorCode, r.Message); } if (_brand == PLCBrand.Omron) { var r = _omronPlc.ReadInt16(address, length); return r.IsSuccess ? OperateResult.CreateSuccessResult(r.Content) : new OperateResult<short[]>(r.ErrorCode, r.Message); } return _plc.ReadInt16(address, length);
    }

    public OperateResult WriteInt16Array(string address, short[] values)
    {
        EnsureConnected();
        return _brand == PLCBrand.Mitsubishi ? _mitsubishiPlc.Write(address, values) : (_brand == PLCBrand.Omron ? _omronPlc.Write(address, values) : _plc.Write(address, values));
    }

    // -- Int32 -------------------------------------------

    public OperateResult<int> ReadInt32(string address)
    {
        EnsureConnected();
        return _brand == PLCBrand.Inovance ? _inovancePlc.ReadInt32(address) : (_brand == PLCBrand.Omron ? _omronPlc.ReadInt32(address) : (_brand == PLCBrand.Mitsubishi ? _mitsubishiPlc.ReadInt32(address) : _plc.ReadInt32(address)));
    }

    public OperateResult WriteInt32(string address, int value)
    {
        EnsureConnected();
        return _brand == PLCBrand.Inovance ? _inovancePlc.Write(address, value) : (_brand == PLCBrand.Omron ? _omronPlc.Write(address, value) : (_brand == PLCBrand.Mitsubishi ? _mitsubishiPlc.Write(address, value) : _plc.Write(address, value)));
    }

    public OperateResult<int[]> ReadInt32Array(string address, ushort length)
    {
        EnsureConnected();
        if (_brand == PLCBrand.Mitsubishi) { var r = _mitsubishiPlc.ReadInt32(address, length); return r.IsSuccess ? OperateResult.CreateSuccessResult(r.Content) : new OperateResult<int[]>(r.ErrorCode, r.Message); } if (_brand == PLCBrand.Omron) { var r = _omronPlc.ReadInt32(address, length); return r.IsSuccess ? OperateResult.CreateSuccessResult(r.Content) : new OperateResult<int[]>(r.ErrorCode, r.Message); } return _plc.ReadInt32(address, length);
    }

    public OperateResult WriteInt32Array(string address, int[] values)
    {
        EnsureConnected();
        return _brand == PLCBrand.Mitsubishi ? _mitsubishiPlc.Write(address, values) : (_brand == PLCBrand.Omron ? _omronPlc.Write(address, values) : _plc.Write(address, values));
    }

    // -- Float -------------------------------------------

    public OperateResult<float> ReadFloat(string address)
    {
        EnsureConnected();
        return _brand == PLCBrand.Inovance ? _inovancePlc.ReadFloat(address) : (_brand == PLCBrand.Omron ? _omronPlc.ReadFloat(address) : (_brand == PLCBrand.Mitsubishi ? _mitsubishiPlc.ReadFloat(address) : _plc.ReadFloat(address)));
    }

    public OperateResult WriteFloat(string address, float value)
    {
        EnsureConnected();
        return _brand == PLCBrand.Inovance ? _inovancePlc.Write(address, value) : (_brand == PLCBrand.Omron ? _omronPlc.Write(address, value) : (_brand == PLCBrand.Mitsubishi ? _mitsubishiPlc.Write(address, value) : _plc.Write(address, value)));
    }

    public OperateResult<float[]> ReadFloatArray(string address, ushort length)
    {
        EnsureConnected();
        if (_brand == PLCBrand.Mitsubishi) { var r = _mitsubishiPlc.ReadFloat(address, length); return r.IsSuccess ? OperateResult.CreateSuccessResult(r.Content) : new OperateResult<float[]>(r.ErrorCode, r.Message); } if (_brand == PLCBrand.Omron) { var r = _omronPlc.ReadFloat(address, length); return r.IsSuccess ? OperateResult.CreateSuccessResult(r.Content) : new OperateResult<float[]>(r.ErrorCode, r.Message); } return _plc.ReadFloat(address, length);
    }

    public OperateResult WriteFloatArray(string address, float[] values)
    {
        EnsureConnected();
        return _brand == PLCBrand.Mitsubishi ? _mitsubishiPlc.Write(address, values) : (_brand == PLCBrand.Omron ? _omronPlc.Write(address, values) : _plc.Write(address, values));
    }

    // -- Double ------------------------------------------

    public OperateResult<double> ReadDouble(string address)
    {
        EnsureConnected();
        return _brand == PLCBrand.Inovance ? _inovancePlc.ReadDouble(address) : (_brand == PLCBrand.Omron ? _omronPlc.ReadDouble(address) : (_brand == PLCBrand.Mitsubishi ? _mitsubishiPlc.ReadDouble(address) : _plc.ReadDouble(address)));
    }

    public OperateResult WriteDouble(string address, double value)
    {
        EnsureConnected();
        return _brand == PLCBrand.Inovance ? _inovancePlc.Write(address, value) : (_brand == PLCBrand.Omron ? _omronPlc.Write(address, value) : (_brand == PLCBrand.Mitsubishi ? _mitsubishiPlc.Write(address, value) : _plc.Write(address, value)));
    }

    public OperateResult<double[]> ReadDoubleArray(string address, ushort length)
    {
        EnsureConnected();
        if (_brand == PLCBrand.Mitsubishi) { var r = _mitsubishiPlc.ReadDouble(address, length); return r.IsSuccess ? OperateResult.CreateSuccessResult(r.Content) : new OperateResult<double[]>(r.ErrorCode, r.Message); } if (_brand == PLCBrand.Omron) { var r = _omronPlc.ReadDouble(address, length); return r.IsSuccess ? OperateResult.CreateSuccessResult(r.Content) : new OperateResult<double[]>(r.ErrorCode, r.Message); } return _plc.ReadDouble(address, length);
    }

    public OperateResult WriteDoubleArray(string address, double[] values)
    {
        EnsureConnected();
        return _brand == PLCBrand.Mitsubishi ? _mitsubishiPlc.Write(address, values) : (_brand == PLCBrand.Omron ? _omronPlc.Write(address, values) : _plc.Write(address, values));
    }

    // -- String ------------------------------------------

    public OperateResult<string> ReadString(string address, ushort length)
    {
        EnsureConnected();
        return _brand == PLCBrand.Inovance ? _inovancePlc.ReadString(address, length) : (_brand == PLCBrand.Omron ? _omronPlc.ReadString(address, length) : (_brand == PLCBrand.Mitsubishi ? _mitsubishiPlc.ReadString(address, length) : _plc.ReadString(address, length)));
    }

    public OperateResult WriteString(string address, string value)
    {
        EnsureConnected();
        return _brand == PLCBrand.Inovance ? _inovancePlc.Write(address, value) : (_brand == PLCBrand.Omron ? _omronPlc.Write(address, value) : (_brand == PLCBrand.Mitsubishi ? _mitsubishiPlc.Write(address, value) : _plc.Write(address, value)));
    }

    // -- Raw / Custom ------------------------------------

    /// <summary>
    /// Read raw bytes from a tag address. Supports array reads via length.
    /// </summary>
    public OperateResult<byte[]> ReadRaw(string address, ushort length)
    {
        EnsureConnected();
        return _brand == PLCBrand.Mitsubishi ? _mitsubishiPlc.Read(address, length) : (_brand == PLCBrand.Omron ? _omronPlc.Read(address, length) : _plc.Read(address, length));
    }

    /// <summary>
    /// Write raw bytes to a tag address.
    /// </summary>
    public OperateResult WriteRaw(string address, byte[] data)
    {
        EnsureConnected();
        return _brand == PLCBrand.Mitsubishi ? _mitsubishiPlc.Write(address, data) : (_brand == PLCBrand.Omron ? _omronPlc.Write(address, data) : _plc.Write(address, data));
    }

    /// <summary>
    /// Write a tag with explicit CIP type code. Use when the auto-detected type is wrong.
    /// Example: WriteTag("A", (ushort)0xD1, someBytes)
    /// See AllenBradleyHelper for type code constants.
    /// </summary>
    public OperateResult WriteTag(string address, ushort typeCode, byte[] value)
    {
        EnsureConnected();
        if (_brand == PLCBrand.Omron)
            throw new NotSupportedException("欧姆龙 EtherNet/IP 不支持 AB 专用 WriteTag 接口，请使用类型化 Write 方法。");
        return _plc.WriteTag(address, typeCode, value);
    }

    /// <summary>
    /// Batch read multiple tags. Each address returns 1 element's bytes.
    /// The raw byte array must be parsed manually using ByteTransform.
    /// </summary>
    public OperateResult<byte[]> ReadBatch(string[] addresses)
    {
        EnsureConnected();
        return _brand == PLCBrand.Omron ? _omronPlc.Read(addresses) : _plc.Read(addresses);
    }

    /// <summary>
    /// Batch read multiple tags and parse into typed results using a transform function.
    /// </summary>
    public OperateResult<T[]> ReadBatch<T>(string[] addresses, Func<byte[], int, T> elementParser, int elementSize)
    {
        EnsureConnected();
        var read = _brand == PLCBrand.Mitsubishi ? _mitsubishiPlc.Read(addresses[0], (ushort)addresses.Length) : (_brand == PLCBrand.Omron ? _omronPlc.Read(addresses) : _plc.Read(addresses));
        if (!read.IsSuccess)
            return new OperateResult<T[]>(read.ErrorCode, read.Message);

        var result = new T[addresses.Length];
        for (int i = 0; i < addresses.Length; i++)
        {
            result[i] = elementParser(read.Content, i * elementSize);
        }

        return OperateResult.CreateSuccessResult(result);
    }

    // -- Async -------------------------------------------

    public async Task<OperateResult<bool>> ReadBoolAsync(string address)
    {
        EnsureConnected();
        return _brand == PLCBrand.Mitsubishi ? await _mitsubishiPlc.ReadBoolAsync(address) : (_brand == PLCBrand.Omron ? await _omronPlc.ReadBoolAsync(address) : await _plc.ReadBoolAsync(address));
    }

    public async Task<OperateResult> WriteBoolAsync(string address, bool value)
    {
        EnsureConnected();
        return _brand == PLCBrand.Mitsubishi ? await _mitsubishiPlc.WriteAsync(address, value) : (_brand == PLCBrand.Omron ? await _omronPlc.WriteAsync(address, value) : await _plc.WriteAsync(address, value));
    }

    public async Task<OperateResult<float>> ReadFloatAsync(string address)
    {
        EnsureConnected();
        return _brand == PLCBrand.Mitsubishi ? await _mitsubishiPlc.ReadFloatAsync(address) : (_brand == PLCBrand.Omron ? await _omronPlc.ReadFloatAsync(address) : await _plc.ReadFloatAsync(address));
    }

    public async Task<OperateResult> WriteFloatAsync(string address, float value)
    {
        EnsureConnected();
        return _brand == PLCBrand.Mitsubishi ? await _mitsubishiPlc.WriteAsync(address, value) : (_brand == PLCBrand.Omron ? await _omronPlc.WriteAsync(address, value) : await _plc.WriteAsync(address, value));
    }

    public async Task<OperateResult<int>> ReadInt32Async(string address)
    {
        EnsureConnected();
        return _brand == PLCBrand.Mitsubishi ? await _mitsubishiPlc.ReadInt32Async(address) : (_brand == PLCBrand.Omron ? await _omronPlc.ReadInt32Async(address) : await _plc.ReadInt32Async(address));
    }

    public async Task<OperateResult> WriteInt32Async(string address, int value)
    {
        EnsureConnected();
        return _brand == PLCBrand.Mitsubishi ? await _mitsubishiPlc.WriteAsync(address, value) : (_brand == PLCBrand.Omron ? await _omronPlc.WriteAsync(address, value) : await _plc.WriteAsync(address, value));
    }

    public async Task<OperateResult<short>> ReadInt16Async(string address)
    {
        EnsureConnected();
        return _brand == PLCBrand.Mitsubishi ? await _mitsubishiPlc.ReadInt16Async(address) : (_brand == PLCBrand.Omron ? await _omronPlc.ReadInt16Async(address) : await _plc.ReadInt16Async(address));
    }

    public async Task<OperateResult> WriteInt16Async(string address, short value)
    {
        EnsureConnected();
        return _brand == PLCBrand.Mitsubishi ? await _mitsubishiPlc.WriteAsync(address, value) : (_brand == PLCBrand.Omron ? await _omronPlc.WriteAsync(address, value) : await _plc.WriteAsync(address, value));
    }

    public async Task<OperateResult<string>> ReadStringAsync(string address, ushort length)
    {
        EnsureConnected();
        return _brand == PLCBrand.Mitsubishi ? await _mitsubishiPlc.ReadStringAsync(address, length) : (_brand == PLCBrand.Omron ? await _omronPlc.ReadStringAsync(address, length) : await _plc.ReadStringAsync(address, length));
    }

    public async Task<OperateResult> WriteStringAsync(string address, string value)
    {
        EnsureConnected();
        return _brand == PLCBrand.Mitsubishi ? await _mitsubishiPlc.WriteAsync(address, value) : (_brand == PLCBrand.Omron ? await _omronPlc.WriteAsync(address, value) : await _plc.WriteAsync(address, value));
    }

    public async Task<OperateResult<byte[]>> ReadRawAsync(string address, ushort length)
    {
        EnsureConnected();
        return _brand == PLCBrand.Mitsubishi ? await _mitsubishiPlc.ReadAsync(address, length) : (_brand == PLCBrand.Omron ? await _omronPlc.ReadAsync(address, length) : await _plc.ReadAsync(address, length));
    }

    public async Task<OperateResult> WriteRawAsync(string address, byte[] data)
    {
        EnsureConnected();
        return _brand == PLCBrand.Mitsubishi ? await _mitsubishiPlc.WriteAsync(address, data) : (_brand == PLCBrand.Omron ? await _omronPlc.WriteAsync(address, data) : await _plc.WriteAsync(address, data));
    }

    // -- Private -----------------------------------------

    private void EnsureConnected()
    {
        if (_disposed)
            throw new ObjectDisposedException("CipCommunication");
        if (!IsConnected)
            throw new InvalidOperationException(
                string.Format("Not connected to PLC at {0}:{1}. Call Connect() first.", IpAddress, Port));
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        _isConnected = false;
        try
        {
            if (_brand == PLCBrand.Inovance && _inovancePlc != null)
                _inovancePlc.ConnectClose();
            else if (_brand == PLCBrand.Omron && _omronPlc != null)
                _omronPlc.ConnectClose();
            else if (_brand == PLCBrand.Mitsubishi && _mitsubishiPlc != null)
                _mitsubishiPlc.ConnectClose();
            else if (_plc != null)
                _plc.ConnectClose();
        }
        catch
        {
            // best effort cleanup
        }
    }
}

} // namespace VMPro
