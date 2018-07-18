namespace Funky.Tokens{
    public enum Associativity{
        Na,
        LeftToRight,
        RightToLeft
    }

    // "Left Expressions" are tokens that use an expression as their first argument.
    // They require a "GetLeft" method, which returns their leftmost expression, and a "SetLeft" method, to overrite it.
    // They should also provide a "GetPrecedence" function, which returns an int representing it's Operator Precedence. Lower numbers are tighter grouped than higher numbers. Eg, + might have a precedence of 1, whilst * has a precedence of 2. (Not neccesarily true, just an example.)
    // When in doubt, this function should return -1, to symbolize to always have priority. (Nothing should have a precedence higher than -1)
    // leftClaim is what's called when this is attempted to be claimed. It passes the Expression being used.
    // ReSharper disable once InconsistentNaming
    public abstract class TLeftExpression : TExpression{
        public abstract TExpression GetLeft();
        public abstract void SetLeft(TExpression newLeft);
        public abstract int GetPrecedence();

        public abstract Associativity GetAssociativity();

        public static TLeftExpression LeftClaim(StringClaimer claimer, TExpression left) => null;

        public new static TLeftExpression Claim(StringClaimer claimer){
            Claim failTo = claimer.FailPoint();
            TExpression newLeft = TExpression.Claim(claimer);
            if(newLeft == null){
                failTo.Fail();
                return null;
            }
            newLeft = LeftClaim(claimer, newLeft);


            if (newLeft != null) return (TLeftExpression) newLeft;
            failTo.Fail();
            return null;
        }
    }
}