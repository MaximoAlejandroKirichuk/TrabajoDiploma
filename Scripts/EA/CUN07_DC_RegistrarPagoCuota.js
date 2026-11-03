!INC Local Scripts.EAConstants-JScript
// Ejecutar dentro de Enterprise Architect: seleccionar el paquete del caso de uso CUN07 y ejecutar este JScript.
// Las clases se crean en el paquete CUN07 seleccionado, nunca en su paquete hijo DC.
// No se modifican elementos ni diagramas existentes. Se rechaza una segunda ejecucion en el mismo paquete.
// Proyeccion del codigo de CUN07: se omiten la UI, SessionManager y los constructores.
// Las abstracciones de base de datos e integridad del DSS no se inventan como operaciones en este DC basado en codigo.
var MARKER = '[CUN07-DC-code-v1]';
var DIAGRAM_NAME = 'CUN07 - Registrar pago de cuota - contratos actuales';
var STAGE = 'validation';
var TYPES = [], LINKS = [];
function type(name, kind, row, source, attrs, ops, note) {
    TYPES.push({name:name, kind:kind, row:row, source:source, attrs:attrs, ops:ops, note:note || ''});
}
var paymentOps = [
    '+ ListarComisionesConCuotasPendientes(dni: string): IEnumerable<ComisionPagoOpcion_83KI>',
    '+ BuscarCuotasPendientes(dni: string, codigoComision: string): BusquedaCuotasPagoResultado_83KI',
    '+ RegistrarPagoCuota(request: PagoCuotaRequest_83KI): PagoCuotaResultado_83KI'
];
var lookupOp = '+ ObtenerPorDniYComision(dni: string, codigoComision: string): SolicitudInscripcion_83KI';
var quotaOp = '+ ValidarPagoCompleto(cuota: Cuota_83KI, monto: decimal): void';
var auditOp = '+ RegistrarEvento(evento: BitacoraEvento_83KI): void';
var dalOps = [paymentOps[0], paymentOps[1],
    '+ ExisteReferenciaPago(numeroReferencia: string): bool',
    '+ RegistrarPago(pago: PagoInscripcion_83KI): PagoCuotaResultado_83KI'];
type('PagoCuotaRequest_83KI','Class',0,'BE/DTOs/Pagos/PagoCuotaRequest_83KI.cs',[
    '+ DNI: string','+ CodigoComision: string','+ IdCuota: int','+ MetodoPago: string',
    '+ MontoRecibido: decimal','+ NumeroReferencia: string','+ FechaPago: DateTime','+ FechaAcreditacion: DateTime'],[]);
type('BusquedaCuotasPagoResultado_83KI','Class',0,'BE/DTOs/Pagos/BusquedaCuotasPagoResultado_83KI.cs',[
    '+ Alumno: Alumno_83KI','+ Solicitud: SolicitudInscripcion_83KI','+ CuotasPendientes: List<Cuota_83KI>',
    '+ Cuotas: List<Cuota_83KI>','+ NombrePlanDePago: string','+ MatriculaPagada: bool',
    '+ CodigoComision: string','+ Curso: string'],[]);
type('PagoCuotaResultado_83KI','Class',0,'BE/DTOs/Pagos/PagoCuotaResultado_83KI.cs',[
    '+ Pago: PagoInscripcion_83KI','+ Cuota: Cuota_83KI','+ Solicitud: SolicitudInscripcion_83KI','+ Alumno: Alumno_83KI',
    '+ CodigoComision: string','+ Curso: string','+ CodigoSolicitud: string','+ AuditoriaFallida: bool',
    '+ NumeroReciboInstitucional: string','+ FacturaTexto: string','+ FacturaConcepto: string',
    '+ FacturaTotal: decimal','+ FacturaFecha: DateTime','+ IdFactura: int'],[]);
