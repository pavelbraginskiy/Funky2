using System.Text.RegularExpressions;

namespace Funky.Tokens{
    // ReSharper disable once InconsistentNaming
    public class TAssignment : TExpression {
        private TExpression _value;
        private TVariable _var;
        private TOperator _op;

        private static readonly Regex Set = new Regex(@"=", RegexOptions.Compiled);

        public new static TAssignment Claim(StringClaimer claimer){
            Claim failTo = claimer.FailPoint();

            TVariable toAssign = TVariable.Claim(claimer);
            if(toAssign == null){
                failTo.Fail();
                return null;
            }

            TOperator newOp = TOperator.Claim(claimer);

            Claim c = claimer.Claim(Set);
            if(!c.Success){
                failTo.Fail();
                return null;
            }
            TExpression assignValue = TExpression.Claim(claimer);
            if(assignValue == null){
                failTo.Fail();
                return null;
            }

            TAssignment newAssign = new TAssignment
            {
                _var = toAssign,
                _op = newOp,
                _value = assignValue
            };

            return newAssign;
        }

        public override Var Parse(Scope scope)
        {
            if (_op == null) return _var.Set(scope, _value.Parse(scope));
            Var left = _var.Get(scope);
            Var val = _value.Parse(scope);
            val = _op.Parse(left, val);
            return _var.Set(scope, val);
        }
    }
}