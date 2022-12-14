using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace VNS.Utils
{
    public class NumberConvert
    {
        static string UpperFirstCharacter(string sentence)
        {
            if (sentence.Length > 0)
                return sentence[0].ToString().ToUpper() + sentence.Remove(0, 1);
            else
                return sentence;
        }
        /// <summary>
        /// Converts number to sentence
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string NumberToSentence(decimal number)
        {
            return UpperFirstCharacter(NumberConvert.NumberToSentence(number, false, ""));
        }
        /// <summary>
        /// Converts number to sentence
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string NumberToSentence(decimal number, bool english)
        {
            return NumberConvert.NumberToSentence(number, english, "");
        }
        /// <summary>
        /// Converts number to sentence
        /// </summary>
        /// <param name="number"></param>
        /// <param name="english"></param>
        /// <returns></returns>
        public static string NumberToSentence(decimal number, bool english, string suffix)
        {
            //tri - add negative prefix
            string negative = "";
            if (number < 0)
            {
                negative = english ? "Negative " : "Âm ";
                number = -number;
            }

            //tri //////////////
            string suffix1 = "";

            suffix1 = english ? "per cent" : "phần trăm";
            suffix = suffix.Trim();
            if (english)
                if (suffix.Equals("USD", StringComparison.InvariantCultureIgnoreCase) || suffix.Equals("VND", StringComparison.InvariantCultureIgnoreCase))
                    suffix1 = "cents";
                else
                    if (suffix == "USD")
                        suffix = "USD";

            suffix1 = suffix.Equals("USD", StringComparison.InvariantCultureIgnoreCase) ? "cents" : "xu";

            if (suffix == "VND")
                suffix = "đồng";
            if (suffix == "USD" && !english)
                suffix = "đô la Mỹ";



            NumberText Dic = english ? NumberText.English : NumberText.VietNamese;

            string tmp = number.ToString("#0.#0");

            int lnAt = tmp.IndexOf(".");

            string _ChLe = (100*((double)number % 1.0)).ToString("00");// RIGHT(allt(str(number%1,3,2)),2); //&&Iif(AT(".",tmp)#0,Substr(tmp,lnAt+1,2),"00")
            string _ChChan = (Math.Floor(number)).ToString("#").Trim(); //&&Left(tmp,Iif(lnAt#0,lnAt-1,Len(tmp)))

            int socap3 = (int)(_ChChan.Length / 3);
            int numberdu = _ChChan.Length % 3;

            string chuoi = "";

            if (numberdu > 0)
            {
                chuoi = Break(_ChChan.Substring(0, numberdu), Dic, false, true);
                //set3 = (int.Parse(_ChChan.Substring(0, numberdu)) > 1);
                //set1 = (int.Parse(_ChChan.Substring(0, numberdu)) < 100 && int.Parse(_ChChan.Substring(0, numberdu)) > 0);
            }
            chuoi = chuoi + (numberdu != 0 ? " " + Dic.donvi[(socap3 > 3 ? (socap3 % 3 == 0 ? 2 : socap3 % 3) : socap3)] + " " : " ");
            for (int i = socap3; i >= 1; i--)
            {
                bool _Last = (i == 1 && _ChLe == "00");
                bool _First = (i == socap3);
                int num = int.Parse(_ChChan.Substring((socap3 - i) * 3 + numberdu, 3));
                if (num > 0)
                {
                    string Ch_tach = Break(_ChChan.Substring((socap3 - i) * 3 + numberdu, 3), Dic, _Last, false).Trim();
                    chuoi = chuoi.Trim() + (num < 100 && num > 0 ? " " + Dic.chu[0] + " " + Dic.tram : "") + " " + Ch_tach + " " + Dic.donvi[(i > 3 ? (i % 3 == 0 ? 3 : i % 3 - 1) : i - 1)] + (i % 3 == 1 && i > 3 ? Dic.donvi[3] : "");
                }
            }

            var _Chuoile = string.Empty;
            if (_ChLe != "00")
            {
                _Chuoile = Break(_ChLe, Dic, true, false);
                if (_Chuoile != "")
                    _Chuoile = english ? "and " : ", " + _Chuoile.Trim() + " " + suffix1;
            }
            //chuoi = Upper(Left(Allt(chuoi),1))+Lower(Subs(Allt(chuoi),2));

            chuoi = chuoi.Trim() + " " + suffix + (!string.IsNullOrEmpty(_Chuoile) ? (" " + _Chuoile) : "");//&&+ Iif(_ChLe#"00",Iif(VietNamese,"vaø ","and ")+_Chuoile,"")
            return UpperFirstCharacter(negative + chuoi.Trim());
        }

        class NumberText
        {
            public NumberText()
            {
            }
            public NumberText(string[] _chu, string[] _chuc, string[] _donvi, string _tram)
            {
                chu = _chu;
                chuc = _chuc;
                donvi = _donvi;
                tram = _tram;
            }
            public NumberText(string[] _chu, string[] _chuc, string[] _donvi, string _tram, string _le)
            {
                chu = _chu;
                chuc = _chuc;
                donvi = _donvi;
                tram = _tram;
                le = _le;
            }
            public string[] chu;
            public string[] chuc;
            public string[] donvi;
            public string tram = "";
            public string le = "";


            public static NumberText VietNamese = new NumberText(new string[] { "không","một",  "hai", "ba", "bốn", "năm", "sáu","bảy","tám","chín",
	                                "mười", "muời một", "mười hai","mười ba","mười bốn","mười lăm","muời sáu", "mười bảy",
                                    "mười tám","mười chín","mốt","lăm"},
                                    new string[] { "hai mươi", "ba mươi", "bốn mươi", "năm mươi", "sáu mươi", "bảy mươi", "tám mươi", "chín mươi" } ,
                                   new string[] { "", "ngàn", "triệu", "tỷ", "ngàn ", "triệu ", "tỷ " } , "trăm", "lẻ");
            public static NumberText English = new NumberText(new string[] { " ","one",  "two", "three", "four", "five", "six","seven","eight","nine",
	                                "ten", "eleven", "twelve","thirteen","fourteen","fifteen","sixteen", "seventeen",
                                    "eightteen","nineteen","one","five"},
                                new string[] { "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" } ,
                                new string[] { "", "thousand", "million", "billion", "thousand ", "million ", "billion " } , " hundred");

        }

        static string Break(string text, NumberText Dic)
        {
            return NumberConvert.Break(text, Dic, false, false);
        }

        static string Break(string text, NumberText Dic, bool last, bool first)
        {

            string tmp = "";

            if (text.Trim().Length != 0)
            {
                text = text.PadLeft(3, '0');
                int dv = int.Parse(text.Substring(2, 1));
                int ch = int.Parse(text.Substring(1, 1));
                int tr = int.Parse(text.Substring(0, 1));

                //tmp = (tr == 0) ? "" : (" " + Dic.chu[tr] + " " + Dic.tram + " " + ((ch == 0 && dv == 0) ? "" : (last ? "and " : "")));
                tmp = (tr == 0) ? "" : (" " + Dic.chu[tr] + " " + Dic.tram + " " );
                if (ch != 1)
                {
                    if (ch != 0)
                    {
                        tmp += Dic.chuc[ch - 2];
                        if (dv != 0)
                            tmp += " " + (dv == 1 ? Dic.chu[20] : (dv == 5 ? Dic.chu[21] : Dic.chu[dv]));

                    }
                    else
                    {
                        if (dv != 0 && !first)
                            tmp += Dic.le;
                        else
                            tmp += " ";

                        if (dv != 0)
                            tmp += " " + Dic.chu[dv];

                    }

                }
                else
                    tmp += Dic.chu[ch * 10 + dv];
            }
            return tmp;
        }


        public static string RemoveDiacritics(string text)
        {
            
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return Regex.Replace(stringBuilder.ToString().Normalize(NormalizationForm.FormC), "đ|Đ", "d");
        }
    }
}
