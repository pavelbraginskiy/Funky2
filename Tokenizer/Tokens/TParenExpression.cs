using System.Text.RegularExpressions;

namespace Funky.Tokens{
    // ReSharper disable once InconsistentNaming
    public class TParenExpression : TExpression{
        private static readonly Regex LeftBracket = new Regex(@"^\(", RegexOptions.Compiled);
        private static readonly Regex RightBracket = new Regex(@"^\)", RegexOptions.Compiled);

        private TExpression _realExpression;

        public new static TParenExpression Claim(StringClaimer claimer){
            Claim lb = claimer.Claim(LeftBracket);
            if(!lb.Success) return null;
            TParenExpression ptoken = new TParenExpression {_realExpression = TExpression.Claim(claimer)};
            if(ptoken._realExpression == null){
                lb.Fail();
                return null;
            }
            lb.Pass();
            Claim rb = claimer.Claim(RightBracket);
            if(rb.Success) rb.Pass(); // right bracket is optional. So just pass it if we get it.
            return ptoken;
        }

        public override Var Parse(Scope scope){
            return _realExpression.Parse(scope);
        }
    }
}