type('ComisionPagoOpcion_83KI','Class',0,'BE/DTOs/Comisiones/ComisionPagoOpcion_83KI.cs',[
    '+ Codigo: string','+ Descripcion: string'],[]);
type('IInscripcionBLL_83KI','Interface',1,'Service/Interfaces/IInscripcionBLL_83KI.cs',[],[lookupOp]);
type('KI68InscripcionBLL','Class',1,'BLL/Negocio/KI68InscripcionBLL.cs',[
    '- _inscripcionDAL: IInscripcionDAL_83KI'],[lookupOp],
    'CUN07 projection. Enrollment creation, initial fee preview and dependencies used only by other operations are omitted.');
type('IPagoCuotaBLL_83KI','Interface',1,'Service/Interfaces/IPagoCuotaBLL_83KI.cs',[],paymentOps);
type('KI68PagoCuotaBLL_83KI','Class',1,'BLL/Negocio/KI68PagoCuotaBLL_83KI.cs',[
    '- _pagoDAL: IPagoCuotaDAL_83KI','- _bitacora: IBitacoraManager_83KI',
    '- _inscripcionBLL: IInscripcionBLL_83KI','- _cuotaBLL: ICuotaBLL_83KI'],paymentOps.concat([
    '- ValidarDatosPago(request: PagoCuotaRequest_83KI): void {static}',
    '- ValidarSolicitudBusqueda(solicitud: SolicitudInscripcion_83KI, busqueda: BusquedaCuotasPagoResultado_83KI): void {static}',
    '- ValidarPermiso(): void']),
    'SessionManager and constructors omitted by request. Authorization remains implemented.');
type('ICuotaBLL_83KI','Interface',1,'Service/Interfaces/ICuotaBLL_83KI.cs',[],[quotaOp]);
type('KI68CuotaBLL','Class',1,'BLL/Negocio/KI68CuotaBLL.cs',[],[quotaOp],
    'Only full-payment validation participates in CUN07. PrevisualizarCuotaInicial belongs to another flow.');
type('IInscripcionDAL_83KI','Interface',2,'Service/Interfaces/IInscripcionDAL_83KI.cs',[],[lookupOp]);
type('InscripcionDAL_83KI','Class',2,'DAL/InscripcionDAL_83KI.cs',[
    '- _accesoDAL: AccesoDAL_83KI'],[lookupOp,
    '- MapearSolicitud(row: DataRow): SolicitudInscripcion_83KI {static}',
    '- Normalizar(value: string): string {static}']);
type('IPagoCuotaDAL_83KI','Interface',2,'Service/Interfaces/IPagoCuotaDAL_83KI.cs',[],dalOps);
type('PagoCuotaDAL_83KI','Class',2,'DAL/PagoCuotaDAL_83KI.cs',[
    '- _accesoDAL: AccesoDAL_83KI','- _integridadDAL: IntegridadDAL_83KI'],dalOps.concat([
    '- CrearParametrosRegistro(pago: PagoInscripcion_83KI): List<SqlParameter> {static}',
    '- MapearBusqueda(row: DataRow): BusquedaCuotasPagoResultado_83KI {static}',
    '- MapearCuota(row: DataRow): Cuota_83KI {static}',
    '- MapearResultado(row: DataRow): PagoCuotaResultado_83KI {static}',
    '- MapearSolicitud(row: DataRow, idAlumno: int, idLead: int?): SolicitudInscripcion_83KI {static}',
    '- RecomputarIntegridad(tabla: string, pk: string, id: int, conn: SqlConnection, tran: SqlTransaction): void',
    '- Normalizar(value: string): string {static}']),
    'RegistrarPago executes sp_CUN07_RegistrarPago and recomputes integrity in the same transaction.');
type('IBitacoraManager_83KI','Interface',2,'Service/Interfaces/IBitacoraManager_83KI.cs',[],[auditOp]);
type('BitacoraBLL_83KI','Class',2,'BLL/BitacoraBLL_83KI.cs',[
    '- _bitacoraDAL: IBitacoraDAL_83KI'],[auditOp]);
