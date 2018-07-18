using Funky.Tokens.Flow;

namespace Funky.Tokens{
    // ReSharper disable once InconsistentNaming
    public abstract class TExpression : Token{
        public new static TExpression Claim(StringClaimer claimer){
            TExpression preClaimed = pre_claim(claimer);
            if(preClaimed == null)
                return null;
            TExpression nextClaim;
            while((nextClaim = post_claim(claimer, preClaimed))!=null)
                preClaimed = nextClaim;
            return preClaimed;
        }

        private static TExpression pre_claim(StringClaimer claimer){
            return TAssignment.Claim(claimer) ??
            TIf.Claim(claimer) ??
            TFor.Claim(claimer) ??
            TWhile.Claim(claimer) ??
            TVariable.Claim(claimer) ??
            TLiteral.Claim(claimer) ??
            TParenExpression.Claim(claimer) ??
            TBlock.Claim(claimer)               as TExpression;
        }

        private static TExpression post_claim(StringClaimer claimer, TExpression lastClaim){
            return  TCall.LeftClaim(claimer, lastClaim) ??
            TArithmetic.LeftClaim(claimer, lastClaim);
        }

        public abstract Var Parse(Scope scope); // Although Expression requires a Parse function, it fails to implement it, because it shouldn't be possible to have a raw "TExpression" token.
    }
}