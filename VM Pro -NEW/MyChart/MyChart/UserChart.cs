using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace MyChart
{
    public partial class UserChart : UserControl
    {
        public UserChart()
        {
            InitializeComponent();
        }

        public bool UpdataData(ChartData data)
        {
            try
            {
                Invoke((MethodInvoker)delegate
                {
                    txtB_EC.Text = data.EC;txtB_T1.Text = data.T1;txtB_T2.Text = data.T2;txtB_T3.Text = data.T3;
                    txtB_T4.Text = data.T4;txtB_Tt.Text = data.Tt;txtB_A1.Text = data.A1;txtB_A2.Text = data.A2;
                    txtB_A3.Text = data.A3;txtB_A4.Text = data.A4;txtB_At.Text = data.At;txtB_MECCT.Text = data.MECCT;
                    if (chart1.Series.Count<data.Index)
                    {
                        var ser = new Series();
                       // ser.Name = data.Name;
                        ser.ChartType = SelcetChartType(data.type);                  
                      //  chart1.Series[data.Index - 1] = ser;
                        chart1.Series.Add(ser);
                    }
                    chart1.ChartAreas[0].AxisY.Title = data.Tparam.TBit;
                    chart1.ChartAreas[0].AxisX.Title = "Angle";
                    chart1.Titles[0].Text = data.Name;
                    var serial = chart1.Series[data.Index - 1];
                    serial.LegendText = data.Name;
                    serial.Points.Clear();
                    for (var i = 0; i < data.YAxisData.Count; i++)
                    {
                        serial.Points.AddXY(data.XAxisData[i], data.YAxisData[i]);
                    }

                });
                return true;
            }
            catch
            {
                return false;
            }
          
        }

        SeriesChartType SelcetChartType(SeriesType type)
        {
            switch (type)
            {
                case SeriesType.Bar:
                    return SeriesChartType.Bar;
                case SeriesType.Column:
                    return SeriesChartType.Column;
                case SeriesType.Line:
                    return SeriesChartType.Line;
                case SeriesType.SpLine:
                    return SeriesChartType.Spline;
                case SeriesType.Point:
                    return SeriesChartType.Point;
                default:
                    return SeriesChartType.Spline;
            }
            
        }
        public void ClearAllData()
        {
            foreach (var item in chart1.Series)
            {
                item.Points.Clear();
            }
        }
    
    }
}
