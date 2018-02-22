using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Wendy
{
    public class WinCCConvertor
    {
        static bool ValidateWithBracket(string origin)
        {
            string brackets = origin;

            while (brackets.Contains('('))
            {
                var s = brackets.IndexOf('(');
                var e = brackets.IndexOf(')');

                if (s >= e)
                    return false;

                var inner = brackets.Substring(s + 1, e - 1 - s);

                if (!ValidateWithBracket(inner))
                    return false;

                brackets = brackets.Substring(0, s) + " " + brackets.Substring(e + 1);
            }

            var parts = brackets.Split(new char[] { ' ' });

            foreach (var exp in parts)
            {
                var p = exp.Trim(new char[] { '\'' });

                //not operators -> must be tag
                if (!(p.Equals("or", StringComparison.InvariantCultureIgnoreCase) || p.Equals("and", StringComparison.InvariantCultureIgnoreCase)
                    || p.Equals("+", StringComparison.InvariantCultureIgnoreCase) || p.Equals("-", StringComparison.InvariantCultureIgnoreCase)
                    || p.Equals("&&", StringComparison.InvariantCultureIgnoreCase) || p.Equals("||", StringComparison.InvariantCultureIgnoreCase)
                    || p.Length == 0))
                {
                    int pos = p.IndexOf("___");
                    if (pos > 0)
                    {
                        var pre = p.Substring(0, pos);
                        var post = p.Substring(pos + 3);

                        if (!post.Equals("pr_stoer_offen"))
                        {
                            var pp = post.Split(new char[] { '_' });

                            if (pp.Length < 2)
                                return false;

                            int nr;
                            if (!int.TryParse(pp[pp.Length - 1], out nr))
                                return false;
                            try
                            {
                                postfix(post);
                            }
                            catch
                            {
                                return false;
                            }
                        }


                        var prep = pre.Split(new char[] { '_' });

                        if (prep.Length < 4)
                            return false;
                    }
                    else if (p.IndexOf("__") > 0)
                    {
                        return false;
                    }
                    //points without "___" can be valid to and are translated without change                        
                }
            }

            return true;
        }

        public static bool Validate(string origin)
        {
            if (origin.Length == 0)
                return true;

            origin = origin.Replace("'||'", "' || '").Replace("'||", "' ||").Replace("||'", "|| '");
            origin = origin.Replace("'&&'", "' && '").Replace("'&&", "' &&").Replace("&&'", "&& '");

            return ValidateWithBracket(origin);
        }

        static string TranslateWithBracket(string origin)
        {
            StringBuilder result = new StringBuilder();
            string brackets = origin;

            if (brackets.Contains('('))
            {
                var s = brackets.IndexOf('(');
                var e = brackets.IndexOf(')');

                if (s >= e)
                    throw new Exception("Brackets mismatch");

                var inner = brackets.Substring(s + 1, e - 1 - s);

                inner = TranslateWithBracket(inner);
                return TranslateWithBracket(brackets.Substring(0, s)) + " (" + inner + ") " + TranslateWithBracket(brackets.Substring(e+1));
            }

            var parts = brackets.Split(new char[] { ' ' });

            foreach (var exp in parts)
            {
                if (exp.Length == 0)
                    continue;
                var p = exp.Trim(new char[] { '\'' });
                if (result.Length > 0)
                    result.Append(" ");

                if (p.Equals("or", StringComparison.InvariantCultureIgnoreCase) || p.Equals("and", StringComparison.InvariantCultureIgnoreCase)
                    || p.Equals("+", StringComparison.InvariantCultureIgnoreCase) || p.Equals("-", StringComparison.InvariantCultureIgnoreCase))
                    result.Append(p);
                else if (p.Equals("&&", StringComparison.InvariantCultureIgnoreCase))
                    result.Append("and");
                else if (p.Equals("||", StringComparison.InvariantCultureIgnoreCase))
                    result.Append("or");
                else
                    result.Append(convertpointname(p));
            }

            return result.ToString();
        }

        public static string Translate(string origin)
        {
            if (origin.Length == 0)
                return "";

            origin = origin.Replace("'||'", "' || '").Replace("'||", "' ||").Replace("||'", "|| '");
            origin = origin.Replace("'&&'", "' && '").Replace("'&&", "' &&").Replace("&&'", "&& '");

            StringBuilder result = new StringBuilder();

            return Regex.Replace(TranslateWithBracket(origin).Trim(), @"\s+", " ");

            /*if (origin.Contains("'"))
            {
                        int a = 0;
            int s = 0;
            int pos;

                while (a < origin.Length)
                {
                    if ((pos = origin.IndexOf("'", a)) < 0)
                    {
                        //add rest
                        result.Append(origin.Substring(a));
                    }
                    else
                    {
                        //append text
                        if (pos > a)                        
                            result.Append(origin.Substring(a,pos-a));


                        s = pos + 1;

                        if ((pos = origin.IndexOf("'", s)) < 0)
                            throw new Exception("Nejde najit tagy podle uvozovek!!!");

                        var tag = origin.Substring(s, pos - s);
                        result.Append(convertpointname(tag));
                        a = pos + 1;
                    }
                }
            }
            else*/
            {
                var parts = origin.Split(new char[] { ' ' });

                foreach (var exp in parts)
                {
                    if (exp.Length == 0)
                        continue;
                    var p = exp.Trim(new char[] { '\'' });
                    if (result.Length > 0)
                        result.Append(" ");

                    if (p.Equals("or", StringComparison.InvariantCultureIgnoreCase) || p.Equals("and", StringComparison.InvariantCultureIgnoreCase)
                        || p.Equals("+", StringComparison.InvariantCultureIgnoreCase) || p.Equals("-", StringComparison.InvariantCultureIgnoreCase))
                        result.Append(p);
                    else if (p.Equals("&&", StringComparison.InvariantCultureIgnoreCase))
                        result.Append("and");
                    else if (p.Equals("||", StringComparison.InvariantCultureIgnoreCase))
                        result.Append("or");
                    else
                        result.Append(convertpointname(p));
                }
            }

            return result.ToString();
        }


        static string postfix(string origin)
        {
            const string bm_bm_bit = "bm_bm_bit_";
            const string bs_soll = "bs_soll_";
            const string bs_ist = "bs_ist_";
            const string di_obau = "di_obau_";
            const string di_obin = "di_obin_";
            const string di_istwert = "di_istwert_";
            const string di_sollwert = "di_sollwert_";
            const string di_unin = "di_unin_";
            const string di_unau = "di_unau_";
            const string sz_sz_ist = "sz_sz_ist_";
            const string sz_sz_soll = "sz_sz_soll_";
            const string za_istwert = "za_istwert_";
            const string za_sollwert = "za_sollwert_";
            const string ac_ascii = "ac_ascii";
            const string pr_stoer_offen = "pr_stoer_offen";
            const string pr_still_prio = "pr_still_prio_";

            if (origin.StartsWith(bm_bm_bit))
            {
                var value = int.Parse(origin.Substring(bm_bm_bit.Length));

                if (value == 1)
                    return "ERR30";
                else if (value == 2)
                    return "ERR31";
                else
                    return $"BM.BM.{string.Format("{0:000}", value)}";
            }
            else if (origin.StartsWith(bs_soll))
            {
                var value = int.Parse(origin.Substring(bs_soll.Length));
                return $"BS.SW.{string.Format("{0:000}", value)}";
            }
            else if (origin.StartsWith(bs_ist))
            {
                var value = int.Parse(origin.Substring(bs_ist.Length));
                return $"BS.IW.{string.Format("{0:000}", value)}";
            }
            else if (origin.StartsWith(di_obau))
            {
                var value = int.Parse(origin.Substring(di_obau.Length));
                return $"DI.OA.{string.Format("{0:000}", value)}";
            }
            else if (origin.StartsWith(di_obin))
            {
                var value = int.Parse(origin.Substring(di_obin.Length));
                return $"DI.OI.{string.Format("{0:000}", value)}";
            }
            else if (origin.StartsWith(di_istwert))
            {
                var value = int.Parse(origin.Substring(di_istwert.Length));
                return $"DI.IW.{string.Format("{0:000}", value)}";
            }
            else if (origin.StartsWith(di_sollwert))
            {
                var value = int.Parse(origin.Substring(di_sollwert.Length));
                return $"DI.SW.{string.Format("{0:000}", value)}";
            }
            else if (origin.StartsWith(di_unin))
            {
                var value = int.Parse(origin.Substring(di_unin.Length));
                return $"DI.UI.{string.Format("{0:000}", value)}";
            }
            else if (origin.StartsWith(di_unau))
            {
                var value = int.Parse(origin.Substring(di_unau.Length));
                return $"DI.UA.{string.Format("{0:000}", value)}";
            }
            else if (origin.StartsWith(sz_sz_ist))
            {
                var value = int.Parse(origin.Substring(sz_sz_ist.Length));
                return $"SZ.IW.{string.Format("{0:000}", value)}";
            }
            else if (origin.StartsWith(sz_sz_soll))
            {
                var value = int.Parse(origin.Substring(sz_sz_soll.Length));
                return $"SZ.SW.{string.Format("{0:000}", value)}";
            }
            else if (origin.StartsWith(za_istwert))
            {
                var value = int.Parse(origin.Substring(za_istwert.Length));
                return $"ZA.IW.{string.Format("{0:000}", value)}";
            }
            else if (origin.StartsWith(za_sollwert))
            {
                var value = int.Parse(origin.Substring(za_sollwert.Length));
                return $"ZA.SW.{string.Format("{0:000}", value)}";
            }
            else if (origin.StartsWith(ac_ascii))
            {
                var value = int.Parse(origin.Substring(ac_ascii.Length));
                return $"AC.IW.{string.Format("{0:000}", value)}";
            }
            else if (origin.StartsWith(pr_stoer_offen))
            {
                return "STOER.OFFEN";
            }
            else if (origin.StartsWith(pr_still_prio))
            {
                var value = int.Parse(origin.Substring(pr_still_prio.Length));
                return $"STILL.P{string.Format("{0:000}", value)}";
            }
            else if (origin.Contains('_'))
                return origin;
            else
                throw new Exception($"Neznamy typ {origin} nelze zkonvertovat!!!");
        }

        static string convertpointname(string origin)
        {
            int pos = origin.IndexOf('_');
            if (pos < 0)
                return  @"\\ELAK\" + origin;

            int num;
            if(int.TryParse(origin.Substring(0, pos), out num))
                return @"\\ELAK\" + origin;

            pos = origin.IndexOf('_', pos + 1);
            if (pos < 0)
                throw new Exception($"Tag {origin} nejde zkonvertovat!!!");

            //odstranen prefix
            var result = origin.Substring(pos + 1);

            if ((pos = result.IndexOf('_')) < 0)
                throw new Exception($"Tag {origin} nejde zkonvertovat!!!");

            int pos2 = result.IndexOf('_', pos + 1);
            if (pos2 < 0)
                throw new Exception($"Tag {origin} nejde zkonvertovat!!!");

            //plc
            var plc = result.Substring(pos + 1, pos2 - pos - 1);
            int plcnr;
            if (!int.TryParse(plc, out plcnr))
            {

            }

            if ((pos = result.IndexOf("___", pos + 1)) >= 0)
                //agregat & typ(postfix)
                result = result.Substring(0, pos) + "." + postfix(result.Substring(pos + 3));

            result = result.Replace('_', '.');

            return @"\\ELAK\" + result;
        }
    }
}
