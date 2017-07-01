using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Prez
{
    public class OritatamiiControl
    {
        private readonly SplitContainer splitContainer;
        private readonly List<Control> controls;
        private readonly int detailSettingOpenSize;
        private readonly int detailSettingCloseSize;
        private readonly Form form;
        private readonly int diffHeight;
        private bool IsClosed { get; set; }

        private OritatamiiControl() {
            throw new NotImplementedException();
        }

        public OritatamiiControl(SplitContainer splitContainer) {
            this.splitContainer = splitContainer;
            this.controls = new List<Control>();
            this.detailSettingOpenSize = this.splitContainer.Size.Height;
            this.detailSettingCloseSize = this.splitContainer.Size.Height / 2;
            this.form = (Form)splitContainer.Parent;
            this.diffHeight = this.detailSettingOpenSize - this.detailSettingCloseSize;
            Action initSplitContainer = () => {
                this.splitContainer.Panel2MinSize = 0;
                this.splitContainer.IsSplitterFixed = true;
            };
            initSplitContainer();
            Action<Form> addControlsUnderSplitContainer = (target) => {
                List<Control> controls = GetAllControls<Control>(target);
                foreach (Control control in controls) {
                    if(control.Location.Y >= splitContainer.Location.Y + detailSettingOpenSize) {
                        Add(control);
                    }
                }
            };
            addControlsUnderSplitContainer(this.form);
            IsClosed = false;
        }

        private void Add(Control control) {
            controls.Add(control);
        }

        private bool OpenDetailSettingPanel() {
            if(!IsClosed) {
                return false;
            }
            this.splitContainer.Panel2Collapsed = false;
            this.splitContainer.Size = new Size(this.splitContainer.Size.Width, this.detailSettingOpenSize);
            foreach (Control control in controls) {
                control.Location = new Point(control.Location.X, control.Location.Y + this.diffHeight);
            }
            this.form.Size = new Size(form.Size.Width, this.form.Height + diffHeight);
            IsClosed = false;
            return true;
        }

        private bool CloseDetailSettingPanel() {
            if (IsClosed) {
                return false;
            }
            this.splitContainer.Panel2Collapsed = true;
            this.splitContainer.Size = new Size(this.splitContainer.Size.Width, this.detailSettingCloseSize);
            foreach (Control control in controls) {
                control.Location = new Point(control.Location.X, control.Location.Y - this.diffHeight);
            }
            this.form.Size = new Size(this.form.Size.Width, this.form.Size.Height - this.diffHeight);
            IsClosed = true;
            return true;
        }

        public void ChangeOritatamiState() {
            if(OpenDetailSettingPanel()) {
                return;
            }
            CloseDetailSettingPanel();
        }

        private List<T> GetAllControls<T>(Control top) where T : Control {
            List<T> buf = new List<T>();
            foreach (Control ctrl in top.Controls) {
                if (ctrl is T)
                    buf.Add((T)ctrl);
                buf.AddRange(GetAllControls<T>(ctrl));
            }
            return buf;
        }
    }
}
