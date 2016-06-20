using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GITS.Util.Json {
    class Parser {
        private int line;
        private int column;
        private StreamReader stream;
        private StringBuilder builder;
        private Token token;

        private char[] buffer = new char[1];

        private char[] json;
        private int index;

        //I'm too lazy to do this properly so this will do
        private bool fromString = false;

        public JsonObject parse(string file) {
            this.stream = new StreamReader(file);
            this.builder = new StringBuilder(256);
            JsonObject obj;

            this.token = nextToken();

            try {
                obj = this.readObject();
            } catch (System.Exception e) {
                this.stream.Close();
                throw e;
            }

            this.stream.Close();
            return obj;
        }

        public JsonObject parseString(string json){
            this.fromString = true;

            this.builder = new StringBuilder(256);
            this.json = json.ToCharArray();
            this.index = 0;

            this.token = nextToken();

            return this.readObject();
        }

        private char nextChar() {
            return this.nextChar(false);
        }
        private char nextChar(bool countSpace) {
            do {
                if (this.fromString) {
                    this.buffer[0] = this.json[this.index];
                    this.index++;
                } else {
                    this.stream.Read(this.buffer, 0, 1);
                }
                
                switch (this.buffer[0]) {
                    case ' ':
                        this.column++;

                        if (countSpace) {
                            return ' ';
                        }
                        break;
                    case '\r':
                        break;
                    case '\n':
                        this.line++;
                        this.column = 0;
                        break;
                    case '\t':
                        this.column += 4;
                        break;
                    case '\f':
                        break;
                    default:
                        this.column++;
                        return this.buffer[0];
                }
            } while (true);
        }

        private JsonObject readObject() {
            JsonObject result = new JsonObject();

            if (this.token.type != TokenType.LEFT_BRACKET) {
                throw new JsonException("Unexpected token " + this.token + " at line " + this.line + " column " + this.column);
            }
            //... {
            // "bla" = "blah",
            while (true) {
                this.token = nextToken();
                string name;

                if (token.type == TokenType.RIGHT_BRACKET) {
                    //this.token = nextToken();
                    break;
                }

                if (token.type != TokenType.STRING) {
                    throw new JsonException("Unexpected token " + this.token + " at line " + this.line + " column " + this.column);
                }

                name = token.stringValue;

                this.token = checkGet(nextToken(), TokenType.COLON);

                switch (token.type) {
                    case TokenType.STRING:
                        result.set(name, token.stringValue);
                        this.token = nextToken();
                        break;
                    case TokenType.NUMBER:
                        result.set(name, this.token.numberValue);
                        this.token = nextToken();
                        break;
                    case TokenType.LEFT_BRACKET:
                        result.set(name, this.readObject());
                        this.token = this.checkGet(this.token, TokenType.RIGHT_BRACKET);
                        break;
                    case TokenType.LEFT_SQUARE_BRACKET:
                        result.set(name, this.readArray());
                        this.token = this.checkGet(this.token, TokenType.RIGHT_SQUARE_BRACKET);
                        break;
                    case TokenType.TRUE:
                    case TokenType.FALSE:
                        result.set(name, token.booleanValue);
                        this.token = nextToken();
                        break;
                    case TokenType.NULL:
                        result.set(name, (JsonVar) null);
                        this.token = nextToken();
                        break;
                    default:
                        throw new JsonException("Unexpected token " + this.token + " at line " + this.line + " column " + this.column);
                }

                if (this.token.type == TokenType.RIGHT_BRACKET) {
                    //this.token = nextToken();
                    break;
                } else if (token.type != TokenType.COMMA) {
                    throw new JsonException("Unexpected token " + this.token + " at line " + this.line + " column " + this.column);
                }
            }

            return result;
        }

        private Token checkGet(Token t, TokenType type) {
            if (t.type == type) {
                return nextToken();
            } else {
                throw new JsonException("Unexpected token " + t + " at line " + this.line + " column " + this.column + " Expected " + type);
            }
        }

        private JsonArray readArray() {
            JsonArray array = new JsonArray("Temp");
            this.token = this.checkGet(this.token, TokenType.LEFT_SQUARE_BRACKET);

            while (true) {
                switch (token.type) {
                    case TokenType.STRING:
                        array.add(token.stringValue);
                        token = nextToken();
                        break;
                    case TokenType.NUMBER:
                        array.add(token.numberValue);
                        token = nextToken();
                        break;
                    case TokenType.LEFT_BRACKET:
                        array.add(this.readObject());
                        this.token = this.checkGet(this.token, TokenType.RIGHT_BRACKET);
                        break;
                    case TokenType.LEFT_SQUARE_BRACKET:
                        array.add(this.readArray());
                        this.token = this.checkGet(this.token, TokenType.RIGHT_SQUARE_BRACKET);
                        break;
                    case TokenType.RIGHT_SQUARE_BRACKET:
                        //this.token = nextToken();
                        return array;
                    case TokenType.TRUE:
                    case TokenType.FALSE:
                        array.add(token.booleanValue);
                        token = nextToken();
                        break;
                    case TokenType.NULL:
                        array.add((JsonVar) null);
                        token = nextToken();
                        break;
                    default:
                        throw new JsonException("Unexpected token " + this.token + " at line " + this.line + " column " + this.column);
                }

                if (this.token.type == TokenType.COMMA) {
                    this.token = nextToken();
                } else if (this.token.type == TokenType.RIGHT_SQUARE_BRACKET) {
                    //this.token = nextToken();
                    return array;
                } else {
                    throw new JsonException("Unexpected token " + this.token + " at line " + this.line + " column " + this.column);
                }
            }
        }

        private Token nextToken() {
            Token token = new Token();
            char c;

            builder.Length = 0;

            c = nextChar();

            switch (c) {
                case '\"':
                    token.type = TokenType.STRING;

                    while (true) {
                        c = nextChar(true);

                        if (c == '\\') {
                            switch ((c = nextChar())) {
                                case '"':
                                case '\'':
                                case '/':
                                    builder.Append(c);
                                    break;
                                case '\f':
                                case '\b':
                                case '\r':
                                    break;
                                case 'n':
                                    builder.Append('\n');
                                    break;
                                case 't':
                                    builder.Append('\t');
                                    break;
                                case 'u':
                                    byte[] bytes = new byte[2];
                                    byte temp = (byte)(Util.hexCharToByte(nextChar()) << 4);

                                    bytes[0] = bytes[1] = 0;

                                    bytes[0] |= temp;

                                    temp = Util.hexCharToByte(nextChar());
                                    bytes[0] |= temp;

                                    temp = (byte)(Util.hexCharToByte(nextChar()) << 4);
                                    bytes[1] |= temp;

                                    temp = Util.hexCharToByte(nextChar());
                                    bytes[1] |= temp;

                                    builder.Append(Encoding.UTF8.GetString(bytes, 0, 2));
                                    break;
                                default:
                                    builder.Append('\\');
                                    builder.Append(c);
                                    break;
                            }
                        } else if (c == '\"') {
                            token.stringValue = builder.ToString();
                            break;
                        } else {
                            builder.Append(c);
                        }
                    }

                    token.stringValue = builder.ToString();
                    break;
                case '{':
                    token.type = TokenType.LEFT_BRACKET;
                    break;
                case '}':
                    token.type = TokenType.RIGHT_BRACKET;
                    break;
                case '[':
                    token.type = TokenType.LEFT_SQUARE_BRACKET;
                    break;
                case ']':
                    token.type = TokenType.RIGHT_SQUARE_BRACKET;
                    break;
                case ',':
                    token.type = TokenType.COMMA;
                    break;
                case ':':
                    token.type = TokenType.COLON;
                    break;
                case 't':
                    if ((c = this.nextChar()) != 'r') {
                        throw new JsonException("Unexpected character " + c + " at line " + this.line + " column " + this.column);
                    }

                    if ((c = this.nextChar()) != 'u') {
                        throw new JsonException("Unexpected character " + c + " at line " + this.line + " column " + this.column);
                    }

                    if ((c = this.nextChar()) != 'e') {
                        throw new JsonException("Unexpected character " + c + " at line " + this.line + " column " + this.column);
                    }

                    token.type = TokenType.TRUE;
                    token.booleanValue = true;

                    return token;
                case 'f':
                    if ((c = this.nextChar()) != 'a') {
                        throw new JsonException("Unexpected character " + c + " at line " + this.line + " column " + this.column);
                    }

                    if ((c = this.nextChar()) != 'l') {
                        throw new JsonException("Unexpected character " + c + " at line " + this.line + " column " + this.column);
                    }

                    if ((c = this.nextChar()) != 's') {
                        throw new JsonException("Unexpected character " + c + " at line " + this.line + " column " + this.column);
                    }

                    if ((c = this.nextChar()) != 'e') {
                        throw new JsonException("Unexpected character " + c + " at line " + this.line + " column " + this.column);
                    }

                    token.type = TokenType.FALSE;
                    token.booleanValue = false;
                    break;
                    //return token;
                case 'n':
                    token.type = TokenType.NULL;
                    if ((c = this.nextChar()) != 'u') {
                        throw new JsonException("Unexpected character " + c + " at line " + this.line + " column " + this.column);
                    }

                    if ((c = this.nextChar()) != 'l') {
                        throw new JsonException("Unexpected character " + c + " at line " + this.line + " column " + this.column);
                    }

                    if ((c = this.nextChar()) != 'l') {
                        throw new JsonException("Unexpected character " + c + " at line " + this.line + " column " + this.column);
                    }
                    break;
                    //return token;
                default:
                    if (Char.IsNumber(c) || c == '-' || c == '+' || c == '.') {
                        char peek;

                        if (this.fromString) {
                            peek = this.json[this.index + 1];
                        } else {
                            peek  = (char) stream.Peek();
                        }

                        token.type = TokenType.NUMBER;
                        builder.Append(c);

                        while (Char.IsNumber(peek) || peek == '.') {
                            builder.Append(nextChar());
							
                            if (this.fromString) {
								peek = this.json[this.index + 1];
							} else {
								peek  = (char) stream.Peek();
							}
                        }

                        token.numberValue = double.Parse(builder.ToString());
                        break;
                    } else {
                        throw new JsonException("Unexpected character " + c + " at line " + this.line + " column " + this.column);
                    }
            }
            return token;
        }
    }
}
