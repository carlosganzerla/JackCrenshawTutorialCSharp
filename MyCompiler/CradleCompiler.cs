using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyCompiler
{
    class CradleCompiler
    {
        private static readonly char[] addOps = new[] { '+', '-' };
        private static readonly char[] mulOps = new[] { '*', '/' };
        private char lookahead;

        public bool IsAddOp(char op)
        {
            return addOps.Contains(op);
        }

        public bool IsWhiteSpace(char c)
        {
            return char.IsWhiteSpace(c) && c != '\r';
        }


        public bool IsMulOp(char op)
        {
            return mulOps.Contains(op);
        }

        public void Init()
        {
            GetChar();
            SkipWhite();
        }

        public void GetChar()
        {
            lookahead = (char)Console.Read();
        }

        public void Expected(string expected)
        {
            Abort($"{expected} Expected");
        }

        public void Match(char input)
        {
            if(lookahead == input)
            {
                GetChar();
                SkipWhite();
            }
            else
            {
                Expected($"\"{input}\"");
            }
        }


        public string GetName()
        {
            StringBuilder token = new StringBuilder();
            if (!char.IsLetter(lookahead))
            {
                Expected("Name");
            }
            while (char.IsLetterOrDigit(lookahead))
            {
                token.Append(char.ToUpper(lookahead));
                GetChar();
            }
            SkipWhite();
            return token.ToString();
        }

        public string GetNumber()
        {
            StringBuilder value = new StringBuilder();
            if (!char.IsNumber(lookahead))
            {
                Expected("Integer");
            }
            while (char.IsDigit(lookahead))
            {
                value.Append(lookahead);
                GetChar();
            }
            SkipWhite();
            return value.ToString();
        }

        public void PadZero()
        {
            if (IsAddOp(lookahead))
            {
                EmitLn("CLR D0");
            }
            else
            {
                Term();
            }
        }

        public void Terminator()
        {
            if (lookahead != '\r')
            {
                Expected("NewLine");
            }
        }

        public void Assignment()
        {
            string name = GetName();
            Match('=');
            Expression();
            EmitLn($"LEA {name}(PC),A0");
            EmitLn("MOVE D0,(A0)");
        }

        public void Expression()
        {
            PadZero();
            while (IsAddOp(lookahead))
            {
                EmitLn("MOVE D0,-(SP)");
                switch (lookahead)
                {
                    case '+':
                        Add();
                        break;
                    case '-':
                        Subtract();
                        break;
                    default:
                        Expected("Addop");
                        break;
                }
            }
        }
        private void Add()
        {
            Match('+');
            Term();
            EmitLn("ADD (SP)+,D0");
        }

        private void Subtract()
        {
            Match('-');
            Term();
            EmitLn("SUB (SP)+,D0");
            EmitLn("NEG D0");

        }


        public void SkipWhite()
        {
            while (IsWhiteSpace(lookahead))
            {
                GetChar();
            }
        }
        public void Ident()
        {
            string name = GetName();
            if (lookahead == '(')
            {
                Match('(');
                Match(')');
                EmitLn($"BSR {name}");
            }
            else
            {
                EmitLn($"MOVE {name}(PC),D0");
            }
        }

        public void Factor()
        {
            if(lookahead == '(')
            {
                Match('(');
                Expression();
                Match(')');
            }
            else if (char.IsLetter(lookahead))
            {
                Ident();
            }
            else
            {
                EmitLn($"MOVE #{GetNumber()},D0");
            }
        }

        public void Term()
        {
            Factor();
            while (IsMulOp(lookahead))
            {
                EmitLn("MOVE D0,-(SP)");
                switch (lookahead)
                {
                    case '*':
                        Multiply();
                        break;
                    case '/':
                        Divide();
                        break;
                    default:
                        Expected("Mulop");
                        break;
                }
            } 
        }


        public void Multiply()
        {
            Match('*');
            Factor();
            EmitLn("MULS (SP)+,D0");
        }

        public void Divide()
        {
            Match('/');
            Factor();
            EmitLn("MOVE (SP)+,D1");
            EmitLn("DIVS D1,D0");
        }

        public void Emit(string message)
        {
            Console.Write($"\t{message}");
        }


        public void EmitLn(string message)
        {
            Emit(message);
            Console.WriteLine();
        }

        public void Abort(string message)
        {
            Error(message);
            Console.WriteLine("Aborting...");
            Environment.Exit(-1);
        }


        public void Error(string message)
        {
            Console.WriteLine();
            Console.WriteLine($"Error: {message}");
        }

    }
}
