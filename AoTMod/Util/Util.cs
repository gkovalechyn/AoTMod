using System;
using System.Text;
using UnityEngine;

namespace GITS.Util {
    class Util {
        public static void drawLine(Vector3 position, Vector3 direction, Color color, float duration) {
            GameObject go = new GameObject();
            LineRenderer renderer = go.AddComponent<LineRenderer>();

            go.name = "Line";

            renderer.SetVertexCount(2);
            renderer.SetColors(color, color);
            renderer.SetWidth(0.1F, 0.1F);

            renderer.SetPosition(0, position);
            renderer.SetPosition(1, position + direction);

            GameObject.Destroy(go, duration);
        }

        public static bool isNumber(object value) {
            //https://stackoverflow.com/questions/1130698/checking-if-an-object-is-a-number-in-c-sharp
            //I prefer methods instead of extensions
            return value is sbyte
            || value is byte
            || value is short
            || value is ushort
            || value is int
            || value is uint
            || value is long
            || value is ulong
            || value is float
            || value is double
            || value is decimal;
        }

        public static string escapeString(string orig) {
            StringBuilder builder = new StringBuilder((int)(orig.Length * 1.2));
            foreach (char c in orig) {
                switch (c) {
                    case '\n':
                        builder.Append("\\n");
                        break;
                    case '\t':
                        builder.Append("\\t");
                        break;
                    case '\\':
                        builder.Append("\\\\");
                        break;
                    case '\r':
                        builder.Append("\\r");
                        break;
                    case '\"':
                        builder.Append("\\\"");
                        break;
                    case '\'':
                        builder.Append("\\\'");
                        break;
                    default:
                        builder.Append(c);
                        break;
                }
            }

            return builder.ToString();
        }

        public static string unescapeString(string orig) {
            if (orig.IndexOf('\\') > 0) {
                StringBuilder builder = new StringBuilder(orig.Length);
                int i = 0;
                char[] arr = orig.ToCharArray();

                while (i < arr.Length) {
                    if (arr[i] == '\\') {
                        i++;

                        if (i == arr.Length) { //There is no way it is going to be greater
                            return builder.ToString();
                        }

                        switch (arr[i]) {
                            case 'n':
                                builder.Append('\n');
                                break;
                            case 't':
                                builder.Append('\t');
                                break;
                            case '\\':
                                builder.Append('\\');
                                break;
                            case 'r':
                                builder.Append('\r');
                                break;
                            case '\"':
                                builder.Append('\"');
                                break;
                            case '\'':
                                builder.Append('\'');
                                break;
                            case 'u':
                                if (i < arr.Length - 4) {
                                    byte[] bytes = new byte[2];
                                    byte temp = (byte)(hexCharToByte(arr[i + 1]) << 4);

                                    bytes[0] = bytes[1] = 0;

                                    bytes[0] |= temp;

                                    temp = hexCharToByte(arr[i + 2]);
                                    bytes[0] |= temp;

                                    temp = (byte)(hexCharToByte(arr[i + 3]) << 4);
                                    bytes[1] |= temp;

                                    temp = hexCharToByte(arr[i + 4]);
                                    bytes[1] |= temp;

                                    builder.Append(Encoding.UTF8.GetString(bytes, 0, 2));
                                    i += 4;
                                } else {
                                    throw new InvalidOperationException("Invalid unicode character");
                                }
                                break;
                            default:
                                builder.Append('\\').Append(arr[i]);
                                break;
                        }
                    } else {
                        builder.Append(arr[i]);
                    }
                    i++;
                }

                return builder.ToString();
            } else {
                return orig;
            }
        }

        public static byte hexCharToByte(char c) {
            switch (c) {
                case '0':
                    return 0x0;
                case '1':
                    return 0x1;
                case '2':
                    return 0x2;
                case '3':
                    return 0x3;
                case '4':
                    return 0x4;
                case '5':
                    return 0x5;
                case '6':
                    return 0x6;
                case '7':
                    return 0x7;
                case '8':
                    return 0x8;
                case '9':
                    return 0x9;
                case 'a':
                case 'A':
                    return 0xA;
                case 'b':
                case 'B':
                    return 0xB;
                case 'c':
                case 'C':
                    return 0xC;
                case 'd':
                case 'D':
                    return 0xD;
                case 'e':
                case 'E':
                    return 0xE;
                case 'f':
                case 'F':
                    return 0xF;
                default:
                    throw new System.Exception("Invalid hexadecimal character");
            }
        }
    }
}
