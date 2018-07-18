using System.Text.RegularExpressions;

namespace Funky.Tokens{

    // Lower Operator Precedence is "More Sticky".
    // ReSharper disable once InconsistentNaming
    public class TOperator : Token{

        private static readonly SOperator[] Operators = {
            new SOperator(@"\+", "add", 7),
            new SOperator(@"-", "sub", 7),
            new SOperator(@"\*", "mult", 6),
            new SOperator(@"//", "intdiv", 6),
            new SOperator(@"/", "div", 6),
            new SOperator(@"\^", "pow", 5,  Associativity.RightToLeft),
            new SOperator(@"\%", "mod", 6),
            new SOperator(@"&&", "and", 15),
            new SOperator(@"\|\|", "or", 16),
            new SOperator(@"\.\.", "concat", 8),
            new SOperator(@"\|", "bitor", 14),
            new SOperator(@"&", "bitand", 12),
            new SOperator(@"~", "bitxor", 13),
            new SOperator(@"<<", "bitshiftl", 9),
            new SOperator(@">>", "bitshiftr", 9),
            new SOperator(@"<", "lt", 10),
            new SOperator(@"<=", "le", 10),
            new SOperator(@">", "gt", 10),
            new SOperator(@">=", "ge", 10),
            new SOperator(@"==", "eq", 11),
            new SOperator(@"!=", "ne", 11)
        };

        private SOperator _op;
        public int GetPrecedence(){
            return _op.Precedence;
        }

        public Associativity GetAssociativity(){
            return _op.Associativity;
        }

        public new static TOperator Claim(StringClaimer claimer)
        {
            foreach (var thisOp in Operators)
            {
                Claim c = claimer.Claim(thisOp.Regex);
                if (!c.Success) continue;
                c.Pass();
                TOperator newOp = new TOperator {_op = thisOp};
                return newOp;
            }

            return null;
        }

        public Var Parse(Var left, Var right){
            Var metaMethod = Meta.LR_Get(left, right, _op.Name, $"left={left.Type}", $"right={right.Type}");
            return metaMethod?.Call(new CallData(left, right));
        }
    }

    public struct SOperator{
        public Regex Regex;
        public string Name;
        public int Precedence;

        public Associativity Associativity;
        public SOperator(string regex, string name, int precedence){
            Regex = new Regex(regex);
            Name = name;
            Precedence = precedence;
            Associativity = Associativity.LeftToRight;
        }

        public SOperator(string regex, string name, int precedence, Associativity assoc){
            Regex = new Regex(regex);
            Name = name;
            Precedence = precedence;
            Associativity = assoc;
        }
    }
}