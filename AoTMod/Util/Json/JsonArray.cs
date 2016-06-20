using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GITS.Util.Json {
    public class JsonArray : JsonVar{
        private List<JsonVar> items = new List<JsonVar>();
        private int hashCode;
        private bool dirty = true;
        public JsonArray() : base("") {

        }
        public JsonArray(string name): base(name) {

        }

        public void add(int i) {
            JsonNumber number = new JsonNumber("ArrayItem");
            number.setValue(i);
            this.items.Add(number);
            this.dirty = true;
        }

        public void add(double d) {
            JsonNumber number = new JsonNumber("ArrayItem");
            number.setValue(d);
            this.items.Add(number);
            this.dirty = true;
        }

        public void add(string s) {
            JsonString number = new JsonString("ArrayItem");
            number.setValue(s);
            this.items.Add(number);
            this.dirty = true;
        }

        public void add(bool b) {
            JsonBool number = new JsonBool("ArrayItem");
            number.setValue(b);
            this.items.Add(number);
            this.dirty = true;
        }
        /*
        public void add(JsonObject obj) {
            this.items.Add(obj);
            this.dirty = true;
        }
         * */
        public void add(JsonVar var) {
            this.items.Add(var);
            this.dirty = true;
        }

        public JsonVar at(int index) {
            return this.items[index];
        }

        public void removeAt(int index) {
            this.items.RemoveAt(index);
            this.dirty = true;
        }

        public void clear() {
            this.items.Clear();
            this.dirty = true;
        }

        public bool contains(JsonVar var) {
            return this.items.Contains(var);
        }
        public bool isEmpty() {
            return this.size() == 0;
        }

        public int size() {
            return this.items.Count;
        }

        public override void writeToStream(System.IO.StreamWriter stream) {
            stream.Write("[");

            for (int i = 0; i < items.Count - 1; i++) {
                items[i].writeToStream(stream);
                stream.Write(", ");
            }

            if (items.Count > 0) {
                items[items.Count - 1].writeToStream(stream);
            }

            stream.Write("]");
        }

        public override string ToString() {
            StringBuilder builder = new StringBuilder(128);
            builder.Append("[");

            for (int i = 0; i < items.Count - 1; i++) {
                builder.Append(items[i].ToString());
                builder.Append(", ");
            }

            if (items.Count > 0) {
                builder.Append(items[items.Count - 1].ToString());
            }

            builder.Append("]");
            return builder.ToString();
        }
        public override bool Equals(object obj) {
            if ((obj != null) && (obj is JsonArray)) {
                JsonArray other = (JsonArray)obj;

                if (this.size() != other.size()) {
                    return false;
                }

                for (int i = 0; i < this.size(); i++) {
                    if (!this.items[i].Equals(other.items[i])) {
                        return false;
                    }
                }

                return true;
            } else {
                return false;
            }
        }

        public override int GetHashCode() {
            if (this.dirty) {
                int hashCode = 13;
                int i = 1;

                foreach (JsonVar var in this.items) {
                    this.hashCode += i * 7 * var.GetHashCode();
                }

                this.hashCode = hashCode;
                this.dirty = false;
            }

            return this.hashCode;
        }
    }
}
