using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GITS.Util.Json {
    public class JsonNumber : JsonVar{
        private double value;

        public JsonNumber(string name): base(name) {

        }
        public void setValue(float val) {
            this.value = val;
        }

        public void setValue(double d) {
            this.value = d;
        }

        public void setValue(int i) {
            this.value = i;
        }

        public float getFloatValue() {
            return (float) this.value;
        }

        public double getDoubleValue() {
            return this.value;
        }

        public int getIntValue() {
            return (int)this.value; //Don't round it
        }

        public override void writeToStream(System.IO.StreamWriter stream) {
            stream.Write(this.value.ToString());
        }

        public override string ToString() {
            return this.value.ToString();
        }

        public override bool Equals(object obj) {
            if (obj != null && obj is JsonNumber) {
                JsonNumber other = (JsonNumber)obj;
                return double.Equals(this.value, other.value);
            }

            return false;
        }

        public override int GetHashCode() {
            unchecked{
                int hash = 31;
                ulong l = (ulong) BitConverter.DoubleToInt64Bits(this.value);
                hash *= (int) (l ^ (l >> 32));

                return hash;
            }
        }
    }
}
