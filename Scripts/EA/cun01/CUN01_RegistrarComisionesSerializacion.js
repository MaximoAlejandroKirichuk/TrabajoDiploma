!INC Local Scripts.EAConstants-JScript
// Instantánea del código fuente de la ruta de serialización de CUN01 (A03). Se excluye deliberadamente la interfaz de usuario; no se dibujan clases XML del entorno de trabajo.
// DATOS: visibilidad de miembros, metadatos de readonly/static/setter y evidencia del código fuente.
var PACKAGE_NAME = 'CUN01 - DC desde codigo';
var DIAGRAM_NAME = 'CUN01 - Registrar comisiones (Serializacion)';
var GAP_X = 130, GAP_Y = 130;
// Estilo CUN07: clases en durazno, interfaces en lila. Sin etiquetas de paquete porque todo vive en un solo paquete.
var DIAGRAM_STYLE = 'OpParams=1;UseAlias=0;';
var FILL = {Class:[253,240,228], Enumeration:[253,240,228], Interface:[222,203,246]};
var BORDER = {Class:[92,76,66], Enumeration:[92,76,66], Interface:[98,72,142]};
var CURRENT_STAGE = 'Starting script', CREATED_PACKAGE = null, DESTINATION_PACKAGE = null;
var TYPES = [
 {name:'KI68ComisionBLL',layer:'BLL',row:1,order:1,source:'BLL/Negocio/KI68ComisionBLL.cs',
  note:'Only CUN01 serialization operations. Its preopening/evaluation operations and IComisionConsultaBLL_83KI are not expanded.',
  attributes:['- _comisionDAL: IComisionDAL_83KI {readonly}','- _sessionManager: ISessionManager_83KI {readonly}'],methods:[
  '+ KI68ComisionBLL(comisionDAL: IComisionDAL_83KI)',
  '+ KI68ComisionBLL(comisionDAL: IComisionDAL_83KI, sessionManager: ISessionManager_83KI)',
  '+ Serializar(ruta: string, comisiones: List<ComisionListado_83KI>): void',
  '+ Deserializar(ruta: string): List<ComisionXml_83KI>',
  '- CrearSerializador(): XmlSerializer {static}',
  '- ConvertirAXml(comision: Comision_83KI, listado: ComisionListado_83KI): ComisionXml_83KI {static}',
  '- ValidarPermiso(permiso: PermisoSistema_83KI): void']},
 {name:'IComisionSerializacionBLL_83KI',kind:'Interface',layer:'BLL',row:1,order:0,source:'Service/Interfaces/IComisionSerializacionBLL_83KI.cs',attributes:[],methods:[
  '+ Serializar(ruta: string, comisiones: List<ComisionListado_83KI>): void','+ Deserializar(ruta: string): List<ComisionXml_83KI>']},
 {name:'IComisionDAL_83KI',kind:'Interface',layer:'DAL',row:1,order:2,source:'Service/Interfaces/IComisionDAL_83KI.cs',
  note:'Only the operation reached by CUN01 serialization; the other contract operations belong to other use cases.',attributes:[],methods:[
  '+ ObtenerComision(idComision: int): Comision_83KI']},
 {name:'ComisionDAL_83KI',layer:'DAL',row:1,order:3,source:'DAL/ComisionDAL_83KI.cs',
  note:'Shared infrastructure state filtered to the CUN01 query (stored procedure sp_CUN01_ObtenerComision). _integridadDAL is not used by it.',
  attributes:['- _accesoDAL: AccesoDAL_83KI {readonly}'],methods:[
  '+ ObtenerComision(idComision: int): Comision_83KI','- MapearComision(row: DataRow): Comision_83KI {static}']},
 {name:'AccesoDAL_83KI',visibility:'Package',layer:'DAL',row:1,order:4,source:'DAL/AccesoDAL_83KI.cs',
  note:'Database access boundary. Only the operation directly reached from the CUN01 query. Connection/configuration internals omitted.',
  attributes:[],methods:[
  '~ AccesoDAL_83KI()','+ LeerStoredProcedure(procedimiento: string, parametros: List<SqlParameter> = null): DataSet']},
 {name:'ISessionManager_83KI',kind:'Interface',layer:'Services',row:2,order:2,source:'Service/Interfaces/ISessionManager_83KI.cs',
  note:'Only the permission check used by Serializar/Deserializar.',attributes:[],methods:[
  '+ TienePermiso(permiso: PermisoSistema_83KI): bool']},
 {name:'PermisoSistema_83KI',kind:'Enumeration',layer:'Services',row:2,order:1,source:'Service/Entidades/Enums.cs',attributes:[],methods:[],literals:['SerializarComisiones = 68','DeserializarComisiones = 69']},
 {name:'ComisionListado_83KI',layer:'BE',row:2,order:0,source:'BE/DTOs/Comisiones/ComisionListado_83KI.cs',complete:true,attributes:[
  '+ IdComision: int {property}','+ Codigo: string {property}','+ IdCurso: int {property}','+ Curso: string {property}',
  '+ IdProfesor: int {property}','+ Profesor: string {property}','+ DiaSemana: DayOfWeek {property}',
  '+ HoraInicio: TimeSpan {property}','+ HoraFin: TimeSpan {property}','+ CupoMinimo: int {property}','+ CupoMaximo: int {property}',
  '+ FechaLimitePago: DateTime {property}','+ FechaInicio: DateTime {property}','+ FechaFin: DateTime {property}',
  '+ ArancelBase: decimal {property}','+ MontoMatricula: decimal {property}','+ VacantesDisponibles: int {property}',
  '+ Estado: string {property}','+ Eliminada: bool {property readonly}','+ Descripcion: string {property readonly}'],methods:[]},
 {name:'ComisionXml_83KI',layer:'BE',row:0,order:0,source:'BE/DTOs/Comisiones/ComisionXml_83KI.cs',complete:true,
  note:'Complete declared state. XML-only class: [XmlType("Comision")]; TimeSpan values are [XmlIgnore] and travel as "HH:mm" text through HoraInicioXml/HoraFinXml.',attributes:[
  '+ Codigo: string {property}','+ IdCurso: int {property}','+ Curso: string {property}','+ IdProfesor: int {property}','+ Profesor: string {property}',
  '+ DiaSemana: DayOfWeek {property}','+ HoraInicio: TimeSpan {property}','+ HoraFin: TimeSpan {property}','+ CupoMinimo: int {property}','+ CupoMaximo: int {property}',
  '+ FechaLimitePago: DateTime {property}','+ FechaInicio: DateTime {property}','+ FechaFin: DateTime {property}',
  '+ ArancelBase: decimal {property}','+ MontoMatricula: decimal {property}','+ IdPlanDePago: int {property}','+ RecargoPlanSnapshot: decimal {property}','+ Estado: string {property}',
  '+ HoraInicioXml: string {property custom}','+ HoraFinXml: string {property custom}'],methods:[
  '- ConvertirHora(valor: string): TimeSpan {static}']},
 {name:'Comision_83KI',layer:'BE',row:0,order:1,source:'BE/Entidades/Comision_83KI.cs',complete:true,
  note:'Complete declared state, only executed reconstruction overload/private constructor. Private persistence validation internals not expanded.',attributes:[
  '+ EstadoPreapertura: string = "preapertura" {const}','+ EstadoConfirmada: string = "confirmada" {const}',
  '+ EstadoCanceladaFaltaQuorum: string = "cancelada_falta_quorum" {const}','+ EstadoEliminada: string = "eliminada" {const}',
  '+ IdComision: int {property private set}','+ Codigo: string {property private set}',
  '+ IdCurso: int {property private set}','+ IdProfesor: int {property private set}','+ DiaSemana: DayOfWeek {property private set}',
  '+ HoraInicio: TimeSpan {property private set}','+ HoraFin: TimeSpan {property private set}',
  '+ CupoMinimo: int {property private set}','+ CupoMaximo: int {property private set}',
  '+ FechaLimitePago: DateTime {property private set}','+ FechaInicio: DateTime {property private set}','+ FechaFin: DateTime {property private set}',
  '+ ArancelBase: decimal {property private set}','+ MontoMatricula: decimal {property private set}',
  '+ IdPlanDePago: int {property private set}','+ RecargoPlanSnapshot: decimal {property private set}',
  '+ Estado: string {property private set}','+ DVH: string {property private set}','+ EstaEliminada: bool {property readonly}'],methods:[
  '- Comision_83KI()',
  '+ ReconstruirDesdePersistencia(idComision: int, codigo: string, idCurso: int, idProfesor: int, diaSemana: DayOfWeek, horaInicio: TimeSpan, horaFin: TimeSpan, cupoMinimo: int, cupoMaximo: int, fechaLimitePago: DateTime, fechaInicio: DateTime, fechaFin: DateTime, arancelBase: decimal, montoMatricula: decimal, idPlanDePago: int, recargoPlanSnapshot: decimal, estado: string, dvh: string): Comision_83KI {static}']}
];
// Multiplicidades de referencias, no claves foráneas inferidas de SQL. La inyección no demuestra propiedad del ciclo de vida.
var LINKS = [
 ['KI68ComisionBLL','IComisionDAL_83KI','Association','_comisionDAL','0..*','0..1'],
 ['KI68ComisionBLL','ISessionManager_83KI','Association','_sessionManager','0..*','0..1'],
 ['KI68ComisionBLL','IComisionSerializacionBLL_83KI','Realisation','implements'],
 ['ComisionDAL_83KI','IComisionDAL_83KI','Realisation','implements'],
 ['ComisionDAL_83KI','AccesoDAL_83KI','Association','_accesoDAL','0..*','1'],
 ['KI68ComisionBLL','ComisionListado_83KI','Dependency','Serializar input'],
 ['KI68ComisionBLL','ComisionXml_83KI','Dependency','Deserializar result / ConvertirAXml'],
 ['KI68ComisionBLL','Comision_83KI','Dependency','Serializar: ObtenerComision result'],
 ['KI68ComisionBLL','PermisoSistema_83KI','Dependency','ValidarPermiso'],
 ['IComisionSerializacionBLL_83KI','ComisionListado_83KI','Dependency','Serializar input'],
 ['IComisionSerializacionBLL_83KI','ComisionXml_83KI','Dependency','Deserializar result'],
 ['IComisionDAL_83KI','Comision_83KI','Dependency','ObtenerComision result'],
 ['ISessionManager_83KI','PermisoSistema_83KI','Dependency','TienePermiso'],
 ['ComisionDAL_83KI','Comision_83KI','Dependency','MapearComision reconstruction']
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
 for(i=0;i<LINKS.length;i++) {r=LINKS[i];key=r[0]+'|'+r[1]+'|'+r[2];if(!names[r[0]] || !names[r[1]] || seen[key]) throw new Error('Invalid/duplicate edge: '+key);seen[key]=true;adj[r[0]].push(r[1]);adj[r[1]].push(r[0]);}
 seen={};queue.push(TYPES[0].name);while(queue.length) {n=queue.pop();if(seen[n]) continue;seen[n]=true;for(i=0;i<adj[n].length;i++) queue.push(adj[n][i]);}
 for(i=0;i<TYPES.length;i++) if(!seen[TYPES[i].name]) throw new Error('Disconnected type: '+TYPES[i].name);
 for(i=0;i<plan.length;i++) for(j=i+1;j<plan.length;j++) {d=plan[i];r=plan[j];if(!(d.r<=r.l || r.r<=d.l || d.b<=r.t || r.b<=d.t)) throw new Error('Overlapping boxes');}
}
function progress(stage) {CURRENT_STAGE=stage;Session.Output('CUN01 | '+stage);}
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
 el.Visibility=d.visibility || 'Public';el.Notes='Source: '+d.source+'\nLayer: '+d.layer+'\n'+(d.note || (d.complete ? 'Complete declared state.' : 'CUN01 operation/state projection.'));
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
 try {d.ExtendedStyle=DIAGRAM_STYLE;d.Notes='CUN01 source projection. See AUDIT.md for DSS differences and scope.';save(d,name+' style');}
 catch(styleError) {Session.Output('CUN01 | WARNING: diagram exists; presentation settings failed: '+(styleError.description || styleError.message || String(styleError)));}
 return d;
}
function place(d,id,p,t) {
 var o;
 progress('DiagramObject AddNew: '+(p.name || 'ElementID '+id)+' in DiagramID '+d.DiagramID);
 o=d.DiagramObjects.AddNew('l='+p.l+';r='+p.r+';t='+p.t+';b='+p.b+';','');
 o.ElementID=id;
 // Los colores son decorativos: un fallo aquí no debe detener la generación.
 try {o.BackgroundColor=rgb(FILL[t.kind || 'Class']);o.BorderColor=rgb(BORDER[t.kind || 'Class']);o.BorderLineWidth=1;}
 catch(styleError) {Session.Output('CUN01 | WARNING: colors not applied to '+p.name+': '+(styleError.description || styleError.message || String(styleError)));}
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
 if(!parent) throw new Error('Select the destination CUN01 package in the Project Browser.');
 if(parent.Name.indexOf(PACKAGE_NAME)==0) throw new Error('Select the parent CU package, not a generated package.');
 DESTINATION_PACKAGE=parent;
 progress('Destination: '+parent.Name+' (PackageID '+parent.PackageID+')');
 progress('Calculating layout and validating graph');
 plan=layout();
 validateModel(plan);
 progress('Creating new package');
 pkg=newPackage(parent,uniqueName(parent));CREATED_PACKAGE=pkg;pkg.Notes='New CUN01 source projection. UI excluded by request. Existing content is never reused.';save(pkg,pkg.Name);
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
 Session.Prompt('CUN01 generated successfully.\nDestination: '+parent.Name+' / '+pkg.Name+'\n'+TYPES.length+' types; '+LINKS.length+' class connectors.\nDiagram: '+DIAGRAM_NAME,promptOK);
}
function run() {
 var description,number,message;
 CURRENT_STAGE='Starting script';CREATED_PACKAGE=null;DESTINATION_PACKAGE=null;
 try {main();} catch(error) {
  description=error.description || error.message || String(error);
  number=typeof error.number!='undefined' ? '\nError number: '+error.number : '';
  message='CUN01 failed.\nStage: '+CURRENT_STAGE+'\n'+description+number;
  if(CREATED_PACKAGE) message+='\nPartial package: '+CREATED_PACKAGE.Name+' (PackageID '+CREATED_PACKAGE.PackageID+'). It has been retained for inspection.';
  try {if(DESTINATION_PACKAGE) Repository.RefreshModelView(DESTINATION_PACKAGE.PackageID);} catch(refreshError) {}
  try {Session.Output(message);} catch(outputError) {}
  try {Session.Prompt(message,promptOK);} catch(promptError) {}
  throw error;
 }
}
run();
