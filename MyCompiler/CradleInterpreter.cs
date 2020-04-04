using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyCompiler
{
    class CradleInterpreter
    {
        private static readonly char[] addOperands = new[] { '+', '-' };
        private static readonly char[] multOperands = new[] { '*', '/' };
        private static readonly char[] whitespaces = new[] { ' ', '\t' };
        private readonly Dictionary<string, int> variables = new Dictionary<string, int>();
        private char lookahead;

        private int GetValue(string var)
        {
            if (!variables.ContainsKey(var))
            {
                variables.Add(var, 0);
            }
            return variables[var];
        }

        private void SetValue(string var, int value)
        {
            variables[var] = value;
        }


        public bool IsAddOp(char op)
        {
            return addOperands.Contains(op);
        }

        public bool IsMulOp(char op)
        {
            return multOperands.Contains(op);
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


        public string GetName()
        {
            StringBuilder name = new StringBuilder();
            if (!char.IsLetter(lookahead))
            {
                Expected("Name");
            }
            while (char.IsLetterOrDigit(lookahead))
            {
                name.Append(char.ToUpper(lookahead));
                GetChar();
            }
            SkipWhite();
            return name.ToString();
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
            SkipWhite();
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
            while (whitespaces.Contains(lookahead))
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
            string name = GetName();
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
        public void Output()
        {
            Match('!');
            Console.WriteLine(GetValue(GetName()));
        }

        public void Run()
        {
            do
            {
                if (lookahead == '!') Output();
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
