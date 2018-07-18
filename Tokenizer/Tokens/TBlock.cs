using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Funky.Tokens{
    // ReSharper disable once InconsistentNaming
    public class TBlock : TExpression {
        private readonly List<TExpression> _expressions = new List<TExpression>();

        private static readonly Regex LeftBracket = new Regex(@"\{", RegexOptions.Compiled);
        private static readonly Regex RightBracket = new Regex(@"\}", RegexOptions.Compiled);
        private static readonly Regex SemiColon = new Regex(@";", RegexOptions.Compiled);

        public override Var Parse(Scope scope){
            Var ret = null;
            Scope newScope = new Scope
            {
                Variables = new VarList {Parent = scope.Variables},
                Escape = scope.Escape
            };
            foreach (var t in _expressions)
            {
                if(scope.Escape.Count > 0){
                    return scope.Escape.Peek().Value;
                }
                ret = t.Parse(newScope);
            }
            return ret;
        }

        public new static TBlock Claim(StringClaimer claimer){
            Claim c = claimer.Claim(LeftBracket);
            if(!c.Success){
                return null;
            }
            c.Pass();
            TBlock newBlock = new TBlock();
            TExpression nExp;
            while((nExp = TExpression.Claim(claimer)) != null){
                newBlock._expressions.Add(nExp);
                claimer.Claim(SemiColon);
                if (!(c = claimer.Claim(RightBracket)).Success) continue;
                c.Pass();
                return newBlock;
            }
            return newBlock;
        }
    }
}