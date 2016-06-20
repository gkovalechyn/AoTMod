using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace GITS.Util.Json {
    class JsonBool : JsonVar{
        private bool value;
        public JsonBool(string name) : base(name){
        }

        public void setValue(bool value) {
            this.value = value;
        }

        public bool getValue() {
            return this.value;
        }
        public override void writeToStream(System.IO.StreamWriter stream) {
            stream.Write(this.value.ToString());
        }

        public override string ToString() {
            return this.value.ToString();
        }

        public override bool Equals(object obj) {
            if (obj != null && obj is JsonBool) {
                return this.value == ((JsonBool)obj).value;
            }

            return false;
        }

        public override int GetHashCode() {
            int hash = 5;
            hash = 89 * hash + (this.value ? 1 : 0);
            return hash;
        }
    }
}
