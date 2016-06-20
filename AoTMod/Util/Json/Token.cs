using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GITS.Util.Json {
    struct Token {
        public TokenType type;
        public string stringValue;
        public bool booleanValue;
        public double numberValue;

        public override string ToString() {
            return "GITS.Util.Token{type=" + type + ", stringValue=" + this.stringValue + ", numberValue=" + this.numberValue + "}";
        }
    }
}
