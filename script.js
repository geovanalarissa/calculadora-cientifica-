function calculate() {
  const raw = expr.trim();
  if (!raw) return;
  try {
    const result = evaluate(raw);
    const resStr = fmt(result);
    const prevExpr = raw;
    lastResult = result;
    renderResult(resStr, prevExpr, false);
    document.getElementById('dHist').textContent =
      history.length ? history[0].expr + ' = ' + history[0].result : '';
    addToHist({ expr: prevExpr, result: resStr });
    expr = resStr;
    justCalc = true;
  } catch(e) {
    renderResult(e.message || 'Erro', raw, true);
    justCalc = false;
  }
}

function evaluate(raw) {
  const src = raw
    .replace(/,/g, '.')
    .replace(/\s/g, '')
    .toLowerCase()
    .replace(/ans/g, lastResult)
    .replace(/\bpi\b/g, Math.PI)
    .replace(/\be(?![a-z])/g, Math.E);

  let pos = 0;

  function peek()  { return src[pos]; }
  function eat(c)  { if (src[pos] !== c) throw new Error(`Esperado '${c}'`); pos++; }

  function parseExpr()  { return parseAdd(); }

  function parseAdd() {
    let v = parseMul();
    while (pos < src.length && (peek() === '+' || peek() === '-')) {
      const op = src[pos++];
      const r = parseMul();
      v = op === '+' ? v + r : v - r;
    }
    return v;
  }

  function parseMul() {
    let v = parsePow();
    while (pos < src.length && '*/%%'.includes(peek())) {
      const op = src[pos++];
      const r = parsePow();
      if (op === '/' && r === 0) throw new Error('Divisão por zero');
      v = op === '*' ? v * r : op === '/' ? v / r : v % r;
    }
    return v;
  }

  function parsePow() {
    let v = parseUnary();
    if (pos < src.length && peek() === '^') { pos++; return Math.pow(v, parseUnary()); }
    return v;
  }

  function parseUnary() {
    if (pos < src.length && peek() === '-') { pos++; return -parseFn(); }
    if (pos < src.length && peek() === '+') { pos++; }
    return parseFn();
  }

  const FUNS = ['asin','acos','atan','sinh','cosh','tanh',
                'sin','cos','tan','sqrt','cbrt','log2',
                'log','ln','exp','abs','ceil','floor','round'];

  function applyFn(fn, a) {
    const r = isRad ? a : a * Math.PI / 180;
    switch(fn) {
      case 'sin':   return Math.sin(r);
      case 'cos':   return Math.cos(r);
      case 'tan':   return Math.tan(r);
      case 'asin':  return isRad ? Math.asin(a) : Math.asin(a)*180/Math.PI;
      case 'acos':  return isRad ? Math.acos(a) : Math.acos(a)*180/Math.PI;
      case 'atan':  return isRad ? Math.atan(a) : Math.atan(a)*180/Math.PI;
      case 'sinh':  return Math.sinh(a);
      case 'cosh':  return Math.cosh(a);
      case 'tanh':  return Math.tanh(a);
      case 'sqrt':  if (a<0) throw new Error('√ de negativo'); return Math.sqrt(a);
      case 'cbrt':  return Math.cbrt(a);
      case 'log':   if (a<=0) throw new Error('log de número ≤ 0'); return Math.log10(a);
      case 'ln':    if (a<=0) throw new Error('ln de número ≤ 0'); return Math.log(a);
      case 'log2':  if (a<=0) throw new Error('log2 de número ≤ 0'); return Math.log2(a);
      case 'exp':   return Math.exp(a);
      case 'abs':   return Math.abs(a);
      case 'ceil':  return Math.ceil(a);
      case 'floor': return Math.floor(a);
      case 'round': return Math.round(a);
    }
  }

  function parseFn() {
    for (const fn of FUNS) {
      if (src.startsWith(fn, pos)) {
        pos += fn.length;
        eat('(');
        const a = parseExpr();
        eat(')');
        return applyFn(fn, a);
      }
    }
    return parseAtom();
  }

  function parseAtom() {
    if (pos >= src.length) throw new Error('Expressão incompleta');
    if (peek() === '(') {
      pos++;
      const v = parseExpr();
      eat(')');
      return v;
    }
    
    const start = pos;
    if (pos < src.length && (peek()==='-'||peek()==='+')) pos++;
    while (pos < src.length && /[\d.]/.test(peek())) pos++;
    if (pos < src.length && peek()==='e') {
      pos++;
      if (pos < src.length && /[+-]/.test(peek())) pos++;
      while (pos < src.length && /\d/.test(peek())) pos++;
    }
    const tok = src.slice(start, pos);
    if (!tok || tok==='-' || tok==='+') throw new Error(`Número inválido na posição ${start}`);
    return parseFloat(tok);
  }

  const result = parseExpr();
  if (pos < src.length) throw new Error(`Caractere inesperado '${src[pos]}'`);
  return result;
}

document.addEventListener('keydown', e => {
  if (e.ctrlKey || e.altKey || e.metaKey) return;
  if (e.key === 'Enter')     { calculate(); return; }
  if (e.key === 'Backspace') { del(); return; }
  if (e.key === 'Escape')    { clearAll(); return; }
  if (/^[0-9+\-*\/().^%]$/.test(e.key)) { ins(e.key); return; }
  if (e.key === ',')         { ins('.'); return; }
});
