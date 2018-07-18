using System.Text.RegularExpressions;

namespace Funky.Tokens.Flow{
    // ReSharper disable once InconsistentNaming
    public class TIf : TExpression{

        private static readonly Regex If = new Regex(@"if", RegexOptions.Compiled);
        private static readonly Regex Else = new Regex(@"else", RegexOptions.Compiled);

        private TExpression _condition;
        private TExpression _action;
        private TExpression _otherwise;

        public new static TIf Claim(StringClaimer claimer){
            Claim failpoint = claimer.FailPoint();
            Claim c = claimer.Claim(If);
            if(!c.Success){
                failpoint.Fail();
                return null;
            }
            TExpression condition = TExpression.Claim(claimer);
            if(condition == null){
                failpoint.Fail();
                return null;
            }
            TExpression action = TExpression.Claim(claimer);
            TIf ifblock = new TIf
            {
                _condition = condition,
                _action = action
            };


            c = claimer.Claim(Else);
            if (!c.Success) return ifblock;
            TExpression otherwise = TExpression.Claim(claimer);
            if(otherwise == null){
                c.Fail();
            }else{
                ifblock._otherwise = otherwise;
                c.Pass();
            }


            return ifblock;
        }

        public override Var Parse(Scope scope){
            Var should = _condition.Parse(scope);
            return should?.AsBool()??false?_action.Parse(scope):_otherwise?.Parse(scope);
        }
    }
}