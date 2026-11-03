using System;
using System.Collections.Generic;
using System.Linq;
using BE.Entidades;
using BE.DTOs.Pagos;
using BE.DTOs.Inscripciones;
using BE.DTOs.Comisiones;
using BLL;
using Service.DTOs;
using Service.Entidades;
using Service.Interfaces;

public static class FinanciacionCuotasBllTests
{
    public static void Run(BusquedaCuotasPagoResultado_83KI data)
    {
        var dal = new FakeDal { Data = data };
        var session = new FakeSession();
        var inscripcionDal = new FakeInscripcionDal { Solicitud = data.Solicitud };
        var inscripcionBll = new KI68InscripcionBLL(inscripcionDal, null, new KI68CuotaBLL(), session);
        var bll = new KI68PagoCuotaBLL_83KI(dal, session, null, inscripcionBll, new KI68CuotaBLL());
        var enrollmentPermissionOnly = new KI68PagoCuotaBLL_83KI(dal,
            new FakeSession(PermisoSistema_83KI.RegistrarPagoInscripcion), null, inscripcionBll, new KI68CuotaBLL());
        Reject(enrollmentPermissionOnly, Request(data.Cuotas.OrderBy(c => c.NumeroCuota).First()), "Errores.SinPermisos");
        if (dal.Writes != 0) throw new Exception("Enrollment payment permission granted course installment access.");
        if (bll.BuscarCuotasPendientes(data.Alumno.DNI, data.CodigoComision) != data || inscripcionDal.Reads != 1)
            throw new Exception("CUN07 did not consult InscripcionBLL using the installment payment permission.");
        var solicitud = inscripcionDal.Solicitud;
        inscripcionDal.Solicitud = null;
        if (bll.BuscarCuotasPendientes(data.Alumno.DNI, data.CodigoComision) != null)
            throw new Exception("An enrollment missing from InscripcionBLL was accepted.");
        Reject(bll, Request(data.Cuotas.First()), "Errores.PagoCuotaNoPendiente");
        inscripcionDal.Solicitud = SolicitudInscripcion_83KI.ReconstruirDesdePersistencia(
            solicitud.IdSolicitudInscripcion + 1, solicitud.Codigo, solicitud.IdAlumno, null,
            solicitud.IdComision, solicitud.IdPlanDePago, 0, DateTime.Today, solicitud.Estado, "", "");
        Reject(bll, Request(data.Cuotas.First()), "Errores.PagoSolicitudesAmbiguas");
        inscripcionDal.Solicitud = solicitud;
        var deniedLookup = new KI68InscripcionBLL(inscripcionDal, null, new KI68CuotaBLL(),
            new FakeSession(PermisoSistema_83KI.RegistrarSolicitudInscripcion));
        int reads = inscripcionDal.Reads;
        try { deniedLookup.ObtenerPorDniYComision(data.Alumno.DNI, data.CodigoComision); throw new Exception("Enrollment creation permission granted installment lookup access."); }
        catch (InvalidOperationException ex) { if (ex.Message != "Errores.SinPermisos") throw; }
        if (inscripcionDal.Reads != reads || dal.Writes != 0) throw new Exception("Rejected enrollment checks reached persistence.");
        var first = data.Cuotas.OrderBy(c => c.NumeroCuota).First();
        var next = data.Cuotas.OrderBy(c => c.NumeroCuota).Skip(1).First();
        Reject(bll, Request(next), "Errores.PagoCuotaAnteriorPendiente");
        if (dal.Writes != 0) throw new Exception("Out-of-order payment reached persistence.");
        data.MatriculaPagada = false;
        Reject(bll, Request(first), "Errores.PagoMatriculaPendiente");
        data.MatriculaPagada = true;
        var partial = Request(first); partial.MontoRecibido /= 2;
        Reject(bll, partial, "Errores.PagoMontoNoCoincide");
        reads = inscripcionDal.Reads;
        bll.RegistrarPagoCuota(Request(first));
        if (inscripcionDal.Reads != reads + 1) throw new Exception("Payment did not recheck enrollment through InscripcionBLL.");
        if (dal.Writes != 1) throw new Exception("The future installment was rejected.");
        data.CuotasPendientes.Remove(first);
        data.Cuotas.Remove(first);
        data.Cuotas.Add(Cuota_83KI.ReconstruirDesdePersistencia(first.IdCuota, first.IdSolicitudInscripcion, first.NumeroCuota,
            first.MontoOriginal, 0, first.FechaVencimiento, Cuota_83KI.EstadoPagada, "", Cuota_83KI.ConceptoCurso, first.Periodo));
        Reject(bll, Request(first), "Errores.PagoCuotaObligatoria");
        bll.RegistrarPagoCuota(Request(next));
        if (dal.Writes != 2) throw new Exception("The next installment was not unlocked.");
        Console.WriteLine("PASS: BLL blocks skipped/partial/paid installments and unpaid enrollment fees; allows sequential advance payments.");
        var cuotaBll = new KI68CuotaBLL();
        var paid = data.Cuotas.Single(c => c.IdCuota == first.IdCuota);
        try { cuotaBll.ValidarPagoCompleto(paid, first.MontoOriginal); throw new Exception("CuotaBLL accepted a paid installment."); }
        catch (InvalidOperationException ex) { if (ex.Message != "Errores.PagoCuotaNoPendiente") throw; }
        var matricula = Cuota_83KI.ReconstruirDesdePersistencia(first.IdCuota, first.IdSolicitudInscripcion, 1,
            100, 100, DateTime.Today, Cuota_83KI.EstadoPendiente, "", Cuota_83KI.ConceptoMatricula);
        try { cuotaBll.ValidarPagoCompleto(matricula, 100); throw new Exception("CuotaBLL accepted an enrollment fee as a course installment."); }
        catch (InvalidOperationException ex) { if (ex.Message != "Errores.PagoCuotaNoPendiente") throw; }
        Console.WriteLine("PASS: CUN07 delegates enrollment checks and full-payment validation to InscripcionBLL and CuotaBLL.");
    }

