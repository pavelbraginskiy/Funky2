using System.Text.RegularExpressions;

namespace Funky.Tokens{
    // ReSharper disable once InconsistentNaming
    public abstract class TVariable : TExpression{
        public new static TVariable Claim(StringClaimer claimer){
            TVariable result;
            return (result = TIdentifier.Claim(claimer))!=null ? result : null;
        }

        public override Var Parse(Scope scope){
            return Get(scope);
        }

        public abstract Var Get(Scope scope);
        public abstract Var Set(Scope scope, Var value);
    }

    // ReSharper disable once InconsistentNaming
    public class TIdentifier : TVariable{
        public string Name;
        private bool _isLocal;
        private static readonly Regex Local = new Regex(@"local|var|let", RegexOptions.Compiled);
        private static readonly Regex Identifier = new Regex(@"^[a-zA-Z_]\w*", RegexOptions.Compiled);

        public new static TIdentifier Claim(StringClaimer claimer){   
            TIdentifier ident = new TIdentifier();
            Claim c = claimer.Claim(Local);
            if(c.Success){
                c.Pass();
                ident._isLocal = true;
            }

            c = claimer.Claim(Identifier);
            if(!c.Success){
                return null;
            }
            ident.Name = c.GetText();
            return ident;
        }

        public override Var Get(Scope scope){
            return _isLocal ? (scope.Variables.StringVars.ContainsKey(Name) ? scope.Variables.StringVars[Name] : null) : scope.Variables.Get(Name);
        }

        public override Var Set(Scope scope, Var value){
            return _isLocal ? scope.Variables.StringVars[Name] = value : scope.Variables.Set(Name, value);
        }
    }
}