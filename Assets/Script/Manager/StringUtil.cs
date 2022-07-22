using System;
using System.Collections.Generic;
using System.Text;
 
namespace StringUtil
{
    
    // HangulJaso에서 한글자소에 대한 정보를 담고 있다.
    
    public struct HANGUL_INFO
    {
        
        //  한글여부(H, NH).
        public string isHangul;
        
        // 분석 한글.
        public char originalChar;
        
        // 분리 된 한글(강 -> ㄱ,ㅏ,ㅇ).
        public char[] chars; 
    }
 
    // 한글 분석 클래스.
    public sealed class HangulJaso
    {
        
        // 초성 리스트.
        public static readonly string HTable_ChoSung = "ㄱㄲㄴㄷㄸㄹㅁㅂㅃㅅㅆㅇㅈㅉㅊㅋㅌㅍㅎ";
        // 중성 리스트.
        public static readonly string HTable_JungSung = "ㅏㅐㅑㅒㅓㅔㅕㅖㅗㅘㅙㅚㅛㅜㅝㅞㅟㅠㅡㅢㅣ";
        // 종성 리스트.
        public static readonly string HTable_JongSung = " ㄱㄲㄳㄴㄵㄶㄷㄹㄺㄻㄼㄽㄾㄿㅀㅁㅂㅄㅅㅆㅇㅈㅊㅋㅌㅍㅎ";
		
        private static readonly ushort m_UniCodeHangulBase = 0xAC00;
        private static readonly ushort m_UniCodeHangulLast = 0xD79F;
 
        // 생성자.
        public HangulJaso() { }
 
    /*
        
        // 초성, 충성, 종성으로 이루어진 한글을 한글자의 한글로 만든다.
        // string choSung = "ㄱ", jungSung = "ㅏ", jongSung = "ㅇ";
        // char hangul = MergeJaso(choSung, jungSung, jongSung);

        public static char MergeJaso(string choSung, string jungSung, string jongSung)
        {
            int ChoSungPos, JungSungPos, JongSungPos;
            int nUniCode;
 
            ChoSungPos = HTable_ChoSung.IndexOf(choSung);    // 초성 위치.
            JungSungPos = HTable_JungSung.IndexOf(jungSung);   // 중성 위치.
            JongSungPos = HTable_JongSung.IndexOf(jongSung);   // 종성 위치.
 
            // 앞서 만들어 낸 계산식.
            nUniCode = m_UniCodeHangulBase + (ChoSungPos * 21 + JungSungPos) * 28 + JongSungPos;
 
            // 코드값을 문자로 변환.
            char temp = Convert.ToChar(nUniCode);
 
            return temp;
        }
 */
        
        // 한글자의 한글을 초성, 중성, 종성으로 나눈다.
        // HANGUL_INFO hinfo = DevideJaso('강');
        // // hinfo.isHangul -> "H" (한글).
        // // hinfo.originalChar -> 강.
        // // hinfo.chars[0] -> ㄱ, hinfo.chars[1] -> ㄴ, hinfo.chars[2] = ㅇ.
        public static HANGUL_INFO DevideJaso(char hanChar)
        {
            int ChoSung, JungSung, JongSung;    // 초성,중성,종성의 인덱스.
            ushort temp = 0x0000;                // 임시로 코드값을 담을 변수.
            HANGUL_INFO hi = new HANGUL_INFO();
 
            //Char을 16비트 부호없는 정수형 형태로 변환 - Unicode.
            temp = Convert.ToUInt16(hanChar);
 
            // 캐릭터가 한글이 아닐 경우 처리.
            if ((temp < m_UniCodeHangulBase) || (temp > m_UniCodeHangulLast))
            {
                hi.isHangul = "NH";
                hi.originalChar = hanChar;
                hi.chars = null;
            }
            else
            {
                // nUniCode에 한글코드에 대한 유니코드 위치를 담고 이를 이용해 인덱스 계산.
                int nUniCode = temp - m_UniCodeHangulBase;
                ChoSung = nUniCode / (21 * 28);
                nUniCode = nUniCode % (21 * 28);
                JungSung = nUniCode / 28;
                nUniCode = nUniCode % 28;
                JongSung = nUniCode;
 
                hi.isHangul = "H";
                hi.originalChar = hanChar;
                hi.chars = new char[] { HTable_ChoSung[ChoSung], HTable_JungSung[JungSung], HTable_JongSung[JongSung] };
            }
 
            return hi;
        }
		
		public static bool IsFinalConsonant(char lastChar)
		{
			int /*ChoSung, JungSung,*/ JongSung;    // 초성,중성,종성의 인덱스.
            ushort temp = 0x0000;                // 임시로 코드값을 담을 변수.
 
            //Char을 16비트 부호없는 정수형 형태로 변환 - Unicode.
            temp = Convert.ToUInt16(lastChar);
 
            // 캐릭터가 한글이 아닐 경우 처리.
            if ((temp < m_UniCodeHangulBase) || (temp > m_UniCodeHangulLast))
            {
				return false;
            }
			
            // nUniCode에 한글코드에 대한 유니코드 위치를 담고 이를 이용해 인덱스 계산.
            int nUniCode = temp - m_UniCodeHangulBase;
            //ChoSung = nUniCode / (21 * 28);
            nUniCode = nUniCode % (21 * 28);
            //JungSung = nUniCode / 28;
            nUniCode = nUniCode % 28;
            JongSung = nUniCode;

            //char [] chars = new char[] { HTable_ChoSung[ChoSung], HTable_JungSung[JungSung], HTable_JongSung[JongSung] };
 
			if (JongSung > 0)
				return true;
			
            return false;
		}
    }
}