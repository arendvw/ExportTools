using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;


namespace Elephant.Types
{
    public class HashType : GH_Goo<KeyValuePair<string,object>>
    {
        public HashType()
        {
            this.Value = new KeyValuePair<string,object>("undefined","undefined");
        }

        // copy constructor
        public HashType(HashType source)
        {
            this.Value = source.Value;
        }

        public HashType(string key, Object value)
        {
            this.Value = new KeyValuePair<string, object>(key, value);
        }

        public override IGH_Goo Duplicate()
        {
            return new HashType(this);
        }

        public override bool IsValid
        {
            get { return true; }
        }

        public override string ToString()
        {
            Object outValue = null;
            if (this.Value.Value == null)
            {
                outValue = "";
            } else {
                outValue = this.Value.Value;
            }
            return String.Format("[{0},{1}]", this.Value.Key, outValue);
        }

        public override string TypeDescription
        {
            get { return "A key/value hash item to store nested variables"; }
        }

        public override string TypeName
        {
            get { return "Hash"; }
        }

        public override object ScriptVariable()
        {
            return new KeyValuePair<string, object>(this.Value.Key, this.Value.Value);
        }

        public string Key { get { return Value.Key; } }

        public Object HashValue { get { return Value.Value; } }
    }
}
