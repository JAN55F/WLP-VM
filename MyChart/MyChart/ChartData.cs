using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyChart
{
    public  class ChartData
    {
        public event Func<ChartData,bool> UpdataEvent;
        private string SampleRate = "";
        internal string TorqueBit = "";
        private double TorqueAngC1 = 0;
        private double TorqueAngC2 = 0;
        private double TorqueSign = 0;
        internal TorqueParam Tparam;
        # region 过程监控数据
        internal string EC = "";
        internal string T1 = "";
        internal string T2 = "";
        internal string T3 = "";
        internal string T4 = "";
        internal string Tt = "";
        internal string A1 = "";
        internal string A2 = "";
        internal string A3 = "";
        internal string A4 = "";
        internal string At = "";
        internal string MECCT = "";
        #endregion
        public List<double> YAxisData { get; set; }

        public List<double> XAxisData { get; set; }

        public int Index { get; set; }

        public string Name { get; set; }

        public SeriesType type { get; set; }

        
        public ChartData(int Index,string Name,SeriesType type=SeriesType.SpLine)
        {
            XAxisData = new List<double>();
            YAxisData = new List<double>();
            Tparam = new TorqueParam();
            this. Name = Name;
            this.Index = Index;
            this.type = type;
        }

        public bool TorqueByteToChartData(byte[] data)
        {
            try
            {
                if (data.Length <= 46)
                {
                    return false;
                }
                byte[] MonitorByte = null;
                byte[] TorqueByte = null;
                for (int i = 0; i < data.Length; i++)
                {
                    string endf= ByteToHexString(new byte[1] { data[i] } );
                    if (endf == "0A")
                    {
                        MonitorByte = ClipByte(data,0,i);
                        TorqueByte = ClipByte(data,i+1,data.Length-1);
                        break;
                    }
                }
                if (MonitorByte == null || TorqueByte == null) return false;
                if (!MonitorDataToChartData(MonitorByte))
                {
                    return false;
                }     
                SampleRate = ByteToHexString(new byte[] { TorqueByte[9] });
                int SampleNum = GetSampleRate(SampleRate);
                if (SampleNum == 0) return false;
                if (TorqueByte.Length != (46 + SampleNum * 4)) return false;
                SetTorqueParam(ByteToHexString(new byte[] { TorqueByte[10] }));
                TorqueAngC1 = int.Parse(ByteToHexString(new byte[] { TorqueByte[21], TorqueByte[22] }), System.Globalization.NumberStyles.HexNumber);
                TorqueAngC2 = int.Parse(ByteToHexString(new byte[] { TorqueByte[23] }), System.Globalization.NumberStyles.HexNumber);
                TorqueSign = int.Parse(ByteToHexString(new byte[] { TorqueByte[24] }), System.Globalization.NumberStyles.HexNumber);
                int angleStartIndexF = 25 + SampleNum * 2;
                int angleStartIndexS = 26 + SampleNum * 2;
                XAxisData.Clear();
                YAxisData.Clear();
                int Threshold = 32768;
                int Thconst = 65535;
                List<double> angle = new List<double>();
                for (int i = 0; i < SampleNum; i++)
                {
                    byte[] TorqueData = new byte[2] { TorqueByte[25 + i * 2], TorqueByte[26 + i * 2] };
                    double YData = int.Parse(ByteToHexString(TorqueData), System.Globalization.NumberStyles.HexNumber);
                    if (YData < Threshold)
                    {
                        YData = YData * Tparam.TPoint;
                    }
                    else
                    {
                        YData = (YData - Thconst - 1) * Tparam.TPoint;
                    }
                    YAxisData.Add(YData);
                    byte[] AngleData = new byte[2] { TorqueByte[angleStartIndexF + i * 2], TorqueByte[angleStartIndexS + i * 2] };
                    double XData = int.Parse(ByteToHexString(AngleData), System.Globalization.NumberStyles.HexNumber);
                    if (XData < Threshold)
                    {
                        XData = XData * TorqueAngC1 / 100000;
                    }
                    else
                    {
                        XData = (XData - Thconst - 1) * TorqueAngC1 / 100000;
                    }
                    angle.Add(XData);
                    XAxisData.Add(angle.Sum());
                }
                if (XAxisData.Count != YAxisData.Count) return false;
                if (UpdataEvent != null) UpdataEvent(this);
                return true;
            }
            catch
            { return false; }
        }
     
        public bool MonitorDataToChartData(byte[] data)
        {
            try
            {
                string str = new ASCIIEncoding().GetString(data);
                string[] DATA = str.Split(',');
                if (DATA.Length <= 0) return false;
                EC = DATA[3];
                T1 = DATA[7];
                T2 = DATA[8];
                T3 = DATA[9];
                T4 = DATA[10];
                Tt = DATA[11];
                A1 = DATA[12];
                A2 = DATA[13];
                A3 = DATA[14];
                A4 = DATA[17];
                At = DATA[18];
                MECCT = DATA[20];
                return true;
            }
            catch
            { return false; }
        }

        byte[] ClipByte(byte[] data ,int startIndex,int endIndex)
        {
            byte[] b = new byte[endIndex+1-startIndex];
            for (int i = startIndex; i < endIndex+1; i++)
            {
                b[i-startIndex] = data[i];
            }     
            return b;
        }
        string ByteToHexString(byte[] b)
        {
            StringBuilder strBuider = new StringBuilder();
            for (int index = 0; index < b.Length; index++)
            {
                strBuider.Append(((int)b[index]).ToString("X2"));
            }
            var str = strBuider.ToString();
            return str;
        }

        int GetSampleRate(string str)
        {
            var s = str.Substring(0,1);
            switch (s)
            {
                case "0":
                    return 125;
                case "1":
                    return 250;
                case "2":
                    return 500;
                case "3":
                    return 1000;
                case "4":
                    return 2000;
                default:
                    return 0;
            }
        }
        void SetTorqueParam(string bit)
        {
            switch (bit)
            {
                case "42":
                    Tparam.TBit = "Kgf.cm";
                    Tparam.TPoint = 0.0001;
                    break;
                case "24":
                    Tparam.TBit = "mN.m";
                    Tparam.TPoint = 0.01;
                    break;
                case "48":
                    Tparam.TBit = "lbf.in";
                    Tparam.TPoint = 0.0001;
                    break;
                default:
                    break;

            }
        }

    }
    public enum SeriesType
    {
        Point,
        Line,
        SpLine,
        Column,
        Bar
    }
    public struct TorqueParam
    {
        private string Bit ;
        private double DecimalPoint;
        public string TBit { get { return Bit; }set { Bit = value; } }
        public double TPoint { get { return DecimalPoint; }set { DecimalPoint = value; } }
    }
}
