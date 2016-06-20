using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GITS.Util.Json {
    enum TokenType {
        LEFT_BRACKET,
        RIGHT_BRACKET,
        LEFT_SQUARE_BRACKET,
        RIGHT_SQUARE_BRACKET,
        STRING,
        NUMBER,
        COMMA,
        COLON,
        TRUE, //True keyword
        FALSE, //False keyword
        NULL  //null keyword
    }
}
