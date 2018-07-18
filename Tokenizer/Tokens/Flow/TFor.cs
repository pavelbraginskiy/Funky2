using System.Text.RegularExpressions;

namespace Funky.Tokens.Flow{
    class TFor : TExpression{
        private static Regex FOR = new Regex(@"for");
        private static Regex LEFT_BRACKET = new Regex("^\\(");
        private static Regex RIGHT_BRACKET = new Regex("^\\)");

        TExpression initial;
        TExpression condition;
        TExpression after;
        TExpression body;
        
        new public static TFor Claim(StringClaimer claimer){
            Claim c = claimer.Claim(FOR);
            if(!c.Success)
                return null;

            TExpression[] exprs = new TExpression[4];
            claimer.Claim(LEFT_BRACKET);
            int claimed = 0;
            for(;claimed < 3; claimed++){
                if((exprs[claimed] = TExpression.Claim(claimer))==null){ // Out of Expressions. :(
                    break;
                }
            }
            claimed--;
            c = claimer.Claim(RIGHT_BRACKET);
            if((exprs[3] = TExpression.Claim(claimer))==null && !c.Success){
                if(claimed >= 0 && exprs[claimed] != null){
                    exprs[3] = exprs[claimed];
                    exprs[claimed] = null;
                }
            }
            TFor newFor = new TFor();
            newFor.initial = exprs[0];
            newFor.condition = exprs[1];
            newFor.after = exprs[2];
            newFor.body = exprs[3];

            return newFor;
        }

        override public Var Parse(Scope scope){
            initial?.Parse(scope);
            Var ret = null;
            while(condition == null || (condition.Parse(scope)?.AsBool() ?? false)){
                ret = body?.Parse(scope);
                if(scope.Escape.Count > 0){
                    Escaper esc = scope.Escape.Peek();
                    if(esc.Method == Escape.Break)
                        scope.Escape.Pop();
                    return esc.Value;
                }
                after?.Parse(scope);
            }

            return ret;
        }
    }
}