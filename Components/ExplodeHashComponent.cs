using System;   
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Elephant.Param;
using Elephant.Types;
using System.Collections;

namespace Elephant
{

    /** 
     * 
     * Test one more time. Explode components by name
     * 
     */

    public class ExplodeHashComponent : GH_Component
    {
        public ExplodeHashComponent() : base("Hash Explode", "HashExplode", "Get the key and value from a hash pair", "Extra", "Elephant")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.RegisterParam(new HashParam(), "Hash", "H", "Input hash", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_StringParam("Key", "K", "A string key");
            pManager.Register_GenericParam("Value", "V", "A value key");
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            HashType value = new HashType();
            DA.GetData("Hash", ref value);
            List<object> valuelist = new List<object>();
            valuelist.Add(value.Key);
            DA.SetDataList(0, valuelist);
            IEnumerable list = value.HashValue as IEnumerable;
            if (list != null)
            {
                DA.SetDataList(1, list);
            }
            else
            {
                List<object> outlist = new List<object>();
                outlist.Add(value.HashValue);
                DA.SetDataList(1, outlist);
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("{6B390060-CB1A-4ecf-8DAB-061579855B13}"); }
        }        

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Icons.icons_06;
            }
        }
    }
}
