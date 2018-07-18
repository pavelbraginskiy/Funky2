using System;
using System.Text;

namespace Funky{
   public static class Globals{
       private static VarList _globals;
       public static VarList Get()
       {
           return _globals ?? (_globals = new VarList
           {
               ["print"] = new VarFunction(dat => {
                   StringBuilder sb = new StringBuilder();
                   string chunker = "";
                   for (int i = 0; dat.NumArgs.ContainsKey(i); i++)
                   {
                       sb.Append(chunker);
                       sb.Append(dat.NumArgs[i]?.AsString() ?? "null");
                       chunker = "\t";
                   }

                   string outStr = sb.ToString();
                   Console.WriteLine(outStr);
                   return outStr;
               })
           });
       }
   } 
}