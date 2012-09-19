﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Elephant.Param;
using System.IO;
using Elephant.Types;

namespace Elephant.Components
{
    public class HashToCsvComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public HashToCsvComponent()
            : base("Hash to CSV", "H2CSV",
                "Export a hash to a CSV File",
                "Extra", "Elephant")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.RegisterParam(new HashParam(), "Hash", "H", "Hash input", GH_ParamAccess.list);
            this.Params.Input[0].DataMapping = GH_DataMapping.Flatten;
            pManager.Register_StringParam("Filename", "F", "The destination file of the hash");
            pManager.Register_BooleanParam("Append", "A", "Append to file", true);
            pManager.Register_BooleanParam("genId", "I", "Generate unique ID column", true);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_BooleanParam("Success", "S", "True if exported, false if failed");
            pManager.Register_IntegerParam("ID", "I", "Autogenerated ID");
        }

        char seperator = ';';
        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string filename = "";
            bool append = false;
            bool genId = false;
            List<HashType> hashes = new List<HashType>();
            // list of values to write
            Dictionary<string, object> outList = new Dictionary<string, object>();
            List<string> keyMap = new List<string>();
            List<int> ids = new List<int>();
            DA.GetData("Filename", ref filename);
            DA.GetData("Append", ref append);
            DA.GetData("genId", ref genId);
            DA.GetDataList("Hash", hashes);
            DA.SetData(0, false);

            foreach (HashType hash in hashes)
            {
                outList.Add(hash.Key, hash.HashValue);
            }

            string outputfilename = filename + ".temp";

            List<string> headers = new List<string>();

            StreamWriter OutputFile = new StreamWriter(outputfilename);

            if (File.Exists(filename) && append)
            {
                StreamReader file = new StreamReader(filename);
                headers.AddRange(file.ReadLine().Split(this.seperator));
                foreach (string key in outList.Keys)
                {
                    if (!headers.Contains(key))
                    {
                        headers.Add(key);
                    }
                }

                if (genId && !headers.Contains("id"))
                {
                    headers.Add("id");
                }
                OutputFile.WriteLine(String.Join(new String(this.seperator, 1), headers.ToArray()));

                if (!genId)
                {
                    OutputFile.Write(file.ReadToEnd());
                }
                else
                {
                    string line;
                    int idColumn = headers.IndexOf("id");
                    while ((line = file.ReadLine()) != null)
                    {
                        OutputFile.WriteLine(line);
                        String[] vals = line.Split(this.seperator);
                        if ((vals.Length - 1) >= idColumn)
                        {
                            Int32 val = -1;
                            if (vals[idColumn] != null)
                            {
                                Int32.TryParse(vals[idColumn], out val);
                            }
                            if (val != -1)
                            {
                                ids.Add(val);
                            }
                        }
                    }

                }

                if (genId)
                {
                    ids.Sort();
                    if (ids.Count > 0)
                    {
                        outList.Add("id", ids[ids.Count - 1] + 1);
                    }
                    else
                    {
                        outList.Add("id", 1);
                    }
                    DA.SetData(1, outList["id"]);
                }

                file.Close();
                List<string> values = new List<string>();
                foreach (string key in headers)
                {
                    if (outList[key] != null)
                    {
                        values.Add(outList[key].ToString());
                    } else {
                        values.Add("");
                    }
                }
                OutputFile.WriteLine(String.Join(new String(this.seperator, 1), values.ToArray()));
                OutputFile.Close();
                System.IO.File.Delete(filename);
                System.IO.File.Move(outputfilename, filename);

            }
            else
            {
                headers.AddRange(outList.Keys);
                List<string> values = new List<string>();
                foreach (string key in headers)
                {
                    values.Add(outList[key].ToString());
                }
                OutputFile.WriteLine(String.Join(new String(this.seperator, 1), headers.ToArray()));
                OutputFile.WriteLine(String.Join(new String(this.seperator, 1), values.ToArray()));
                OutputFile.Close();
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }

                System.IO.File.Move(outputfilename, filename);
            }
            DA.SetData(0, true);
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
                return Icons.csv;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{ede6e4ce-57b3-46b0-bce0-1487052d79f3}"); }
        }
    }
}