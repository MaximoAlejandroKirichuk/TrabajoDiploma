using System.Reflection;
using System.Runtime.CompilerServices;

// permite que BLL acceda a miembros internal de DAL (ej. AccesoDAL_83KI.EstablecerProveedorPredeterminado)
[assembly: InternalsVisibleTo("BLL")]

// La información general de un ensamblado se controla mediante el siguiente 
// conjunto de atributos. Cambie estos valores de atributo para modificar la información
// asociada con un ensamblado.
[assembly: AssemblyTitle("DAL")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("DAL")]
[assembly: AssemblyCopyright("Copyright ©  2026")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Si establece ComVisible en false, los tipos de este ensamblado no estarán visibles 
// para los componentes COM.  Si es necesario obtener acceso a un tipo en este ensamblado desde 
// COM, establezca el atributo ComVisible en true en este tipo.

// El siguiente GUID sirve como id. de typelib si este proyecto se expone a COM.

// La información de versión de un ensamblado consta de los cuatro valores siguientes:
//
//      Versión principal
//      Versión secundaria
//      Número de compilación
//      Revisión
//
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
