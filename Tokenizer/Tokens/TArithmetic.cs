namespace Funky.Tokens{
    // ReSharper disable once InconsistentNaming
    public class TArithmetic : TLeftExpression{
        private TExpression _leftArg;
        private TExpression _rightArg;
        private TOperator _op;

        public override TExpression GetLeft(){
            return _leftArg;
        }

        public override void SetLeft(TExpression newLeft){
            _leftArg = newLeft;
        }

        public override int GetPrecedence(){
            return _op.GetPrecedence();
        }

        public override Associativity GetAssociativity(){
            return _op.GetAssociativity();
        }

        public new static TLeftExpression LeftClaim(StringClaimer claimer, TExpression left){
            Claim failTo = claimer.FailPoint();
            TOperator operand = TOperator.Claim(claimer);
            if(operand == null){
                failTo.Fail();
                return null;
            }
            TExpression right = TExpression.Claim(claimer);
            if(right == null){
                failTo.Fail();
                return null;
            }

            TArithmetic newArith = new TArithmetic
            {
                _leftArg = left,
                _rightArg = right,
                _op = operand
            };

            if (!(right is TLeftExpression t) || t.GetAssociativity() == Associativity.Na) return newArith;
            int prec = operand.GetPrecedence();
            int rPrec = t.GetPrecedence();
            if (prec >= rPrec && (prec != rPrec || operand.GetAssociativity() != Associativity.LeftToRight))
                return newArith;
            newArith._rightArg = t.GetLeft();
            t.SetLeft(newArith);
            return t;

        }

        public override Var Parse(Scope scope){
            return _op.Parse(_leftArg.Parse(scope), _rightArg.Parse(scope));
        }

    }
}