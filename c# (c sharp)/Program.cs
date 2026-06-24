using System;
using System.Collections.Generic;

namespace CalculadoraCientifica
{
    class Calculadora
    {
        private double _valor = 0;
        private double _memoria = 0;
        private string _expressaoAtual = "";
        private List<string> _historico = new List<string>();
        private bool _novaNumeracao = true;
        private double _ultimoResultado = 0;
        private bool _modoRad = true; // RAD ou DEG

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var calc = new Calculadora();
            calc.Executar();
        }

        public void Executar()
        {
            Console.CursorVisible = false;
            Console.Title = "Calculadora Científica";

            while (true)
            {
                DesenharInterface();
                var entrada = LerEntrada();
                if (entrada.ToLower() == "sair" || entrada.ToLower() == "exit")
                    break;
                Processar(entrada);
            }

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n  Obrigado por usar a Calculadora Científica!");
            Console.ResetColor();
        }

        private void DesenharInterface()
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();

            Cor(ConsoleColor.DarkCyan);
            Console.WriteLine("  ╔══════════════════════════════════════════════╗");
            Console.Write("  ║");
            Cor(ConsoleColor.Cyan);
            Console.Write("       ◈  CALCULADORA CIENTÍFICA  ◈             ");
            Cor(ConsoleColor.DarkCyan);
            Console.WriteLine("║");
            Console.Write("  ║");
            Cor(ConsoleColor.DarkGray);
            string modo = _modoRad ? " [RAD]" : " [DEG]";
            string modoMem = _memoria != 0 ? "  [M]" : "     ";
            Console.Write($"  v1.0  C# Console Edition  {modo}{modoMem}          ");
            Cor(ConsoleColor.DarkCyan);
            Console.WriteLine("║");
            Console.WriteLine("  ╠══════════════════════════════════════════════╣");

            Console.Write("  ║");
            Cor(ConsoleColor.DarkGray);
            string exprDisplay = _expressaoAtual.Length > 42
                ? "…" + _expressaoAtual.Substring(_expressaoAtual.Length - 41)
                : _expressaoAtual;
            Console.Write($" {exprDisplay,-44}");
            Cor(ConsoleColor.DarkCyan);
            Console.WriteLine("║");

            Console.Write("  ║");
            Cor(ConsoleColor.Cyan);
            string valorStr = FormatarValor(_valor);
            Console.Write($" {valorStr,44} ");
            Cor(ConsoleColor.DarkCyan);
            Console.WriteLine("║");

            Console.WriteLine("  ╠══════════════════════════════════════════════╣");

            Console.Write("  ║");
            Cor(ConsoleColor.DarkGray);
            Console.Write("  Histórico:                                   ");
            Cor(ConsoleColor.DarkCyan);
            Console.WriteLine("║");

            for (int i = Math.Max(0, _historico.Count - 3); i < _historico.Count; i++)
            {
                Console.Write("  ║");
                Cor(ConsoleColor.DarkGray);
                string h = _historico[i].Length > 44 ? _historico[i].Substring(0, 41) + "..." : _historico[i];
                Console.Write($"   {h,-43}");
                Cor(ConsoleColor.DarkCyan);
                Console.WriteLine("║");
            }

            for (int i = _historico.Count; i < 3; i++)
            {
                Console.Write("  ║");
                Console.Write(new string(' ', 46));
                Cor(ConsoleColor.DarkCyan);
                Console.WriteLine("║");
            }

            Console.WriteLine("  ╠══════════════════════════════════════════════╣");

            DesenharTabela();

            Cor(ConsoleColor.DarkCyan);
            Console.WriteLine("  ╚══════════════════════════════════════════════╝");
            Console.ResetColor();

