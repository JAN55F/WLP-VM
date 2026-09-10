using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Controls.Properties;

namespace Controls
{
    public delegate void DValueChanged(double value);
    public partial class CNumericUpDown : ModernInputControl
    {
        private static readonly Font CompactValueFont = new Font(
            "Microsoft YaHei UI", 8F, FontStyle.Regular, GraphicsUnit.Point, 134);
        private const int StackedStepperMinimumWidth = 68;
        private const int HorizontalStepperMinimumWidth = 90;

        private TextBox valueTextBox;
        private bool syncingText;
        private bool editingTextValue;

        public CNumericUpDown()
        {
            InitializeComponent();
            ConfigureModernInput();
            nud_value.Text = Value.ToString();
        }

        private void ConfigureModernInput()
        {
            BackColor = Color.Transparent;
            lbl_line.Visible = false;
            nud_value.Visible = false;
            nud_value.BorderStyle = BorderStyle.None;
            nud_value.BackColor = InputSurfaceColor;
            nud_value.ForeColor = InputTextColor;
            valueTextBox = new TextBox();
            valueTextBox.Name = "modernNumericTextBox";
            valueTextBox.BorderStyle = BorderStyle.None;
            valueTextBox.BackColor = InputSurfaceColor;
            valueTextBox.ForeColor = InputTextColor;
            valueTextBox.Font = Font;
            valueTextBox.TextAlign = HorizontalAlignment.Left;
            valueTextBox.KeyPress += valueTextBox_KeyPress;
            valueTextBox.KeyDown += valueTextBox_KeyDown;
            valueTextBox.TextChanged += valueTextBox_TextChanged;
            valueTextBox.Leave += valueTextBox_Leave;
            valueTextBox.MouseWheel += valueTextBox_MouseWheel;
            Controls.Add(valueTextBox);
            ConfigureStepButton(btn_sub, false);
            ConfigureStepButton(btn_add, true);
            LayoutInputChildren();
            SyncTextFromValue();
        }

        private void ConfigureStepButton(Button button, bool add)
        {
            button.Image = null;
            button.Text = string.Empty;
            button.BackColor = Color.Transparent;
            button.UseVisualStyleBackColor = false;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(232, 243, 252);
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(218, 235, 249);
            if (add)
                button.Paint += btn_add_Paint;
            else
                button.Paint += btn_sub_Paint;
        }

        protected override void OnModernPaletteChanged()
        {
            if (nud_value == null || valueTextBox == null)
                return;
            nud_value.BackColor = InputSurfaceColor;
            nud_value.ForeColor = InputTextColor;
            valueTextBox.BackColor = InputSurfaceColor;
            valueTextBox.ForeColor = InputTextColor;
            btn_sub.BackColor = Color.Transparent;
            btn_add.BackColor = Color.Transparent;
            Invalidate(true);
        }

        protected override void LayoutInputChildren()
        {
            if (nud_value == null || valueTextBox == null || btn_sub == null || btn_add == null || lbl_line == null)
                return;

            lbl_line.Visible = false;
            nud_value.Visible = false;
            bool compactTextOnly = Width < StackedStepperMinimumWidth;
            bool stackedStepper = !compactTextOnly && Width < HorizontalStepperMinimumWidth;
            valueTextBox.Font = compactTextOnly || stackedStepper ? CompactValueFont : Font;
            int numericHeight = Math.Min(valueTextBox.PreferredHeight, Math.Max(1, Height - 6));

            if (compactTextOnly)
            {
                // 50px 内无法同时容纳 100.00、左右留白及两个可点击按钮。
                // 紧凑模式优先保证数值不裁切，步进仍可通过 ↑/↓ 和滚轮完成。
                btn_sub.Visible = false;
                btn_add.Visible = false;
                valueTextBox.SetBounds(3, Math.Max(2, (Height - numericHeight) / 2),
                    Math.Max(1, Width - 6), numericHeight);
            }
            else if (stackedStepper)
            {
                // 68~89px 采用常见的竖向步进器，只占一列；70px 参数框可完整
                // 显示 -0.50 / 100.00，同时保留鼠标加减功能。
                int buttonWidth = Math.Max(14, Math.Min(16, Height - 10));
                int spinnerLeft = Math.Max(3, Width - buttonWidth - 4);
                int spinnerHeight = Math.Max(2, Height - 6);
                int addHeight = Math.Max(1, spinnerHeight / 2);
                btn_add.SetBounds(spinnerLeft, 3, buttonWidth, addHeight);
                btn_sub.SetBounds(spinnerLeft, 3 + addHeight, buttonWidth,
                    Math.Max(1, spinnerHeight - addHeight));
                btn_sub.Visible = true;
                btn_add.Visible = true;
                valueTextBox.SetBounds(3, Math.Max(2, (Height - numericHeight) / 2),
                    Math.Max(1, spinnerLeft - 4), numericHeight);
            }
            else
            {
                int buttonWidth = Math.Max(14, Math.Min(16, Height - 11));
                int buttonHeight = Math.Max(1, Height - 6);
                int addLeft = Math.Max(3, Width - buttonWidth - 4);
                int subLeft = Math.Max(2, addLeft - buttonWidth);
                btn_sub.SetBounds(subLeft, 3, buttonWidth, buttonHeight);
                btn_add.SetBounds(addLeft, 3, buttonWidth, buttonHeight);
                btn_sub.Visible = true;
                btn_add.Visible = true;
                valueTextBox.SetBounds(4, Math.Max(2, (Height - numericHeight) / 2),
                    Math.Max(1, subLeft - 5), numericHeight);
            }

            valueTextBox.BringToFront();
            if (btn_sub.Visible)
                btn_sub.BringToFront();
            if (btn_add.Visible)
                btn_add.BringToFront();
        }

