using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Elephant.Components
{
    public class PathinfoComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public PathinfoComponent()
            : base("Pathinfo", "Pathinfo",
                "The folder of the current grasshopper file",
                "Extra", "Elephant")
        {
        }

        protected bool started = false;

        protected void refreshComponent(Object Sender, Object args)
        {
            this.ExpireSolution(true);
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
            pManager.Register_StringParam("Directory", "D", "The current working directory of the grasshopper definition");
            pManager.Register_StringParam("Filename", "F", "The current filename");
            pManager.Register_StringParam("Extension", "E", "The current extension");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (!this.started)
            {
                this.OnPingDocument().ModifiedChanged += refreshComponent;
                this.started = true;
            }

            if (OnPingDocument().IsFilePathDefined)
            {
                string path = OnPingDocument().FilePath;
                DA.SetData("Directory", System.IO.Path.GetDirectoryName(path));
                DA.SetData("Filename", System.IO.Path.GetFileNameWithoutExtension(path));
                DA.SetData("Extension", System.IO.Path.GetExtension(path));
            }  else {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The current file is not saved. Unable to get path");
            }
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
                return Elephant.Icons.folder;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{3e98be13-531c-4767-9eee-d3356233d6da}"); }
        }
    }
}