using UnityEngine;
using System.Collections;

public enum Size{
	SMALL,
	MEDIUM,
	LARGE
}

public class TitanSize{

    public static Size getBySize(float f){
        //since all the scale values are the same we can use just one of them
		//and the titan scale values are from 1 to 3 so
		//small = 1.666-
		//medium = 1.666 - 2.333
		//large 2.3+
        if (f <= 1.666){
			return Size.SMALL;
		}else if (f > 1.666 && f < 2.333){
			return Size.MEDIUM;
		}else {
			return Size.LARGE;
		}
    }
	public static Size getByScale(Vector3 scale){
        return TitanSize.getBySize(scale.x);
		
	}

    public static AbnormalType getTitanType(int i) {
        switch (i) {
            case (int)AbnormalType.NORMAL:
                return AbnormalType.NORMAL;
            case (int)AbnormalType.TYPE_CRAWLER:
                return AbnormalType.TYPE_CRAWLER;
            case (int)AbnormalType.TYPE_I:
                return AbnormalType.TYPE_I;
            case (int)AbnormalType.TYPE_JUMPER:
                return AbnormalType.TYPE_JUMPER;
            case (int)AbnormalType.TYPE_PUNK:
                return AbnormalType.TYPE_PUNK;
            default:
                return AbnormalType.NORMAL;
        }
    }

	public static Size getByChar(char c){
		switch(c){
			case 's':
			case'S':
				return Size.SMALL;
			case 'm':
			case 'M':
				return Size.MEDIUM;
			case 'l':
			case 'L':
				return Size.LARGE;
			default:
				return Size.MEDIUM;
		}
	}

	public static AbnormalType getTitanType(char c){
		switch(c){
			case'N':
			case 'n':
				return AbnormalType.NORMAL;
			case'A':
			case 'a':
				return AbnormalType.TYPE_I;
			case 'j':
			case'J':
				return AbnormalType.TYPE_JUMPER;
			case 'c':
			case'C':
				return AbnormalType.TYPE_CRAWLER;
			case 'p':
			case 'P':
				return AbnormalType.TYPE_PUNK;
			default:
				return AbnormalType.NORMAL;
		}
	}

	public static char getRepresentativeChar(AbnormalType type){
		switch(type){
			case AbnormalType.NORMAL:
				return 'n';
			case AbnormalType.TYPE_CRAWLER:
				return 'c';
			case AbnormalType.TYPE_I:
				return 'a';
			case AbnormalType.TYPE_JUMPER:
				return 'j';
			case AbnormalType.TYPE_PUNK:
				return 'p';
			default:
				return 'n';
		}
	}

	public static char getRepresentativeChar(Size size){
		switch(size){
			case Size.SMALL:
				return 's';
			case Size.MEDIUM:
				return 'm';
			case Size.LARGE:
				return 'l';
			default:
				return 'l';
		}
	}
}

