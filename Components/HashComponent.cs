using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Elephant.Types;
using Elephant.Param;

namespace Elephant
{
    /**
     *  Create a pair manually from a name and a value
     */
    public class HashComponent : GH_Component
    {
        public HashComponent() : base("Hash", "Hash", "Create a simple key/value pair", "Extra", "Elephant")
        {
        }


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Key", "K", "Key", GH_ParamAccess.item);
            pManager.AddTextParameter("Value", "V", "Value", GH_ParamAccess.item, (string) "");
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new HashParam(), "Hash", "H", "A key/value hash");
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            String name = null;
            Object value = null;
            DA.GetData(0, ref name);
            DA.GetData(1, ref value);
            DA.SetData(0, new HashType(name, value));
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("{2DCDCDDE-F90E-4b3c-8270-2F9FB5A829B5}"); }
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Elephant.Icons.icons_02;
            }
        }
    }
}
