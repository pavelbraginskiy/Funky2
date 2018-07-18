using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Funky.Tokens{
    // ReSharper disable once InconsistentNaming
    public abstract class TLiteral : TExpression{
        public new static TLiteral Claim(StringClaimer claimer){
            return TLiteralNumber.Claim(claimer) ??
            TLiteralStringSimple.Claim(claimer)     as TLiteral;
        }
    }

    // ReSharper disable once InconsistentNaming
    public class TLiteralStringSimple : TLiteral{
        private VarString _value;
        private static readonly Regex String = new Regex(@"^(?<qoute>'|"")(?<text>(\\\\|\\[^\\]|[^\\])*?)\k<qoute>", RegexOptions.Compiled);

        public new static TLiteralStringSimple Claim(StringClaimer claimer){
            Claim c = claimer.Claim(String);
            if(!c.Success){
                return null;
            }
            c.Pass();
            TLiteralStringSimple str = new TLiteralStringSimple
            {
                _value = new VarString(Regex.Unescape(c.GetMatch().Groups["text"].Value))
            };
            return str;
        }

        public override Var Parse(Scope scope){
            return _value;
        }
    }

    // ReSharper disable once InconsistentNaming
    public class TLiteralNumber : TLiteral{
        private VarNumber _value;
        private static readonly Regex Number = new Regex(
            // WTF is this regex?
            @"^(?<negative>-?)((?<integer>0(x(?<hex_val>[0-9A-Fa-f]+)|b(?<bin_val>[01]+)))|((?<float>(?<int_comp>\d*)\.(?<float_comp>\d+))|(?<int>\d+))(e(?<expon>-?\d+))?)",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
            //ExplicitCapture lets you use `()` isntead of `(?:)`
            //I'd use RegexOptions.IgnorePatternWhiteSpace to break the regex up into a few lines for readability
            //But honestly it's kinda beyond hope
        );
        public new static TLiteralNumber Claim(StringClaimer claimer){
            TLiteralNumber numb = new TLiteralNumber();

            Claim claim = claimer.Claim(Number);

            if(!claim.Success){
                return null;
            }
            claim.Pass();

            double v;
            Match m = claim.GetMatch();


            if(m.Groups["integer"].Length > 0)
            {
                // x or b integer format.
                v = m.Groups["hex_val"].Length > 0 ? int.Parse(m.Groups["hex_val"].Value, NumberStyles.HexNumber) : Convert.ToInt32(m.Groups["bin_val"].Value, 2);
            }else{
                var num = m.Groups["int"].Length > 0 ? m.Groups["int"].Value : m.Groups["float"].Value;
                v = Convert.ToDouble(num);
                if(m.Groups["expon"].Length > 0){
                    for(int i = Convert.ToInt32(m.Groups["expon"].Value); i>0; i--)
                        v *= 10;
                }
            }

            if(m.Groups["negative"].Length > 0) // Has a -
                v *= -1;
            numb._value = new VarNumber(v);
            return numb;
        }

        public override Var Parse(Scope scope){
            return _value;
        }
    }
}