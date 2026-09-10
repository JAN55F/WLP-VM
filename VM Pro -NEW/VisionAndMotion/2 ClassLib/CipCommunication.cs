using System;
using System.Threading.Tasks;
using HslCommunication;
using HslCommunication.Profinet.AllenBradley;

namespace VMPro
{

/// <summary>
/// CIP (Common Industrial Protocol) communication wrapper for Allen-Bradley PLCs.
/// Supports both unconnected (UCMM) and connected CIP messaging via HslCommunication.
/// </summary>
public class CipCommunication : IDisposable
{
    private readonly AllenBradleyNet _plc;
    private bool _disposed;
    private bool _isConnected;

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
    public CipCommunication(string ipAddress, int port = 44818)
    {
        if (ipAddress == null)
            throw new ArgumentNullException("ipAddress");

        IpAddress = ipAddress;
        Port = port;
        _plc = new AllenBradleyNet(ipAddress, port);
    }

    /// <summary>
    /// Optional: set a custom message router path.
    /// Call before Connect() if needed. Example: "1.15.2.18.1.12"
    /// </summary>
    public void SetMessageRouter(string routerPath)
    {
        if (string.IsNullOrEmpty(routerPath))
            throw new ArgumentNullException("routerPath");
        _plc.MessageRouter = new HslCommunication.Profinet.AllenBradley.MessageRouter(routerPath);
    }

    /// <summary>
    /// Set the CPU slot number for routing. Call before Connect() if needed.
    /// </summary>
    public void SetSlot(byte slot)
    {
        _plc.Slot = slot;
    }

    /// <summary>
    /// Connect to the PLC. Returns success status and error message.
    /// </summary>
    public OperateResult Connect()
    {
        if (_disposed)
            return new OperateResult("Object has been disposed.");

        var result = _plc.ConnectServer();
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

        var result = await _plc.ConnectServerAsync();
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

        var result = _plc.ConnectClose();
        _isConnected = false;
        return result;
    }

    // -- Bool --------------------------------------------

    public OperateResult<bool> ReadBool(string address)
    {
        EnsureConnected();
        return _plc.ReadBool(address);
    }

    public OperateResult WriteBool(string address, bool value)
    {
        EnsureConnected();
        return _plc.Write(address, value);
    }

    public OperateResult<bool[]> ReadBoolArray(string address, ushort length)
    {
        EnsureConnected();
        return _plc.ReadBool(address, length);
    }

    public OperateResult WriteBoolArray(string address, bool[] values)
    {
        EnsureConnected();
        return _plc.Write(address, values);
    }

    // -- Byte --------------------------------------------

    public OperateResult<byte> ReadByte(string address)
    {
        EnsureConnected();
        return _plc.ReadByte(address);
    }

    public OperateResult WriteByte(string address, byte value)
    {
        EnsureConnected();
        return _plc.Write(address, value);
    }

    public OperateResult<byte[]> ReadByteArray(string address, ushort length)
    {
        EnsureConnected();
        return _plc.Read(address, length);
    }

    public OperateResult WriteByteArray(string address, byte[] values)
    {
        EnsureConnected();
        return _plc.Write(address, values);
    }

    // -- Int16 -------------------------------------------

    public OperateResult<short> ReadInt16(string address)
    {
        EnsureConnected();
        return _plc.ReadInt16(address);
    }

    public OperateResult WriteInt16(string address, short value)
    {
        EnsureConnected();
        return _plc.Write(address, value);
    }

    public OperateResult<short[]> ReadInt16Array(string address, ushort length)
    {
        EnsureConnected();
        return _plc.ReadInt16(address, length);
    }

    public OperateResult WriteInt16Array(string address, short[] values)
    {
        EnsureConnected();
        return _plc.Write(address, values);
    }

    // -- Int32 -------------------------------------------

    public OperateResult<int> ReadInt32(string address)
    {
        EnsureConnected();
        return _plc.ReadInt32(address);
    }

    public OperateResult WriteInt32(string address, int value)
    {
        EnsureConnected();
        return _plc.Write(address, value);
    }

    public OperateResult<int[]> ReadInt32Array(string address, ushort length)
    {
        EnsureConnected();
        return _plc.ReadInt32(address, length);
    }

    public OperateResult WriteInt32Array(string address, int[] values)
    {
        EnsureConnected();
        return _plc.Write(address, values);
    }

    // -- Float -------------------------------------------

    public OperateResult<float> ReadFloat(string address)
    {
        EnsureConnected();
        return _plc.ReadFloat(address);
    }

    public OperateResult WriteFloat(string address, float value)
    {
        EnsureConnected();
        return _plc.Write(address, value);
    }

