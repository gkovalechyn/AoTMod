using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GITS.Util.Json {
    public class JsonObject : JsonVar{
        private Dictionary<string, JsonVar> children = new Dictionary<string, JsonVar>();
        private bool dirty = true;
        private int hashCode;
        public JsonObject(string name): base(name) {
        }

        public JsonObject() : base(null){ 
        }

        public void set(string name, JsonVar var) {
            var.setName(name);
            this.children[name] = var;
            this.dirty = true;
        }
        public JsonVar get(string name) {
            if (name.Contains('.')) {
                string[] temp = name.Split('.');
                if (!this.children.ContainsKey(temp[0])) {
                    return null;
                } else if (!(this.children[temp[0]] is JsonObject)){
                    throw new JsonException("Key " + temp[0] + " is not an object");
                } else {
                    JsonObject obj = (JsonObject) this.children[temp[0]];
                    return obj.get(string.Join(".", temp, 1, temp.Length - 1));
                }
            } else {
                if (this.children.ContainsKey(name)) {
                    return children[name];
                } else {
                    return null;
                }
            }
        }

        public bool hasChildren() {
            return this.children.Count > 0;
        }

        public List<string> getChildren() {
            return new List<string>(this.children.Keys);
        }

        public void set(string name, int val) {
            JsonNumber number = new JsonNumber(name);
            number.setValue(val);
            this.children[name] = number;
            this.dirty = true;
        }

        public int getInt(string name) {
            JsonVar var = this.get(name);

            if (var != null) {
                if (var is JsonNumber) {
                    return ((JsonNumber)var).getIntValue();
                } else {
                    throw new System.InvalidOperationException("Key " + name + " is not a number");
                }
            } else {
                throw new System.InvalidOperationException("No such key: " + name);
            }
        }

        public JsonArray getArray(string name) {
            JsonVar var = this.get(name);

            if (var != null) {
                if (var is JsonArray) {
                    return (JsonArray)var;
                } else {
                    throw new System.InvalidOperationException("Key " + name + " is not an array");
                }
            } else {
                throw new System.InvalidOperationException("No such key: " + name);
            }
        }

        public void set(string name, double val) {
            JsonNumber number = new JsonNumber(name);
            number.setValue(val);
            this.children[name] = number;
            this.dirty = true;
        }

        public double getDouble(string name) {
            JsonVar var = this.get(name);

            if (var != null) {
                if (var is JsonNumber) {
                    return ((JsonNumber)var).getDoubleValue();
                } else {
                    throw new System.InvalidOperationException("Key " + name + " is not a number");
                }
            } else {
                throw new System.InvalidOperationException("No such key: " + name);
            }
        }

        public double getFloat(string name) {
            return (float) this.getDouble(name);
        }

        public void set(string name, string value) {
            JsonString number = new JsonString(name);
            number.setValue(value);
            this.children[name] = number;
            this.dirty = true;
        }

        public string getString(string name) {
            JsonVar var = this.get(name);

            if (var != null) {
                if (var is JsonString) {
                    return ((JsonString)var).getValue();
                } else if ((var is JsonObject) || (var is JsonArray)){
                    throw new System.InvalidOperationException("Key " + name + " is not convertible to string.");
                } else {
                    if (var is JsonNumber) {
                        return Convert.ToString(((JsonNumber)var).getDoubleValue());
                    } else if (var is JsonBool) {
                        return Convert.ToString(((JsonBool)var).getValue());
                    } else {
                        throw new System.Exception();
                    }
                }
            } else {
                throw new System.InvalidOperationException("No such key: " + name);
            }
        }

        public void set(string name, JsonObject obj) {
            if (obj != null) {
                obj.setName(name);
                this.children[name] = obj;
                this.dirty = true;
            }
        }

        public JsonObject getObject(string name) {
            JsonVar var = this.get(name);

            if (var != null) {
                if (var is JsonObject) {
                    return (JsonObject) var;
                } else {
                    throw new System.InvalidOperationException("Key " + name + " is not an object");
                }
            } else {
                throw new System.InvalidOperationException("No such key: " + name);
            }
        }

        public void set(string name, bool b) {
            JsonBool var = new JsonBool(name);

            var.setValue(b);
            this.children[name] = var;
            this.dirty = true;
        }

        public bool getBool(string name) {
            JsonVar var = this.get(name);

            if (var != null) {
                if (var is JsonBool) {
                    return ((JsonBool)var).getValue();
                } else {
                    throw new System.InvalidOperationException("Key " + name + " is not a bool");
                }
            } else {
                throw new System.InvalidOperationException("No such key: " + name);
            }
        }

        public void remove(string name) {
            this.children.Remove(name);
            this.dirty = true;
        }

        public bool contains(string name) {
            return this.children.ContainsKey(name);
        }

        public void save(string filename) {
            StreamWriter writer = new StreamWriter(filename);
            try {
                this.writeToStream(writer);
                writer.Close();
            } catch (System.Exception e) {
                writer.Close();
                throw e;
            }
        }

        public static JsonObject fromFile(string filename) {
            if (File.Exists(filename)) {
               return new Parser().parse(filename);
            } else {
                throw new FileNotFoundException("File does not exist", filename);
            }
        }

        public static JsonObject fromString(string json) {
            return new Parser().parseString(json);
        }


        public override void writeToStream(System.IO.StreamWriter stream) {
            List<string> keys = this.getChildren();

            stream.Write('{');

            for (int i = 0; i < keys.Count - 1; i++) {
                JsonVar var = this.children[keys[i]];

                stream.Write('\"');
                stream.Write(var.getName());
                stream.Write('\"');

                stream.Write(" : ");

                var.writeToStream(stream);

                stream.Write(", ");
            }

            if (keys.Count > 0) {
                JsonVar var = this.children[keys[keys.Count - 1]];

                stream.Write('\"');
                stream.Write(var.getName());
                stream.Write('\"');

                stream.Write(" : ");

                var.writeToStream(stream);
            }
            
            stream.Write('}');
        }

        public override string ToString() {
            StringBuilder builder = new StringBuilder(128);
            List<string> keys = this.getChildren();

            builder.Append('{');

            for (int i = 0; i < keys.Count - 1; i++) {
                JsonVar var = this.children[keys[i]];

                builder.Append('\"');
                builder.Append(var.getName());
                builder.Append('\"');

                builder.Append(" : ");

                builder.Append(var.ToString());

                builder.Append(", ");
            }

            if (keys.Count > 0) {
                JsonVar var = this.children[keys[keys.Count - 1]];

                builder.Append('\"');
                builder.Append(var.getName());
                builder.Append('\"');

                builder.Append(" : ");

                builder.Append(var.ToString());
            }

            builder.Append('}');
            return builder.ToString();
        }

        public override bool Equals(object o){
            if (o != null && (o is JsonObject)) {
                JsonObject other = (JsonObject)o;

                if (this.children.Count != other.children.Count) {
                    return false;
                }

                foreach (KeyValuePair<string, JsonVar> pair in this.children) {
                    if (!pair.Value.Equals(other.children[pair.Key])) {
                        return false;
                    }
                }

                return true;
            } else {
                return false;
            }
        }

        public override int GetHashCode() {
            if (this.dirty){
                int hash = 89 * 13;

                foreach (KeyValuePair<string, JsonVar> pair in this.children) {
                    hash += 71 * (pair.Key.GetHashCode() ^ pair.Value.GetHashCode());
                }

                this.hashCode = hash;
                this.dirty = false;
            }

            return this.hashCode;
        }
    }
}
