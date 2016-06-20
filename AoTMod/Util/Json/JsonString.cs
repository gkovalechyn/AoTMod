using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GITS.Util.Json {
    public class JsonString : JsonVar{
        private string value;
        public JsonString(string name): base(name) {
        }

        public string getValue() {
            return this.value;
        }

        public void setValue(string value) {
            this.value = value;
        }
        public override void writeToStream(System.IO.StreamWriter stream) {
            stream.Write('\"');
            stream.Write(Util.escapeString(this.value));
            stream.Write('\"');
        }

        public override bool Equals(object obj) {
            if (obj != null && obj is JsonString) {
                return this.value.Equals(((JsonString)obj).value);
            }

            return false;
        }

        public override string ToString() {
            return "\"" + this.value + "\"";
        }
        public override int GetHashCode() {
            int hash = 347 * 2903;
            return hash * (this.value != null ? this.value.GetHashCode() : 13);
        }
    }
}
