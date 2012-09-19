using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Elephant.Types;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Elephant.Param;

namespace Elephant
{

    /**
     *  The keyValuePairs component allows the keys to be named on the left hand side, and creates an output on the right hand side.
     *  The input is made by creating names in the ZUI
     */
    public class HashPairsComponent : GH_Component, IGH_VariableParameterComponent
    {
        public HashPairsComponent()
            : base("Hash Pairs From Labels", "HashPairs", "Create an automatic key/value pair from a string", "Extra", "Elephant")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.Register_GenericParam("Label_1", "Input key/value", "Input any value here to create a key / value pair");
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_GenericParam("Hash", "H", "A key/value hash");
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<HashType> Output = new List<HashType>();
            foreach (IGH_Param Input in this.Params.Input)
            {
                object value = "";
                DA.GetData(Input.Name, ref value);
                Output.Add(new HashType(Input.NickName, value));
            }

            //Output.Add(
            
            DA.SetDataList(0, Output);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("{df79d0f6-f23f-4bfc-befb-3ebcbc472a8e}"); }
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Elephant.Icons.icons_05;
            }
        }
        #region IGH_VariableParameterComponent Members

        public bool CanInsertParameter(GH_ParameterSide side, int index)
        {
            if (side == GH_ParameterSide.Output)
            {
                return false;
            }
            return true;
        }

        public bool CanRemoveParameter(GH_ParameterSide side, int index)
        {
            if (side == GH_ParameterSide.Output)
            {
                return false;
            }

            return true;
        }

        /**
         * CreateParameter: check if the name already exists on the left hand side. If it does, do not create it.
         * Create new parameters on the fly
         */
        public IGH_Param CreateParameter(GH_ParameterSide side, int index)
        {
            Param_String param = new Param_String();


            string name = "Label";
            string compareName = name;
            int iteration = 0;
            bool nameFound = false;

            while (nameFound == false)
            {
                bool NameExists = false;

                if (iteration > 0)
                {
                    compareName = name + "_" + iteration.ToString();
                }

                foreach (IGH_Param Input in this.Params.Input)
                {
                    if (Input.NickName == compareName)
                    {
                        NameExists = true;
                        break;
                    }
                }
                if (NameExists)
                {
                    iteration++;
                } else {
                    nameFound = true;
                }
            }

            param.NickName = compareName;

            return param;
        }

        // allways allow the parameter to be destroyed.
        public bool DestroyParameter(GH_ParameterSide side, int index)
        {
            return true;
        }

        // When something changed, expire the solution
        public void VariableParameterMaintenance()
        {
            this.ExpireSolution(true);
        }

        #endregion
    }
}