    private static PagoCuotaRequest_83KI Request(Cuota_83KI cuota)
    {
        return new PagoCuotaRequest_83KI { DNI = "99776611", CodigoComision = "FIN-LEG-COM", IdCuota = cuota.IdCuota,
            MetodoPago = "transferencia", MontoRecibido = cuota.BalanceAdeudado > 0 ? cuota.BalanceAdeudado : cuota.MontoOriginal,
            NumeroReferencia = "TEST", FechaPago = DateTime.Today, FechaAcreditacion = DateTime.Today };
    }

    private static PagoInscripcionRequest_83KI EnrollmentRequest(Cuota_83KI cuota)
    {
        return new PagoInscripcionRequest_83KI { DNI = "99776611", CodigoComision = "FIN-LEG-COM", IdCuota = cuota.IdCuota,
            MetodoPago = "transferencia", MontoRecibido = cuota.BalanceAdeudado > 0 ? cuota.BalanceAdeudado : cuota.MontoOriginal,
            NumeroReferencia = "TEST", FechaPago = DateTime.Today, FechaAcreditacion = DateTime.Today };
    }

    public static void RunMatriculas(BusquedaPagoInscripcionResultado_83KI data)
    {
        if (data.MatriculasPendientes.Count != 2) throw new Exception("Expected two enrollment fees for the same student.");
        var selected = data.MatriculasPendientes.Last();
        if (selected.Solicitud.IdSolicitudInscripcion == data.Solicitud.IdSolicitudInscripcion)
            throw new Exception("The regression must select a fee outside the first enrollment.");
        var dal = new FakeEnrollmentDal { Data = data };
        var wrongPermissionBll = new KI68PagoInscripcionBLL(dal,
            new FakeSession(PermisoSistema_83KI.RegistrarPagoCuotas), null);
        var unauthorizedRequest = EnrollmentRequest(selected.Cuota);
        unauthorizedRequest.DNI = data.Alumno.DNI;
        try { wrongPermissionBll.RegistrarPagoInscripcion(unauthorizedRequest); throw new Exception("Installment payment permission granted enrollment payment access."); }
        catch (InvalidOperationException ex) { if (ex.Message != "Errores.SinPermisos") throw; }
        var bll = new KI68PagoInscripcionBLL(dal, new FakeSession(PermisoSistema_83KI.RegistrarPagoInscripcion), null);
        var request = EnrollmentRequest(selected.Cuota);
        request.DNI = data.Alumno.DNI;
        request.FechaPago = DateTime.Today.AddDays(1);
        try { bll.RegistrarPagoInscripcion(request); throw new Exception("Future enrollment payment was accepted."); }
        catch (ArgumentException ex) { if (ex.Message != "Errores.PagoFechaFutura") throw; }
        if (dal.LastPayment != null) throw new Exception("A future payment reached persistence.");
        request.FechaPago = DateTime.Today;
        bll.RegistrarPagoInscripcion(request);
        if (dal.LastPayment == null || dal.LastPayment.IdSolicitudInscripcion != selected.Solicitud.IdSolicitudInscripcion
            || dal.LastPayment.IdCuota != selected.Cuota.IdCuota)
            throw new Exception("The selected enrollment fee was associated with another enrollment.");
        Console.WriteLine("PASS: CUN05 maps multiple enrollments independently and rejects future dates in BLL.");
    }

