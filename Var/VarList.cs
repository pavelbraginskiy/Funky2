using System.Collections.Generic;

namespace Funky{
    public class VarList : Var{
        public Dictionary<string, Var> StringVars = new Dictionary<string, Var>();
        public List<string> Defined = new List<string>();
        public Dictionary<double, Var> DoubleVars = new Dictionary<double, Var>();
        public Dictionary<Var, Var> OtherVars     = new Dictionary<Var, Var>();

        public VarList Parent;

        public override Var Get(Var key){
            return ThisGet(key) ?? Parent?.Get(key);
        }

        public Var ThisGet(Var key){
            switch (key)
            {
                case VarNumber n:
                    return DoubleVars.ContainsKey(n) ? DoubleVars[n] : base.Get(key);
                case VarString s:
                    return StringVars.ContainsKey(s) ? StringVars[s] : base.Get(key);
            }

            return OtherVars.ContainsKey(key) ? OtherVars[key] : base.Get(key);
        }

        public override Var Set(Var key, Var value){
            return ThisSet(key, value); // Not sure if I need this..?
        }

        public Var ThisSet(Var key, Var val){
            Var metaFunc = Funky.Meta.Get(this, "set", $"key({key.Type})", $"value({val.Type})");

            if(metaFunc != null)
                return metaFunc.Call(new CallData((VarString)key, val));
            
            bool assignHere = false;
            if(Parent == null)
                assignHere = true;
            else if (key is VarString s && Defined.Contains(s))
                assignHere = true;
            else if (ThisGet(key) != null)
                assignHere = true;

            if (!assignHere) return Parent.Set(key, val);
            
            switch (key)
            {
                case VarNumber n:
                    return DoubleVars[n] = val;
                case VarString s:
                    return StringVars[s] = val;
            }

            return OtherVars[key] = val;
            
        }
    }
}