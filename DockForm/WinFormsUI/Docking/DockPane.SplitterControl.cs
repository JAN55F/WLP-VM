using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace WeifenLuo.WinFormsUI.Docking
{
    partial class DockPane
    {
        private class SplitterControl : Control, ISplitterDragSource
        {
            DockPane m_pane;

            public SplitterControl(DockPane pane)
            {
                SetStyle(ControlStyles.Selectable, false);
                m_pane = pane;
            }

            public DockPane DockPane
            {
                get { return m_pane; }
            }

            private DockAlignment m_alignment;
            public DockAlignment Alignment
            {
                get { return m_alignment; }
                set
                {
                    m_alignment = value;
                    if (m_alignment == DockAlignment.Left || m_alignment == DockAlignment.Right)
                        Cursor = Cursors.VSplit;
                    else if (m_alignment == DockAlignment.Top || m_alignment == DockAlignment.Bottom)
                        Cursor = Cursors.HSplit;
                    else
                        Cursor = Cursors.Default;

                    if (DockPane.DockState == DockState.Document)
                        Invalidate();
                }
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);

                if (DockPane.DockState != DockState.Document)
                    return;

                Graphics g = e.Graphics;
                Rectangle rect = ClientRectangle;
                if (Alignment == DockAlignment.Top || Alignment == DockAlignment.Bottom)
                    g.DrawLine(SystemPens.ControlDark, rect.Left, rect.Bottom - 1, rect.Right, rect.Bottom - 1);
                else if (Alignment == DockAlignment.Left || Alignment == DockAlignment.Right)
                    g.DrawLine(SystemPens.ControlDarkDark, rect.Right - 1, rect.Top, rect.Right - 1, rect.Bottom);
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                base.OnMouseDown(e);

                if (e.Button != MouseButtons.Left)
                    return;

                DockPane.DockPanel.BeginDrag(this, Parent.RectangleToScreen(Bounds));
            }

            #region ISplitterDragSource Members

            void ISplitterDragSource.BeginDrag(Rectangle rectSplitter)
            {
            }

            void ISplitterDragSource.EndDrag()
            {
            }

            bool ISplitterDragSource.IsVertical
            {
                get
                {
                    NestedDockingStatus status = DockPane.NestedDockingStatus;
                    return (status.DisplayingAlignment == DockAlignment.Left ||
                        status.DisplayingAlignment == DockAlignment.Right);
                }
            }

            Rectangle ISplitterDragSource.DragLimitBounds
            {
                get
                {
                    NestedDockingStatus status = DockPane.NestedDockingStatus;
                    Rectangle rectLimit = Parent.RectangleToScreen(status.LogicalBounds);
                    int minimumThisPane = GetMinimumPaneDimension(
                        DockPane, ((ISplitterDragSource)this).IsVertical, null);
                    int minimumRemainingArea = GetMinimumPaneDimension(
                        status.DisplayingPreviousPane, ((ISplitterDragSource)this).IsVertical, DockPane);
                    if (((ISplitterDragSource)this).IsVertical)
                    {
                        if (status.DisplayingAlignment == DockAlignment.Left)
                        {
                            rectLimit.X += minimumThisPane;
                            rectLimit.Width -= minimumThisPane + minimumRemainingArea;
                        }
                        else
                        {
                            rectLimit.X += minimumRemainingArea;
                            rectLimit.Width -= minimumRemainingArea + minimumThisPane;
                        }
                    }
                    else
                    {
                        if (status.DisplayingAlignment == DockAlignment.Top)
                        {
                            rectLimit.Y += minimumThisPane;
                            rectLimit.Height -= minimumThisPane + minimumRemainingArea;
                        }
                        else
                        {
                            rectLimit.Y += minimumRemainingArea;
                            rectLimit.Height -= minimumRemainingArea + minimumThisPane;
                        }
                    }

                    return rectLimit;
                }
            }

            private static int GetMinimumPaneDimension(DockPane pane, bool vertical, DockPane excludedPane)
            {
                if (pane == null || pane.ActiveContent == null)
                    return MeasurePane.MinSize;

                Form form = pane.ActiveContent.DockHandler.Form;
                if (form == null)
                    return MeasurePane.MinSize;

                int minimum = Math.Max(MeasurePane.MinSize,
                    vertical ? form.MinimumSize.Width : form.MinimumSize.Height);
                NestedPaneCollection nestedPanes = pane.NestedDockingStatus.NestedPanes;
                if (nestedPanes == null)
                    return minimum;

                foreach (DockPane childPane in nestedPanes)
                {
                    if (childPane == excludedPane)
                        continue;

                    NestedDockingStatus childStatus = childPane.NestedDockingStatus;
                    if (childStatus.DisplayingPreviousPane != pane)
                        continue;

                    int childMinimum = GetMinimumPaneDimension(childPane, vertical, null);
                    bool splitsAlongDimension = vertical
                        ? (childStatus.DisplayingAlignment == DockAlignment.Left ||
                           childStatus.DisplayingAlignment == DockAlignment.Right)
                        : (childStatus.DisplayingAlignment == DockAlignment.Top ||
                           childStatus.DisplayingAlignment == DockAlignment.Bottom);
                    minimum = splitsAlongDimension
                        ? minimum + Measures.SplitterSize + childMinimum
                        : Math.Max(minimum, childMinimum);
                }
                return minimum;
            }

            void ISplitterDragSource.MoveSplitter(int offset)
            {
                NestedDockingStatus status = DockPane.NestedDockingStatus;
                double proportion = status.Proportion;
                if (status.LogicalBounds.Width <= 0 || status.LogicalBounds.Height <= 0)
                    return;
                else if (status.DisplayingAlignment == DockAlignment.Left)
                    proportion += ((double)offset) / (double)status.LogicalBounds.Width;
                else if (status.DisplayingAlignment == DockAlignment.Right)
                    proportion -= ((double)offset) / (double)status.LogicalBounds.Width;
                else if (status.DisplayingAlignment == DockAlignment.Top)
                    proportion += ((double)offset) / (double)status.LogicalBounds.Height;
                else
                    proportion -= ((double)offset) / (double)status.LogicalBounds.Height;

                DockPane.SetNestedDockingProportion(proportion);
            }

            #region IDragSource Members

            Control IDragSource.DragControl
            {
                get { return this; }
            }

            #endregion

            #endregion
        }

        private SplitterControl m_splitter;
        private SplitterControl Splitter
        {
            get { return m_splitter; }
        }

        internal Rectangle SplitterBounds
        {
            set { Splitter.Bounds = value; }
        }

        internal DockAlignment SplitterAlignment
        {
            set { Splitter.Alignment = value; }
        }
    }
}
