using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Funky{
    public static class Meta{
        private static VarList _meta;

        public static VarList GetMeta(){
            return _meta ?? (_meta = GenerateMeta());
        }

        public static VarList GenerateMeta(){
            VarList metas = new VarList
            {
                ["list"] = _List(),
                ["string"] = _String(),
                ["number"] = _Number(),
                ["function"] = _Function()
            };

            return metas;
        }

        private static VarList _String(){
            VarList str = new VarList
            {
                ["tobool"] = new VarFunction(dat => new VarNumber(dat.NumArgs[0].AsString().Data.Length)),
                ["lt[side=left,left=string,right=string]"] = new VarFunction(dat =>
                    new VarNumber(string.Compare(dat.NumArgs[0].AsString().Data, dat.NumArgs[1].AsString().Data, StringComparison.Ordinal) == -1
                        ? 1
                        : 0)),
                ["le[side=left,left=string,right=string]"] = new VarFunction(dat =>
                    new VarNumber(string.Compare(dat.NumArgs[0].AsString().Data, dat.NumArgs[1].AsString().Data, StringComparison.Ordinal) <= 0
                        ? 1
                        : 0)),
                ["gt[side=left,left=string,right=string]"] = new VarFunction(dat =>
                    new VarNumber(string.Compare(dat.NumArgs[0].AsString().Data, dat.NumArgs[1].AsString().Data, StringComparison.Ordinal) == 1
                        ? 1
                        : 0)),
                ["ge[side=left,left=string,right=string]"] = new VarFunction(dat =>
                    new VarNumber(string.Compare(dat.NumArgs[0].AsString().Data, dat.NumArgs[1].AsString().Data, StringComparison.Ordinal) >= 0
                        ? 1
                        : 0)),
                ["eq[side=left,left=string,right=string]"] = new VarFunction(dat =>
                    new VarNumber(dat.NumArgs[0].AsString().Data == dat.NumArgs[1].AsString().Data ? 1 : 0)),
                ["ne[side=left,left=string,right=string]"] = new VarFunction(dat =>
                    new VarNumber(dat.NumArgs[0].AsString().Data != dat.NumArgs[1].AsString().Data ? 1 : 0)),
                ["eq"] = new VarFunction(dat => new VarNumber(0)),
                ["ne"] = new VarFunction(dat => new VarNumber(1)),
                ["add"] = new VarFunction(dat =>
                    new VarString(dat.NumArgs[0].AsString().Data + dat.NumArgs[1].AsString().Data)),
                ["concat"] = new VarFunction(dat =>
                    new VarString(dat.NumArgs[0].AsString().Data + dat.NumArgs[1].AsString().Data))
            };




            return str;
        }

        private static VarList _Function(){
            VarList fnc = new VarList
            {
                ["tobool"] = new VarFunction(dat => new VarNumber(1)),
                ["eq"] = new VarFunction(dat => new VarNumber(dat.NumArgs[0] == dat.NumArgs[1] ? 1 : 0)),
                ["ne"] = new VarFunction(dat => new VarNumber(dat.NumArgs[0] == dat.NumArgs[1] ? 0 : 1))
            };



            return fnc;
        }

        private static VarList _List(){
            VarList lst = new VarList
            {
                ["tobool"] = new VarFunction(dat => new VarNumber(1)),
                ["eq"] = new VarFunction(dat => new VarNumber(dat.NumArgs[0] == dat.NumArgs[1] ? 1 : 0)),
                ["ne"] = new VarFunction(dat => new VarNumber(dat.NumArgs[0] == dat.NumArgs[1] ? 0 : 1))
            };



            return lst;
        }

        private static VarList _Number(){
            VarList num = new VarList
            {
                ["tobool"] = new VarFunction(dat => dat.NumArgs[0]),
                ["add[side=left,left=number,right=number]"] = new VarFunction(dat =>
                    new VarNumber(dat.NumArgs[0].AsNumber() + dat.NumArgs[1].AsNumber())),
                ["sub[side=left,left=number,right=number]"] = new VarFunction(dat =>
                    new VarNumber(dat.NumArgs[0].AsNumber() - dat.NumArgs[1].AsNumber())),
                ["mult[side=left,left=number,right=number]"] = new VarFunction(dat =>
                    new VarNumber(dat.NumArgs[0].AsNumber() * dat.NumArgs[1].AsNumber())),
                ["div[side=left,left=number,right=number]"] = new VarFunction(dat =>
                    new VarNumber(dat.NumArgs[0].AsNumber() / dat.NumArgs[1].AsNumber())),
                ["intdiv[side=left,left=number,right=number]"] = new VarFunction(dat =>
                    new VarNumber((int) (dat.NumArgs[0].AsNumber() / dat.NumArgs[1].AsNumber()))),
                ["pow[side=left,left=number,right=number]"] = new VarFunction(dat =>
                    new VarNumber(Math.Pow(dat.NumArgs[0].AsNumber(), dat.NumArgs[1].AsNumber()))),
                ["mod[side=left,left=number,right=number]"] = new VarFunction(dat =>
                    new VarNumber(dat.NumArgs[0].AsNumber() % dat.NumArgs[1].AsNumber())),
                ["concat[side=left]"] = new VarFunction(dat =>
                    new VarString(dat.NumArgs[0].AsString() + dat.NumArgs[1].AsString())),
                ["bitor[side=left,left=number,right=number]"] = new VarFunction(dat =>
                    new VarNumber((int) dat.NumArgs[0].AsNumber().Value | (int) dat.NumArgs[1].AsNumber().Value)),
                ["bitand[side=left,left=number,right=number]"] = new VarFunction(dat =>
                    new VarNumber((int) dat.NumArgs[0].AsNumber().Value & (int) dat.NumArgs[1].AsNumber().Value)),
                ["bitxor[side=left,left=number,right=number]"] = new VarFunction(dat =>
                    new VarNumber((int) dat.NumArgs[0].AsNumber().Value ^ (int) dat.NumArgs[1].AsNumber().Value)),
                ["bitshiftl[side=left,left=number,right=number]"] = new VarFunction(dat =>
                    new VarNumber((int) dat.NumArgs[0].AsNumber().Value << (int) dat.NumArgs[1].AsNumber().Value)),
                ["bitshiftr[side=left,left=number,right=number]"] = new VarFunction(dat =>
                    new VarNumber((int) dat.NumArgs[0].AsNumber().Value >> (int) dat.NumArgs[1].AsNumber().Value)),
                ["lt[side=left,left=number,right=number]"] = new VarFunction(dat =>
                    new VarNumber(dat.NumArgs[0].AsNumber().Value < dat.NumArgs[1].AsNumber().Value ? 1 : 0)),
                ["le[side=left,left=number,right=number]"] = new VarFunction(dat =>
                    new VarNumber(dat.NumArgs[0].AsNumber().Value <= dat.NumArgs[1].AsNumber().Value ? 1 : 0)),
                ["gt[side=left,left=number,right=number]"] = new VarFunction(dat =>
                    new VarNumber(dat.NumArgs[0].AsNumber().Value > dat.NumArgs[1].AsNumber().Value ? 1 : 0)),
                ["ge[side=left,left=number,right=number]"] = new VarFunction(dat =>
                    new VarNumber(dat.NumArgs[0].AsNumber().Value >= dat.NumArgs[1].AsNumber().Value ? 1 : 0)),
                ["eq[side=left,left=number,right=number]"] = new VarFunction(dat =>
                    new VarNumber(dat.NumArgs[0].AsNumber().Value == dat.NumArgs[1].AsNumber().Value ? 1 : 0)),
                ["ne[side=left,left=number,right=number]"] = new VarFunction(dat =>
                    new VarNumber(dat.NumArgs[0].AsNumber().Value != dat.NumArgs[1].AsNumber().Value ? 1 : 0)),
                ["eq"] = new VarFunction(dat => new VarNumber(0)),
                ["ne"] = new VarFunction(dat => new VarNumber(1)),
                ["tostring"] = new VarFunction(dat =>
                    new VarString((dat.NumArgs[0] as VarNumber)?.Value.ToString(CultureInfo.InvariantCulture)))
            };





            return num;
        }

        private static string MakeOptions(IReadOnlyList<string> options, IReadOnlyList<bool> toggled){
            StringBuilder sb = new StringBuilder();
            string flooper = string.Empty;
            for(int i=0; i < options.Count; i++){
                if (!toggled[i]) continue;
                sb.Append(flooper);
                sb.Append(options[i]);
                flooper = ",";
            }

            return sb.ToString();
        }

        private static bool Flop(IList<bool> list, int pos = 0)
        {
            while (true)
            {
                if (list[pos])
                {
                    list[pos] = false;
                    return false;
                }

                list[pos] = true;
                if (pos + 1 < list.Count)
                {
                    pos = pos + 1;
                }
                else
                {
                    return true;
                }
            }
        }

        public static Var LR_Get(Var l, Var r, string name, params string[] options){
            bool lValue = l.Meta != null;
            bool rValue = r.Meta != null;
            string[] newOptions = new string[options.Length+1];
            for(int i = 0; i < options.Length; i++){
                newOptions[i+1] = options[i];
            }
            newOptions[0] = "side=left";
            Var lMeta = Get(l, name, newOptions);
            newOptions[0] = "side=right";
            if(lMeta == null || (!lValue && rValue))
                return Get(r, name, newOptions) ?? lMeta;
            return lMeta;
        }

        public static Var Get(Var val, string name, params string[] options){
            VarList varMeta;
            if(val.Meta != null){
                varMeta = val.Meta;
            }else{
                if(_meta == null)
                    return null;
                if(!_meta.StringVars.ContainsKey(val.Type)){
                    return null;
                }
                varMeta = (VarList)_meta.StringVars[val.Type];
            }
            bool[] opUse = new bool[options.Length];
            for(int i=0; i < opUse.Length; i++){
                opUse[i] = true;
            }

            while(opUse.Length > 0){
                string checkName = $"{name}[{MakeOptions(options, opUse)}]";
                if(varMeta.StringVars.ContainsKey(checkName))
                    return varMeta.StringVars[checkName];
                
                if(Flop(opUse))
                    break;
            }
            return varMeta.StringVars.ContainsKey(name) ? varMeta.StringVars[name] : null;
        }
    }
}