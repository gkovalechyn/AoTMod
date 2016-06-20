using UnityEngine;
using System.Collections;

public class Colorizer{
	public sealed class Color{
		public static Color WHITE = new Color("FFFFFF");
		public static Color BLACK = new Color("000000");

		public static Color GREY = new Color("C0C0C0");
		public static Color GRAY = new Color("C0C0C0");

		public static Color DARK_GREY = new Color("808080");
		public static Color DARK_GRAY = new Color("808080");
		
		public static Color RED = new Color("FF0000");
		public static Color GREEN = new Color("00FF00");
		public static Color BLUE = new Color("0000FF");

		public static Color YELLOW = new Color("FFFF00");
		public static Color DARK_GREEN = new Color("008000");
		public static Color PURPLE = new Color("800080");

		private readonly string color;
		public Color(string color){
			this.color = color;
		}

		public override string ToString (){
			return this.color;
		}
	}

	public static string colorize(string src, Color color){
		return "[" + color + "]" + src  + "[-]";
	}

	public static string colorize(string src, Color color, bool isChat){
		if (!isChat){
			return Colorizer.colorize(src, color);
		}else{
			return "<color=#" + color + ">" + src  + "</color>";
		}
	}

	public static string colorize(string src, string rgb, bool isChat){
		if (isChat){
			return "<color=#" + rgb + ">" + src  + "</color>";
		}else{
			return "[" + rgb + "]" + src  + "[-]";
		}
	}

	public static void colorize(string[] src, Color[] colors, bool isChat){
		int j = 0;
		int i = 0;

		while(j < src.Length){
			while(i < colors.Length){
				src[j] = Colorizer.colorize(src[j], colors[i], isChat);
				j++;
				i++;

				if (j >= src.Length){
					break;
				}
			}
			i = 0;
		}

	}

	public static void colorize(string[] src, string[] rgbs, bool isChat){
		int j = 0;
		int i = 0;
		
		while(j < src.Length){
			while(i < rgbs.Length){
				src[j] = Colorizer.colorize(src[j], rgbs[j], isChat);
				j++;
				i++;

				if (j >= src.Length){
					break;
				}
			}
			i = 0;
		}
	}
}