    public OperateResult<float[]> ReadFloatArray(string address, ushort length)
    {
        EnsureConnected();
        return _plc.ReadFloat(address, length);
    }

    public OperateResult WriteFloatArray(string address, float[] values)
    {
        EnsureConnected();
        return _plc.Write(address, values);
    }

    // -- Double ------------------------------------------

    public OperateResult<double> ReadDouble(string address)
    {
        EnsureConnected();
        return _plc.ReadDouble(address);
    }

    public OperateResult WriteDouble(string address, double value)
    {
        EnsureConnected();
        return _plc.Write(address, value);
    }

    public OperateResult<double[]> ReadDoubleArray(string address, ushort length)
    {
        EnsureConnected();
        return _plc.ReadDouble(address, length);
    }

    public OperateResult WriteDoubleArray(string address, double[] values)
    {
        EnsureConnected();
        return _plc.Write(address, values);
    }

    // -- String ------------------------------------------

    public OperateResult<string> ReadString(string address, ushort length)
    {
        EnsureConnected();
        return _plc.ReadString(address, length);
    }

    public OperateResult WriteString(string address, string value)
    {
        EnsureConnected();
        return _plc.Write(address, value);
    }

    // -- Raw / Custom ------------------------------------

    /// <summary>
    /// Read raw bytes from a tag address. Supports array reads via length.
    /// </summary>
    public OperateResult<byte[]> ReadRaw(string address, ushort length)
    {
        EnsureConnected();
        return _plc.Read(address, length);
    }

    /// <summary>
    /// Write raw bytes to a tag address.
    /// </summary>
    public OperateResult WriteRaw(string address, byte[] data)
    {
        EnsureConnected();
        return _plc.Write(address, data);
    }

    /// <summary>
    /// Write a tag with explicit CIP type code. Use when the auto-detected type is wrong.
    /// Example: WriteTag("A", (ushort)0xD1, someBytes)
    /// See AllenBradleyHelper for type code constants.
    /// </summary>
    public OperateResult WriteTag(string address, ushort typeCode, byte[] value)
    {
        EnsureConnected();
        return _plc.WriteTag(address, typeCode, value);
    }

    /// <summary>
    /// Batch read multiple tags. Each address returns 1 element's bytes.
    /// The raw byte array must be parsed manually using ByteTransform.
    /// </summary>
    public OperateResult<byte[]> ReadBatch(string[] addresses)
    {
        EnsureConnected();
        return _plc.Read(addresses);
    }

    /// <summary>
    /// Batch read multiple tags and parse into typed results using a transform function.
    /// </summary>
    public OperateResult<T[]> ReadBatch<T>(string[] addresses, Func<byte[], int, T> elementParser, int elementSize)
    {
        EnsureConnected();
        var read = _plc.Read(addresses);
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
        return await _plc.ReadBoolAsync(address);
    }

    public async Task<OperateResult> WriteBoolAsync(string address, bool value)
    {
        EnsureConnected();
        return await _plc.WriteAsync(address, value);
    }

    public async Task<OperateResult<float>> ReadFloatAsync(string address)
    {
        EnsureConnected();
        return await _plc.ReadFloatAsync(address);
    }

    public async Task<OperateResult> WriteFloatAsync(string address, float value)
    {
        EnsureConnected();
        return await _plc.WriteAsync(address, value);
    }

    public async Task<OperateResult<int>> ReadInt32Async(string address)
    {
        EnsureConnected();
        return await _plc.ReadInt32Async(address);
    }

    public async Task<OperateResult> WriteInt32Async(string address, int value)
    {
        EnsureConnected();
        return await _plc.WriteAsync(address, value);
    }

    public async Task<OperateResult<short>> ReadInt16Async(string address)
    {
        EnsureConnected();
        return await _plc.ReadInt16Async(address);
    }

    public async Task<OperateResult> WriteInt16Async(string address, short value)
    {
        EnsureConnected();
        return await _plc.WriteAsync(address, value);
    }

    public async Task<OperateResult<string>> ReadStringAsync(string address, ushort length)
    {
        EnsureConnected();
        return await _plc.ReadStringAsync(address, length);
    }

    public async Task<OperateResult> WriteStringAsync(string address, string value)
    {
        EnsureConnected();
        return await _plc.WriteAsync(address, value);
    }

    public async Task<OperateResult<byte[]>> ReadRawAsync(string address, ushort length)
    {
        EnsureConnected();
        return await _plc.ReadAsync(address, length);
    }

    public async Task<OperateResult> WriteRawAsync(string address, byte[] data)
    {
        EnsureConnected();
        return await _plc.WriteAsync(address, data);
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
            if (_plc != null)
                _plc.ConnectClose();
        }
        catch
        {
            // best effort cleanup
        }
    }
}

} // namespace VMPro
