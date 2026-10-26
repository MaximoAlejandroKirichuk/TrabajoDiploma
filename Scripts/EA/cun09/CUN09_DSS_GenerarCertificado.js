!INC Local Scripts.EAConstants-JScript
// DSS del CUN09 "Generar certificado de finalizacion". Fiel al codigo y a los metodos del diagrama de clases del CUN09.
// Reglas del profesor: lifelines = actor, formulario, BLL, Bitacora, Integridad y una unica KI68BaseDeDatos; sin estereotipos;
// sin validaciones en el formulario; el registro no devuelve datos; Begin/Commit solo al registrar, con la integridad antes del Commit
// y la Bitacora despues. Crea paquetes NUEVOS (no pisa nada). Los fragmentos alt se dibujan a mano (ver Notes del diagrama y la salida).
// DATOS: todo se declara antes de la logica.
var ROOT_NAME = 'CUN09 - DSS desde codigo';
var LL_W = 220, LL_GAP = 60, LL_TOP = 40, MSG_Y0 = 150, MSG_STEP = 46;
var CURRENT_STAGE = 'Starting script', CREATED_PACKAGE = null, DESTINATION_PACKAGE = null;
var LIFELINES = {ACT:{name:'Coordinadora Academica',kind:'Actor'},FRM:{name:'FrmGenerarCertificado_83KI',kind:'Lifeline',cls:'FrmGenerarCertificado_83KI'},BLL:{name:'KI68CertificadoBLL',kind:'Lifeline',cls:'KI68CertificadoBLL'},BIT:{name:'BitacoraBLL_83KI',kind:'Lifeline',cls:'BitacoraBLL_83KI'},INT:{name:'IntegridadDAL_83KI',kind:'Lifeline',cls:'IntegridadDAL_83KI'},BD:{name:'KI68BaseDeDatos',kind:'Lifeline'}};
var DIAGRAMS = [
 {name:'CUN09 - DSS Generar certificado',pkg:'CUN09 - DSS Generar',title:'Escenario principal con sus alternativas (2.1, 4.1, 6.1, 8.1, 8.2, 8.3, 11.1)',lifelines:['ACT','FRM','BLL','BIT','INT','BD'],messages:[{f:'ACT',t:'FRM',m:'Seleccionar "Generar certificado de finalizacion"',k:'call',n:1},{f:'FRM',t:'FRM',m:'OnShown(e)',k:'self',op:'FrmGenerarCertificado_83KI.OnShown',n:2},{f:'FRM',t:'BLL',m:'ListarFinalizadas()',k:'call',op:'KI68CertificadoBLL.ListarFinalizadas',n:3},{f:'BLL',t:'BLL',m:'Autorizar()',k:'self',op:'KI68CertificadoBLL.Autorizar',n:4},{f:'BLL',t:'BD',m:'ExecuteQuery(comisiones con fecha fin <= hoy)',k:'call',n:5},{f:'BD',t:'BLL',m:'comisiones',k:'return',n:6},{f:'BLL',t:'FRM',m:'comisiones',k:'return',n:7},{f:'FRM',t:'ACT',m:'Informar que no hay comisiones finalizadas',k:'return',n:8},{f:'FRM',t:'ACT',m:'Mostrar pantalla con el listado de comisiones',k:'return',n:9},{f:'ACT',t:'FRM',m:'Seleccionar la comision y presionar Buscar',k:'call',n:10},{f:'FRM',t:'FRM',m:'Buscar()',k:'self',op:'FrmGenerarCertificado_83KI.Buscar',n:11},{f:'FRM',t:'BLL',m:'Buscar(idComision)',k:'call',op:'KI68CertificadoBLL.Buscar',n:12},{f:'BLL',t:'BLL',m:'Autorizar()',k:'self',op:'KI68CertificadoBLL.Autorizar',n:13},{f:'BLL',t:'BD',m:'ExecuteQuery(inscripciones con dos evaluaciones)',k:'call',n:14},{f:'BD',t:'BLL',m:'alumnos',k:'return',n:15},{f:'BLL',t:'FRM',m:'alumnos',k:'return',n:16},{f:'FRM',t:'ACT',m:'Informar que no hay resultados',k:'return',n:17},{f:'FRM',t:'ACT',m:'Mostrar listado de alumnos',k:'return',n:18},{f:'ACT',t:'FRM',m:'Seleccionar un alumno',k:'call',n:19},{f:'FRM',t:'FRM',m:'Seleccionar()',k:'self',op:'FrmGenerarCertificado_83KI.Seleccionar',n:20},{f:'FRM',t:'BLL',m:'Seleccionar(id)',k:'call',op:'KI68CertificadoBLL.Seleccionar',n:21},{f:'BLL',t:'BLL',m:'Autorizar()',k:'self',op:'KI68CertificadoBLL.Autorizar',n:22},{f:'BLL',t:'BD',m:'ExecuteQuery(inscripcion, notas, cuotas, observaciones y certificado)',k:'call',n:23},{f:'BD',t:'BLL',m:'datos de la inscripcion',k:'return',n:24},{f:'BLL',t:'BLL',m:'ValidarEvaluaciones(a)',k:'self',op:'KI68CertificadoBLL.ValidarEvaluaciones',n:25},{f:'BLL',t:'FRM',m:'alumno con certificado',k:'return',n:26},{f:'FRM',t:'ACT',m:'Mostrar certificado original y habilitar Guardar certificado',k:'return',n:27},{f:'BLL',t:'FRM',m:'alumno con motivos y cuotas pendientes',k:'return',n:28},{f:'FRM',t:'ACT',m:'Mostrar motivos y cuotas pendientes y habilitar Retener',k:'return',n:29},{f:'BLL',t:'BLL',m:'PrefijoFolio(a)',k:'self',op:'KI68CertificadoBLL.PrefijoFolio',n:30},{f:'BLL',t:'FRM',m:'alumno',k:'return',n:31},{f:'FRM',t:'ACT',m:'Precargar datos y prefijo del folio y habilitar los campos',k:'return',n:32},{f:'ACT',t:'FRM',m:'Ingresar folio, modalidad, email, mencion y observaciones y presionar Generar',k:'call',n:33},{f:'FRM',t:'FRM',m:'Generar()',k:'self',op:'FrmGenerarCertificado_83KI.Generar',n:34},{f:'FRM',t:'BLL',m:'Generar(r)',k:'call',op:'KI68CertificadoBLL.Generar',n:35},{f:'BLL',t:'BLL',m:'Autorizar()',k:'self',op:'KI68CertificadoBLL.Autorizar',n:36},{f:'BLL',t:'BLL',m:'Validar(r)',k:'self',op:'KI68CertificadoBLL.Validar',n:37},{f:'BLL',t:'FRM',m:'error de datos',k:'return',n:38},{f:'FRM',t:'ACT',m:'Informar el error y volver a ingresar los datos',k:'return',n:39},{f:'BLL',t:'BD',m:'BeginTransaction()',k:'call',n:40},{f:'BLL',t:'BD',m:'ExecuteQuery(inscripcion, notas, cuotas y certificado)',k:'call',n:41},{f:'BD',t:'BLL',m:'datos de la inscripcion',k:'return',n:42},{f:'BLL',t:'BLL',m:'ValidarEvaluaciones(a)',k:'self',op:'KI68CertificadoBLL.ValidarEvaluaciones',n:43},{f:'BLL',t:'BD',m:'RollbackTransaction()',k:'call',n:44},{f:'BLL',t:'FRM',m:'certificado existente',k:'return',n:45},{f:'FRM',t:'ACT',m:'Informar y mostrar el certificado original',k:'return',n:46},{f:'BLL',t:'BD',m:'RollbackTransaction()',k:'call',n:47},{f:'BLL',t:'FRM',m:'inscripcion no habilitada',k:'return',n:48},{f:'FRM',t:'ACT',m:'Informar el cambio y mostrar las cuotas pendientes',k:'return',n:49},{f:'BLL',t:'BLL',m:'ComponerFolio(a, numero)',k:'self',op:'KI68CertificadoBLL.ComponerFolio',n:50},{f:'BLL',t:'BD',m:'ExecuteNonQuery(INSERT CertificadoFinalizacion)',k:'call',n:51},{f:'BD',t:'BLL',m:'folio duplicado',k:'return',n:52},{f:'BLL',t:'BD',m:'RollbackTransaction()',k:'call',n:53},{f:'BLL',t:'FRM',m:'folio duplicado',k:'return',n:54},{f:'FRM',t:'ACT',m:'Informar que el folio ya fue usado',k:'return',n:55},{f:'BLL',t:'INT',m:'ActualizarCertificado(id, conn, tran)',k:'call',op:'IntegridadDAL_83KI.ActualizarCertificado',n:56},{f:'INT',t:'BD',m:'ExecuteNonQuery(DVH y DVV de CertificadoFinalizacion)',k:'call',n:57},{f:'BLL',t:'BD',m:'CommitTransaction()',k:'call',n:58},{f:'BLL',t:'BLL',m:'Registrar(accion, a, detalle)',k:'self',op:'KI68CertificadoBLL.Registrar',n:59},{f:'BLL',t:'BIT',m:'RegistrarEvento(evento)',k:'call',op:'BitacoraBLL_83KI.RegistrarEvento',n:60},{f:'BIT',t:'BD',m:'ExecuteNonQuery(INSERT Bitacora)',k:'call',n:61},{f:'BLL',t:'FRM',m:'confirmacion',k:'return',n:62},{f:'FRM',t:'ACT',m:'Informar que el certificado fue generado y habilitar Guardar certificado',k:'return',n:63},{f:'ACT',t:'FRM',m:'Presionar Guardar certificado',k:'call',n:64},{f:'FRM',t:'FRM',m:'Guardar()',k:'self',op:'FrmGenerarCertificado_83KI.Guardar',n:65},{f:'FRM',t:'FRM',m:'NombreArchivo(c)',k:'self',op:'CertificadoPdfExporter_83KI.NombreArchivo',n:66},{f:'FRM',t:'ACT',m:'Abrir el dialogo de guardado',k:'return',n:67},{f:'ACT',t:'FRM',m:'Confirmar la ubicacion',k:'call',n:68},{f:'FRM',t:'FRM',m:'Exportar(c, path)',k:'self',op:'CertificadoPdfExporter_83KI.Exportar',n:69},{f:'FRM',t:'ACT',m:'Informar la cancelacion o el error (el certificado no cambia)',k:'return',n:70},{f:'FRM',t:'ACT',m:'Mostrar confirmacion de guardado',k:'return',n:71}],frames:[{id:'A1',op:'alt',depth:1,first:8,last:9,operands:[{guard:'[no hay comisiones finalizadas]',ecu:'2.1',first:8},{guard:'[hay comisiones]',ecu:'',first:9}]},{id:'B1',op:'alt',depth:1,first:17,last:18,operands:[{guard:'[sin resultados]',ecu:'4.1',first:17},{guard:'[con resultados]',ecu:'',first:18}]},{id:'C1',op:'alt',depth:1,first:26,last:32,operands:[{guard:'[ya existe certificado]',ecu:'6.1',first:26},{guard:'[desaprobado o saldo pendiente > 0]',ecu:'5.1 / 6.2',first:28},{guard:'[aprobado sin saldo y sin certificado]',ecu:'principal',first:30}]},{id:'D1',op:'alt',depth:1,first:38,last:63,operands:[{guard:'[datos invalidos]',ecu:'8.1',first:38},{guard:'[datos validos]',ecu:'',first:40}]},{id:'D2',op:'alt',depth:2,first:44,last:63,operands:[{guard:'[ya existe certificado]',ecu:'8.3',first:44},{guard:'[con saldo pendiente o no aprobado]',ecu:'8.2',first:47},{guard:'[aprobado, sin saldo y sin certificado]',ecu:'principal',first:50}]},{id:'D3',op:'alt',depth:3,first:52,last:63,operands:[{guard:'[folio ya usado]',ecu:'8.1',first:52},{guard:'[folio disponible]',ecu:'principal',first:56}]},{id:'E1',op:'alt',depth:1,first:70,last:71,operands:[{guard:'[cancelado o error al escribir el archivo]',ecu:'11.1',first:70},{guard:'[archivo guardado]',ecu:'principal',first:71}]}],blocks:[[1,'A. Abrir la pantalla (ECU 1 y 2)'],[10,'B. Buscar alumnos (ECU 3 y 4)'],[19,'C. Seleccionar alumno (ECU 5 y 6)'],[33,'D. Generar certificado (ECU 7, 8 y 9)'],[64,'E. Guardar certificado (ECU 10 y 11)']]},
 {name:'CUN09 - DSS Retener certificado',pkg:'CUN09 - DSS Retener',title:'Alternativas 5.1 y 6.2 (retencion)',lifelines:['ACT','FRM','BLL','BIT','BD'],messages:[{f:'ACT',t:'FRM',m:'Ingresar observaciones y presionar Retener',k:'call',n:1},{f:'FRM',t:'FRM',m:'Retener()',k:'self',op:'FrmGenerarCertificado_83KI.Retener',n:2},{f:'FRM',t:'BLL',m:'Retener(id, observaciones)',k:'call',op:'KI68CertificadoBLL.Retener',n:3},{f:'BLL',t:'BLL',m:'Autorizar()',k:'self',op:'KI68CertificadoBLL.Autorizar',n:4},{f:'BLL',t:'FRM',m:'error de datos',k:'return',n:5},{f:'FRM',t:'ACT',m:'Informar el error y volver a ingresar las observaciones',k:'return',n:6},{f:'BLL',t:'BD',m:'ExecuteQuery(inscripcion, notas, cuotas y certificado)',k:'call',n:7},{f:'BD',t:'BLL',m:'datos de la inscripcion',k:'return',n:8},{f:'BLL',t:'BLL',m:'ValidarEvaluaciones(a)',k:'self',op:'KI68CertificadoBLL.ValidarEvaluaciones',n:9},{f:'BLL',t:'FRM',m:'reconfirmar motivos',k:'return',n:10},{f:'FRM',t:'ACT',m:'Informar el cambio y mostrar los motivos actualizados',k:'return',n:11},{f:'BLL',t:'BLL',m:'Registrar(accion, a, detalle)',k:'self',op:'KI68CertificadoBLL.Registrar',n:12},{f:'BLL',t:'BIT',m:'RegistrarEvento(evento)',k:'call',op:'BitacoraBLL_83KI.RegistrarEvento',n:13},{f:'BIT',t:'BD',m:'ExecuteNonQuery(INSERT Bitacora)',k:'call',n:14},{f:'BLL',t:'FRM',m:'confirmacion',k:'return',n:15},{f:'FRM',t:'ACT',m:'Informar que la retencion fue asentada',k:'return',n:16}],frames:[{id:'F1',op:'alt',depth:1,first:5,last:16,operands:[{guard:'[observaciones > 500 caracteres]',ecu:'5.1.4.2 / 6.2.4.2',first:5},{guard:'[observaciones validas]',ecu:'',first:7}]},{id:'F2',op:'alt',depth:2,first:10,last:16,operands:[{guard:'[cambiaron los motivos]',ecu:'5.1.4.1 / 6.2.4.1',first:10},{guard:'[los motivos continuan]',ecu:'principal',first:12}]}],blocks:[[1,'F. Retener certificado (ECU 5.1 y 6.2)']]}
];
// LOGICA: todos los datos se declaran antes de esta linea.
function progress(stage) {CURRENT_STAGE=stage;Session.Output('CUN09 DSS | '+stage);}
function describe(error) {return error.description || error.message || String(error);}
function save(o,context) {
 var detail='';
 if(!o.Update()) {
  try {detail=String(o.GetLastError());} catch(ignored) {}
  throw new Error('EA Update failed: '+context+(detail ? '\n'+detail : ''));
 }
}
function newPackage(parent,name) {var p=parent.Packages.AddNew(name,'Package');save(p,name);parent.Packages.Refresh();return p;}
function uniqueName(parent,base) {
 var name=base,n=1,i,exists;
 do {exists=false;for(i=0;i<parent.Packages.Count;i++) if(parent.Packages.GetAt(i).Name==name) exists=true;if(exists) {n++;name=base+' ('+n+')';}} while(exists);
 return name;
}
function lifelineX(i) {return 40+i*(LL_W+LL_GAP);}
function messageY(n) {return MSG_Y0+(n-1)*MSG_STEP;}
function createLifeline(pkg,key) {
 var def=LIFELINES[key],el=null,kind=def.kind=='Actor' ? 'Actor' : 'Sequence';
 // Un lifeline de EA es un elemento de tipo Sequence; si esta version no lo acepta se usa Object (se dibuja igual como lifeline).
 try {el=pkg.Elements.AddNew(def.name,kind);} catch(typeError) {el=null;}
 if(!el && kind!='Actor') {progress('Type Sequence not accepted for '+def.name+'; using Object');el=pkg.Elements.AddNew(def.name,'Object');}
 if(!el) throw new Error('Elements.AddNew returned nothing for '+def.name);
 el.Notes=def.cls ? 'Lifeline de la clase '+def.cls+' (ver DC del CUN09).' : (key=='BD' ? 'Abstraccion academica de la base de datos (no es una clase del codigo): agrupa las consultas y la persistencia de las DAL.' : 'Actor principal del caso de uso.');
 save(el,def.name);pkg.Elements.Refresh();
 return el;
}
function createDiagram(pkg,d) {
 var dg,notes=[],i,j,f,o;
 progress('Diagram AddNew: '+d.name);
 pkg.Diagrams.Refresh();dg=pkg.Diagrams.AddNew(d.name,'Sequence');
 if(!dg) throw new Error('Diagrams.AddNew did not return a diagram: '+d.name);
 notes.push(d.title);notes.push('');
 notes.push('Fragmentos alt a dibujar (Toolbox > Combined Fragment); numeros = SequenceNo de los mensajes:');
 for(i=0;i<d.frames.length;i++) {
  f=d.frames[i];
  notes.push(f.id+' ('+f.op+', nivel '+f.depth+'): mensajes '+f.first+' a '+f.last);
  for(j=0;j<f.operands.length;j++) {o=f.operands[j];notes.push('   desde '+o.first+': '+o.guard+(o.ecu ? '   (ECU '+o.ecu+')' : ''));}
 }
 dg.Notes=notes.join('\n');save(dg,d.name);pkg.Diagrams.Refresh();
 return dg;
}
function placeLifeline(dg,el,index,height) {
 var l=lifelineX(index),o=dg.DiagramObjects.AddNew('l='+l+';r='+(l+LL_W)+';t='+LL_TOP+';b='+(LL_TOP+height)+';','');
 o.ElementID=el.ElementID;save(o,'placement of '+el.Name);
}
function setReturnStyle(con) {
 // El estilo punteado de "Return" depende de la version de EA; si no se acepta, queda como llamada y se cambia a mano.
 try {con.Subtype='Return';} catch(styleError) {return false;}
 return true;
}
function setPosition(con,y) {
 try {con.StartPointY=-y;con.EndPointY=-y;return true;} catch(positionError) {return false;}
}
function createMessage(elements,m) {
 var src=elements[m.f],dst=elements[m.t],con=src.Connectors.AddNew(m.m,'Sequence');
 con.SupplierID=dst.ElementID;
 save(con,'new message '+m.n+' '+m.m);
 con.SequenceNo=m.n;
 if(m.k=='return') setReturnStyle(con);
 setPosition(con,messageY(m.n));
 con.Notes=m.op ? 'Metodo del DC: '+m.op : '';
 save(con,'message '+m.n+' '+m.m);
 src.Connectors.Refresh();
 return con;
}
function buildDiagram(parent,d) {
 var pkg=newPackage(parent,d.pkg),elements={},dg,i,height;
 progress('Package '+pkg.Name+' (PackageID '+pkg.PackageID+')');
 for(i=0;i<d.lifelines.length;i++) elements[d.lifelines[i]]=createLifeline(pkg,d.lifelines[i]);
 dg=createDiagram(pkg,d);
 height=(MSG_Y0-LL_TOP)+d.messages.length*MSG_STEP+60;
 for(i=0;i<d.lifelines.length;i++) placeLifeline(dg,elements[d.lifelines[i]],i,height);
 dg.DiagramObjects.Refresh();save(dg,d.name);
 for(i=0;i<d.messages.length;i++) {
  if(i%10==0) progress(d.name+': message '+(i+1)+'/'+d.messages.length);
  createMessage(elements,d.messages[i]);
 }
 try {Repository.ReloadDiagram(dg.DiagramID);} catch(reloadError) {}
 return dg;
}
function main() {
 var parent,root,first=null,i,dg,total=0;
 progress('START - reading selected destination package');
 parent=Repository.GetTreeSelectedPackage();
 if(!parent) throw new Error('Select the destination CUN09 package in the Project Browser.');
 if(parent.Name.indexOf(ROOT_NAME)==0) throw new Error('Select the parent CU package, not a generated package.');
 DESTINATION_PACKAGE=parent;
 root=newPackage(parent,uniqueName(parent,ROOT_NAME));CREATED_PACKAGE=root;
 root.Notes='DSS del CUN09 proyectado desde el codigo y el DC del CUN09. Paquete nuevo: no se reutiliza contenido existente.';save(root,root.Name);
 Repository.RefreshModelView(parent.PackageID);
 for(i=0;i<DIAGRAMS.length;i++) {
  dg=buildDiagram(root,DIAGRAMS[i]);total+=DIAGRAMS[i].messages.length;
  if(!first) first=dg;
 }
 Repository.RefreshModelView(parent.PackageID);
 Repository.OpenDiagram(first.DiagramID);
 Session.Output('Created '+root.Name+': '+DIAGRAMS.length+' diagrams, '+total+' messages.');
 for(i=0;i<DIAGRAMS.length;i++) printFrames(DIAGRAMS[i]);
 Session.Output('Inspect message order/vertical positions and the Return style in live EA; offline checks cannot prove EA rendering.');
 Session.Prompt('CUN09 DSS generated.\nDestination: '+parent.Name+' / '+root.Name+'\n'+DIAGRAMS.length+' diagrams; '+total+' messages.\nAdd the alt fragments by hand (listed in the Output and in each diagram Notes).',promptOK);
}
function printFrames(d) {
 var i,j,f,o;
 Session.Output('--- '+d.name+' ---');
 for(i=0;i<d.blocks.length;i++) Session.Output('Block from message '+d.blocks[i][0]+': '+d.blocks[i][1]);
 for(i=0;i<d.frames.length;i++) {
  f=d.frames[i];Session.Output('Fragment '+f.id+' ('+f.op+', level '+f.depth+'): messages '+f.first+'-'+f.last);
  for(j=0;j<f.operands.length;j++) {o=f.operands[j];Session.Output('    from '+o.first+': '+o.guard+(o.ecu ? ' (ECU '+o.ecu+')' : ''));}
 }
}
function run() {
 var message;
 CURRENT_STAGE='Starting script';CREATED_PACKAGE=null;DESTINATION_PACKAGE=null;
 try {main();} catch(error) {
  message='CUN09 DSS failed.\nStage: '+CURRENT_STAGE+'\n'+describe(error)+(typeof error.number!='undefined' ? '\nError number: '+error.number : '');
  if(CREATED_PACKAGE) message+='\nPartial package: '+CREATED_PACKAGE.Name+' (PackageID '+CREATED_PACKAGE.PackageID+'). It has been retained for inspection.';
  try {if(DESTINATION_PACKAGE) Repository.RefreshModelView(DESTINATION_PACKAGE.PackageID);} catch(refreshError) {}
  try {Session.Output(message);} catch(outputError) {}
  try {Session.Prompt(message,promptOK);} catch(promptError) {}
  throw error;
 }
}
run();
