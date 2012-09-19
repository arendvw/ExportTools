using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
/**
 * 
 * This class generates a unique id from a set of data. This means that it will create a unique name for a set if input values. You can create a unique fingerprint for a set of inputs;
 * 
 */
 
namespace Elephant
{
    public class GenerateUniqueIdComponent : GH_Component
    {
        public GenerateUniqueIdComponent()
            : base("Generate Unique ID", "GenID", "Generate a unique Id from a set of tuples", "Extra", "Elephant")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.Register_GenericParam("Input", "I", "Input any serializable data", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_StringParam("Fingerprint", "F", "A sha1 hash of the set of data");
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<IGH_Goo> DataList = new List<IGH_Goo>();
            List<Object> DataListNative = new List<Object>();
            //foreach (O

            DA.GetDataList<IGH_Goo>("Input", DataList);

            foreach (IGH_Goo item in DataList)
            {
                DataListNative.Add(item.ScriptVariable());
            }
            // create a new fingerprint from the native datalist.
            string dirty = Tools.getHash(DataListNative);
            string clean = System.Text.RegularExpressions.Regex.Replace(dirty, @"[\W]", "");
            DA.SetData(0,clean);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("{CF93EA0C-708D-4fd6-8715-52179A02544A}"); }
        }
        
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Elephant.Icons.fingerprint;
            }
        }
    }
}