    private static void Reject(KI68PagoCuotaBLL_83KI bll, PagoCuotaRequest_83KI request, string key)
    {
        try { bll.RegistrarPagoCuota(request); }
        catch (Exception ex) { if (ex.Message == key) return; throw; }
        throw new Exception("Expected rejection: " + key);
    }

    private sealed class FakeInscripcionDal : IInscripcionDAL_83KI
    {
        public SolicitudInscripcion_83KI Solicitud;
        public int Reads;
        public SolicitudInscripcion_83KI ObtenerPorDniYComision(string dni, string codigo) { Reads++; return Solicitud; }
        public IEnumerable<BusquedaInscripcionPersonaResultado_83KI> BuscarPersonas(string texto) { throw new NotSupportedException(); }
        public IEnumerable<ComisionListado_83KI> ListarComisionesElegibles() { throw new NotSupportedException(); }
        public IEnumerable<PlanDePago_83KI> ObtenerPlanesComision(int id) { throw new NotSupportedException(); }
        public SolicitudInscripcionResultado_83KI RegistrarSolicitud(SolicitudInscripcionRequest_83KI request) { throw new NotSupportedException(); }
    }

    private sealed class FakeDal : IPagoCuotaDAL_83KI
    {
        public BusquedaCuotasPagoResultado_83KI Data;
        public int Writes;
        public IEnumerable<ComisionPagoOpcion_83KI> ListarComisionesConCuotasPendientes(string dni) { return new ComisionPagoOpcion_83KI[0]; }
        public BusquedaCuotasPagoResultado_83KI BuscarCuotasPendientes(string dni, string codigo) { return Data; }
        public bool ExisteReferenciaPago(string reference) { return false; }
        public PagoCuotaResultado_83KI RegistrarPago(PagoInscripcion_83KI pago)
        {
            Writes++;
            return new PagoCuotaResultado_83KI { Pago = pago, Alumno = Data.Alumno, Solicitud = Data.Solicitud,
                Cuota = Data.Cuotas.Single(c => c.IdCuota == pago.IdCuota) };
        }
    }

    private sealed class FakeSession : ISessionManager_83KI
    {
        private readonly PermisoSistema_83KI _permission;
        public FakeSession(PermisoSistema_83KI permission = PermisoSistema_83KI.RegistrarPagoCuotas) { _permission = permission; }
        public Usuario_83KI UsuarioActivo { get { return null; } }
        public bool RequiereRecuperacionIntegridad { get { return false; } }
        public IReadOnlyList<string> TablasAfectadasIntegridad { get { return new string[0]; } }
        public bool TienePermiso(PermisoSistema_83KI permiso) { return permiso == _permission; }
        public void IniciarSesion(Usuario_83KI usuario) { }
        public void CerrarSesion() { }
        public IEnumerable<Patente_83KI> ObtenerPermisos() { return new Patente_83KI[0]; }
        public void EstablecerRecuperacionIntegridad(IEnumerable<string> tablas) { }
        public void LimpiarRecuperacionIntegridad() { }
    }

    private sealed class FakeEnrollmentDal : IPagoInscripcionDAL_83KI
    {
        public BusquedaPagoInscripcionResultado_83KI Data;
        public PagoInscripcion_83KI LastPayment;
        public BusquedaPagoInscripcionResultado_83KI BuscarCuotasPendientesPorDni(string dni) { return Data; }
        public bool ExisteReferenciaPago(string reference) { return false; }
        public PagoInscripcionResultado_83KI RegistrarPago(PagoInscripcion_83KI pago)
        {
            LastPayment = pago;
            var selected = Data.MatriculasPendientes.Single(m => m.Cuota.IdCuota == pago.IdCuota);
            return new PagoInscripcionResultado_83KI { Pago = pago, Alumno = Data.Alumno, Solicitud = selected.Solicitud,
                Cuota = selected.Cuota, CodigoComision = selected.CodigoComision };
        }
    }
}