type('Alumno_83KI','Class',3,'BE/Entidades/Alumno_83KI.cs',[
    '+ IdAlumno: int','+ DNI: string','+ Nombre: string','+ Apellido: string','+ Email: string',
    '+ Telefono: string','+ FechaAlta: DateTime','+ Estado: string','+ DVH: string'],[],
    'Entity state projection. Reconstruction methods omitted for readability.');
type('SolicitudInscripcion_83KI','Class',3,'BE/Entidades/SolicitudInscripcion_83KI.cs',[
    '+ IdSolicitudInscripcion: int','+ Codigo: string','+ IdAlumno: int','+ IdLeadOrigen: int?',
    '+ IdComision: int','+ IdPlanDePago: int','+ RecargoPlanSnapshot: decimal','+ FechaSolicitud: DateTime',
    '+ Estado: string','+ Observaciones: string','+ DVH: string','+ ArancelCursoSnapshot: decimal?',
    '+ CantidadCuotasSnapshot: int?','+ TotalCursoSnapshot: decimal?',
    '+ FechaPrimerVencimientoSnapshot: DateTime?','+ NombrePlanSnapshot: string'],[],
    'Entity state projection; CUN07 does not calculate administrative validity.');
type('Cuota_83KI','Class',3,'BE/Entidades/Cuota_83KI.cs',[
    '+ IdCuota: int','+ IdSolicitudInscripcion: int','+ NumeroCuota: int','+ MontoOriginal: decimal',
    '+ BalanceAdeudado: decimal','+ FechaVencimiento: DateTime','+ Estado: string','+ DVH: string',
    '+ TipoConcepto: string','+ Periodo: DateTime?','+ EsMatricula: bool','+ NumeroMensualidad: int'],[],
    'EsMatricula and NumeroMensualidad are computed read-only properties. Reconstruction methods/constants omitted.');
type('PagoInscripcion_83KI','Class',3,'BE/Entidades/PagoInscripcion_83KI.cs',[
    '+ IdPagoInscripcion: int','+ IdAlumno: int','+ IdSolicitudInscripcion: int','+ IdCuota: int',
    '+ MetodoPago: string','+ MontoPagado: decimal','+ NumeroReferencia: string','+ FechaPago: DateTime',
    '+ FechaAcreditacion: DateTime','+ CodigoComision: string','+ DVH: string'],[
    '+ Crear(idAlumno: int, idSolicitudInscripcion: int, idCuota: int, metodoPago: string, montoPagado: decimal, numeroReferencia: string, fechaPago: DateTime, fechaAcreditacion: DateTime, codigoComision: string): PagoInscripcion_83KI {static}',
    '+ MetodoValido(metodoPago: string): bool {static}'],
    'Shared persisted payment entity, deliberately not renamed. Only operations directly used by PagoCuotaBLL are expanded.');
type('BitacoraEvento_83KI','Class',3,'Service/Entidades/BitacoraEvento_83KI.cs',[
    '+ Id: int','+ Fecha: DateTime','+ Descripcion: string','+ Criticidad: Criticidad','+ Modulo: Modulo','+ Username: string'],[
    '+ CrearNuevo(descripcion: string, criticidad: Criticidad, modulo: Modulo, username: string): BitacoraEvento_83KI {static}']);
type('AccesoDAL_83KI','Class',4,'DAL/AccesoDAL_83KI.cs',[],[
    '+ Leer(consulta: string, parametros: List<SqlParameter>): DataSet',
    '+ Escribir(consulta: string, parametros: List<SqlParameter>): int',
    '+ LeerStoredProcedure(procedimiento: string, parametros: List<SqlParameter>): DataSet',
    '~ EjecutarTransaccion(operacion: Action<SqlConnection, SqlTransaction>): void',
    '~ LeerStoredProcedureTransaccional(conn: SqlConnection, tran: SqlTransaction, procedimiento: string, parametros: List<SqlParameter>): DataSet {static}',
    '~ EscribirTransaccional(conn: SqlConnection, tran: SqlTransaction, consulta: string, parametros: List<SqlParameter>): int {static}'],
    'CUN07 access boundary projection. Optional defaults and connection-provider internals omitted.');
