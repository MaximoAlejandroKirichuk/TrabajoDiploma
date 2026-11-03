// Source provenance, ES3 grammar and EA automation simulation. Does not open EA.
// node Scripts/Tests/CUN09.EaGenerator.Tests.js <path-to-acorn-module>
const fs = require('fs');
const path = require('path');
const vm = require('vm');
const assert = require('assert');
const root = path.resolve(__dirname, '../..');
const acorn = require(process.argv[2] || 'acorn');
const filename = path.join(root, 'Scripts/EA/cun09/CUN09_DC_DesdeCodigo.js');
const raw = fs.readFileSync(filename, 'utf8');
assert(raw.startsWith('!INC Local Scripts.EAConstants-JScript\n'));
const source = raw.replace(/^!INC[^\r\n]*/, '');
const ast = acorn.parse(source, { ecmaVersion: 3 });
let logicStarted = false;
for (const statement of ast.body) {
    if (statement.type === 'FunctionDeclaration') logicStarted = true;
    if (statement.type === 'VariableDeclaration') assert(!logicStarted, 'Top-level data must precede logic');
}
// ES3 accepts trailing commas in some contexts; classic EA JScript does not.
assert(!/,\s*[}\]]/.test(source.replace(/\/\/[^\n]*|\/\*[\s\S]*?\*\//g, '')), 'Trailing comma');

let nextId = 1;
const elementsById = {};
const messages = [];
function collection(factory) {
    const entries = [];
    return {
        get Count() { return entries.length; },
        AddNew(name, type) { const result = factory(name, type); entries.push(result); return result; },
        GetAt(index) { return entries[index]; },
        Refresh() {}
    };
}
function item(name, type) {
    return { Name: name, Type: type, Update() { return true; }, GetLastError() { return ''; } };
}
function element(name, type, parentId) {
    const e = item(name, type);
    e.ElementID = nextId++;
    e.ParentID = parentId || 0;
    e.Elements = collection((n, t) => element(n, t, e.ElementID));
    e.Attributes = collection(item);
    e.Methods = collection((n, t) => Object.assign(item(n, t), { ReturnType: t, Parameters: collection(item) }));
    e.Connectors = collection((n, t) => Object.assign(item(n, t), { SupplierEnd: item('', ''), ClientEnd: item('', '') }));
    elementsById[e.ElementID] = e;
    return e;
}
function diagramObject(bounds, type) {
    const o = item(bounds, type);
    const coords = {};
    for (const pair of bounds.split(';').filter(Boolean)) {
        const [key, value] = pair.split('='); coords[key] = Number(value);
    }
    // EA stores top/bottom as negative Y after AddNew's positive coordinates.
    Object.assign(o, { left: coords.l, right: coords.r, top: -coords.t, bottom: -coords.b });
    return o;
}
function pkg(name) {
    return Object.assign(item(name, 'Package'), {
        PackageID: nextId++,
        Packages: collection(n => pkg(n)),
        Elements: collection(element),
        Diagrams: collection((n, t) => Object.assign(item(n, t), { DiagramID: nextId++, DiagramObjects: collection(diagramObject) }))
    });
}
const parent = pkg('CUN09');
const existing = parent.Packages.AddNew('CUN09 - DC verificado', 'Package');
existing.Elements.AddNew('User-owned class', 'Class');
let selected = parent;
const context = {
    Repository: {
        GetTreeSelectedPackage() { return selected; },
        GetElementByID(id) { return elementsById[id]; },
        RefreshModelView() {}, OpenDiagram() {}
    },
    Session: { Output(text) { messages.push(text); }, Prompt(text) { messages.push(text); } },
    promptOK: 0
};
vm.createContext(context);
vm.runInContext(source, context, { filename });
assert(messages.some(m => m.startsWith('OK:')), messages.join('\n'));
const generated = parent.Packages.GetAt(1);
assert.equal(generated.Name, 'CUN09 - DC verificado (2)');
assert.equal(generated.Diagrams.Count, 1);
assert.equal(existing.Elements.Count, 1, 'Existing package modified');
assert.equal(existing.Elements.GetAt(0).Name, 'User-owned class');

// Mask C# comments and string contents while retaining offsets/braces outside them.
function maskCs(code) {
    return code.replace(/\/\*[\s\S]*?\*\/|\/\/[^\r\n]*|\$?@"(?:""|[^"])*"|\$?"(?:\\.|[^"\\])*"|'(?:\\.|[^'\\])*'/g,
        match => match.replace(/[^\r\n]/g, ' '));
}
function declarationBody(code, name) {
    const masked = maskCs(code);
    const match = new RegExp('\\b(class|interface|enum)\\s+' + name + '\\b').exec(masked);
    assert(match, `Missing declaration: ${name}`);
    const start = masked.indexOf('{', match.index);
    let depth = 1;
    let end = start + 1;
    while (depth && end < masked.length) {
        if (masked[end] === '{') depth++;
        if (masked[end] === '}') depth--;
        end++;
    }
    assert.equal(depth, 0);
    return { code: code.slice(start + 1, end - 1), masked: masked.slice(start + 1, end - 1), kind: match[1] };
}
const normalize = s => s.replace(/\s+/g, '');
const visibility = s => ({ public: 'Public', private: 'Private', internal: 'Package', protected: 'Protected' })[s];
const types = Array.from(context.TYPES);
const byName = Object.fromEntries(types.map(t => [t.name, t]));
let operationCount = 0;
let attributeCount = 0;
for (const type of types) {
    assert(!/^Frm|Exporter|Permiso|Rol_/.test(type.name));
    const code = fs.readFileSync(path.join(root, type.source), 'utf8');
    const body = declarationBody(code, type.name);
    if (type.kind === 'Interface') assert.equal(body.kind, 'interface');
    if (type.kind === 'Enumeration') assert.equal(body.kind, 'enum');
    for (const text of type.ops) {
        const op = context.operation(text);
        const expression = new RegExp('(?:^|\\n)[ \\t]*(?:(public|private|internal|protected)\\s+)?((?:(?:static|virtual|override|sealed|async)\\s+)*)([\\w<>, ?]+?)\\s+' + op.name + '\\s*\\(([^)]*)\\)', 'g');
        let candidate;
        let found = false;
        while ((candidate = expression.exec(body.masked))) {
            const params = Array.from(context.parameters(candidate[4])).map(p => {
                const parts = /^(.*?)\s+(\w+)(?:\s*=\s*(.*))?$/.exec(p.trim());
                return parts && { type: parts[1], name: parts[2], value: parts[3] };
            });
            const expectedVisibility = visibility(candidate[1]) || (body.kind === 'interface' ? 'Public' : 'Private');
            if (normalize(candidate[3]) !== normalize(op.type) || expectedVisibility !== op.visibility || /\bstatic\b/.test(candidate[2]) !== op.isStatic || params.length !== op.params.length) continue;
            found = params.every((p, i) => p && normalize(p.type) === normalize(op.params[i].type) && p.name === op.params[i].name && (p.value || undefined) === op.params[i].value);
            if (found) break;
        }
        assert(found, `Source signature mismatch: ${type.name}.${text}`);
        operationCount++;
    }
    for (const text of type.attrs) {
        const a = context.attribute(text);
        const expression = new RegExp('(?:^|\\n)[ \\t]*(?:(public|private|internal|protected)\\s+)?((?:(?:static|readonly|const)\\s+)*)([\\w<>, ?]+?)\\s+' + a.name + '\\s*(?=[{=;])', 'g');
        let match;
        let found;
        while ((match = expression.exec(body.masked))) {
            const access = visibility(match[1]) || (body.kind === 'interface' ? 'Public' : 'Private');
            if (normalize(match[3]) === normalize(a.type) && access === a.visibility) { found = match; break; }
        }
        assert(found, `Source attribute mismatch: ${type.name}.${text}`);
        assert.equal(/\bstatic\b|\bconst\b/.test(found[2]), /static|const/.test(a.flags), `${type.name}.${a.name} static`);
        assert.equal(/\bconst\b/.test(found[2]), /const/.test(a.flags), `${type.name}.${a.name} const`);
        if (/readonly/.test(a.flags) && !/property/.test(a.flags)) assert(/\breadonly\b/.test(found[2]));
        const tail = body.masked.slice(found.index + found[0].length);
        if (/property/.test(a.flags)) {
            assert(/^\s*(?:{|=>)/.test(tail), `${type.name}.${a.name} is not a property`);
            if (/private set/.test(a.flags)) assert(/^\s*{[^{}]*get;\s*private\s+set;/.test(tail));
            if (/readonly/.test(a.flags)) {
                const propertyEnd = tail.indexOf('}');
                assert(/^\s*=>/.test(tail) || !/\bset\b/.test(tail.slice(0, propertyEnd + 1)), `${type.name}.${a.name} has setter`);
            }
        }
        if (a.value !== undefined) {
            const originalTail = body.code.slice(found.index + found[0].length);
            assert(originalTail.trim().startsWith('= ' + a.value), `Default mismatch: ${type.name}.${a.name}`);
        }
        attributeCount++;
    }
    if (type.literals) {
        const values = {};
        let current = -1;
        for (const entry of body.masked.split(',')) {
            const m = /^\s*(\w+)\s*(?:=\s*(\d+))?\s*$/.exec(entry);
            if (!m) continue;
            current = m[2] ? Number(m[2]) : current + 1;
            values[m[1]] = current;
        }
        for (const literal of type.literals) {
            const [name, value] = literal.split('=').map(s => s.trim());
            assert.equal(values[name], Number(value), `Enum mismatch: ${literal}`);
        }
    }
}

for (const link of context.LINKS) {
    if (link[2] === 'Association') {
        const field = byName[link[0]].attrs.map(s => context.attribute(s)).find(a => a.name === link[3]);
        assert(field && new RegExp('\\b' + link[1] + '\\b').test(field.type), `Association has no source reference: ${link[0]}.${link[3]}`);
        if (link[4] === '0..*') assert(/List<|Dictionary</.test(field.type), 'Many end without collection');
    }
    if (link[2] !== 'Realisation') continue;
    const implementation = byName[link[0]];
    const contract = byName[link[1]];
    const sourceCode = fs.readFileSync(path.join(root, implementation.source), 'utf8');
    assert(new RegExp('class\\s+' + implementation.name + '\\s*:\\s*' + contract.name + '\\b').test(sourceCode));
    for (const op of contract.ops) assert(implementation.ops.includes(op), `Interface mismatch: ${link[0]} / ${op}`);
}
const instance = Object.values(elementsById).find(e => e.Name === 'EstadoRetencion');
assert.equal(elementsById[instance.ParentID].Name, 'KI68CertificadoBLL', 'Nested class flattened');
assert(!byName.IIntegridadDAL_83KI.ops.some(s => /ActualizarCertificado\(/.test(s)));
assert.equal(byName.ISessionManager_83KI.ops.length, 0);
assert.equal(byName.SessionManager_83KI.ops.length, 0);
const factory = fs.readFileSync(path.join(root, 'UI/Aplicacion/ServiceFactory_83KI.cs'), 'utf8');
assert(/session\.UsuarioActivo\.DNI,\s*integridad\.ActualizarCertificado/.test(factory), 'Integrity callback wiring changed');

// Fault injection proves validation catches malformed graphs and geometry.
const plan = context.layout();
const badPlan = plan.map(p => Object.assign({}, p));
badPlan[1] = Object.assign({}, badPlan[0]);
assert.throws(() => context.validateModel(badPlan), /superpuestas/);
const savedLinks = context.LINKS;
context.LINKS = savedLinks.filter(r => r[0] !== 'Usuario_83KI' && r[1] !== 'Usuario_83KI');
assert.throws(() => context.validateModel(plan), /desconectada/);
context.LINKS = savedLinks;
const connectors = Object.values(elementsById).reduce((count, e) => count + e.Connectors.Count, 0);
assert.equal(connectors, context.LINKS.length);
assert.equal(generated.Diagrams.GetAt(0).DiagramObjects.Count, types.length);
// Another execution creates a distinct new package; earlier model stays untouched.
const oldElements = Object.keys(elementsById).length;
context.main();
assert.equal(parent.Packages.GetAt(2).Name, 'CUN09 - DC verificado (3)');
assert.equal(Object.keys(elementsById).length - oldElements, types.length);
assert.equal(existing.Elements.Count, 1);
selected = generated;
assert.throws(() => context.main(), /paquete padre/);
selected = null;
assert.throws(() => context.main(), /Seleccionar un paquete/);
console.log(`PASS: ES3; ${types.length} source-backed types; ${operationCount} signatures; ${attributeCount} attributes; ${savedLinks.length} connectors.`);
console.log('PASS: enum values, nested ownership, interface contracts, callback wiring, saved geometry, connected graph, rerun isolation and destination guards.');
console.log('Live EA COM execution/font rendering is not simulated by these checks.');
