using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyCompiler
{
    class CradleInterpreter
    {
        private static readonly char[] addOps = new[] { '+', '-' };
        private static readonly char[] mulOps = new[] { '*', '/' };
        private readonly int[] variables = new int[26];
        private char lookahead;

        private int GetValue(char var)
        {
            return variables[char.ToUpper(var) - 'A'];
        }

        private void SetValue(char var, int value)
        {
            variables[char.ToUpper(var) - 'A'] = value;
        }


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

        public int Convert(char c)
        {
            return (int)char.GetNumericValue(c);
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


        public char GetName()
        {
            if (!char.IsLetter(lookahead))
            {
                Expected("Name");
            }
            char name = lookahead;
            GetChar();
            return name;
        }

        public int GetNumber()
        {
            if (!char.IsDigit(lookahead))
            {
                Expected("Integer");
            }
            int value = 0;
            while (char.IsDigit(lookahead))
            {
                value = 10 * value + Convert(lookahead);
                GetChar();
            }
            return value;
        }

        public void Terminator()
        {
            if (lookahead != '\r')
            {
                Expected("NewLine");
            }
        }
        public int Expression()
        {
            int value = 0;
            if (!IsAddOp(lookahead))
            {
                value = Term();
            }
            while (IsAddOp(lookahead))
            {
                if(lookahead == '+')
                {
                    Match('+');
                    value += Term();
                }
                else if(lookahead == '-')
                {
                    Match('-');
                    value -= Term();
                }
            }
            return value;
        }

        public void SkipWhite()
        {
            while (IsWhiteSpace(lookahead))
            {
                GetChar();
            }
        }

        public int Factor()
        {
            int factor;
            if (lookahead == '(')
            {
                Match('(');
                factor = Expression();
                Match(')');
            }
            else if (char.IsLetter(lookahead))
            {
                factor = GetValue(GetName());
            }
            else
            {
                factor = GetNumber();
            }
            return factor;
        }

        public int Term()
        {
            int value = Factor();
            while (IsMulOp(lookahead))
            {
                if (lookahead == '*')
                {
                    Match('*');
                    value *= Factor();
                }
                else if (lookahead == '/')
                {
                    Match('/');
                    value /= Factor();
                }
            }
            return value;
        }

        public void Assignment()
        {
            char name = GetName();
            Match('=');
            SetValue(name, Expression());
        }

        public void NewLine()
        {
            if(lookahead == '\r')
            {
                GetChar();
                if(lookahead == '\n')
                {
                    GetChar();
                }
            }
        }

        public void Input()
        {
            Match('?');
            SetValue(GetName(), GetNumber());
        }

        public void Output()
        {
            Match('!');
            Console.WriteLine(GetValue(GetName()));
        }

        public void Run()
        {
            do
            {
                
                if (lookahead == '?') Input();
                else if (lookahead == '!') Output();
                else Assignment();
                NewLine();
            } while (lookahead != ';');
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
