using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace CalcWPF
{
    public partial class MainWindow : Window
    {
        private string _expr = "";
        private double _ultimoResultado = 0;
        private double _memoria = 0;
        private bool _modoRad = true;
        private bool _acabouDeCalcular = false;
        private readonly List<string> _historico = new();

        public MainWindow()
        {
            InitializeComponent();
            KeyDown += JanelaPrincipal_KeyDown;
        }

        private void BtnNum_Click(object sender, RoutedEventArgs e)
        {
            var btn = (System.Windows.Controls.Button)sender;
            string digito = btn.Tag.ToString()!;

            if (_acabouDeCalcular && (char.IsDigit(digito[0]) || digito == "."))
            {
                _expr = "";
                _acabouDeCalcular = false;
            }

            _expr += digito;
            AtualizarDisplay();
        }

        private void BtnOp_Click(object sender, RoutedEventArgs e)
        {
            var btn = (System.Windows.Controls.Button)sender;
            _expr += btn.Tag.ToString();
            _acabouDeCalcular = false;
            AtualizarDisplay();
        }

        private void BtnFn_Click(object sender, RoutedEventArgs e)
        {
            var btn = (System.Windows.Controls.Button)sender;
            _expr += btn.Tag.ToString(); 
            _acabouDeCalcular = false;
            AtualizarDisplay();
        }

        private void BtnConst_Click(object sender, RoutedEventArgs e)
        {
            var btn = (System.Windows.Controls.Button)sender;
            string tag = btn.Tag.ToString()!;

            if (tag == "ans")
                _expr += _ultimoResultado.ToString("R");
            else
                _expr += tag;   

            _acabouDeCalcular = false;
            AtualizarDisplay();
        }

        private void BtnParen_Click(object sender, RoutedEventArgs e)
        {
            int abre  = ContarChar(_expr, '(');
            int fecha = ContarChar(_expr, ')');
            _expr += (abre > fecha) ? ")" : "(";
            _acabouDeCalcular = false;
            AtualizarDisplay();
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            _expr = "";
            TbResultado.Text  = "0";
            TbExpr.Text       = "";
            TbHistorico.Text  = "";
            _acabouDeCalcular = false;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (_expr.Length > 0)
                _expr = _expr[..^1];
            AtualizarDisplay();
        }

        private void BtnIgual_Click(object sender, RoutedEventArgs e) => Calcular();

        private void BtnModo_Click(object sender, RoutedEventArgs e)
        {
            _modoRad = !_modoRad;
            TbModo.Text     = _modoRad ? "RAD" : "DEG";
            TbBtnModo.Text  = _modoRad ? "DEG" : "RAD";   
        }

        private void BtnMem_Click(object sender, RoutedEventArgs e)
        {
            var btn = (System.Windows.Controls.Button)sender;
            switch (btn.Tag.ToString())
            {
                case "ms": _memoria = _ultimoResultado; BadgeMemoria.Visibility = Visibility.Visible;  break;
                case "mr": _expr += _memoria.ToString("R"); AtualizarDisplay(); break;
                case "mc": _memoria = 0;                    BadgeMemoria.Visibility = Visibility.Collapsed; break;
                case "m+": _memoria += _ultimoResultado;    BadgeMemoria.Visibility = Visibility.Visible;  break;
            }
        }

        private void JanelaPrincipal_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:      Calcular(); break;
                case Key.Back:       BtnBack_Click(sender, new RoutedEventArgs()); break;
                case Key.Escape:     BtnClear_Click(sender, new RoutedEventArgs()); break;
                case Key.D0: case Key.NumPad0: _expr += "0"; AtualizarDisplay(); break;
                case Key.D1: case Key.NumPad1: _expr += "1"; AtualizarDisplay(); break;
                case Key.D2: case Key.NumPad2: _expr += "2"; AtualizarDisplay(); break;
                case Key.D3: case Key.NumPad3: _expr += "3"; AtualizarDisplay(); break;
                case Key.D4: case Key.NumPad4: _expr += "4"; AtualizarDisplay(); break;
                case Key.D5: case Key.NumPad5: _expr += "5"; AtualizarDisplay(); break;
                case Key.D6: case Key.NumPad6: _expr += "6"; AtualizarDisplay(); break;
                case Key.D7: case Key.NumPad7: _expr += "7"; AtualizarDisplay(); break;
                case Key.D8: case Key.NumPad8: _expr += "8"; AtualizarDisplay(); break;
                case Key.D9: case Key.NumPad9: _expr += "9"; AtualizarDisplay(); break;
                case Key.OemPlus:              _expr += "+"; AtualizarDisplay(); break;
                case Key.OemMinus:             _expr += "-"; AtualizarDisplay(); break;
                case Key.Multiply:             _expr += "*"; AtualizarDisplay(); break;
                case Key.Divide:               _expr += "/"; AtualizarDisplay(); break;
                case Key.OemPeriod:
                case Key.Decimal:              _expr += "."; AtualizarDisplay(); break;
            }
            e.Handled = true;
        }

        private void Calcular()
        {
            if (string.IsNullOrWhiteSpace(_expr)) return;

            try
            {
                var avaliador = new Avaliador(_expr, _modoRad);
                double resultado = avaliador.Avaliar();

                string resStr = FormatarValor(resultado);
                _historico.Add($"{_expr} = {resStr}");
                if (_historico.Count > 20) _historico.RemoveAt(0);

                _ultimoResultado  = resultado;
                TbExpr.Text       = _expr + " =";
                TbResultado.Text  = resStr;
                TbHistorico.Text  = _historico.Count > 1
                                     ? _historico[^2]
                                     : "";
                _expr             = resStr;
                _acabouDeCalcular = true;
            }
            catch (Exception ex)
            {
                TbExpr.Text      = _expr;
                TbResultado.Text = "Erro: " + ex.Message;
                _acabouDeCalcular = false;
            }
        }

        private void AtualizarDisplay()
        {
            TbResultado.Text = string.IsNullOrEmpty(_expr) ? "0" : _expr;
            TbExpr.Text      = "";
        }

        private static string FormatarValor(double v)
        {
            if (double.IsNaN(v))              return "Inválido";
            if (double.IsPositiveInfinity(v)) return "+∞";
            if (double.IsNegativeInfinity(v)) return "−∞";
            if (Math.Abs(v) >= 1e15 || (Math.Abs(v) < 1e-10 && v != 0))
                return v.ToString("E10");
            return v.ToString("G14");
        }

        private static int ContarChar(string s, char c)
        {
            int n = 0;
            foreach (char x in s) if (x == c) n++;
            return n;
        }
    }

    internal class Avaliador
    {
        private readonly string _expr;
        private readonly bool   _modoRad;
        private int _pos;

        public Avaliador(string expr, bool modoRad)
        {
            _expr   = expr.Replace(",", ".").Replace(" ", "").ToLower();
            _modoRad = modoRad;
            _pos    = 0;
        }

        public double Avaliar()
        {
            double resultado = ParseExpr();
            if (_pos < _expr.Length)
                throw new Exception($"Caractere inesperado '{_expr[_pos]}'");
            return resultado;
        }

        private double ParseExpr() => ParseAdicao();

        private double ParseAdicao()
        {
            double esq = ParseMult();
            while (_pos < _expr.Length && (_expr[_pos] == '+' || _expr[_pos] == '-'))
            {
                char op = _expr[_pos++];
                double dir = ParseMult();
                esq = op == '+' ? esq + dir : esq - dir;
            }
            return esq;
        }

        private double ParseMult()
        {
            double esq = ParsePot();
            while (_pos < _expr.Length && (_expr[_pos] == '*' || _expr[_pos] == '/' || _expr[_pos] == '%'))
            {
                char op = _expr[_pos++];
                double dir = ParsePot();
                esq = op == '*' ? esq * dir
                    : op == '/' ? (dir == 0 ? throw new DivideByZeroException("Divisão por zero") : esq / dir)
                    : esq % dir;
            }
            return esq;
        }

        private double ParsePot()
        {
            double b = ParseUnario();
            if (_pos < _expr.Length && _expr[_pos] == '^')
            { _pos++; return Math.Pow(b, ParseUnario()); }
            return b;
        }

        private double ParseUnario()
        {
            if (_pos < _expr.Length && _expr[_pos] == '-') { _pos++; return -ParseFuncao(); }
            if (_pos < _expr.Length && _expr[_pos] == '+') { _pos++; }
            return ParseFuncao();
        }

        private static readonly string[] Funcs =
        {
            "asin","acos","atan","sinh","cosh","tanh",
            "sin","cos","tan","sqrt","cbrt","log2",
            "log","ln","exp","abs","ceil","floor","round"
        };

        private double ParseFuncao()
        {
            if (_pos >= _expr.Length) throw new Exception("Expressão incompleta");

            foreach (var fn in Funcs)
            {
                if (_expr.Length >= _pos + fn.Length &&
                    _expr.Substring(_pos, fn.Length) == fn)
                {
                    _pos += fn.Length;
                    if (_pos >= _expr.Length || _expr[_pos] != '(')
                        throw new Exception($"Esperado '(' após {fn}");
                    _pos++;
                    double arg = ParseExpr();
                    if (_pos >= _expr.Length || _expr[_pos] != ')')
                        throw new Exception($"Falta ')' em {fn}(...)");
                    _pos++;
                    return AplicarFuncao(fn, arg);
                }
            }
            return ParseAtomo();
        }

        private double AplicarFuncao(string fn, double arg)
        {
            double rad = _modoRad ? arg : arg * Math.PI / 180;
            return fn switch
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
                _       => throw new Exception($"Função desconhecida: {fn}")
            };
        }

        private double ParseAtomo()
        {
            if (_expr[_pos] == '(')
            {
                _pos++;
                double v = ParseExpr();
                if (_pos >= _expr.Length || _expr[_pos] != ')')
                    throw new Exception("Falta ')'");
                _pos++;
                return v;
            }

            if (_expr.Length >= _pos + 2 && _expr.Substring(_pos, 2) == "pi") { _pos += 2; return Math.PI; }
            if (_expr[_pos] == 'e' && (_pos + 1 >= _expr.Length || !char.IsLetterOrDigit(_expr[_pos + 1]))) { _pos++; return Math.E; }

            int inicio = _pos;
            while (_pos < _expr.Length && (char.IsDigit(_expr[_pos]) || _expr[_pos] == '.')) _pos++;
            if (_pos < _expr.Length && _expr[_pos] == 'e')
            {
                _pos++;
                if (_pos < _expr.Length && (_expr[_pos] == '+' || _expr[_pos] == '-')) _pos++;
                while (_pos < _expr.Length && char.IsDigit(_expr[_pos])) _pos++;
            }

            string numStr = _expr.Substring(inicio, _pos - inicio);
            if (string.IsNullOrEmpty(numStr)) throw new Exception($"Número inválido na posição {inicio}");
            return double.Parse(numStr, System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}