using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Funky.Tokens{
    // ReSharper disable once InconsistentNaming
    public class TCall : TLeftExpression{
        private TExpression _caller;
        private readonly List<TArgument> _arguments = new List<TArgument>();

        private static readonly Regex LeftBracket = new Regex("^\\(", RegexOptions.Compiled);
        private static readonly Regex RightBracket = new Regex("^\\)", RegexOptions.Compiled);
        private static readonly Regex Comma = new Regex("^,", RegexOptions.Compiled);

        public static VarList HackMeta;
        

        public override TExpression GetLeft(){
            return _caller;
        }

        public override void SetLeft(TExpression newLeft){
            _caller = newLeft;
        }

        public override int GetPrecedence(){
            return -1;
        }

        public override Associativity GetAssociativity(){
            return Associativity.Na;
        }

        public new static TCall LeftClaim(StringClaimer claimer, TExpression left){
            Claim lb = claimer.Claim(LeftBracket);
            if(!lb.Success){ // Left Bracket is a requirement.
                return null;
            }
            lb.Pass(); // At this point, we cannot fail.

            TCall newCall = new TCall {_caller = left};

            while(true){
                Claim rb = claimer.Claim(RightBracket);
                if(rb.Success){
                    rb.Pass();
                    break;
                }
                TArgument newArg = TArgument.Claim(claimer);
                if(newArg == null){
                    break;
                }
                newCall._arguments.Add(newArg);
                claimer.Claim(Comma);
            }

            return newCall;
        }

        public override Var Parse(Scope scope){
            Scope hackedScope = new Scope();
            VarList argList = new VarList();
            hackedScope.Variables = argList;
            argList.StringVars["_parent"] = scope.Variables;
            argList.Meta = GetHackMeta();
            int index = 0;
            foreach (var t in _arguments)
            {
                index = t.AppendArguments(argList, index, hackedScope);
            }
            Var callVar = _caller.Parse(scope);
            if(callVar == null){
                return null;
            }

            CallData callData = new CallData
            {
                NumArgs = argList.DoubleVars,
                StrArgs = argList.StringVars,
                VarArgs = argList.OtherVars
            };
            return callVar.Call(callData);
        }

        private static VarList GetHackMeta(){
            if (HackMeta != null) return HackMeta;
            HackMeta = new VarList
            {
                ["get"] = new VarFunction(delegate(CallData data)
                {
                    Var father = data.NumArgs[0].Get("_parent");

                    return father?.Get(data.NumArgs[1]);
                })
            };
            return HackMeta;
        }

    }

    // ReSharper disable once InconsistentNaming
    public abstract class TArgument : TExpression{
        public abstract int AppendArguments(VarList argumentList, int index, Scope scope);
        public override Var Parse(Scope scope) => null;

        public new static TArgument Claim(StringClaimer claimer){
            return TArgExpression.Claim(claimer);
        } 
    }

    // ReSharper disable once InconsistentNaming
    public class TArgExpression : TArgument{
        private TExpression _heldExp;
        public override int AppendArguments(VarList argumentList, int index, Scope scope){
            argumentList.DoubleVars[index] = _heldExp.Parse(scope);
            return index+1;
        }

        public new static TArgExpression Claim(StringClaimer claimer){
            TExpression heldExpr = TExpression.Claim(claimer);
            if(heldExpr == null)
                return null;
            TArgExpression newArgExp = new TArgExpression {_heldExp = heldExpr};
            return newArgExp;
        }
    }
}