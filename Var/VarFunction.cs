using System.Collections.Generic;
namespace Funky{

    struct CallData{
        public Dictionary<double, Var> num_args;
        public Dictionary<string, Var> str_args;
        public Dictionary<Var, Var>    var_args;

        public CallData(params Var[] args){
            num_args = new Dictionary<double, Var>();
            str_args = new Dictionary<string, Var>();
            var_args = new Dictionary<Var,    Var>();
            for(int i=0; i < args.Length; i++){
                num_args[i] = args[i];
            }
        }
    }

    class VarFunction : Var{
        public System.Func<CallData, Var> action;

        public string FunctionText = "[internal function]";

        public VarFunction(System.Func<CallData, Var> todo) : base(){
            this.action = todo;
        }

        public override Var Call(CallData callData){
            return action(callData);
        }

        public override VarFunction asFunction(){
            return this;
        }
    }


}