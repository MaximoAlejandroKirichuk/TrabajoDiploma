// Comprobaciones locales de contratos y del modelo de automatizacion; no abre un repositorio de EA.
const fs = require('fs');
const path = require('path');
const vm = require('vm');
const assert = require('assert');
const root = path.resolve(__dirname, '../..');
const source = fs.readFileSync(path.join(root, 'Scripts/EA/CUN07_DC_RegistrarPagoCuota.js'), 'utf8').replace(/^!INC[^\r\n]*/, '');
let nextId = 1;
const byId = {};
function collection(factory) {
    const entries = [];
    return {
        get Count() { return entries.length; },
        AddNew(name, type) { const value = factory(name, type); entries.push(value); return value; },
        GetAt(index) { return entries[index]; },
        Refresh() {}
    };
}
function item(name, type) {
    return { Name: name, Type: type, Update() { return true; } };
}
function element(name, type) {
    const e = item(name, type);
    e.ElementID = nextId++;
    e.Attributes = collection(item);
    e.Methods = collection((n, t) => Object.assign(item(n, t), { ReturnType: t, Parameters: collection(item) }));
    e.Connectors = collection((n, t) => Object.assign(item(n, t), { SupplierEnd: item('', ''), ClientEnd: item('', '') }));
    byId[e.ElementID] = e;
    return e;
}
const pkg = {
    Name: 'CUN07 "Registrar pago de cuota"', PackageID: 7,
    Elements: collection(element),
    Diagrams: collection((n, t) => Object.assign(item(n, t), { DiagramID: nextId++, DiagramObjects: collection(item) }))
};
const messages = [];
const context = {
    Repository: {
        GetTreeSelectedPackage() { return pkg; },
        GetElementByID(id) { return byId[id]; },
        RefreshModelView() {}, OpenDiagram() {}
    },
    Session: { Output(text) { messages.push(text); }, Prompt(text) { messages.push(text); } },
    promptOK: 0
};
vm.createContext(context);
vm.runInContext(source, context);
assert(messages.some(m => m.includes('Signature readback OK')), messages.join('\n'));
assert.equal(pkg.Elements.Count, context.TYPES.length);
assert.equal(pkg.Diagrams.Count, 1);
for (const t of context.TYPES) {
    assert(fs.existsSync(path.join(root, t.source)), `Missing source: ${t.source}`);
    assert(!/SessionManager|^Frm|Exporter/.test(t.name), `Excluded type: ${t.name}`);
    const code = fs.readFileSync(path.join(root, t.source), 'utf8');
    for (const opText of t.ops) {
        const op = context.operation(opText);
        assert.notEqual(op.name, t.name, 'Constructors must be omitted');
        const declarations = new RegExp('([\\w<>, ?]+?)\\s+' + op.name + '\\s*\\(([^)]*)\\)', 'g');
        let declaration;
        let matching = false;
        const normalize = text => text.replace(/\s+/g, '');
        while ((declaration = declarations.exec(code))) {
            const result = declaration[1].trim().split(/\b(?:public|private|internal|protected|static)\s+/).pop().trim();
            const args = context.parameters(declaration[2]).map(p => {
                const withoutDefault = p.replace(/\s*=.*$/, '').trim();
                return withoutDefault.substring(0, withoutDefault.lastIndexOf(' '));
            });
            if (normalize(result) === normalize(op.result) && args.length === op.params.length &&
                args.every((arg, i) => normalize(arg) === normalize(op.params[i].type))) matching = true;
        }
        assert(matching, `Code signature mismatch: ${t.name}.${opText}`);
    }
}
// Cada operacion de interfaz representada debe aparecer con la misma firma en su implementacion.
for (const r of context.LINKS.filter(r => r.kind === 'Realisation')) {
    const implementation = context.TYPES.find(t => t.name === r.from);
    const contract = context.TYPES.find(t => t.name === r.to);
    for (const op of contract.ops) assert(implementation.ops.includes(op), `${r.from} does not match ${r.to}: ${op}`);
}
const diagram = pkg.Diagrams.GetAt(0);
const bounds = [];
for (let i = 0; i < diagram.DiagramObjects.Count; i++) {
    const o = diagram.DiagramObjects.GetAt(i);
    const b = Object.fromEntries(o.Name.split(';').filter(Boolean).map(s => s.split('=').map(NumberOrString)));
    function NumberOrString(value, index) { return index ? Number(value) : value; }
    assert(b.r > b.l && b.b > b.t, 'Invalid diagram bounds');
    for (const previous of bounds) assert(b.r <= previous.l || previous.r <= b.l || b.b <= previous.t || previous.b <= b.t, 'Overlapping class boxes');
    bounds.push(b);
}
const oldCount = pkg.Elements.Count;
assert.throws(() => context.main(), /already created elements/);
assert.equal(pkg.Elements.Count, oldCount, 'Rerun modified existing content');
pkg.Name = 'DC';
assert.throws(() => context.main(), /root CUN07 package/);
console.log(`PASS: ${context.TYPES.length} source-backed types, interface signatures, saved members, layout, excluded types and rerun guards.`);
console.log('Live EA rendering/COM execution must be verified inside Enterprise Architect.');