            Console.WriteLine();
            Cor(ConsoleColor.DarkGray);
            Console.Write("  Dica: use vírgula ou ponto como decimal. Ex: ");
            Cor(ConsoleColor.Gray);
            Console.WriteLine("sin(30)  2^8  sqrt(16)");
            Console.WriteLine();
            Cor(ConsoleColor.Cyan);
            Console.Write("  ► ");
            Console.ResetColor();
            Console.CursorVisible = true;
        }

        private void DesenharTabela()
        {
            var lines = new string[][]
            {
                new[]{ "Operações Básicas", "+  -  * /  ^  %", "Funções Trigon.", "sin  cos  tan" },
                new[]{ "Trig. Inversa",     "asin acos atan",   "Logaritmos",      "log  ln  log2" },
                new[]{ "Raízes/Pot.",        "sqrt  cbrt  exp",  "Constantes",      "pi  e" },
                new[]{ "Outros",            "abs  ceil  floor", "Modo Ang.",        "rad  deg" },
                new[]{ "Memória",           "ms  mr  mc  m+",  "Atalhos",          "ans  his  limpar" },
            };

            foreach (var linha in lines)
            {
                Console.Write("  ║ ");
                Cor(ConsoleColor.DarkYellow);
                Console.Write($"{linha[0],-16}");
                Cor(ConsoleColor.Gray);
                Console.Write($"{linha[1],-18}");
                Cor(ConsoleColor.DarkYellow);
                Console.Write($"{linha[2],-15}");
                Cor(ConsoleColor.Gray);
                Console.Write($"{linha[3],-12}");
                Cor(ConsoleColor.DarkCyan);
                Console.WriteLine(" ║");
            }

            Cor(ConsoleColor.DarkCyan);
            Console.WriteLine("  ╠══════════════════════════════════════════════╣");
        }

        private string LerEntrada()
        {
            Console.CursorVisible = true;
            string entrada = Console.ReadLine() ?? "";
            Console.CursorVisible = false;
            return entrada.Trim();
        }

        private void Processar(string entrada)
        {
            if (string.IsNullOrWhiteSpace(entrada)) return;

            entrada = entrada.Replace(",", ".").ToLower();

            try
            {
                switch (entrada)
                {
                    case "limpar":
                    case "c":
                        _valor = 0;
                        _expressaoAtual = "";
                        _novaNumeracao = true;
                        return;

                    case "his":
                        _historico.Clear();
                        return;

                    case "rad":
                        _modoRad = true;
                        return;

                    case "deg":
                        _modoRad = false;
                        return;

                    case "ms":
                        _memoria = _valor;
                        AdicionarHistorico($"M← {FormatarValor(_valor)}");
                        return;

                    case "mr":
                        _expressaoAtual = $"mr = {FormatarValor(_memoria)}";
                        _valor = _memoria;
                        return;

                    case "mc":
                        _memoria = 0;
                        AdicionarHistorico("Memória limpa");
                        return;

                    case "m+":
                        _memoria += _valor;
                        AdicionarHistorico($"M+ {FormatarValor(_valor)} → M={FormatarValor(_memoria)}");
                        return;

                    case "pi":
                        _valor = Math.PI;
                        _expressaoAtual = "π";
                        return;

                    case "e":
                        _valor = Math.E;
                        _expressaoAtual = "e";
                        return;

                    case "ans":
                        _valor = _ultimoResultado;
                        _expressaoAtual = "ans";
                        return;
                }

                double resultado = Avaliar(entrada);
                _ultimoResultado = resultado;
                AdicionarHistorico($"{entrada} = {FormatarValor(resultado)}");
                _valor = resultado;
                _expressaoAtual = $"{entrada} =";
            }
            catch (Exception ex)
            {
                _expressaoAtual = $"Erro: {ex.Message}";
                _valor = double.NaN;
            }
        }

        private int _pos;
        private string _expr = "";

        private double Avaliar(string expr)
        {
            _expr = expr.Replace(" ", "").Replace("ans", _ultimoResultado.ToString("R"));
            _pos = 0;
            double resultado = ParseExpr();
            if (_pos < _expr.Length)
                throw new Exception($"Caractere inesperado '{_expr[_pos]}'");
            return resultado;
        }

        private double ParseExpr() => ParseAdicao();

        private double ParseAdicao()
        {
            double esq = ParseMultiplicacao();
            while (_pos < _expr.Length && (_expr[_pos] == '+' || _expr[_pos] == '-'))
            {
                char op = _expr[_pos++];
                double dir = ParseMultiplicacao();
                esq = op == '+' ? esq + dir : esq - dir;
            }
            return esq;
        }

        private double ParseMultiplicacao()
        {
            double esq = ParsePotencia();
            while (_pos < _expr.Length && (_expr[_pos] == '*' || _expr[_pos] == '/' || _expr[_pos] == '%'))
            {
                char op = _expr[_pos++];
                double dir = ParsePotencia();
                esq = op == '*' ? esq * dir :
                      op == '/' ? (dir == 0 ? throw new DivideByZeroException("Divisão por zero") : esq / dir) :
                      esq % dir;
            }
            return esq;
        }

        private double ParsePotencia()
        {
            double base_ = ParseUnario();
            if (_pos < _expr.Length && _expr[_pos] == '^')
            {
                _pos++;
                double exp = ParseUnario();
                return Math.Pow(base_, exp);
            }
            return base_;
        }

        private double ParseUnario()
        {
            if (_pos < _expr.Length && _expr[_pos] == '-')
            {
                _pos++;
                return -ParseFuncao();
            }
            if (_pos < _expr.Length && _expr[_pos] == '+')
            {
                _pos++;
            }
            return ParseFuncao();
        }

        private double ParseFuncao()
        {
            if (_pos >= _expr.Length) throw new Exception("Expressão incompleta");

            string[] funcs = { "asin", "acos", "atan", "sinh", "cosh", "tanh",
                                "sin", "cos", "tan", "sqrt", "cbrt", "log2",
                                "log", "ln", "exp", "abs", "ceil", "floor", "round" };

            foreach (var func in funcs)
            {
                if (_expr.Length >= _pos + func.Length &&
                    _expr.Substring(_pos, func.Length) == func)
                {
                    _pos += func.Length;
                    if (_pos < _expr.Length && _expr[_pos] == '(')
                    {
                        _pos++;
                        double arg = ParseExpr();
                        if (_pos >= _expr.Length || _expr[_pos] != ')')
                            throw new Exception($"Falta ')' em {func}(...)");
                        _pos++;
                        return AplicarFuncao(func, arg);
                    }
                    throw new Exception($"Esperado '(' após {func}");
                }
            }

            return ParseAtomo();
        }

        private double AplicarFuncao(string func, double arg)
        {
            double rad = _modoRad ? arg : arg * Math.PI / 180;
            return func switch
            {
                "sin"   => Math.Sin(rad),
                "cos"   => Math.Cos(rad),
                "tan"   => Math.Tan(rad),
                "asin"  => _modoRad ? Math.Asin(arg) : Math.Asin(arg) * 180 / Math.PI,
                "acos"  => _modoRad ? Math.Acos(arg) : Math.Acos(arg) * 180 / Math.PI,
                "atan"  => _modoRad ? Math.Atan(arg) : Math.Atan(arg) * 180 / Math.PI,
                "sinh"  => Math.Sinh(arg),
                "cosh"  => Math.Cosh(arg),
                "tanh"  => Math.Tanh(arg),
                "sqrt"  => arg < 0 ? throw new Exception("√ de número negativo") : Math.Sqrt(arg),
                "cbrt"  => Math.Cbrt(arg),
                "log"   => arg <= 0 ? throw new Exception("log de número ≤ 0") : Math.Log10(arg),
                "ln"    => arg <= 0 ? throw new Exception("ln de número ≤ 0") : Math.Log(arg),
                "log2"  => arg <= 0 ? throw new Exception("log2 de número ≤ 0") : Math.Log2(arg),
                "exp"   => Math.Exp(arg),
                "abs"   => Math.Abs(arg),
                "ceil"  => Math.Ceiling(arg),
                "floor" => Math.Floor(arg),
                "round" => Math.Round(arg),
                _       => throw new Exception($"Função desconhecida: {func}")
            };
        }

        private double ParseAtomo()
        {
            if (_expr[_pos] == '(')
            {
                _pos++;
                double val = ParseExpr();
                if (_pos >= _expr.Length || _expr[_pos] != ')')
                    throw new Exception("Falta ')'");
                _pos++;
                return val;
            }

            if (_expr.Length >= _pos + 2 && _expr.Substring(_pos, 2) == "pi")
            {
                _pos += 2;
                return Math.PI;
            }
            if (_expr[_pos] == 'e' && (_pos + 1 >= _expr.Length || !char.IsLetterOrDigit(_expr[_pos + 1])))
            {
                _pos++;
                return Math.E;
            }

            int inicio = _pos;
            // CORRIGIDO: Removido o avanço de sinal redundante daqui (+/-) que quebrava o parser de tokens
            while (_pos < _expr.Length && (char.IsDigit(_expr[_pos]) || _expr[_pos] == '.')) _pos++;
            if (_pos < _expr.Length && _expr[_pos] == 'e')
            {
                _pos++;
                if (_pos < _expr.Length && (_expr[_pos] == '+' || _expr[_pos] == '-')) _pos++;
                while (_pos < _expr.Length && char.IsDigit(_expr[_pos])) _pos++;
            }

            string numStr = _expr.Substring(inicio, _pos - inicio);
            if (string.IsNullOrEmpty(numStr))
                throw new Exception($"Número inválido na posição {inicio}");

            return double.Parse(numStr, System.Globalization.CultureInfo.InvariantCulture);
        }

        private string FormatarValor(double v)
        {
            if (double.IsNaN(v)) return "NaN";
            if (double.IsPositiveInfinity(v)) return "+∞";
            if (double.IsNegativeInfinity(v)) return "-∞";

            if (Math.Abs(v) >= 1e15 || (Math.Abs(v) < 1e-10 && v != 0))
                return v.ToString("E10");

            return v.ToString("G14");
        }

        private void AdicionarHistorico(string linha)
        {
            _historico.Add(linha);
            if (_historico.Count > 20) _historico.RemoveAt(0);
        }

        private static void Cor(ConsoleColor c) => Console.ForegroundColor = c;
    }
}