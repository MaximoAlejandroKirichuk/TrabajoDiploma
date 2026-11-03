!INC Local Scripts.EAConstants-JScript
// CUN09: proyeccion del codigo C#. Seleccionar el paquete de destino y ejecutar.
// No analiza C# desde EA: los datos siguientes son una instantanea verificada por
// Scripts/Tests/CUN09.EaGenerator.Tests.js contra los archivos fuente indicados.
// Sin UI, permisos, roles ni notas graficas. SessionManager solo expone consultas.
// DATOS: todos los metadatos del modelo se declaran antes de las funciones.
var PACKAGE_NAME = 'CUN09 - DC verificado';
var DIAGRAM_NAME = 'CUN09 - Generar certificado de finalizacion';
var GAP_X = 140, GAP_Y = 160;
var CURRENT_STAGE = 'Inicio', CREATED_PACKAGE = null;
var COLORS = {Class: 15003901, Interface: 16174046, Enumeration: 15003901};
var TYPES = [
 {name:'Usuario_83KI',source:'Service/Entidades/Usuario_83KI.cs',row:0,col:0,
  attrs:['+ DNI: int {property private set}','+ Nombre: string {property private set}','+ Apellido: string {property private set}','+ Email: string {property private set}','+ UserName: string {property readonly derived}'],ops:[]},
 {name:'ISessionManager_83KI',kind:'Interface',source:'Service/Interfaces/ISessionManager_83KI.cs',row:0,col:1,
  attrs:['+ UsuarioActivo: Usuario_83KI {property readonly}'],ops:[]},
 {name:'SessionManager_83KI',source:'Service/SessionManager_83KI.cs',row:0,col:2,
  attrs:['+ Instancia: SessionManager_83KI {property readonly static}','+ UsuarioActivo: Usuario_83KI {property private set}'],ops:[]},
 {name:'KI68CertificadoBLL',source:'BLL/Negocio/KI68CertificadoBLL.cs',row:1,col:1,
  attrs:['+ CargoResponsable: string = "Coordinadora Academica" {const}','- _dal: ICertificadoDAL_83KI {readonly}','- _session: ISessionManager_83KI {readonly}','- _audit: IBitacoraManager_83KI {readonly}','- _retenciones: Dictionary<int, EstadoRetencion> {readonly}','- _prefijosVistos: Dictionary<int, string> {readonly}'],
  ops:['+ ListarFinalizadas(): List<Comision_83KI>','+ Buscar(idComision: int): List<AlumnoCertificacion_83KI>','+ Seleccionar(id: int): AlumnoCertificacion_83KI','+ Retener(id: int, observaciones: string): ResultadoCertificacion_83KI','+ Generar(r: GenerarCertificadoRequest_83KI): ResultadoCertificacion_83KI','+ PrefijoFolio(a: AlumnoCertificacion_83KI): string {static}','+ ComponerFolio(a: AlumnoCertificacion_83KI, numero: string): string {static}','+ Validar(r: GenerarCertificadoRequest_83KI): void {static}','- ValidarNumero(texto: string): long {static}','- ValidarEvaluaciones(a: AlumnoCertificacion_83KI): void {static}','- Registrar(accion: string, a: AlumnoCertificacion_83KI, detalle: string): void'],
  detail:'Autorizar y recuperacion de integridad se omiten por alcance. Los controles siguen en el codigo.'},
 {name:'EstadoRetencion',visibility:'Private',owner:'KI68CertificadoBLL',source:'BLL/Negocio/KI68CertificadoBLL.cs',row:1,col:0,
  attrs:['+ MotivosVistos: string','+ Registrada: bool'],ops:[]},
 {name:'GenerarCertificadoRequest_83KI',source:'BE/DTOs/Certificaciones/Certificacion_83KI.cs',row:2,col:0,
  attrs:['+ IdInscripcion: int {property}','+ Firmante: string {property}','+ Cargo: string {property}','+ NumeroFolio: string {property}','+ Modalidad: string {property}','+ Email: string {property}','+ Mencion: string {property}','+ Observaciones: string {property}'],ops:[],
  detail:'Firmante y Cargo viajan desde UI pero Generar no los utiliza: firmante de sesion y CargoResponsable.'},
 {name:'ResultadoCertificacion_83KI',source:'BE/DTOs/Certificaciones/Certificacion_83KI.cs',row:2,col:1,
  attrs:['+ Alumno: AlumnoCertificacion_83KI {property}','+ Existente: bool {property}','+ Retenido: bool {property}','+ Reconfirmar: bool {property}','+ AuditoriaFallida: bool {property}'],ops:[]},
 {name:'AlumnoCertificacion_83KI',source:'BE/DTOs/Certificaciones/Certificacion_83KI.cs',row:2,col:2,
  attrs:['+ IdInscripcion: int {property}','+ DNI: string {property}','+ Nombre: string {property}','+ Apellido: string {property}','+ Email: string {property}','+ Curso: string {property}','+ CodigoComision: string {property}','+ FechaFinComision: DateTime {property}','+ CantidadNotas: int {property}','+ Evaluacion1: decimal {property}','+ Evaluacion2: decimal {property}','+ Promedio: decimal {property readonly derived}','+ Aprobado: bool {property readonly derived}','+ ObservacionesProfesor: string {property}','+ Balance: decimal {property}','+ Hoy: DateTime {property}','+ Cuotas: List<CuotaCertificacion_83KI> {property}','+ Certificado: CertificadoFinalizacion_83KI {property}','+ Motivos: string {property readonly derived}'],ops:[],
  detail:'Condicion no se proyecta: UI muestra la condicion a partir de Aprobado. Balance y Cuotas participan aunque el DSS no desarrolle deuda.'},
 {name:'CuotaCertificacion_83KI',source:'BE/DTOs/Certificaciones/Certificacion_83KI.cs',row:2,col:3,
  attrs:['+ Numero: int {property}','+ Balance: decimal {property}','+ Vencimiento: DateTime {property}'],ops:[]},
 {name:'CertificadoFinalizacion_83KI',source:'BE/Entidades/CertificadoFinalizacion_83KI.cs',row:3,col:2,
  attrs:['+ IdCertificado: int {property}','+ IdInscripcion: int {property}','+ Nombre: string {property}','+ Apellido: string {property}','+ Curso: string {property}','+ Promedio: decimal {property}','+ Firmante: string {property}','+ Cargo: string {property}','+ Folio: string {property}','+ Fecha: DateTime {property}','+ Modalidad: string {property}','+ Email: string {property}','+ Mencion: string {property}','+ Observaciones: string {property}'],ops:[]},
 {name:'Comision_83KI',source:'BE/Entidades/Comision_83KI.cs',row:3,col:0,
  attrs:['+ IdComision: int {property private set}','+ Codigo: string {property private set}'],ops:[],detail:'Propiedades consumidas por el selector de CUN09. No se muestran operaciones de otros casos de uso.'},
 {name:'ICertificadoDAL_83KI',kind:'Interface',source:'Service/Interfaces/ICertificadoDAL_83KI.cs',row:3,col:1,
  attrs:[],ops:['+ ListarFinalizadas(): List<Comision_83KI>','+ Buscar(idComision: int): List<AlumnoCertificacion_83KI>','+ RevalidarEnTransaccion(idInscripcion: int, resolver: Func<AlumnoCertificacion_83KI, CertificadoFinalizacion_83KI>): AlumnoCertificacion_83KI']},
 {name:'CertificadoDAL_83KI',source:'DAL/CertificadoDAL_83KI.cs',row:4,col:1,
  attrs:['- _acceso: AccesoDAL_83KI {readonly}','- _dni: Func<int> {readonly}','- _integridad: Action<int, SqlConnection, SqlTransaction> {readonly}'],
  ops:['+ ListarFinalizadas(): List<Comision_83KI>','+ Buscar(idComision: int): List<AlumnoCertificacion_83KI>','+ RevalidarEnTransaccion(idInscripcion: int, resolver: Func<AlumnoCertificacion_83KI, CertificadoFinalizacion_83KI>): AlumnoCertificacion_83KI','- P(id: int): List<SqlParameter> {static}','- Mapear(r: DataRow): AlumnoCertificacion_83KI {static}','- MapearCertificado(r: DataRow): CertificadoFinalizacion_83KI {static}','- Valores(s: CertificadoFinalizacion_83KI): List<SqlParameter> {static}'],
  detail:'ServiceFactory_83KI.CrearCertificadoBLL inyecta integridad.ActualizarCertificado en _integridad. No hay referencia almacenada a IntegridadDAL ni metodos DAL Generar/Retener.'},
 {name:'IComisionDAL_83KI',kind:'Interface',source:'Service/Interfaces/IComisionDAL_83KI.cs',row:4,col:0,
  attrs:[],ops:['+ ObtenerComision(idComision: int): Comision_83KI']},
 {name:'ComisionDAL_83KI',source:'DAL/ComisionDAL_83KI.cs',row:5,col:0,
  attrs:['- _accesoDAL: AccesoDAL_83KI {readonly}'],ops:['+ ObtenerComision(idComision: int): Comision_83KI'],detail:'Se encapsula el mapeo de comision. Otros metodos de esta DAL no corresponden a CUN09.'},
 {name:'IBitacoraManager_83KI',kind:'Interface',source:'Service/Interfaces/IBitacoraManager_83KI.cs',row:0,col:3,
  attrs:[],ops:['+ RegistrarEvento(evento: BitacoraEvento_83KI): void']},
 {name:'BitacoraBLL_83KI',source:'BLL/BitacoraBLL_83KI.cs',row:1,col:3,
  attrs:['- _bitacoraDAL: IBitacoraDAL_83KI {readonly}'],ops:['+ RegistrarEvento(evento: BitacoraEvento_83KI): void']},
 {name:'IBitacoraDAL_83KI',kind:'Interface',source:'DAL/interfaces/IBitacoraDAL_83KI.cs',row:3,col:3,
  attrs:[],ops:['+ Registrar(evento: BitacoraEvento_83KI): void']},
 {name:'BitacoraEventoDAL_83KI',source:'DAL/BitacoraEventoDAL_83KI.cs',row:4,col:3,
  attrs:['- _acceso: AccesoDAL_83KI {readonly}'],ops:['+ Registrar(evento: BitacoraEvento_83KI): void']},
 {name:'BitacoraEvento_83KI',source:'Service/Entidades/BitacoraEvento_83KI.cs',row:1,col:4,
  attrs:['+ Fecha: DateTime {property private set}','+ Descripcion: string {property private set}','+ Criticidad: Criticidad {property private set}','+ Modulo: Modulo {property private set}','+ Username: string {property private set}'],
  ops:['+ CrearNuevo(descripcion: string, criticidad: Criticidad, modulo: Modulo, username: string): BitacoraEvento_83KI {static}'],detail:'Id no participa en la insercion. No se muestran reconstruccion ni consultas de auditoria.'},
 {name:'Criticidad',kind:'Enumeration',source:'Service/Entidades/Enums.cs',row:0,col:4,attrs:[],ops:[],literals:['Medio = 2'],detail:'Solo el literal utilizado en CUN09; valor declarado en codigo.'},
 {name:'Modulo',kind:'Enumeration',source:'Service/Entidades/Enums.cs',row:2,col:4,attrs:[],ops:[],literals:['PlanificacionAcademica = 2'],detail:'Solo el literal utilizado en CUN09; valor implicito segun orden del enum completo.'},
 {name:'IntegridadDAL_83KI',source:'DAL/IntegridadDAL_83KI.cs',row:4,col:2,
  attrs:['- _encriptador: IEncriptador_83KI {readonly}'],
  ops:['+ ActualizarCertificado(id: int, conn: SqlConnection, tran: SqlTransaction): void','+ CalcularDVH(nombreTabla: string, valoresColumna: Dictionary<string, object>): string','+ ActualizarDVHyDVVTransaccional(nombreTabla: string, valoresColumna: Dictionary<string, object>, whereClause: string, whereParams: List<SqlParameter>, conn: SqlConnection, tran: SqlTransaction): void'],detail:'Se encapsulan lectura/canonicalizacion de filas, configuracion de tablas y recalculo DVV internos. ActualizarCertificado NO esta en IIntegridadDAL_83KI.'},
 {name:'IIntegridadDAL_83KI',kind:'Interface',source:'DAL/interfaces/IIntegridadDAL_83KI.cs',row:5,col:2,
  attrs:[],ops:['+ CalcularDVH(nombreTabla: string, valoresColumna: Dictionary<string, object>): string','+ ActualizarDVHyDVVTransaccional(nombreTabla: string, valoresColumna: Dictionary<string, object>, whereClause: string, whereParams: List<SqlParameter>, conn: SqlConnection, tran: SqlTransaction): void']},
 {name:'IEncriptador_83KI',kind:'Interface',source:'Service/Interfaces/IEncriptador_83KI.cs',row:5,col:3,
  attrs:[],ops:['+ CalcularHashIntegridad(texto: string): string']},
 {name:'Encriptador_83KI',source:'Service/Encriptador_83KI.cs',row:6,col:3,
  attrs:[],ops:['+ CalcularHashIntegridad(texto: string): string']},
 {name:'AccesoDAL_83KI',visibility:'Package',source:'DAL/AccesoDAL_83KI.cs',row:6,col:1,
  attrs:[],ops:['+ Leer(consulta: string, parametros: List<SqlParameter> = null): DataSet','+ Escribir(consulta: string, parametros: List<SqlParameter>): int','+ LeerStoredProcedure(procedimiento: string, parametros: List<SqlParameter> = null): DataSet','~ EjecutarTransaccion(operacion: Action<SqlConnection, SqlTransaction>): void','~ LeerTransaccional(conn: SqlConnection, tran: SqlTransaction, consulta: string, parametros: List<SqlParameter> = null): DataSet {static}','~ LeerEscalarTransaccional(conn: SqlConnection, tran: SqlTransaction, consulta: string, parametros: List<SqlParameter> = null): object {static}','~ EscribirTransaccional(conn: SqlConnection, tran: SqlTransaction, consulta: string, parametros: List<SqlParameter>): int {static}'],detail:'BeginTransaction y Commit/Rollback se ejecutan dentro de EjecutarTransaccion. No existen metodos de proyecto ExecuteQuery, ExecuteNonQuery o CommitTransaction. Se omite configuracion de conexion.'}
];
// Asociaciones unidireccionales: solo se fija multiplicidad del extremo navegado.
// No se infiere propiedad exclusiva del ciclo de vida por usar new o readonly.
var LINKS = [
 ['SessionManager_83KI','ISessionManager_83KI','Realisation',''],
 ['ISessionManager_83KI','Usuario_83KI','Association','UsuarioActivo','0..1'],
 ['SessionManager_83KI','Usuario_83KI','Association','UsuarioActivo','0..1'],
 ['KI68CertificadoBLL','ISessionManager_83KI','Association','_session','1'],
 ['KI68CertificadoBLL','ICertificadoDAL_83KI','Association','_dal','1'],
 ['KI68CertificadoBLL','IBitacoraManager_83KI','Association','_audit','1'],
 ['KI68CertificadoBLL','EstadoRetencion','Association','_retenciones','0..*'],
 ['KI68CertificadoBLL','GenerarCertificadoRequest_83KI','Dependency','Generar / Validar'],
 ['KI68CertificadoBLL','ResultadoCertificacion_83KI','Dependency','Generar / Retener'],
 ['KI68CertificadoBLL','AlumnoCertificacion_83KI','Dependency','Buscar / Seleccionar / validaciones'],
 ['KI68CertificadoBLL','CertificadoFinalizacion_83KI','Dependency','crea instantanea'],
 ['KI68CertificadoBLL','BitacoraEvento_83KI','Dependency','CrearNuevo'],
 ['ResultadoCertificacion_83KI','AlumnoCertificacion_83KI','Association','Alumno','0..1'],
 ['AlumnoCertificacion_83KI','CertificadoFinalizacion_83KI','Association','Certificado','0..1'],
 ['AlumnoCertificacion_83KI','CuotaCertificacion_83KI','Association','Cuotas','0..*'],
 ['CertificadoDAL_83KI','ICertificadoDAL_83KI','Realisation',''],
 ['ICertificadoDAL_83KI','Comision_83KI','Dependency','ListarFinalizadas'],
 ['ICertificadoDAL_83KI','AlumnoCertificacion_83KI','Dependency','Buscar / RevalidarEnTransaccion'],
 ['ICertificadoDAL_83KI','CertificadoFinalizacion_83KI','Dependency','resolver'],
 ['CertificadoDAL_83KI','AccesoDAL_83KI','Association','_acceso','1'],
 ['CertificadoDAL_83KI','ComisionDAL_83KI','Dependency','ListarFinalizadas: instancia local'],
 ['CertificadoDAL_83KI','IntegridadDAL_83KI','Dependency','callback _integridad: ActualizarCertificado'],
 ['ComisionDAL_83KI','IComisionDAL_83KI','Realisation',''],
 ['ComisionDAL_83KI','AccesoDAL_83KI','Association','_accesoDAL','1'],
 ['IComisionDAL_83KI','Comision_83KI','Dependency','ObtenerComision'],
 ['BitacoraBLL_83KI','IBitacoraManager_83KI','Realisation',''],
 ['BitacoraBLL_83KI','IBitacoraDAL_83KI','Association','_bitacoraDAL','1'],
 ['IBitacoraManager_83KI','BitacoraEvento_83KI','Dependency','RegistrarEvento'],
 ['BitacoraEventoDAL_83KI','IBitacoraDAL_83KI','Realisation',''],
 ['BitacoraEventoDAL_83KI','AccesoDAL_83KI','Association','_acceso','1'],
 ['IBitacoraDAL_83KI','BitacoraEvento_83KI','Dependency','Registrar'],
 ['BitacoraEvento_83KI','Criticidad','Dependency','Criticidad'],
 ['BitacoraEvento_83KI','Modulo','Dependency','Modulo'],
 ['IntegridadDAL_83KI','IIntegridadDAL_83KI','Realisation',''],
 ['IntegridadDAL_83KI','AccesoDAL_83KI','Dependency','EscribirTransaccional (estatico)'],
 ['IntegridadDAL_83KI','IEncriptador_83KI','Association','_encriptador','1'],
 ['Encriptador_83KI','IEncriptador_83KI','Realisation','']
];
// LOGICA. No se modifica ningun elemento preexistente.
function trim(s) { return String(s).replace(/^\s+|\s+$/g, ''); }
function visibility(s) { return s == '-' ? 'Private' : s == '~' ? 'Package' : s == '#' ? 'Protected' : 'Public'; }
function parameters(s) {
 var parts = [], start = 0, depth = 0, i, c;
 if (!trim(s)) return parts;
 for (i = 0; i < s.length; i++) {
  c = s.charAt(i);
  if (c == '<') depth++;
  if (c == '>') depth--;
  if (c == ',' && depth == 0) { parts.push(trim(s.substring(start, i))); start = i + 1; }
 }
 if (depth != 0) throw new Error('Tipo generico invalido: ' + s);
 parts.push(trim(s.substring(start)));
 return parts;
}
function attribute(s) {
 var flags = /\{([^}]+)\}/.exec(s);
 var m = /^([+\-~#])\s+(\w+):\s*([^=]+?)(?:\s*=\s*(.*))?$/.exec(trim(s.replace(/\{[^}]*\}/g, '')));
 if (!m) throw new Error('Atributo invalido: ' + s);
 return {name:m[2], type:trim(m[3]), visibility:visibility(m[1]), flags:flags ? flags[1] : '', value:m[4]};
}
function operation(s) {
 var m = /^([+\-~#])\s+(\w+)\((.*)\):\s*(.+)$/.exec(trim(s.replace(/\{static\}/g, ''))), list, result, i, p;
 if (!m) throw new Error('Operacion invalida: ' + s);
 list = parameters(m[3]); result = [];
 for (i = 0; i < list.length; i++) {
  p = /^(\w+):\s*(.+?)(?:\s*=\s*(.+))?$/.exec(list[i]);
  if (!p) throw new Error('Parametro invalido: ' + list[i]);
  result.push({name:p[1], type:trim(p[2]), value:p[3]});
 }
 return {name:m[2], type:trim(m[4]), visibility:visibility(m[1]), isStatic:/\{static\}/.test(s), params:result};
}
function sizeOf(d) {
 var longest = d.name.length, i;
 // Estimacion conservadora: EA muestra nombres, parametros, valores y tipos.
 for (i = 0; i < d.attrs.length; i++) longest = Math.max(longest, d.attrs[i].length);
 for (i = 0; i < d.ops.length; i++) longest = Math.max(longest, d.ops[i].length);
 return {w:Math.max(290, longest * 8 + 100), h:130 + (d.attrs.length + d.ops.length + (d.literals || []).length) * 23};
}
function layout() {
 var rows = {}, keys = [], result = [], i, j, d, list, x, y = 50, h, sz;
 for (i = 0; i < TYPES.length; i++) {
  d = TYPES[i];
  if (!rows[d.row]) { rows[d.row] = []; keys.push(d.row); }
  rows[d.row].push(d);
 }
 keys.sort(function(a, b) { return a - b; });
 for (i = 0; i < keys.length; i++) {
  list = rows[keys[i]];
  list.sort(function(a, b) { return a.col - b.col; });
  x = 50; h = 0;
  for (j = 0; j < list.length; j++) {
   d = list[j]; sz = sizeOf(d);
   result.push({name:d.name, l:x, r:x + sz.w, t:y, b:y + sz.h});
   x += sz.w + GAP_X; h = Math.max(h, sz.h);
  }
  y += h + GAP_Y;
 }
 return result;
}
function validateModel(plan) {
 var names = {}, edges = {}, adj = {}, seen = {}, queue = [], i, j, d, r, key, n, slots = {};
 for (i = 0; i < TYPES.length; i++) {
  d = TYPES[i]; key = d.row + '/' + d.col;
  if (names[d.name] || slots[key]) throw new Error('Tipo o posicion duplicada: ' + d.name);
  if (/^Frm|Exporter|Permiso|Rol_/.test(d.name)) throw new Error('Tipo fuera del alcance: ' + d.name);
  slots[key] = true; names[d.name] = d; adj[d.name] = [];
  for (j = 0; j < d.attrs.length; j++) attribute(d.attrs[j]);
  for (j = 0; j < d.ops.length; j++) operation(d.ops[j]);
 }
 for (i = 0; i < TYPES.length; i++) if (TYPES[i].owner && !names[TYPES[i].owner]) throw new Error('Contenedor inexistente');
 for (i = 0; i < LINKS.length; i++) {
  r = LINKS[i]; key = r[0] + '|' + r[1] + '|' + r[2];
  if (!names[r[0]] || !names[r[1]] || edges[key]) throw new Error('Relacion invalida: ' + key);
  if (r[2] == 'Association' && !/^(1|0\.\.1|0\.\.\*)$/.test(r[4])) throw new Error('Multiplicidad invalida');
  if (r[2] == 'Realisation' && names[r[1]].kind != 'Interface') throw new Error('Realizacion sin interfaz');
  edges[key] = true; adj[r[0]].push(r[1]); adj[r[1]].push(r[0]);
 }
 queue.push('KI68CertificadoBLL');
 while (queue.length) { n = queue.pop(); if (seen[n]) continue; seen[n] = true; for (i = 0; i < adj[n].length; i++) queue.push(adj[n][i]); }
 for (i = 0; i < TYPES.length; i++) if (!seen[TYPES[i].name]) throw new Error('Clase desconectada: ' + TYPES[i].name);
 if (plan.length != TYPES.length) throw new Error('Faltan cajas');
 for (i = 0; i < plan.length; i++) {
  d = plan[i];
  if (!isFinite(d.l) || !isFinite(d.r) || !isFinite(d.t) || !isFinite(d.b) || d.r <= d.l || d.b <= d.t) throw new Error('Caja invalida');
  for (j = i + 1; j < plan.length; j++) {
   r = plan[j];
   if (!(d.r <= r.l || r.r <= d.l || d.b <= r.t || r.b <= d.t)) throw new Error('Cajas superpuestas: ' + d.name + ' / ' + r.name);
  }
 }
}
function save(obj, context) {
 if (!obj.Update()) throw new Error('EA no pudo guardar ' + context + ': ' + obj.GetLastError());
}
function progress(stage) { CURRENT_STAGE = stage; Session.Output('CUN09 DC | ' + stage); }
function uniqueName(parent) {
 var name = PACKAGE_NAME, suffix = 1, exists, i;
 parent.Packages.Refresh();
 do {
  exists = false;
  for (i = 0; i < parent.Packages.Count; i++) if (parent.Packages.GetAt(i).Name == name) exists = true;
  if (exists) { suffix++; name = PACKAGE_NAME + ' (' + suffix + ')'; }
 } while (exists);
 return name;
}
function createType(parent, d) {
 var el = parent.Elements.AddNew(d.name, d.kind || 'Class'), i, j, a, at, op, me, p, pa, literal;
 el.Visibility = d.visibility || 'Public';
 el.Notes = 'Fuente: ' + d.source + '\nVista parcial CUN09: solo miembros participantes; no es el tipo completo.\n' + (d.detail || '');
 save(el, d.name);
 for (i = 0; i < d.attrs.length; i++) {
  a = attribute(d.attrs[i]); at = el.Attributes.AddNew(a.name, a.type);
  at.Visibility = a.visibility; at.IsStatic = /static|const/.test(a.flags); at.IsConst = /const/.test(a.flags);
  if (typeof a.value != 'undefined') at.Default = a.value;
  at.Notes = 'Fuente: ' + d.source + '\n' + a.flags;
  if (/derived/.test(a.flags)) at.IsDerived = true;
  save(at, d.name + '.' + a.name);
 }
 for (i = 0; i < (d.literals || []).length; i++) {
  literal = d.literals[i].split('=');
  at = el.Attributes.AddNew(trim(literal[0]), ''); at.Visibility = 'Public'; at.Stereotype = 'enum';
  at.Default = trim(literal[1]);
  save(at, d.name + '.' + d.literals[i]);
 }
 el.Attributes.Refresh();
 for (i = 0; i < d.ops.length; i++) {
  op = operation(d.ops[i]); me = el.Methods.AddNew(op.name, op.type);
  me.Visibility = op.visibility; me.IsStatic = op.isStatic; me.Abstract = d.kind == 'Interface'; me.Notes = d.source;
  save(me, d.name + '.' + op.name);
  for (j = 0; j < op.params.length; j++) {
   p = op.params[j]; pa = me.Parameters.AddNew(p.name, p.type); pa.Kind = 'in'; pa.Position = j;
   if (typeof p.value != 'undefined') pa.Default = p.value;
   save(pa, p.name);
  }
  me.Parameters.Refresh(); save(me, op.name);
 }
 el.Methods.Refresh(); save(el, d.name); parent.Elements.Refresh();
 return el;
}
function connect(elements, r) {
 var con = elements[r[0]].Connectors.AddNew('', r[2]);
 con.SupplierID = elements[r[1]].ElementID; con.Notes = r[3];
 save(con, r[0] + ' -> ' + r[1]);
 if (r[2] == 'Association') {
  con.Direction = 'Source -> Destination';
  con.SupplierEnd.Role = r[3]; con.SupplierEnd.Cardinality = r[4]; con.SupplierEnd.Navigable = 'Navigable';
  con.ClientEnd.Navigable = 'Non-Navigable';
  save(con.SupplierEnd, 'extremo destino'); save(con.ClientEnd, 'extremo origen');
 } else if (r[2] == 'Dependency') con.Name = r[3];
 save(con, 'relacion'); elements[r[0]].Connectors.Refresh();
}
function verifyReadback(elements, dg, plan) {
 var i, j, k, d, el, a, op, found, at, me, p, expected, actual, o;
 for (i = 0; i < TYPES.length; i++) {
  d = TYPES[i]; el = Repository.GetElementByID(elements[d.name].ElementID);
  el.Attributes.Refresh(); el.Methods.Refresh();
  if (el.Attributes.Count != d.attrs.length + (d.literals || []).length || el.Methods.Count != d.ops.length) throw new Error('Miembros no persistidos: ' + d.name);
  for (j = 0; j < d.attrs.length; j++) {
   a = attribute(d.attrs[j]); found = false;
   for (k = 0; k < el.Attributes.Count; k++) {
    at = el.Attributes.GetAt(k);
    if (at.Name == a.name && at.Type == a.type && at.Visibility == a.visibility && !!at.IsStatic == /static|const/.test(a.flags) && !!at.IsConst == /const/.test(a.flags)) found = true;
   }
   if (!found) throw new Error('Atributo no coincide: ' + d.name + '.' + a.name);
  }
  for (j = 0; j < d.ops.length; j++) {
   op = operation(d.ops[j]); found = false;
   for (k = 0; k < el.Methods.Count; k++) {
    me = el.Methods.GetAt(k);
    if (me.Name != op.name || me.ReturnType != op.type || me.Visibility != op.visibility || !!me.IsStatic != op.isStatic || me.Parameters.Count != op.params.length) continue;
    found = true;
    for (p = 0; p < op.params.length; p++) if (me.Parameters.GetAt(p).Name != op.params[p].name || me.Parameters.GetAt(p).Type != op.params[p].type) found = false;
   }
   if (!found) throw new Error('Firma no coincide: ' + d.name + '.' + op.name);
  }
 }
 dg.DiagramObjects.Refresh();
 if (dg.DiagramObjects.Count != plan.length) throw new Error('Faltan cajas persistidas');
 // EA usa Y negativas en DiagramObject. Comparar rectangulos guardados, no solo el plan.
 actual = [];
 for (i = 0; i < dg.DiagramObjects.Count; i++) {
  o = dg.DiagramObjects.GetAt(i);
  actual.push({name:String(o.ElementID), l:o.left, r:o.right, t:-o.top, b:-o.bottom});
 }
 validateModel(actual);
}
function main() {
 var parent, plan, pkg, elements = {}, i, d, container, dg, o, byName = {};
 progress('Validando modelo y geometria');
 plan = layout(); validateModel(plan);
 parent = Repository.GetTreeSelectedPackage();
 if (!parent) throw new Error('Seleccionar un paquete de destino en el navegador de EA.');
 if (parent.Name.indexOf(PACKAGE_NAME) == 0) throw new Error('Seleccionar el paquete padre del CU, no un paquete generado.');
 progress('Creando paquete nuevo');
 pkg = parent.Packages.AddNew(uniqueName(parent), 'Package'); save(pkg, 'paquete'); CREATED_PACKAGE = pkg;
 parent.Packages.Refresh();
 pkg.Notes = 'CUN09 desde codigo C#. Sin UI; sesion solo consulta. No contiene notas graficas. Datos de fuente verificados offline. Los metodos de infraestructura internos se encapsulan segun el recorte del CU.';
 save(pkg, 'descripcion del paquete');
 for (i = 0; i < TYPES.length; i++) {
  d = TYPES[i]; byName[d.name] = d; container = d.owner ? elements[d.owner] : pkg;
  if (!container) throw new Error('Crear antes el contenedor de ' + d.name);
  progress('Creando ' + d.name); elements[d.name] = createType(container, d);
 }
 for (i = 0; i < LINKS.length; i++) connect(elements, LINKS[i]);
 progress('Creando diagrama');
 dg = pkg.Diagrams.AddNew(DIAGRAM_NAME, 'Class'); save(dg, 'diagrama'); pkg.Diagrams.Refresh();
 dg.ShowPrivate = true; dg.ShowProtected = true; dg.ShowPublic = true;
 dg.ExtendedStyle = 'OpParams=1;UseAlias=0;'; save(dg, 'visibilidad');
 for (i = 0; i < plan.length; i++) {
  d = plan[i]; o = dg.DiagramObjects.AddNew('l=' + d.l + ';r=' + d.r + ';t=' + d.t + ';b=' + d.b + ';', '');
  o.ElementID = elements[d.name].ElementID;
  try { o.BackgroundColor = COLORS[byName[d.name].kind || 'Class']; } catch (styleError) { Session.Output('Color omitido: ' + d.name); }
  save(o, d.name);
 }
 dg.DiagramObjects.Refresh(); save(dg, 'diagrama completo');
 progress('Verificando firmas y cajas guardadas'); verifyReadback(elements, dg, plan);
 Repository.RefreshModelView(parent.PackageID); Repository.OpenDiagram(dg.DiagramID);
 Session.Output('OK: ' + TYPES.length + ' tipos, ' + LINKS.length + ' relaciones. Firmas y cajas guardadas verificadas.');
 Session.Output('La ejecucion COM y el renderizado final se comprueban dentro de EA. No se modifica contenido anterior.');
 Session.Prompt('Creado: ' + pkg.Name + '\n' + TYPES.length + ' tipos y ' + LINKS.length + ' relaciones.\nDiagrama: ' + DIAGRAM_NAME, promptOK);
}
function run() {
 var message;
 CREATED_PACKAGE = null;
 try { main(); }
 catch (error) {
  message = 'CUN09 DC fallo en: ' + CURRENT_STAGE + '\n' + (error.description || error.message || String(error));
  if (CREATED_PACKAGE) message += '\nPaquete parcial conservado para inspeccion: ' + CREATED_PACKAGE.Name;
  Session.Output(message); Session.Prompt(message, promptOK);
  throw error;
 }
}
run();
