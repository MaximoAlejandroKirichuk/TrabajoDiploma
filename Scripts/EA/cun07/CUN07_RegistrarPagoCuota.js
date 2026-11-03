!INC Local Scripts.EAConstants-JScript
// Instantanea del codigo fuente del CUN07 "Registrar pago de cuota". Diagrama de clases con las clases reales que participan.
// Se excluyen la UI, SessionManager y los constructores. Las abstracciones del DSS (IntegridadBLL, base de datos) no se inventan como operaciones.
// DATOS: visibilidad de miembros, metadatos de readonly/static/setter y evidencia del codigo fuente.
var PACKAGE_NAME = 'CUN07 - DC desde codigo';
var DIAGRAM_NAME = 'CUN07 - Registrar pago de cuota';
var GAP_X = 130, GAP_Y = 130;
// Estilo CUN07: clases en durazno, interfaces en lila. Sin etiquetas de paquete porque todo vive en un solo paquete.
var DIAGRAM_STYLE = 'OpParams=1;UseAlias=0;';
var FILL = {Class:[253,240,228], Enumeration:[253,240,228], Interface:[222,203,246]};
var BORDER = {Class:[92,76,66], Enumeration:[92,76,66], Interface:[98,72,142]};
var CURRENT_STAGE = 'Starting script', CREATED_PACKAGE = null, DESTINATION_PACKAGE = null;
var TYPES = [
 {name:'PagoCuotaRequest_83KI',layer:'BE',row:0,order:1,source:'BE/DTOs/Pagos/PagoCuotaRequest_83KI.cs',complete:true,
  attributes:['+ DNI: string','+ CodigoComision: string','+ IdCuota: int','+ MetodoPago: string','+ MontoRecibido: decimal','+ NumeroReferencia: string','+ FechaPago: DateTime','+ FechaAcreditacion: DateTime'],methods:[]},
 {name:'BusquedaCuotasPagoResultado_83KI',layer:'BE',row:4,order:1,source:'BE/DTOs/Pagos/BusquedaCuotasPagoResultado_83KI.cs',complete:true,
  attributes:['+ Alumno: Alumno_83KI','+ Solicitud: SolicitudInscripcion_83KI','+ CuotasPendientes: List<Cuota_83KI>','+ Cuotas: List<Cuota_83KI>','+ NombrePlanDePago: string','+ MatriculaPagada: bool','+ CodigoComision: string','+ Curso: string'],methods:[]},
 {name:'PagoCuotaResultado_83KI',layer:'BE',row:2,order:0,source:'BE/DTOs/Pagos/PagoCuotaResultado_83KI.cs',complete:true,
  attributes:['+ Pago: PagoInscripcion_83KI','+ Cuota: Cuota_83KI','+ Solicitud: SolicitudInscripcion_83KI','+ Alumno: Alumno_83KI','+ CodigoComision: string','+ Curso: string','+ CodigoSolicitud: string','+ AuditoriaFallida: bool','+ NumeroReciboInstitucional: string','+ FacturaTexto: string','+ FacturaConcepto: string','+ FacturaTotal: decimal','+ FacturaFecha: DateTime','+ IdFactura: int'],methods:[]},
 {name:'ComisionPagoOpcion_83KI',layer:'BE',row:0,order:3,source:'BE/DTOs/Comisiones/ComisionPagoOpcion_83KI.cs',complete:true,
  attributes:['+ Codigo: string','+ Descripcion: string'],methods:[]},
 {name:'IInscripcionBLL_83KI',kind:'Interface',layer:'Services',row:1,order:1,source:'Service/Interfaces/IInscripcionBLL_83KI.cs',
  attributes:[],methods:['+ ObtenerPorDniYComision(dni: string, codigoComision: string): SolicitudInscripcion_83KI']},
 {name:'KI68InscripcionBLL',layer:'BLL',row:1,order:2,source:'BLL/Negocio/KI68InscripcionBLL.cs',
  note:'CUN07 projection. Enrollment creation, initial fee preview and dependencies used only by other operations are omitted.',
  attributes:['- _inscripcionDAL: IInscripcionDAL_83KI'],methods:['+ ObtenerPorDniYComision(dni: string, codigoComision: string): SolicitudInscripcion_83KI']},
 {name:'IPagoCuotaBLL_83KI',kind:'Interface',layer:'Services',row:0,order:2,source:'Service/Interfaces/IPagoCuotaBLL_83KI.cs',
  attributes:[],methods:['+ ListarComisionesConCuotasPendientes(dni: string): IEnumerable<ComisionPagoOpcion_83KI>','+ BuscarCuotasPendientes(dni: string, codigoComision: string): BusquedaCuotasPagoResultado_83KI','+ RegistrarPagoCuota(request: PagoCuotaRequest_83KI): PagoCuotaResultado_83KI']},
 {name:'KI68PagoCuotaBLL_83KI',layer:'BLL',row:1,order:0,source:'BLL/Negocio/KI68PagoCuotaBLL_83KI.cs',
  note:'SessionManager and constructors omitted by request. Authorization remains implemented.',
  attributes:['- _pagoDAL: IPagoCuotaDAL_83KI','- _bitacora: IBitacoraManager_83KI','- _inscripcionBLL: IInscripcionBLL_83KI','- _cuotaBLL: ICuotaBLL_83KI'],methods:['+ ListarComisionesConCuotasPendientes(dni: string): IEnumerable<ComisionPagoOpcion_83KI>','+ BuscarCuotasPendientes(dni: string, codigoComision: string): BusquedaCuotasPagoResultado_83KI','+ RegistrarPagoCuota(request: PagoCuotaRequest_83KI): PagoCuotaResultado_83KI','- ValidarDatosPago(request: PagoCuotaRequest_83KI): void {static}','- ValidarSolicitudBusqueda(solicitud: SolicitudInscripcion_83KI, busqueda: BusquedaCuotasPagoResultado_83KI): void {static}','- ValidarPermiso(): void']},
 {name:'ICuotaBLL_83KI',kind:'Interface',layer:'Services',row:2,order:1,source:'Service/Interfaces/ICuotaBLL_83KI.cs',
  attributes:[],methods:['+ ValidarPagoCompleto(cuota: Cuota_83KI, monto: decimal): void']},
 {name:'KI68CuotaBLL',layer:'BLL',row:2,order:2,source:'BLL/Negocio/KI68CuotaBLL.cs',
  note:'Only full-payment validation participates in CUN07. PrevisualizarCuotaInicial belongs to another flow.',
  attributes:[],methods:['+ ValidarPagoCompleto(cuota: Cuota_83KI, monto: decimal): void']},
 {name:'IInscripcionDAL_83KI',kind:'Interface',layer:'Services',row:1,order:3,source:'Service/Interfaces/IInscripcionDAL_83KI.cs',
  attributes:[],methods:['+ ObtenerPorDniYComision(dni: string, codigoComision: string): SolicitudInscripcion_83KI']},
 {name:'InscripcionDAL_83KI',layer:'DAL',row:1,order:4,source:'DAL/InscripcionDAL_83KI.cs',
  attributes:['- _accesoDAL: AccesoDAL_83KI'],methods:['+ ObtenerPorDniYComision(dni: string, codigoComision: string): SolicitudInscripcion_83KI','- MapearSolicitud(row: DataRow): SolicitudInscripcion_83KI {static}','- Normalizar(value: string): string {static}']},
 {name:'IPagoCuotaDAL_83KI',kind:'Interface',layer:'Services',row:2,order:3,source:'Service/Interfaces/IPagoCuotaDAL_83KI.cs',
  attributes:[],methods:['+ ListarComisionesConCuotasPendientes(dni: string): IEnumerable<ComisionPagoOpcion_83KI>','+ BuscarCuotasPendientes(dni: string, codigoComision: string): BusquedaCuotasPagoResultado_83KI','+ ExisteReferenciaPago(numeroReferencia: string): bool','+ RegistrarPago(pago: PagoInscripcion_83KI): PagoCuotaResultado_83KI']},
 {name:'PagoCuotaDAL_83KI',layer:'DAL',row:3,order:4,source:'DAL/PagoCuotaDAL_83KI.cs',
  note:'RegistrarPago executes sp_CUN07_RegistrarPago and recomputes integrity in the same transaction.',
  attributes:['- _accesoDAL: AccesoDAL_83KI','- _integridadDAL: IntegridadDAL_83KI'],methods:['+ ListarComisionesConCuotasPendientes(dni: string): IEnumerable<ComisionPagoOpcion_83KI>','+ BuscarCuotasPendientes(dni: string, codigoComision: string): BusquedaCuotasPagoResultado_83KI','+ ExisteReferenciaPago(numeroReferencia: string): bool','+ RegistrarPago(pago: PagoInscripcion_83KI): PagoCuotaResultado_83KI','- CrearParametrosRegistro(pago: PagoInscripcion_83KI): List<SqlParameter> {static}','- MapearBusqueda(row: DataRow): BusquedaCuotasPagoResultado_83KI {static}','- MapearCuota(row: DataRow): Cuota_83KI {static}','- MapearResultado(row: DataRow): PagoCuotaResultado_83KI {static}','- MapearSolicitud(row: DataRow, idAlumno: int, idLead: int?): SolicitudInscripcion_83KI {static}','- RecomputarIntegridad(tabla: string, pk: string, id: int, conn: SqlConnection, tran: SqlTransaction): void','- Normalizar(value: string): string {static}']},
 {name:'IBitacoraManager_83KI',kind:'Interface',layer:'Services',row:0,order:4,source:'Service/Interfaces/IBitacoraManager_83KI.cs',
  attributes:[],methods:['+ RegistrarEvento(evento: BitacoraEvento_83KI): void']},
 {name:'BitacoraBLL_83KI',layer:'BLL',row:0,order:5,source:'BLL/BitacoraBLL_83KI.cs',
  attributes:['- _bitacoraDAL: IBitacoraDAL_83KI'],methods:['+ RegistrarEvento(evento: BitacoraEvento_83KI): void']},
 {name:'Alumno_83KI',layer:'BE',row:3,order:1,source:'BE/Entidades/Alumno_83KI.cs',
  note:'Entity state projection. Reconstruction methods omitted for readability.',
  attributes:['+ IdAlumno: int','+ DNI: string','+ Nombre: string','+ Apellido: string','+ Email: string','+ Telefono: string','+ FechaAlta: DateTime','+ Estado: string','+ DVH: string'],methods:[]},
 {name:'SolicitudInscripcion_83KI',layer:'BE',row:4,order:0,source:'BE/Entidades/SolicitudInscripcion_83KI.cs',
  note:'Entity state projection; CUN07 does not calculate administrative validity.',
  attributes:['+ IdSolicitudInscripcion: int','+ Codigo: string','+ IdAlumno: int','+ IdLeadOrigen: int?','+ IdComision: int','+ IdPlanDePago: int','+ RecargoPlanSnapshot: decimal','+ FechaSolicitud: DateTime','+ Estado: string','+ Observaciones: string','+ DVH: string','+ ArancelCursoSnapshot: decimal?','+ CantidadCuotasSnapshot: int?','+ TotalCursoSnapshot: decimal?','+ FechaPrimerVencimientoSnapshot: DateTime?','+ NombrePlanSnapshot: string'],methods:[]},
 {name:'Cuota_83KI',layer:'BE',row:3,order:2,source:'BE/Entidades/Cuota_83KI.cs',
  note:'EsMatricula and NumeroMensualidad are computed read-only properties. Reconstruction methods/constants omitted.',
  attributes:['+ IdCuota: int','+ IdSolicitudInscripcion: int','+ NumeroCuota: int','+ MontoOriginal: decimal','+ BalanceAdeudado: decimal','+ FechaVencimiento: DateTime','+ Estado: string','+ DVH: string','+ TipoConcepto: string','+ Periodo: DateTime?','+ EsMatricula: bool','+ NumeroMensualidad: int'],methods:[]},
 {name:'PagoInscripcion_83KI',layer:'BE',row:3,order:0,source:'BE/Entidades/PagoInscripcion_83KI.cs',
  note:'Shared persisted payment entity, deliberately not renamed. Only operations directly used by PagoCuotaBLL are expanded.',
  attributes:['+ IdPagoInscripcion: int','+ IdAlumno: int','+ IdSolicitudInscripcion: int','+ IdCuota: int','+ MetodoPago: string','+ MontoPagado: decimal','+ NumeroReferencia: string','+ FechaPago: DateTime','+ FechaAcreditacion: DateTime','+ CodigoComision: string','+ DVH: string'],methods:['+ Crear(idAlumno: int, idSolicitudInscripcion: int, idCuota: int, metodoPago: string, montoPagado: decimal, numeroReferencia: string, fechaPago: DateTime, fechaAcreditacion: DateTime, codigoComision: string): PagoInscripcion_83KI {static}','+ MetodoValido(metodoPago: string): bool {static}']},
 {name:'BitacoraEvento_83KI',layer:'Services',row:0,order:0,source:'Service/Entidades/BitacoraEvento_83KI.cs',
  attributes:['+ Id: int','+ Fecha: DateTime','+ Descripcion: string','+ Criticidad: Criticidad','+ Modulo: Modulo','+ Username: string'],methods:['+ CrearNuevo(descripcion: string, criticidad: Criticidad, modulo: Modulo, username: string): BitacoraEvento_83KI {static}']},
 {name:'AccesoDAL_83KI',visibility:'Package',layer:'DAL',row:3,order:5,source:'DAL/AccesoDAL_83KI.cs',
  note:'CUN07 access boundary projection. Optional defaults and connection-provider internals omitted.',
  attributes:[],methods:['+ Leer(consulta: string, parametros: List<SqlParameter>): DataSet','+ Escribir(consulta: string, parametros: List<SqlParameter>): int','+ LeerStoredProcedure(procedimiento: string, parametros: List<SqlParameter>): DataSet','~ EjecutarTransaccion(operacion: Action<SqlConnection, SqlTransaction>): void','~ LeerStoredProcedureTransaccional(conn: SqlConnection, tran: SqlTransaction, procedimiento: string, parametros: List<SqlParameter>): DataSet {static}','~ EscribirTransaccional(conn: SqlConnection, tran: SqlTransaction, consulta: string, parametros: List<SqlParameter>): int {static}']},
 {name:'IntegridadDAL_83KI',layer:'DAL',row:4,order:3,source:'DAL/IntegridadDAL_83KI.cs',
  note:'Real CUN07 integration boundary. DSS IntegridadBLL.ActualizarDVHyDVV is an academic abstraction, not an extra code operation. Internal hashing/configuration algorithms are not expanded.',
  attributes:['- _accesoDAL: AccesoDAL_83KI','- _encriptador: IEncriptador_83KI'],methods:['~ LeerFilaTransaccional(nombreTabla: string, whereClause: string, whereParams: List<SqlParameter>, conn: SqlConnection, tran: SqlTransaction): Dictionary<string, object> {static}','~ RecomputarIntegridadFila(nombreTabla: string, valoresColumna: Dictionary<string, object>, conn: SqlConnection, tran: SqlTransaction): void']},
 {name:'IBitacoraDAL_83KI',kind:'Interface',layer:'DAL',row:0,order:6,source:'DAL/interfaces/IBitacoraDAL_83KI.cs',
  attributes:[],methods:['+ Registrar(evento: BitacoraEvento_83KI): void']},
 {name:'BitacoraEventoDAL_83KI',layer:'DAL',row:0,order:7,source:'DAL/BitacoraEventoDAL_83KI.cs',
  attributes:['- _acceso: AccesoDAL_83KI'],methods:['+ Registrar(evento: BitacoraEvento_83KI): void']},
 {name:'IEncriptador_83KI',kind:'Interface',layer:'Services',row:4,order:2,source:'Service/Interfaces/IEncriptador_83KI.cs',
  attributes:[],methods:['+ CalcularHashIntegridad(texto: string): string']},
 {name:'Encriptador_83KI',layer:'Services',row:3,order:3,source:'Service/Encriptador_83KI.cs',
  attributes:[],methods:['+ CalcularHashIntegridad(texto: string): string']},
 {name:'SnapshotMapper_83KI',layer:'DAL',row:2,order:4,source:'DAL/SnapshotMapper_83KI.cs',
  attributes:[],methods:['~ Decimal(value: object): decimal? {static}','~ Integer(value: object): int? {static}','~ Date(value: object): DateTime? {static}','~ Text(value: object): string {static}']}
];
// Multiplicidades de referencias, no claves foraneas inferidas de SQL. La inyeccion no demuestra propiedad del ciclo de vida.
var LINKS = [
 ['KI68PagoCuotaBLL_83KI','IPagoCuotaBLL_83KI','Realisation','implements'],
 ['KI68InscripcionBLL','IInscripcionBLL_83KI','Realisation','implements'],
 ['KI68CuotaBLL','ICuotaBLL_83KI','Realisation','implements'],
 ['PagoCuotaDAL_83KI','IPagoCuotaDAL_83KI','Realisation','implements'],
 ['InscripcionDAL_83KI','IInscripcionDAL_83KI','Realisation','implements'],
 ['BitacoraBLL_83KI','IBitacoraManager_83KI','Realisation','implements'],
 ['BitacoraEventoDAL_83KI','IBitacoraDAL_83KI','Realisation','implements'],
 ['Encriptador_83KI','IEncriptador_83KI','Realisation','implements'],
 ['KI68PagoCuotaBLL_83KI','IPagoCuotaDAL_83KI','Association','_pagoDAL','0..*','0..1'],
 ['KI68PagoCuotaBLL_83KI','IInscripcionBLL_83KI','Association','_inscripcionBLL','0..*','0..1'],
 ['KI68PagoCuotaBLL_83KI','ICuotaBLL_83KI','Association','_cuotaBLL','0..*','0..1'],
 ['KI68PagoCuotaBLL_83KI','IBitacoraManager_83KI','Association','_bitacora','0..*','0..1'],
 ['KI68InscripcionBLL','IInscripcionDAL_83KI','Association','_inscripcionDAL','0..*','0..1'],
 ['InscripcionDAL_83KI','AccesoDAL_83KI','Association','_accesoDAL','0..*','1'],
 ['PagoCuotaDAL_83KI','AccesoDAL_83KI','Association','_accesoDAL','0..*','1'],
 ['PagoCuotaDAL_83KI','IntegridadDAL_83KI','Association','_integridadDAL','0..*','0..1'],
 ['BitacoraBLL_83KI','IBitacoraDAL_83KI','Association','_bitacoraDAL','0..*','0..1'],
 ['BitacoraEventoDAL_83KI','AccesoDAL_83KI','Association','_acceso','0..*','1'],
 ['IntegridadDAL_83KI','AccesoDAL_83KI','Association','_accesoDAL','0..*','1'],
 ['IntegridadDAL_83KI','IEncriptador_83KI','Association','_encriptador','0..*','0..1'],
 ['InscripcionDAL_83KI','SnapshotMapper_83KI','Dependency','Text / Integer / Date'],
 ['PagoCuotaDAL_83KI','SnapshotMapper_83KI','Dependency','Text / Decimal / Integer / Date'],
 ['KI68PagoCuotaBLL_83KI','PagoCuotaRequest_83KI','Dependency','RegistrarPagoCuota input'],
 ['KI68PagoCuotaBLL_83KI','PagoCuotaResultado_83KI','Dependency','RegistrarPagoCuota result'],
 ['KI68PagoCuotaBLL_83KI','PagoInscripcion_83KI','Dependency','RegistrarPagoCuota: Crear / MetodoValido'],
 ['KI68PagoCuotaBLL_83KI','ComisionPagoOpcion_83KI','Dependency','ListarComisiones result'],
 ['KI68PagoCuotaBLL_83KI','BitacoraEvento_83KI','Dependency','CrearNuevo'],
 ['KI68CuotaBLL','Cuota_83KI','Dependency','ValidarPagoCompleto'],
 ['BusquedaCuotasPagoResultado_83KI','Alumno_83KI','Association','Alumno','0..*','0..1'],
 ['BusquedaCuotasPagoResultado_83KI','SolicitudInscripcion_83KI','Association','Solicitud','0..*','0..1'],
 ['BusquedaCuotasPagoResultado_83KI','Cuota_83KI','Association','Cuotas','1','0..*'],
 ['BusquedaCuotasPagoResultado_83KI','Cuota_83KI','Association','CuotasPendientes','1','0..*'],
 ['PagoCuotaResultado_83KI','Alumno_83KI','Association','Alumno','0..*','0..1'],
 ['PagoCuotaResultado_83KI','SolicitudInscripcion_83KI','Association','Solicitud','0..*','0..1'],
 ['PagoCuotaResultado_83KI','Cuota_83KI','Association','Cuota','0..*','0..1'],
 ['PagoCuotaResultado_83KI','PagoInscripcion_83KI','Association','Pago','0..*','0..1']
];
// LÓGICA: todos los datos de la instantánea se declaran antes de esta línea.
function trim(s) { return String(s).replace(/^\s+|\s+$/g,''); }
function visibility(s) { return s=='-' ? 'Private' : s=='~' ? 'Package' : s=='#' ? 'Protected' : 'Public'; }
function splitParams(s) {
 var result=[],depth=0,start=0,i,c;
 if(!trim(s)) return result;
 for(i=0;i<s.length;i++) { c=s.charAt(i); if(c=='<') depth++; if(c=='>') depth--; if(c==',' && depth==0) {result.push(trim(s.substring(start,i)));start=i+1;} }
 result.push(trim(s.substring(start))); return result;
}
function attribute(s) {
 var flags=/\{([^}]+)\}/.exec(s),m=/^([+\-~#])\s+(\w+):\s*([^=]+?)(?:\s*=\s*(.*))?$/.exec(trim(s.replace(/\{[^}]*\}/g,'')));
 if(!m) throw new Error('Invalid attribute: '+s);
 return {name:m[2],type:trim(m[3]),visibility:visibility(m[1]),flags:flags ? flags[1] : '',value:m[4]};
}
function method(s) {
 var m=/^([+\-~#])\s+(\w+)\((.*)\)(?::\s*(.+))?$/.exec(trim(s.replace(/\{static\}/g,''))),parts,i,p,r=[];
 if(!m) throw new Error('Invalid operation: '+s);
 parts=splitParams(m[3]);
 for(i=0;i<parts.length;i++) {p=/^(\w+):\s*(.+?)(?:\s*=\s*(.+))?$/.exec(parts[i]);if(!p) throw new Error('Invalid parameter');r.push({name:p[1],type:p[2],value:p[3]});}
 return {name:m[2],type:m[4] || '',visibility:visibility(m[1]),isStatic:/\{static\}/.test(s),params:r};
}
function dimensions(d) {
 var max=d.name.length,i,a,m,ps,j;
 for(i=0;i<d.attributes.length;i++) {a=attribute(d.attributes[i]);max=Math.max(max,(a.name+': '+a.type).length);}
 for(i=0;i<d.methods.length;i++) {m=method(d.methods[i]);ps=[];for(j=0;j<m.params.length;j++) ps.push(m.params[j].type);max=Math.max(max,(m.name+'('+ps.join(', ')+'): '+m.type).length);}
 for(i=0;i<(d.literals || []).length;i++) max=Math.max(max,d.literals[i].length);
 return {w:Math.max(190,max*6+48),h:96+(d.attributes.length+d.methods.length+(d.literals || []).length)*16};
}
function rgb(c) { return c[0]+c[1]*256+c[2]*65536; }
function layout() {
 // Filas y orden dentro de cada fila vienen de TYPES (row/order): disposicion precalculada
 // para que las lineas se crucen lo menos posible. Aqui solo se miden las cajas y se acomodan.
 var rows={},keys=[],result=[],i,j,k,d,item,list,w,x,y=40,h,maxW=0,widths={},seen={},tmp;
 for(i=0;i<TYPES.length;i++) {
  d=TYPES[i];
  if(typeof d.row!='number' || typeof d.order!='number') throw new Error('Missing row/order: '+d.name);
  if(seen[d.row+'/'+d.order]) throw new Error('Duplicate slot: '+d.name);seen[d.row+'/'+d.order]=true;
  if(!rows[d.row]) {rows[d.row]=[];keys.push(d.row);}
  rows[d.row].push({d:d,size:dimensions(d)});
 }
 keys.sort(function(p,q) {return p-q;});
 for(i=0;i<keys.length;i++) {
  list=rows[keys[i]];
  for(j=1;j<list.length;j++) {tmp=list[j];k=j-1;while(k>=0 && list[k].d.order>tmp.d.order) {list[k+1]=list[k];k--;}list[k+1]=tmp;}
  w=0;for(j=0;j<list.length;j++) w+=list[j].size.w+(j ? GAP_X : 0);
  widths[keys[i]]=w;maxW=Math.max(maxW,w);
 }
 for(i=0;i<keys.length;i++) {
  list=rows[keys[i]];h=0;for(j=0;j<list.length;j++) h=Math.max(h,list[j].size.h);
  x=40+Math.round((maxW-widths[keys[i]])/2);
  for(j=0;j<list.length;j++) {
   item=list[j];k=y+Math.round((h-item.size.h)/2);
   result.push({name:item.d.name,l:x,r:x+item.size.w,t:k,b:k+item.size.h});x+=item.size.w+GAP_X;
  }
  y+=h+GAP_Y;
 }
 return result;
}
function validateModel(plan) {
 var names={},seen={},adj={},i,j,d,r,key,queue=[],n;
 for(i=0;i<TYPES.length;i++) {d=TYPES[i];if(names[d.name]) throw new Error('Duplicate type');names[d.name]=true;adj[d.name]=[];}
 for(i=0;i<LINKS.length;i++) {r=LINKS[i];key=r[0]+'|'+r[1]+'|'+r[2]+'|'+r[3];if(!names[r[0]] || !names[r[1]] || seen[key]) throw new Error('Invalid/duplicate edge: '+key);seen[key]=true;adj[r[0]].push(r[1]);adj[r[1]].push(r[0]);}
 seen={};queue.push(TYPES[0].name);while(queue.length) {n=queue.pop();if(seen[n]) continue;seen[n]=true;for(i=0;i<adj[n].length;i++) queue.push(adj[n][i]);}
 for(i=0;i<TYPES.length;i++) if(!seen[TYPES[i].name]) throw new Error('Disconnected type: '+TYPES[i].name);
 for(i=0;i<plan.length;i++) for(j=i+1;j<plan.length;j++) {d=plan[i];r=plan[j];if(!(d.r<=r.l || r.r<=d.l || d.b<=r.t || r.b<=d.t)) throw new Error('Overlapping boxes');}
}
function progress(stage) {CURRENT_STAGE=stage;Session.Output('CUN07 | '+stage);}
function save(o,context) {
 var detail='';
 if(!o.Update()) {
  try {detail=String(o.GetLastError());} catch(ignored) {}
  throw new Error('EA Update failed: '+context+(detail ? '\n'+detail : ''));
 }
}
function newPackage(parent,name) {var p=parent.Packages.AddNew(name,'Package');save(p,name);parent.Packages.Refresh();return p;}
function uniqueName(parent) {
 var name=PACKAGE_NAME,n=1,i,exists;
 do {exists=false;for(i=0;i<parent.Packages.Count;i++) if(parent.Packages.GetAt(i).Name==name) exists=true;if(exists) {n++;name=PACKAGE_NAME+' ('+n+')';}} while(exists);
 return name;
}
function createType(pkg,d) {
 var el=pkg.Elements.AddNew(d.name,d.kind || 'Class'),i,j,a,at,m,me,p,pa;
 el.Visibility=d.visibility || 'Public';el.Notes='Source: '+d.source+'\nLayer: '+d.layer+'\n'+(d.note || (d.complete ? 'Complete declared state.' : 'CUN07 operation/state projection.'));
 save(el,d.name);
 for(i=0;i<d.attributes.length;i++) {a=attribute(d.attributes[i]);at=el.Attributes.AddNew(a.name,a.type);at.Visibility=a.visibility;at.IsStatic=/static|const/.test(a.flags);at.IsConst=/const/.test(a.flags);if(/property/.test(a.flags)) at.Stereotype='property';if(typeof a.value!='undefined') at.Default=a.value;at.Notes=d.source+'\n'+a.flags;save(at,a.name);}
 for(i=0;i<(d.literals || []).length;i++) {a=d.literals[i].split('=');at=el.Attributes.AddNew(trim(a[0]),'');at.Visibility='Public';at.Stereotype='enum';at.Default=trim(a[1]);save(at,a[0]);}
 el.Attributes.Refresh();
 for(i=0;i<d.methods.length;i++) {m=method(d.methods[i]);me=el.Methods.AddNew(m.name,m.type);me.Visibility=m.visibility;me.IsStatic=m.isStatic;me.Abstract=d.kind=='Interface';if(m.name==d.name) me.Stereotype='create';me.Notes=d.source;save(me,m.name);
  for(j=0;j<m.params.length;j++) {p=m.params[j];pa=me.Parameters.AddNew(p.name,p.type);pa.Kind='in';pa.Position=j;if(typeof p.value!='undefined') pa.Default=p.value;save(pa,p.name);}me.Parameters.Refresh();save(me,m.name);
 } el.Methods.Refresh();save(el,d.name);pkg.Elements.Refresh();return el;
}
function connect(elements,r) {
 var con=elements[r[0]].Connectors.AddNew('',r[2]);con.SupplierID=elements[r[1]].ElementID;con.Notes=r[3];
 // Persistir el conector antes de actualizar sus extremos: comparten su fila en el repositorio.
 save(con,'new connector '+r[0]+' -> '+r[1]);
 if(r[2]=='Association') {try {con.Direction='Source -> Destination';} catch(directionError) {}con.ClientEnd.Cardinality=r[4];con.SupplierEnd.Cardinality=r[5];con.SupplierEnd.Role=r[3].split(';')[0];save(con.ClientEnd,'client');save(con.SupplierEnd,'supplier');}
 save(con,r[3]);elements[r[0]].Connectors.Refresh();
}
function diagram(pkg,name) {
 var d;
 progress('Diagram AddNew: '+name+' in PackageID '+pkg.PackageID);
 pkg.Diagrams.Refresh();d=pkg.Diagrams.AddNew(name,'Class');
 if(!d) throw new Error('Diagrams.AddNew did not return a diagram: '+name);
 progress('Diagram Update: '+name);save(d,name);
 progress('Diagram persisted: DiagramID '+d.DiagramID);
 pkg.Diagrams.Refresh();Repository.RefreshModelView(pkg.PackageID);
 // Asegurar que los compartimentos de atributos y operaciones no se excluyan por su visibilidad.
 try {d.ShowPrivate=true;d.ShowProtected=true;d.ShowPublic=true;save(d,name+' visibility');} catch(visibilityError) {}
 // Las preferencias de presentación no deben impedir la creación de un diagrama en otra versión de EA.
 try {d.ExtendedStyle=DIAGRAM_STYLE;d.Notes='CUN07 source projection. Class diagram projected from the real code (see ECU CUN07).';save(d,name+' style');}
 catch(styleError) {Session.Output('CUN07 | WARNING: diagram exists; presentation settings failed: '+(styleError.description || styleError.message || String(styleError)));}
 return d;
}
function place(d,id,p,t) {
 var o;
 progress('DiagramObject AddNew: '+(p.name || 'ElementID '+id)+' in DiagramID '+d.DiagramID);
 o=d.DiagramObjects.AddNew('l='+p.l+';r='+p.r+';t='+p.t+';b='+p.b+';','');
 o.ElementID=id;
 // Los colores son decorativos: un fallo aquí no debe detener la generación.
 try {o.BackgroundColor=rgb(FILL[t.kind || 'Class']);o.BorderColor=rgb(BORDER[t.kind || 'Class']);o.BorderLineWidth=1;}
 catch(styleError) {Session.Output('CUN07 | WARNING: colors not applied to '+p.name+': '+(styleError.description || styleError.message || String(styleError)));}
 save(o,'placement of ElementID '+id);
}
function readback(elements) {
 var i,j,k,d,el,a,m,found,at,me,pa;
 for(i=0;i<TYPES.length;i++) {d=TYPES[i];el=Repository.GetElementByID(elements[d.name].ElementID);el.Attributes.Refresh();el.Methods.Refresh();
  if(el.Attributes.Count!=d.attributes.length+(d.literals || []).length || el.Methods.Count!=d.methods.length) throw new Error('Missing persisted members: '+d.name);
  for(j=0;j<d.attributes.length;j++) {a=attribute(d.attributes[j]);found=false;for(k=0;k<el.Attributes.Count;k++) {at=el.Attributes.GetAt(k);if(at.Name==a.name && at.Type==a.type && at.Visibility==a.visibility) found=true;}if(!found) throw new Error('Attribute readback: '+d.name+'.'+a.name);}
  for(j=0;j<d.methods.length;j++) {m=method(d.methods[j]);found=false;for(k=0;k<el.Methods.Count;k++) {me=el.Methods.GetAt(k);if(me.Name!=m.name || me.ReturnType!=m.type || me.Visibility!=m.visibility || me.Parameters.Count!=m.params.length) continue;found=true;for(pa=0;pa<m.params.length;pa++) if(me.Parameters.GetAt(pa).Name!=m.params[pa].name || me.Parameters.GetAt(pa).Type!=m.params[pa].type) found=false;}if(!found) throw new Error('Operation readback: '+d.name+'.'+m.name);}
 }
}
function main() {
 var parent,plan,pkg,elements={},byName={},i,d,dc;
 progress('START - reading selected destination package');
 parent=Repository.GetTreeSelectedPackage();
 if(!parent) throw new Error('Select the destination CUN07 package in the Project Browser.');
 if(parent.Name.indexOf(PACKAGE_NAME)==0) throw new Error('Select the parent CU package, not a generated package.');
 DESTINATION_PACKAGE=parent;
 progress('Destination: '+parent.Name+' (PackageID '+parent.PackageID+')');
 progress('Calculating layout and validating graph');
 plan=layout();
 validateModel(plan);
 progress('Creating new package');
 pkg=newPackage(parent,uniqueName(parent));CREATED_PACKAGE=pkg;pkg.Notes='New CUN07 source projection. UI excluded by request. Existing content is never reused.';save(pkg,pkg.Name);
 Repository.RefreshModelView(parent.PackageID);
 progress('Created '+parent.Name+' / '+pkg.Name+' (PackageID '+pkg.PackageID+')');
 // Todos los tipos pertenecen a UN paquete, el mismo que contiene el diagrama: así EA no dibuja la etiqueta "(from Package)".
 for(i=0;i<TYPES.length;i++) {d=TYPES[i];byName[d.name]=d;progress('Creating type '+(i+1)+'/'+TYPES.length+': '+d.name);elements[d.name]=createType(pkg,d);}
 progress('Creating and placing class diagram');
 dc=diagram(pkg,DIAGRAM_NAME);for(i=0;i<plan.length;i++) place(dc,elements[plan[i].name].ElementID,plan[i],byName[plan[i].name]);dc.DiagramObjects.Refresh();save(dc,dc.Name);
 pkg.Diagrams.Refresh();Repository.RefreshModelView(parent.PackageID);
 for(i=0;i<LINKS.length;i++) {progress('Creating connector '+(i+1)+'/'+LINKS.length+': '+LINKS[i][0]+' -> '+LINKS[i][1]);connect(elements,LINKS[i]);}
 dc.DiagramObjects.Refresh();save(dc,dc.Name);pkg.Diagrams.Refresh();
 progress('Verifying persisted members');readback(elements);
 progress('Refreshing browser and opening diagram');
 Repository.RefreshModelView(parent.PackageID);Repository.OpenDiagram(dc.DiagramID);
 Session.Output('Created '+pkg.Name+': '+TYPES.length+' types, '+LINKS.length+' class connectors. Readback OK.');
 Session.Output('Inspect visible member compartments and connector routing in live EA; offline geometry is not rendering proof.');
 Session.Prompt('CUN07 generated successfully.\nDestination: '+parent.Name+' / '+pkg.Name+'\n'+TYPES.length+' types; '+LINKS.length+' class connectors.\nDiagram: '+DIAGRAM_NAME,promptOK);
}
function run() {
 var description,number,message;
 CURRENT_STAGE='Starting script';CREATED_PACKAGE=null;DESTINATION_PACKAGE=null;
 try {main();} catch(error) {
  description=error.description || error.message || String(error);
  number=typeof error.number!='undefined' ? '\nError number: '+error.number : '';
  message='CUN07 failed.\nStage: '+CURRENT_STAGE+'\n'+description+number;
  if(CREATED_PACKAGE) message+='\nPartial package: '+CREATED_PACKAGE.Name+' (PackageID '+CREATED_PACKAGE.PackageID+'). It has been retained for inspection.';
  try {if(DESTINATION_PACKAGE) Repository.RefreshModelView(DESTINATION_PACKAGE.PackageID);} catch(refreshError) {}
  try {Session.Output(message);} catch(outputError) {}
  try {Session.Prompt(message,promptOK);} catch(promptError) {}
  throw error;
 }
}
run();
