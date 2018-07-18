using System.Collections.Generic;
using System.Text.RegularExpressions;
using Funky.Tokens;

namespace Funky{
    public class Tokenizer{
        
    }

    public enum Escape{
        Return, Break
    }

    public struct Scope {
        public VarList Variables;
        public Stack<Escaper> Escape;

        public Scope(VarList v){
            Variables = v;
            Escape = new Stack<Escaper>();
        }
    }

    public struct Escaper{
        public Escape Method;
        public Var Value;

        public Escaper(Escape method, Var value){
            Method = method;
            Value = value;
        }
    }

    // ReSharper disable once InconsistentNaming
    public class TProgram : Token{
        private readonly List<TExpression> _expressions = new List<TExpression>();
        private static readonly Regex SemiColon = new Regex(@";", RegexOptions.Compiled);
        public new static TProgram Claim(StringClaimer claimer){
            TProgram prog = new TProgram();

            TExpression e;
            while((e = TExpression.Claim(claimer))!=null){
                claimer.Claim(SemiColon);
                prog._expressions.Add(e);
            }
            return prog;
        }

        public void Parse(){
            VarList scopeList = new VarList {Parent = Globals.Get()};
            Scope scope = new Scope(scopeList);
            Var[] results = new Var[_expressions.Count];
            for(int i=0; i < _expressions.Count; i++){
                results[i] = _expressions[i].Parse(scope);
            }
        }
    }
}