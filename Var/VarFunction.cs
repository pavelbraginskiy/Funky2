using System;
using System.Collections.Generic;

namespace Funky{
    public struct CallData{
        public Dictionary<double, Var> NumArgs;
        public Dictionary<string, Var> StrArgs;
        public Dictionary<Var, Var>    VarArgs;

        public CallData(params Var[] args){
            NumArgs = new Dictionary<double, Var>();
            StrArgs = new Dictionary<string, Var>();
            VarArgs = new Dictionary<Var,    Var>();
            for(int i=0; i < args.Length; i++){
                NumArgs[i] = args[i];
            }
        }
    }

    public class VarFunction : Var{
        public Func<CallData, Var> Action;

        public string FunctionText = "[internal function]";

        public VarFunction(Func<CallData, Var> todo)
        {
            Action = todo;
        }

        public override Var Call(CallData callData){
            return Action(callData);
        }

        public override VarFunction AsFunction(){
            return this;
        }
    }


}