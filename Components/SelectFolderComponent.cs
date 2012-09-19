using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows.Forms;

namespace Elephant.Components
{
    public class SelectFolderComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SelectFolderComponent class.
        /// </summary>
        public SelectFolderComponent()
            : base("SelectFolderComponent", "Folder",
                "Select a folder",
                "Extra", "Elephant")
        {
        }

        protected string selectedFolder = null;

        public override void CreateAttributes()
        {

            m_attributes = new SelectFolderAttributes(this);

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
            pManager.Register_StringParam("path", "P", "The selected path");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DA.SetData("path", this.selectedFolder);
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
            get { return new Guid("{b797dbd1-fe38-4d30-83ce-80c9cc898f75}"); }
        }


        internal Grasshopper.GUI.Canvas.GH_ObjectResponse SelectFolderDialog()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowNewFolderButton = true;
            if (this.selectedFolder != null && System.IO.Directory.Exists(this.selectedFolder))
            {
                fbd.SelectedPath = this.selectedFolder;
            }
            else if (this.OnPingDocument().IsFilePathDefined)
            {
                fbd.SelectedPath = System.IO.Path.GetDirectoryName(this.OnPingDocument().FilePath);
            }
            else
            {
                fbd.SelectedPath = System.Environment.SpecialFolder.Personal.ToString();
            }

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                this.selectedFolder = fbd.SelectedPath;
                this.ExpireSolution(true);
            }
            return Grasshopper.GUI.Canvas.GH_ObjectResponse.Handled;
        }


        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            writer.SetString("path", this.selectedFolder);
            return base.Write(writer);
        }


        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            reader.TryGetString("path", ref selectedFolder);
            return base.Read(reader);
        }
    }
}