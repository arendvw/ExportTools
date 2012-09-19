using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elephant.Components
{
    class SelectFolderAttributes : Grasshopper.Kernel.Attributes.GH_ComponentAttributes 
    {
        SelectFolderComponent c;
        public SelectFolderAttributes(SelectFolderComponent _c) : base(_c)
        {
            c = _c;
        }

        public override Grasshopper.GUI.Canvas.GH_ObjectResponse RespondToMouseDoubleClick(Grasshopper.GUI.Canvas.GH_Canvas sender, Grasshopper.GUI.GH_CanvasMouseEvent e)
        {

            return c.SelectFolderDialog();
        }
    }
}
