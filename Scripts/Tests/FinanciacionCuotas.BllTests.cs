using System;
using System.Collections.Generic;
using System.Linq;
using BE.Entidades;
using BLL;
using Service.DTOs;
using Service.Entidades;
using Service.Interfaces;

public static class FinanciacionCuotasBllTests
{
    public static void Run(BusquedaPagoInscripcionResultado_83KI data)
    {
        var dal = new FakeDal { Data = data };
        var bll = new KI68PagoCuotaBLL_83KI(dal, new FakeSession(), null);
        var enrollmentPermissionOnly = new KI68PagoCuotaBLL_83KI(dal,
            new FakeSession(PermisoSistema_83KI.RegistrarPagoInscripcion), null);
        Reject(enrollmentPermissionOnly, Request(data.Cuotas.OrderBy(c => c.NumeroCuota).First()), "Errores.SinPermisos");
        if (dal.Writes != 0) throw new Exception("Enrollment payment permission granted course installment access.");
        var first = data.Cuotas.OrderBy(c => c.NumeroCuota).First();
        var next = data.Cuotas.OrderBy(c => c.NumeroCuota).Skip(1).First();
        Reject(bll, Request(next), "Errores.PagoCuotaAnteriorPendiente");
        if (dal.Writes != 0) throw new Exception("Out-of-order payment reached persistence.");
        data.MatriculaPagada = false;
        Reject(bll, Request(first), "Errores.PagoMatriculaPendiente");
        data.MatriculaPagada = true;
        var partial = Request(first); partial.MontoRecibido /= 2;
        Reject(bll, partial, "Errores.PagoMontoNoCoincide");
        bll.RegistrarPagoCuota(Request(first));
        if (dal.Writes != 1) throw new Exception("The future installment was rejected.");
        data.CuotasPendientes.Remove(first);
        data.Cuotas.Remove(first);
        data.Cuotas.Add(Cuota_83KI.ReconstruirDesdePersistencia(first.IdCuota, first.IdSolicitudInscripcion, first.NumeroCuota,
            first.MontoOriginal, 0, first.FechaVencimiento, Cuota_83KI.EstadoPagada, "", Cuota_83KI.ConceptoCurso, first.Periodo));
        Reject(bll, Request(first), "Errores.PagoCuotaObligatoria");
        bll.RegistrarPagoCuota(Request(next));
        if (dal.Writes != 2) throw new Exception("The next installment was not unlocked.");
        Console.WriteLine("PASS: BLL blocks skipped/partial/paid installments and unpaid enrollment fees; allows sequential advance payments.");
    }

    private static PagoInscripcionRequest_83KI Request(Cuota_83KI cuota)
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
        var unauthorizedRequest = Request(selected.Cuota);
        unauthorizedRequest.DNI = data.Alumno.DNI;
        try { wrongPermissionBll.RegistrarPagoInscripcion(unauthorizedRequest); throw new Exception("Installment payment permission granted enrollment payment access."); }
        catch (InvalidOperationException ex) { if (ex.Message != "Errores.SinPermisos") throw; }
        var bll = new KI68PagoInscripcionBLL(dal, new FakeSession(PermisoSistema_83KI.RegistrarPagoInscripcion), null);
        var request = Request(selected.Cuota);
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

    private static void Reject(KI68PagoCuotaBLL_83KI bll, PagoInscripcionRequest_83KI request, string key)
    {
        try { bll.RegistrarPagoCuota(request); }
        catch (Exception ex) { if (ex.Message == key) return; throw; }
        throw new Exception("Expected rejection: " + key);
    }

    private sealed class FakeDal : IPagoCuotaDAL_83KI
    {
        public BusquedaPagoInscripcionResultado_83KI Data;
        public int Writes;
        public IEnumerable<ComisionPagoOpcion_83KI> ListarComisionesConCuotasPendientes(string dni) { return new ComisionPagoOpcion_83KI[0]; }
        public BusquedaPagoInscripcionResultado_83KI BuscarCuotasPendientes(string dni, string codigo) { return Data; }
        public bool ExisteReferenciaPago(string reference) { return false; }
        public PagoInscripcionResultado_83KI RegistrarPago(PagoInscripcion_83KI pago)
        {
            Writes++;
            return new PagoInscripcionResultado_83KI { Pago = pago, Alumno = Data.Alumno, Solicitud = Data.Solicitud,
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
