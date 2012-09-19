using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Elephant.Types;
using Elephant.Param;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using System.Collections;

namespace Elephant
{
    /** 
     * The auto pairs component allows the creation of a tuple by looking downstream to the name of the tuple. This means that outputs have to be named, and
     * saves typing the pairs.
     */
    public class AutoHashComponent : GH_Component
    {
        public AutoHashComponent()
            : base("AutoHash Input", "AutoHash", "Create an automatic key/value pair from a string. Note: this does not work with tree input!!", "Extra", "Elephant")
        {
        }


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.Register_GenericParam("Input", "I", "Input value", GH_ParamAccess.tree);
            pManager.Register_BooleanParam("Enumerate", "E", "Enumerate lists to sequentally named items, or create a list of values", false, GH_ParamAccess.item);
            this.Params.Input[0].ObjectChanged += resetMapping;
            this.Params.Input[0].AttributesChanged += resetMapping;
        }

        protected void resetMapping (Object sender, Object ev)
        {
            this.Params.Input[0].Simplify = false;
            this.Params.Input[0].Reverse = false;
            this.Params.Input[0].DataMapping = GH_DataMapping.None;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new HashParam(), "Hash", "H", "A key/value Hash");
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_Structure<IGH_Goo> Input = new GH_Structure<IGH_Goo>();
            DA.GetDataTree("Input", out Input);

            GH_Structure<HashType> Output = new GH_Structure<HashType>();
            bool bEnumerate = false;
            DA.GetData("Enumerate", ref bEnumerate);
            // iterate through all the sources by the Params.input value (and thus bypass the DA object, is this ugly?)

            foreach (IGH_Param Param in this.Params.Input[0].Sources)
            {

                foreach (GH_Path path in Input.Paths)
                {
                    String nickname = Param.NickName;
                    if (nickname.Length == 0)
                    {
                        this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Nickname of the connected component is empty");
                        nickname = "Anonymous";
                    }
                    List<object> DataList = new List<object>();
                    IEnumerable Data = Param.VolatileData.get_Branch(path);
                    if (Data != null)
                    {
                        foreach (object item in Data)
                        {
                            DataList.Add(item);
                        }
                    }
                    //
                    // Add the data to the list. If result is a tree or something similar: this means the results will be flattened.
                    
                    if (Data == null || DataList.Count == 0)
                    {
                        
                    }
                    else if (DataList.Count == 1)
                    {
                        // If the component has a single output: name it singular
                        Output.Append(new HashType(nickname, DataList[0]), path);
                    }
                    else if (DataList.Count > 1)
                    {
                        // .. otherwise: add a
                        int j = 0;
                        if (bEnumerate)
                        {
                            foreach (object item in DataList)
                            {
                                Output.Append(new HashType(String.Format("{0}_{1}", nickname, j), item), path);
                                j++;
                            }
                        }
                        else
                        {
                            Output.Append(new HashType(nickname, DataList), path);
                        }
                    }
                }
            }
            DA.SetDataTree(0, Output);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("{2C479128-F6CA-435d-8CED-C2B3B99A03CC}"); }
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Elephant.Icons.icons_04;
            }
        }


    }
}