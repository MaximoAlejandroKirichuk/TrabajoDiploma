!INC Local Scripts.EAConstants-JScript
// Cambia el lifeline del formulario a "boundary" en el diagrama ABIERTO, sin tocar mensajes ni fragmentos.
// DATOS
var LIFELINE_NAME = 'FrmGenerarCertificado_83KI';
var NEW_STEREOTYPE = 'boundary';
// LOGICA
function main() {
 var dg=Repository.GetCurrentDiagram(),i,o,el,changed=0;
 if(!dg) throw new Error('Abri primero el diagrama de secuencia del CUN09 y volve a ejecutar el script.');
 dg.DiagramObjects.Refresh();
 for(i=0;i<dg.DiagramObjects.Count;i++) {
  o=dg.DiagramObjects.GetAt(i);
  el=Repository.GetElementByID(o.ElementID);
  if(el && el.Name==LIFELINE_NAME) {
   el.Stereotype=NEW_STEREOTYPE;
   if(!el.Update()) throw new Error('EA Update failed: '+el.GetLastError());
   changed++;
  }
 }
 if(changed==0) throw new Error('No encontre el lifeline '+LIFELINE_NAME+' en el diagrama "'+dg.Name+'".');
 Repository.ReloadDiagram(dg.DiagramID);
 Session.Output('Listo: '+changed+' lifeline(s) de '+LIFELINE_NAME+' ahora son '+NEW_STEREOTYPE+' en "'+dg.Name+'".');
}
try {main();} catch(error) {
 Session.Output('Error: '+(error.description || error.message || String(error)));
 Session.Prompt('Error: '+(error.description || error.message || String(error)),promptOK);
 throw error;
}
