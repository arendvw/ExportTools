using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Elephant
{
    public class UnitsComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public UnitsComponent()
            : base("Current Units", "Units",
                "Get the current units of the rhino document",
                "Extra", "Elephant")
        {
            // make sure the component expires when the document properties change.
            Rhino.RhinoDoc.DocumentPropertiesChanged += propertiesChanged;
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_StringParam("Unit", "U", "The unit of the current rhino document");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DA.SetData(0, Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem.ToString());
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Elephant.Icons.units;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{c5f9342b-377c-47c9-b445-8fd572563960}"); }
        }

        public void propertiesChanged(Object sender, Rhino.DocumentEventArgs args)
        {
            this.ExpireSolution(true);
        }
    }
}