using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper;
using Grasshopper.Kernel;
using Elephant.Types;

namespace Elephant.Param
{
    class HashParam : GH_PersistentParam<HashType>
    {
        // We need to supply a constructor without arguments that calls the base class constructor.
        public HashParam()
            : base(new GH_InstanceDescription("HashParam", "Hash", "Represents a collection Key/Value pairs", "Extra", "Elephant"))
        { }

        public override Guid ComponentGuid
        {
            get { return new Guid("{069a5151-2fd9-494f-b634-0ff9740feede}"); }
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Icons.hash;
            }
        }

        protected override GH_GetterResult Prompt_Plural(ref List<HashType> values)
        {
            return GH_GetterResult.cancel;
        }

        protected override GH_GetterResult Prompt_Singular(ref HashType value)
        {
            return GH_GetterResult.cancel;
        }
    }
}