using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GITS.Util.Json {
    public class JsonException : System.Exception{
        public JsonException(string message)
            : base(message) {

        }
    }
}
