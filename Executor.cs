using System.IO;
using System.Linq;

namespace Funky
{
    public static class Executer
    {
        public static void Main(string[] args)
        {
            var file = args.FirstOrDefault();
            var code = file is null
                ? @"
            for i=0 i<10 i+=1
                print(i)
            "
                : File.ReadAllText(file);
            
            Meta.GetMeta();
            TProgram prog = TProgram.Claim(new StringClaimer(code));
            prog.Parse();
        }
    }
}