type('IntegridadDAL_83KI','Class',4,'DAL/IntegridadDAL_83KI.cs',[
    '- _accesoDAL: AccesoDAL_83KI','- _encriptador: IEncriptador_83KI'],[
    '~ LeerFilaTransaccional(nombreTabla: string, whereClause: string, whereParams: List<SqlParameter>, conn: SqlConnection, tran: SqlTransaction): Dictionary<string, object> {static}',
    '~ RecomputarIntegridadFila(nombreTabla: string, valoresColumna: Dictionary<string, object>, conn: SqlConnection, tran: SqlTransaction): void'],
    'Real CUN07 integration boundary. DSS IntegridadBLL.ActualizarDVHyDVV is an academic abstraction, not an extra code operation. Internal hashing/configuration algorithms are not expanded.');
type('IBitacoraDAL_83KI','Interface',4,'DAL/interfaces/IBitacoraDAL_83KI.cs',[],[
    '+ Registrar(evento: BitacoraEvento_83KI): void']);
type('BitacoraEventoDAL_83KI','Class',4,'DAL/BitacoraEventoDAL_83KI.cs',[
    '- _acceso: AccesoDAL_83KI'],['+ Registrar(evento: BitacoraEvento_83KI): void']);
type('IEncriptador_83KI','Interface',4,'Service/Interfaces/IEncriptador_83KI.cs',[],[
    '+ CalcularHashIntegridad(texto: string): string']);
type('Encriptador_83KI','Class',4,'Service/Encriptador_83KI.cs',[],[
    '+ CalcularHashIntegridad(texto: string): string']);
type('SnapshotMapper_83KI','Class',4,'DAL/SnapshotMapper_83KI.cs',[],[
    '~ Decimal(value: object): decimal? {static}','~ Integer(value: object): int? {static}',
    '~ Date(value: object): DateTime? {static}','~ Text(value: object): string {static}']);
