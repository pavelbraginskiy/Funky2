using System.Text.RegularExpressions;

namespace Funky.Tokens.Flow{
    // ReSharper disable once InconsistentNaming
    public class TWhile : TExpression{

        private static readonly Regex While = new Regex(@"while", RegexOptions.Compiled);

        private TExpression _condition;
        private TExpression _action;

        public new static TWhile Claim(StringClaimer claimer){
            Claim failpoint = claimer.FailPoint();
            Claim c = claimer.Claim(While);
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
            TWhile whileBlock = new TWhile
            {
                _condition = condition,
                _action = action
            };

            return whileBlock;
        }

        public override Var Parse(Scope scope){
            Var ret = null;
            while(_condition.Parse(scope)?.AsBool() ?? false){
                ret = _action.Parse(scope);
                if (scope.Escape.Count <= 0) continue;
                Escaper esc = scope.Escape.Peek();
                if(esc.Method == Escape.Break)
                    scope.Escape.Pop();
                return esc.Value;
            }
            return ret;
        }
    }
}