using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pepperSoft.Vue
{
    [System.ComponentModel.DesignerCategory("Code")]
    class DragListView : System.Windows.Controls.ListView
    {
      /*  protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            // easy mouse selection
            if (MouseButtons == MouseButtons.Left)
            {
                var item = this.HitTest(e.Location).Item;
                if (item != null)
                    item.Selected = true;
            }
            base.OnMouseMove(e);
        }
        */
        /*protected override void OnKeyDown(KeyEventArgs e)
        {
            // ctrl-a - select all
            if (e.KeyCode == Keys.A && e.Control)
                SelectAll();
            base.OnKeyDown(e);
        }*/
    }
}
