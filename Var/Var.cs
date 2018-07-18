namespace Funky{
    public class Var{
        public string Type;
        public VarList Meta;

        // Sometimes you want a string representation of the type, but you want to do it Programatically.
        // AKA: Making real coders cry.
        public Var(){Type = GetType().Name.Substring(3).ToLower();}

        public virtual Var Call(CallData callData){
            return this;
        }

        public virtual Var Get(Var key){
            Var callFunc = Funky.Meta.Get(this, "get", $"key({key.Type})");
            return callFunc?.Call(new CallData(this, (VarString)key));
        }

        public virtual Var Set(Var key, Var val){
            Var callFunc = Funky.Meta.Get(this, "set", $"key({key.Type})", $"value({val.Type})");
            return callFunc != null ? callFunc.Call(new CallData(this, (VarString)key, val)) : val;
        }

        public Var this[string key]{
            get => Get(key);
            set => Set(key, value);
        }

        public Var this[double key]{
            get => Get(key);
            set => Set(key, value);
        }

        public Var this[Var key]{
            get => Get(key);
            set => Set(key, value);
        }

        public virtual VarString AsString(){
            Var callFunc = Funky.Meta.Get(this, "tostring");
            if (callFunc == null) return new VarString("");
            Var outp = callFunc.Call(new CallData(this));
            if(outp is VarString s)
            {
                return s;
            }
            return outp.AsString();
        }
        public virtual VarNumber AsNumber(){
            Var callFunc = Funky.Meta.Get(this, "tonumber");
            if (callFunc == null) return new VarNumber(0);
            Var outp = callFunc.Call(new CallData(this));
            if(outp is VarNumber n){
                return n;
            }
            return outp.AsNumber();
        }
        public virtual VarList AsList(){
            Var callFunc = Funky.Meta.Get(this, "tolist");
            if(callFunc != null){
                Var outp = callFunc.Call(new CallData(this));
                if(!(outp is VarList)){
                    return outp.AsList();
                }
                return outp as VarList;
            }

            VarList n = new VarList {DoubleVars = {[0] = this}};
            return n;
        }

        public virtual VarFunction AsFunction(){
            Var callFunc = Funky.Meta.Get(this, "tofunction");
            if (callFunc == null)
                return new VarFunction(Call);
            Var outp = callFunc.Call(new CallData(this));
            if(!(outp is VarFunction)){
                return outp.AsFunction();
            }
            return outp as VarFunction;
        }

        public virtual bool AsBool(){
            Var callFunc = Funky.Meta.Get(this, "tobool");
            if (callFunc == null) return false;
            Var outp = callFunc.Call(new CallData(this));
            if(!(outp is VarNumber n)){
                return outp.AsBool();
            }
            return n.Value != 0;
        }

        public static implicit operator Var(string v){
            return new VarString(v);
        }

        public static implicit operator Var(double v){
            return new VarNumber(v);
        }
    }

    public class VarNumber : Var{
        public double Value;

        public static implicit operator double(VarNumber var){
            return var.Value;
        }
        public VarNumber(double v)
        {
            Value = v;
        }

        public override VarNumber AsNumber(){
            return this;
        }
        public override bool AsBool(){
            return this!=0;
        }
    }

    public class VarString : Var {
        public string Data;
        public static implicit operator string(VarString var){
            return var.Data;
        }

        public VarString(string d)
        {
            Data = d;
        }

        public override VarString AsString(){
            return this;
        }

    }
}