function link(from,to,kind,role,many) { LINKS.push({from:from,to:to,kind:kind,role:role || '',many:many || ''}); }
function assoc(from,to,role,many) { link(from,to,'Association',role,many); }
function dep(from,to) { link(from,to,'Dependency'); }
function realizes(from,to) { link(from,to,'Realisation'); }
realizes('KI68PagoCuotaBLL_83KI','IPagoCuotaBLL_83KI');
realizes('KI68InscripcionBLL','IInscripcionBLL_83KI');
realizes('KI68CuotaBLL','ICuotaBLL_83KI');
realizes('PagoCuotaDAL_83KI','IPagoCuotaDAL_83KI');
realizes('InscripcionDAL_83KI','IInscripcionDAL_83KI');
realizes('BitacoraBLL_83KI','IBitacoraManager_83KI');
realizes('BitacoraEventoDAL_83KI','IBitacoraDAL_83KI');
realizes('Encriptador_83KI','IEncriptador_83KI');
assoc('KI68PagoCuotaBLL_83KI','IPagoCuotaDAL_83KI','_pagoDAL');
assoc('KI68PagoCuotaBLL_83KI','IInscripcionBLL_83KI','_inscripcionBLL');
assoc('KI68PagoCuotaBLL_83KI','ICuotaBLL_83KI','_cuotaBLL');
assoc('KI68PagoCuotaBLL_83KI','IBitacoraManager_83KI','_bitacora');
assoc('KI68InscripcionBLL','IInscripcionDAL_83KI','_inscripcionDAL');
assoc('InscripcionDAL_83KI','AccesoDAL_83KI','_accesoDAL');
assoc('PagoCuotaDAL_83KI','AccesoDAL_83KI','_accesoDAL');
assoc('PagoCuotaDAL_83KI','IntegridadDAL_83KI','_integridadDAL');
assoc('BitacoraBLL_83KI','IBitacoraDAL_83KI','_bitacoraDAL');
assoc('BitacoraEventoDAL_83KI','AccesoDAL_83KI','_acceso');
assoc('IntegridadDAL_83KI','AccesoDAL_83KI','_accesoDAL');
assoc('IntegridadDAL_83KI','IEncriptador_83KI','_encriptador');
dep('InscripcionDAL_83KI','SnapshotMapper_83KI'); dep('PagoCuotaDAL_83KI','SnapshotMapper_83KI');
dep('KI68PagoCuotaBLL_83KI','PagoCuotaRequest_83KI'); dep('KI68PagoCuotaBLL_83KI','PagoCuotaResultado_83KI');
dep('KI68PagoCuotaBLL_83KI','BusquedaCuotasPagoResultado_83KI'); dep('KI68PagoCuotaBLL_83KI','PagoInscripcion_83KI');
dep('KI68PagoCuotaBLL_83KI','ComisionPagoOpcion_83KI'); dep('KI68PagoCuotaBLL_83KI','BitacoraEvento_83KI');
dep('KI68CuotaBLL','Cuota_83KI'); dep('KI68InscripcionBLL','SolicitudInscripcion_83KI');
dep('PagoCuotaDAL_83KI','PagoCuotaResultado_83KI'); dep('PagoCuotaDAL_83KI','BusquedaCuotasPagoResultado_83KI');
dep('PagoCuotaDAL_83KI','PagoInscripcion_83KI'); dep('InscripcionDAL_83KI','SolicitudInscripcion_83KI');
dep('BitacoraEventoDAL_83KI','BitacoraEvento_83KI');
assoc('BusquedaCuotasPagoResultado_83KI','Alumno_83KI','Alumno','0..1');
assoc('BusquedaCuotasPagoResultado_83KI','SolicitudInscripcion_83KI','Solicitud','0..1');
assoc('BusquedaCuotasPagoResultado_83KI','Cuota_83KI','Cuotas','0..*');
assoc('BusquedaCuotasPagoResultado_83KI','Cuota_83KI','CuotasPendientes','0..*');
assoc('PagoCuotaResultado_83KI','Alumno_83KI','Alumno','0..1');
assoc('PagoCuotaResultado_83KI','SolicitudInscripcion_83KI','Solicitud','0..1');
assoc('PagoCuotaResultado_83KI','Cuota_83KI','Cuota','0..1');
assoc('PagoCuotaResultado_83KI','PagoInscripcion_83KI','Pago','0..1');

