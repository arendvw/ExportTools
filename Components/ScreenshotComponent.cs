using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Drawing;
using System.IO;

namespace Elephant.Components
{
    public class ScreenshotComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public ScreenshotComponent()
            : base("ScreenshotComponent", "Screenshot",
                "Takes a screenshot from a viewport",
                "Extra", "Elephant")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("filename", "F", "The destination filename", GH_ParamAccess.item);
            pManager.AddIntegerParameter("height", "H", "Height (pixels) of screenshot", GH_ParamAccess.item, 600);
            pManager.AddIntegerParameter("width", "W", "Width (pixels) of screenshot", GH_ParamAccess.item, 800);
            pManager.AddTextParameter("viewport", "VP", "Viewport to capture", GH_ParamAccess.item, "Perspective");
            pManager.AddBooleanParameter("enabled", "E", "Enable the screenshot", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_StringParam("file", "F", "The output filename");
            pManager.Register_BooleanParam("success", "S", "True if successfully written, false if failed");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string filename = "";
            DA.GetData("filename", ref filename);
            string viewport = "";
            DA.GetData("viewport", ref viewport);
            int width = 0;
            DA.GetData("width", ref width);
            int height = 0;
            DA.GetData("height", ref height);
            bool enabled = false;
            DA.GetData("enabled", ref enabled);

            Rhino.RhinoDoc doc = Rhino.RhinoDoc.ActiveDoc;


            if (!enabled)
            {
                DA.SetData("file", null);
                DA.SetData("success", false);
                return;
            }

            try
            {

                // get the viewport named views
                Rhino.Display.RhinoView vp = doc.Views.Find(viewport, false);

                // get the bitmap in the right size
                Bitmap bitmap = Rhino.Display.DisplayPipeline.DrawToBitmap(vp.ActiveViewport, width, height);

                string ext = Path.GetExtension(filename).ToLower().Replace(".", "");


                System.Drawing.Imaging.ImageFormat imageFormat;

                switch (ext)
                {
                    case "jpeg":
                    case "jpg":
                        imageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
                        break;
                    case "gif":
                        imageFormat = System.Drawing.Imaging.ImageFormat.Gif;
                        break;
                    case "bmp":
                        imageFormat = System.Drawing.Imaging.ImageFormat.Bmp;
                        break;
                    case "png":
                        imageFormat = System.Drawing.Imaging.ImageFormat.Png;
                        break;
                    case "tif":
                    case "tiff":
                        imageFormat = System.Drawing.Imaging.ImageFormat.Tiff;
                        break;

                    default:
                        throw new Exception(String.Format("unknown image format: {0}", ext));
                }

                // save the bitmap
                bitmap.Save(filename, imageFormat);
                DA.SetData("file", filename);
                DA.SetData("success", System.IO.File.Exists(filename));
            }
            catch (Exception e)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
                DA.SetData("file", null);
                DA.SetData("success", false);
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
                return Elephant.Icons.screenshot;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{17931631-cc54-47f7-a0b7-4f334cd00473}"); }
        }
    }
}