using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GITS.Util.Json {
    public abstract class JsonVar {
        private string name;
        public JsonVar(string name) {
            this.name = name;
        }

        public void setName(string name) {
            this.name = name;
        }

        public string getName() {
            return this.name;
        }

        /// <summary>
        /// Writes the value of this node to the stream. Note, this should not write anything before the ":" symbol.
        /// For example, if this is a string note, it should output Exactly this: "<value>The node value</value>"
        /// Nothing else, the comma will be added by the serializer
        /// Another example, if this is a JSonNumber and it's value is 133.7, the output must be "133.7" (without the ")
        /// </summary>
        /// <param name="?"></param>
        public abstract void writeToStream(StreamWriter stream);
    }
}
