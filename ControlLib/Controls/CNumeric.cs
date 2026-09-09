using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Controls
{
    public partial class CNumeric : ModernInputControl
    {
        private bool syncingText;
        private bool hasValidValue;
        private double lastValidNumericValue;
        private string lastValidText = "0";

        public CNumeric()
        {
            InitializeComponent();
            ConfigureModernInput();
            SetEditorText(_value);
        }

        private void ConfigureModernInput()
        {
            BackColor = Color.Transparent;
            lbl_line.Visible = false;
            tbx_value.BorderStyle = BorderStyle.None;
            tbx_value.BackColor = InputSurfaceColor;
            tbx_value.ForeColor = InputTextColor;
            LayoutInputChildren();
        }

        protected override void OnModernPaletteChanged()
        {
            if (tbx_value == null)
                return;
            tbx_value.BackColor = InputSurfaceColor;
            tbx_value.ForeColor = InputTextColor;
        }

        protected override void LayoutInputChildren()
        {
            if (tbx_value == null || lbl_line == null)
                return;

            lbl_line.Visible = false;
            int preferredHeight = Math.Min(tbx_value.PreferredHeight, Math.Max(1, Height - 6));
            tbx_value.SetBounds(8, Math.Max(2, (Height - preferredHeight) / 2),
                Math.Max(1, Width - 16), preferredHeight);
        }

        /// <summary>
        /// 值改变事件
        /// </summary>
        public event DValueChanged ValueChanged;
        /// <summary>
        /// 值
        /// </summary>
        private string _value = string.Empty;
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                string requestedText = value ?? string.Empty;
                double numericValue;
                if (TryParseCompleteNumber(requestedText, out numericValue))
                {
                    CommitValidValue(requestedText, numericValue, true);
                    return;
                }

                // 程序可以显式清空输入框；空值和仅有负号都属于编辑态，
                // 不向数值订阅方发送事件，也不会进入 Convert.ToDouble。
                _value = requestedText;
                hasValidValue = false;
                SetEditorText(requestedText);
            }
        }

        private static bool TryParseCompleteNumber(string text, out double value)
        {
            value = 0D;
            if (string.IsNullOrEmpty(text) || text == "-" || text == "+")
                return false;

            string decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            if (!string.IsNullOrEmpty(decimalSeparator) && text.EndsWith(decimalSeparator, StringComparison.Ordinal))
                return false;

            const NumberStyles styles = NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint;
            return double.TryParse(text, styles, CultureInfo.CurrentCulture, out value) &&
                   !double.IsNaN(value) && !double.IsInfinity(value);
        }

        private void SetEditorText(string text)
        {
            string displayText = text ?? string.Empty;
            if (tbx_value.Text == displayText)
                return;

            syncingText = true;
            try
            {
                tbx_value.Text = displayText;
            }
            finally
            {
                syncingText = false;
            }
        }

        private void CommitValidValue(string text, double numericValue, bool notify)
        {
            bool changed = !hasValidValue || numericValue != lastValidNumericValue;
            _value = text;
            lastValidText = text;
            lastValidNumericValue = numericValue;
            hasValidValue = true;
            SetEditorText(text);
            if (notify && changed && ValueChanged != null)
                ValueChanged(numericValue);
        }

        private void NormalizeEditorValue()
        {
            double numericValue;
            if (TryParseCompleteNumber(tbx_value.Text, out numericValue))
            {
                CommitValidValue(tbx_value.Text, numericValue, true);
                return;
            }

            if (hasValidValue)
            {
                _value = lastValidText;
                SetEditorText(lastValidText);
                return;
            }

            // 没有历史合法值时，离焦统一落到安全的数值零。
            CommitValidValue("0", 0D, true);
        }


        private void tbx_value_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                char decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
                /*只能数字键、退格键、负号、小数点*/
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) &&
                    e.KeyChar != '-' && e.KeyChar != decimalSeparator) e.Handled = true;
                /*输入为负号和小数点时，且只能输入一次(负号只能最前面输入，小数点不可最前面输入)*/
                if (e.KeyChar == '-' && (((TextBox)sender).SelectionStart != 0 ||
                   ((TextBox)sender).Text.IndexOf("-") >= 0)) e.Handled = true;
                if (e.KeyChar == decimalSeparator && (((TextBox)sender).SelectionStart == 0 ||
                   ((TextBox)sender).Text.IndexOf(decimalSeparator) >= 0)) e.Handled = true;
            }
            catch { }
        }
        private void Numeric_Enter(object sender, EventArgs e)
        {
            Invalidate();
        }
        private void Numeric_Leave(object sender, EventArgs e)
        {
            NormalizeEditorValue();
            Invalidate();
        }
        private void tbx_value_TextChanged(object sender, EventArgs e)
        {
            if (syncingText)
                return;

            double numericValue;
            if (!TryParseCompleteNumber(tbx_value.Text, out numericValue))
                return;

            CommitValidValue(tbx_value.Text, numericValue, true);
        }

    }
}