        private void SyncTextFromValue()
        {
            if (valueTextBox == null)
                return;
            string formatted = nud_value.Value.ToString("F" + DecimalPlaces, CultureInfo.CurrentCulture);
            if (valueTextBox.Text == formatted)
                return;

            syncingText = true;
            try
            {
                valueTextBox.Text = formatted;
            }
            finally
            {
                syncingText = false;
            }
        }

        private void valueTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar))
                return;

            string decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            char separator = decimalSeparator.Length > 0 ? decimalSeparator[0] : '.';
            if (e.KeyChar == separator && DecimalPlaces > 0 && valueTextBox.Text.IndexOf(separator) < 0)
                return;
            if (e.KeyChar == '-' && MinValue < 0 && valueTextBox.SelectionStart == 0 && valueTextBox.Text.IndexOf('-') < 0)
                return;
            e.Handled = true;
        }

        private void valueTextBox_TextChanged(object sender, EventArgs e)
        {
            if (syncingText)
                return;

            decimal parsed;
            if (!decimal.TryParse(valueTextBox.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out parsed))
                return;
            if (parsed < MinValue || parsed > MaxValue || parsed == nud_value.Value)
                return;
            editingTextValue = true;
            try
            {
                nud_value.Value = parsed;
            }
            finally
            {
                editingTextValue = false;
            }
        }

        private void valueTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                StepValue(Incremeent);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Down)
            {
                StepValue(-Incremeent);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void valueTextBox_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta != 0)
                StepValue(e.Delta > 0 ? Incremeent : -Incremeent);
            HandledMouseEventArgs handled = e as HandledMouseEventArgs;
            if (handled != null)
                handled.Handled = true;
        }

        private void StepValue(decimal delta)
        {
            decimal target = nud_value.Value + delta;
            if (target > MaxValue)
                target = MaxValue;
            if (target < MinValue)
                target = MinValue;
            if (target != nud_value.Value)
                nud_value.Value = target;
        }

        private void valueTextBox_Leave(object sender, EventArgs e)
        {
            SyncTextFromValue();
        }

        private void btn_sub_Paint(object sender, PaintEventArgs e)
        {
            DrawStepGlyph(e.Graphics, btn_sub.ClientRectangle, false,
                nud_value.Value > MinValue && Enabled);
        }

        private void btn_add_Paint(object sender, PaintEventArgs e)
        {
            DrawStepGlyph(e.Graphics, btn_add.ClientRectangle, true,
                nud_value.Value < MaxValue && Enabled);
        }

        private void DrawStepGlyph(Graphics graphics, Rectangle bounds, bool add, bool active)
        {
            ConfigureQuality(graphics);
            Color glyphColor = active ? (ContainsFocus ? InputFocusColor : Color.FromArgb(99, 116, 130)) : Color.FromArgb(185, 194, 201);
            float centerX = bounds.Width / 2F;
            float centerY = bounds.Height / 2F;
            float arm = Math.Max(3F, Math.Min(5F, bounds.Width / 4F));
            using (Pen pen = new Pen(glyphColor, 1.65F))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                graphics.DrawLine(pen, centerX - arm, centerY, centerX + arm, centerY);
                if (add)
                    graphics.DrawLine(pen, centerX, centerY - arm, centerX, centerY + arm);
            }
        }

        /// <summary>
        /// 值改变事件
        /// </summary>
        public event DValueChanged ValueChanged;
        /// <summary>
        /// 点击一下值的变化量
        /// </summary>
        private decimal _incremeent = 1;
        public decimal Incremeent
        {
            get { return _incremeent; }
            set { _incremeent = value; }
        }

        /// <summary>
        /// 小数位数
        /// </summary>
        private int _decimalPlaces = 0;
        public int DecimalPlaces
        {
            get { return _decimalPlaces; }
            set
            {
                _decimalPlaces = value;
                nud_value.DecimalPlaces = value;
                SyncTextFromValue();
            }
        }

        /// <summary>
        /// 最小值
        /// </summary>
        private decimal _minValue = 0;
        public decimal MinValue
        {
            get { return _minValue; }
            set
            {
                _minValue = value;
                nud_value.Minimum = value;
            }
        }

        /// <summary>
        /// 最大值
        /// </summary>
        private decimal _maxValue = 100;
        public decimal MaxValue
        {
            get { return _maxValue; }
            set
            {
                _maxValue = value;
                nud_value.Maximum = value;
            }
        }

        /// <summary>
        /// 值
        /// </summary>
        private double _value = 0;
        public double Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                nud_value.Text = value.ToString();
                if (!editingTextValue)
                    SyncTextFromValue();
            }
        }


        private void btn_add_MouseEnter(object sender, EventArgs e)
        {
            try
            {
                btn_add.BringToFront();
                if (nud_value.Value < MaxValue)
                {
                    btn_add.FlatAppearance.MouseDownBackColor = Color.FromArgb(218, 235, 249);
                    btn_add.FlatAppearance.MouseOverBackColor = Color.FromArgb(232, 243, 252);
                }
                else
                {
                    btn_add.FlatAppearance.MouseDownBackColor = InputSurfaceColor;
                    btn_add.FlatAppearance.MouseOverBackColor = InputSurfaceColor;
                }
                btn_add.Invalidate();
            }
            catch { }
        }
        private void btn_add_MouseLeave(object sender, EventArgs e)
        {
            btn_add.Invalidate();
        }
        private void btn_sub_MouseEnter(object sender, EventArgs e)
        {
            try
            {
                btn_sub.BringToFront();
                if (nud_value.Value > MinValue)
                {
                    btn_sub.FlatAppearance.MouseDownBackColor = Color.FromArgb(218, 235, 249);
                    btn_sub.FlatAppearance.MouseOverBackColor = Color.FromArgb(232, 243, 252);
                }
                else
                {
                    btn_sub.FlatAppearance.MouseDownBackColor = InputSurfaceColor;
                    btn_sub.FlatAppearance.MouseOverBackColor = InputSurfaceColor;
                }
                btn_sub.Invalidate();
            }
            catch { }
        }
        private void btn_add_Click(object sender, EventArgs e)
        {
            try
            {
                StepValue(Incremeent);

                if (nud_value.Value >= MaxValue)
                {
                    btn_add.FlatAppearance.MouseDownBackColor = InputSurfaceColor;
                    btn_add.FlatAppearance.MouseOverBackColor = InputSurfaceColor;
                }
                btn_add.Invalidate();
            }
            catch { }
        }
        private void btn_sub_Click(object sender, EventArgs e)
        {
            try
            {
                StepValue(-Incremeent);

                if (nud_value.Value <= MinValue)
                {
                    btn_sub.FlatAppearance.MouseDownBackColor = InputSurfaceColor;
                    btn_sub.FlatAppearance.MouseOverBackColor = InputSurfaceColor;
                }
                btn_sub.Invalidate();
            }
            catch { }
        }
        private void nud_value_ValueChanged(object sender, EventArgs e)
        {
            _value = (double)nud_value.Value;
            if (!editingTextValue)
                SyncTextFromValue();
            btn_sub.Invalidate();
            btn_add.Invalidate();
            if (ValueChanged != null)
                ValueChanged(Value);
        }
        private void UserControl1_Leave(object sender, EventArgs e)
        {
            Invalidate();
            btn_sub.Invalidate();
            btn_add.Invalidate();
        }
        private void UserControl1_Enter(object sender, EventArgs e)
        {
            Invalidate();
            btn_sub.Invalidate();
            btn_add.Invalidate();
        }
        private void btn_sub_MouseLeave(object sender, EventArgs e)
        {
            btn_sub.Invalidate();
        }

    }
}