function trim(s) { return String(s).replace(/^\s+|\s+$/g,''); }
function visibility(v) { return v=='-' ? 'Private' : v=='~' ? 'Package' : 'Public'; }
function parameters(s) {
    var out=[], start=0, depth=0, i, c;
    if(!trim(s)) return out;
    for(i=0;i<s.length;i++) { c=s.charAt(i); if(c=='<') depth++; if(c=='>') depth--; if(c==',' && depth==0) {out.push(trim(s.substring(start,i)));start=i+1;} }
    out.push(trim(s.substring(start))); return out;
}
function operation(s) {
    var m=/^([+~\-])\s+(\w+)\((.*)\):\s*(.+)$/.exec(trim(s.replace(/\s*\{static\}/,''))), p=[], parts, i, a;
    if(!m) throw new Error('Invalid operation: '+s);
    parts=parameters(m[3]);
    for(i=0;i<parts.length;i++) { a=/^(\w+):\s*(.+)$/.exec(parts[i]);if(!a) throw new Error('Invalid parameter: '+parts[i]);p.push({name:a[1],type:a[2]}); }
    return {name:m[2],result:m[4],visibility:visibility(m[1]),params:p,staticOp:s.indexOf('{static}')>=0};
}
function attribute(s) {
    var m=/^([+~\-])\s+(\w+):\s*(.+)$/.exec(s);
    if(!m) throw new Error('Invalid attribute: '+s);
    return {name:m[2],type:m[3],visibility:visibility(m[1])};
}
function save(o,label) { if(!o.Update()) throw new Error('Update failed: '+label); }
function validate() {
    var names={}, i,j,d;
    for(i=0;i<TYPES.length;i++) { d=TYPES[i];if(names[d.name]) throw new Error('Duplicate type: '+d.name);names[d.name]=true;
        for(j=0;j<d.attrs.length;j++) attribute(d.attrs[j]);for(j=0;j<d.ops.length;j++) operation(d.ops[j]); }
    for(i=0;i<LINKS.length;i++) if(!names[LINKS[i].from] || !names[LINKS[i].to]) throw new Error('Unknown connector endpoint');
}
function createElement(pkg,d) {
    var e=pkg.Elements.AddNew(d.name,d.kind),i,j,a,o,m,p;
    e.Notes=MARKER+'\nSource: '+d.source+'\nCUN07 projection. Constructors omitted; UI and SessionManager excluded.\n'+d.note;
    if(d.name=='AccesoDAL_83KI' || d.name=='SnapshotMapper_83KI') e.Visibility='Package';
    save(e,d.name);
    for(i=0;i<d.attrs.length;i++) { a=attribute(d.attrs[i]);p=e.Attributes.AddNew(a.name,a.type);p.Visibility=a.visibility;save(p,a.name); }
    e.Attributes.Refresh();
    for(i=0;i<d.ops.length;i++) { o=operation(d.ops[i]);m=e.Methods.AddNew(o.name,o.result);m.Visibility=o.visibility;m.IsStatic=o.staticOp;m.Abstract=d.kind=='Interface';save(m,o.name);
        for(j=0;j<o.params.length;j++) {a=o.params[j];p=m.Parameters.AddNew(a.name,a.type);p.Position=j;p.Kind='in';save(p,a.name);}m.Parameters.Refresh();save(m,o.name); }
    e.Methods.Refresh();pkg.Elements.Refresh();return e;
}
function createLinks(elements) {
    var i,r,c;
    for(i=0;i<LINKS.length;i++) {r=LINKS[i];c=elements[r.from].Connectors.AddNew('',r.kind);c.SupplierID=elements[r.to].ElementID;save(c,r.from+' -> '+r.to);
        if(r.kind=='Association') {c.Direction='Source -> Destination';c.SupplierEnd.Role=r.role;if(r.many)c.SupplierEnd.Cardinality=r.many;save(c.SupplierEnd,r.role);}
        save(c,r.from+' -> '+r.to); }
}
function createDiagram(pkg,elements) {
    var d=pkg.Diagrams.AddNew(DIAGRAM_NAME,'Class'),i,j,row,x=40,y=40,h,w,item,o,maxH,cols=3;
    d.Notes=MARKER+'\nCode-based CUN07 view. UI, session and constructors omitted. Classes show only the CUN07 member projection. DSS database/integrity abstractions are intentionally not literal code signatures.';
    save(d,DIAGRAM_NAME);
    try {d.ShowPrivate=true;d.ShowProtected=true;d.ShowPublic=true;save(d,'member visibility');}
    catch(styleError) {Session.Output('Presentation preference could not be set; inspect member visibility in EA.');}
    for(row=0;row<=4;row++) {
        var items=[];for(i=0;i<TYPES.length;i++) if(TYPES[i].row==row) items.push(TYPES[i]);
        for(i=0;i<items.length;i+=cols) { x=40;maxH=0;
            for(j=i;j<Math.min(i+cols,items.length);j++) {item=items[j];w=340;
                var k;for(k=0;k<item.attrs.length;k++) w=Math.max(w,item.attrs[k].length*6+35);
                for(k=0;k<item.ops.length;k++) w=Math.max(w,item.ops[k].replace(/\w+: /g,'').length*6+35);
                h=90+(item.attrs.length+item.ops.length)*19;maxH=Math.max(maxH,h);
                o=d.DiagramObjects.AddNew('l='+x+';r='+(x+w)+';t='+y+';b='+(y+h)+';','');o.ElementID=elements[item.name].ElementID;
                o.BackgroundColor=item.kind=='Interface' ? 16174046 : 15003901;
                save(o,item.name+' placement');x+=w+120;
            } y+=maxH+120;
        }
    }
    d.DiagramObjects.Refresh();save(d,DIAGRAM_NAME);pkg.Diagrams.Refresh();return d;
}
function verify(elements,d) {
    var i,j,k,t,e,expected,actual,found,p;
    for(i=0;i<TYPES.length;i++) {t=TYPES[i];e=Repository.GetElementByID(elements[t.name].ElementID);e.Methods.Refresh();e.Attributes.Refresh();
        if(e.Methods.Count!=t.ops.length || e.Attributes.Count!=t.attrs.length) throw new Error('Member count mismatch: '+t.name);
        for(j=0;j<t.ops.length;j++) {expected=operation(t.ops[j]);found=false;
            for(k=0;k<e.Methods.Count;k++) {actual=e.Methods.GetAt(k);actual.Parameters.Refresh();
                if(actual.Name!=expected.name || actual.ReturnType!=expected.result || actual.Parameters.Count!=expected.params.length) continue;
                found=true;for(p=0;p<expected.params.length;p++) if(actual.Parameters.GetAt(p).Type!=expected.params[p].type) found=false;
            }if(!found)throw new Error('Signature readback failed: '+t.name+'.'+expected.name);
        }
    }
    d.DiagramObjects.Refresh();if(d.DiagramObjects.Count!=TYPES.length) throw new Error('Missing diagram objects');
}
function main() {
    var pkg=Repository.GetTreeSelectedPackage(),name,i,elements={},d;
    if(!pkg) throw new Error('Select the CUN07 use-case package in the Project Browser.');
    name=String(pkg.Name).replace(/["\u201c\u201d]/g,'');
    if(!/CUN\s*0?7\b/i.test(name)) throw new Error('Select the root CUN07 package, not DC or another use case.');
    validate();pkg.Elements.Refresh();
    for(i=0;i<pkg.Elements.Count;i++) if(String(pkg.Elements.GetAt(i).Notes).indexOf(MARKER)>=0)
        throw new Error('This generator has already created elements here. Existing generated content was retained; no changes made.');
    pkg.Diagrams.Refresh();for(i=0;i<pkg.Diagrams.Count;i++) if(String(pkg.Diagrams.GetAt(i).Name)==DIAGRAM_NAME)
        throw new Error('The target diagram already exists; no changes made.');
    for(i=0;i<TYPES.length;i++) {STAGE='Creating '+TYPES[i].name;Session.Output(STAGE);elements[TYPES[i].name]=createElement(pkg,TYPES[i]);}
    STAGE='Creating connectors';createLinks(elements);
    STAGE='Creating diagram';d=createDiagram(pkg,elements);
    STAGE='Verifying saved signatures';verify(elements,d);
    Repository.RefreshModelView(pkg.PackageID);Repository.OpenDiagram(d.DiagramID);
    Session.Output('CUN07: '+TYPES.length+' types and '+LINKS.length+' connectors created. Signature readback OK.');
    Session.Prompt('CUN07 DC created.\n'+TYPES.length+' types, '+LINKS.length+' connectors.\nExisting model content was retained.',promptOK);
}
try {main();} catch(error) {
    var message='CUN07 DC failed at '+STAGE+': '+(error.description || error.message || String(error));
    Session.Output(message);Session.Prompt(message+'\nAny partially generated elements were retained for inspection.',promptOK);
}
