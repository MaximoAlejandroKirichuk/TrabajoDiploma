// Pruebas de las reglas de KI68ReporteMorosidadBLL (RF2 - Informe de morosidad).
// No usan base de datos: trabajan con una DAL y una sesión falsas, y con una fecha "hoy" fija.
// Este archivo no forma parte de Solucion.sln (igual que FinanciacionCuotas.BllTests.cs).
//
// Cómo ejecutarlas (desde Solucion\, después de compilar en Debug), en "Developer Command Prompt for VS":
//   csc /nologo /out:Scripts\Tests\ReporteMorosidadTests.exe /r:UI\bin\Debug\BE.dll /r:UI\bin\Debug\Service.dll /r:UI\bin\Debug\BLL.dll Scripts\Tests\ReporteMorosidad.BllTests.cs
//   copy UI\bin\Debug\*.dll Scripts\Tests\
//   Scripts\Tests\ReporteMorosidadTests.exe
// Debe terminar con "RESULTADO: N pruebas OK, 0 fallidas" y código de salida 0.
using BE.Entidades;
using BLL;
using Service.DTOs;
using Service.Entidades;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

public static class ReporteMorosidadBllTests
{
    private static readonly DateTime Hoy = new DateTime(2026, 10, 26);
    private static int _ok;
    private static int _fallidas;
    private static int _idCuota = 1;

    public static int Main()
    {
        Probar("Cuota vencida impaga: saldo > 0 y vencimiento anterior a hoy", CuotaVencidaImpaga);
        Probar("Inscripción vigente según estado de inscripción y de comisión", InscripcionVigente);
        Probar("Rangos de atraso en los bordes 1/30/31/60/61", RangosDeAtraso);
        Probar("Cálculo de deuda con varias cuotas vencidas y una futura", CalculoDeDeuda);
        Probar("Escenario completo: quién aparece y en qué orden", EscenarioCompleto);
        Probar("Filtro de días mínimos", FiltroDiasMinimos);
        Probar("Filtro de deuda vencida mínima", FiltroDeudaMinima);
        Probar("Conteo para la torta por rango", ConteoParaTorta);
        Probar("Validación de filtros", ValidacionDeFiltros);
        Probar("Generar exige el permiso 75", GenerarSinPermiso);
        Probar("Generar no llama a la DAL si el filtro es inválido", GenerarFiltroInvalido);
        Probar("Generar traduce un error de la DAL", GenerarErrorDal);
        Probar("Comisiones del filtro excluyen eliminadas y canceladas", ComisionesDelFiltro);
        Probar("Detalle conserva el origen y concilia saldos sin modificar las cuotas", DetalleDeuda);
        Probar("Detalle vacío y lista de cuotas ausente", DetalleVacio);

        Console.WriteLine();
        Console.WriteLine("RESULTADO: {0} pruebas OK, {1} fallidas", _ok, _fallidas);
        return _fallidas == 0 ? 0 : 1;
    }

    // ------------------------------------------------------------------ pruebas

    private static void CuotaVencidaImpaga()
    {
        Igual(true, KI68ReporteMorosidadBLL.EsCuotaVencidaImpaga(Cuota(2, 1000m, -1), Hoy), "vencida ayer");
        Igual(false, KI68ReporteMorosidadBLL.EsCuotaVencidaImpaga(Cuota(2, 1000m, 0), Hoy), "vence hoy: todavía no está vencida");
        Igual(false, KI68ReporteMorosidadBLL.EsCuotaVencidaImpaga(Cuota(2, 1000m, 5), Hoy), "vence en el futuro");
        Igual(false, KI68ReporteMorosidadBLL.EsCuotaVencidaImpaga(Cuota(2, 0m, -10), Hoy), "pagada (saldo 0)");
        Igual(true, KI68ReporteMorosidadBLL.EsCuotaVencidaImpaga(Cuota(1, 500m, -3), Hoy), "matrícula impaga");
        Igual(false, KI68ReporteMorosidadBLL.EsCuotaVencidaImpaga(null, Hoy), "null");
    }

    private static void InscripcionVigente()
    {
        foreach (var estado in new[] { "pendiente", "confirmada", "activa", "ACTIVA" })
            Igual(true, KI68ReporteMorosidadBLL.EsInscripcionVigente(estado, "confirmada"), estado);
        foreach (var estado in new[] { "cancelada", "anulada", "Al día", "Con deuda", "", null })
            Igual(false, KI68ReporteMorosidadBLL.EsInscripcionVigente(estado, "confirmada"), estado ?? "null");
        Igual(false, KI68ReporteMorosidadBLL.EsInscripcionVigente("pendiente", "cancelada_falta_quorum"), "comisión cancelada");
        Igual(false, KI68ReporteMorosidadBLL.EsInscripcionVigente("activa", "eliminada"), "comisión eliminada");
        Igual(true, KI68ReporteMorosidadBLL.EsInscripcionVigente("activa", "preapertura"), "comisión en preapertura");
    }

    private static void RangosDeAtraso()
    {
        Igual("", KI68ReporteMorosidadBLL.ObtenerRangoAtraso(0), "0 días");
        Igual("1-30", KI68ReporteMorosidadBLL.ObtenerRangoAtraso(1), "1 día");
        Igual("1-30", KI68ReporteMorosidadBLL.ObtenerRangoAtraso(30), "30 días");
        Igual("31-60", KI68ReporteMorosidadBLL.ObtenerRangoAtraso(31), "31 días");
        Igual("31-60", KI68ReporteMorosidadBLL.ObtenerRangoAtraso(60), "60 días");
        Igual(">60", KI68ReporteMorosidadBLL.ObtenerRangoAtraso(61), "61 días");
    }

    private static void CalculoDeDeuda()
    {
        var fila = Fila("C07", "activa", "confirmada",
            Cuota(2, 10000m, -90), Cuota(3, 10000m, -60), Cuota(4, 10000m, -10), Cuota(5, 10000m, 20));
        KI68ReporteMorosidadBLL.CalcularDeuda(fila, Hoy);
        Igual(3, fila.CantidadCuotasVencidas, "cuotas vencidas");
        Igual(30000m, fila.DeudaVencida, "deuda vencida");
        Igual(40000m, fila.DeudaTotal, "deuda total (incluye la futura)");
        Igual(Hoy.AddDays(-90), fila.VencimientoImpagoMasAntiguo, "vencimiento más antiguo");
        Igual(90, fila.DiasAtraso, "días de atraso");
        Igual(">60", fila.RangoAtraso, "rango");

        var alDia = Fila("C02", "activa", "confirmada", Cuota(2, 5000m, 0), Cuota(3, 5000m, 30));
        KI68ReporteMorosidadBLL.CalcularDeuda(alDia, Hoy);
        Igual(0, alDia.CantidadCuotasVencidas, "solo futuras: sin vencidas");
        Igual(0m, alDia.DeudaVencida, "solo futuras: deuda vencida 0");
        Igual(10000m, alDia.DeudaTotal, "solo futuras: deuda total");
        Igual((DateTime?)null, alDia.VencimientoImpagoMasAntiguo, "solo futuras: sin vencimiento");
        Igual(false, KI68ReporteMorosidadBLL.EsMorosa(alDia), "solo futuras: no es morosa");
    }

    private static void EscenarioCompleto()
    {
        var reporte = KI68ReporteMorosidadBLL.ArmarReporte(Escenario(), new FiltroReporteMorosidad_83KI(), Hoy);
        Igual("C07,C06,C05,C04,C03", string.Join(",", reporte.Select(f => f.CodigoInscripcion)),
            "aparecen solo los morosos, por deuda vencida desc y días desc");

        var c03 = reporte.Single(f => f.CodigoInscripcion == "C03");
        Igual(1, c03.CantidadCuotasVencidas, "C03 matrícula impaga: 1 cuota");
        Igual(5, c03.DiasAtraso, "C03 días");
        Igual(15000m, c03.DeudaVencida, "C03 deuda vencida");
        Igual(55000m, c03.DeudaTotal, "C03 deuda total");

        var c04 = reporte.Single(f => f.CodigoInscripcion == "C04");
        var c05 = reporte.Single(f => f.CodigoInscripcion == "C05");
        Igual(c04.DeudaVencida, c05.DeudaVencida, "C04 y C05 tienen igual deuda vencida");
        Igual(true, reporte.IndexOf(c05) < reporte.IndexOf(c04), "a igual deuda, primero el de más días (C05 31 > C04 30)");
    }

    private static void FiltroDiasMinimos()
    {
        var reporte = KI68ReporteMorosidadBLL.ArmarReporte(Escenario(), new FiltroReporteMorosidad_83KI { DiasAtrasoMinimo = 31 }, Hoy);
        Igual("C07,C06,C05", string.Join(",", reporte.Select(f => f.CodigoInscripcion)), "mínimo 31 días");
    }

    private static void FiltroDeudaMinima()
    {
        var reporte = KI68ReporteMorosidadBLL.ArmarReporte(Escenario(), new FiltroReporteMorosidad_83KI { DeudaVencidaMinima = 20000m }, Hoy);
        Igual("C07,C06,C05,C04", string.Join(",", reporte.Select(f => f.CodigoInscripcion)), "mínimo $ 20.000");

        var ambos = KI68ReporteMorosidadBLL.ArmarReporte(Escenario(), new FiltroReporteMorosidad_83KI { DeudaVencidaMinima = 25000m, DiasAtrasoMinimo = 61 }, Hoy);
        Igual("C07,C06", string.Join(",", ambos.Select(f => f.CodigoInscripcion)), "mínimo $ 25.000 y 61 días");
    }

    private static void ConteoParaTorta()
    {
        var reporte = KI68ReporteMorosidadBLL.ArmarReporte(Escenario(), new FiltroReporteMorosidad_83KI(), Hoy);
        var conteo = KI68ReporteMorosidadBLL.ContarPorRango(reporte);
        Igual(2, conteo["1-30"], "1-30: C03 y C04");
        Igual(1, conteo["31-60"], "31-60: C05");
        Igual(2, conteo[">60"], ">60: C06 y C07");
        Igual(3, KI68ReporteMorosidadBLL.ContarPorRango(new List<ReporteMorosidadFila_83KI>()).Count, "vacío: igual devuelve los 3 rangos");
    }

    private static void ValidacionDeFiltros()
    {
        Lanza("Errores.ReporteMorosidadDniInvalido", () => KI68ReporteMorosidadBLL.ValidarFiltro(new FiltroReporteMorosidad_83KI { DNI = "12a" }));
        Lanza("Errores.ReporteMorosidadDniInvalido", () => KI68ReporteMorosidadBLL.ValidarFiltro(new FiltroReporteMorosidad_83KI { DNI = "99%" }));
        Lanza("Errores.ReporteMorosidadMinimoInvalido", () => KI68ReporteMorosidadBLL.ValidarFiltro(new FiltroReporteMorosidad_83KI { DiasAtrasoMinimo = -1 }));
        Lanza("Errores.ReporteMorosidadMinimoInvalido", () => KI68ReporteMorosidadBLL.ValidarFiltro(new FiltroReporteMorosidad_83KI { DeudaVencidaMinima = -0.01m }));
        KI68ReporteMorosidadBLL.ValidarFiltro(new FiltroReporteMorosidad_83KI { DNI = " 99075 ", DiasAtrasoMinimo = 0, DeudaVencidaMinima = 0m });
        KI68ReporteMorosidadBLL.ValidarFiltro(null);
    }

    private static void GenerarSinPermiso()
    {
        var dal = new DalFalsa { Filas = Escenario() };
        Lanza("Errores.SinPermisos", () => new KI68ReporteMorosidadBLL(dal, new SesionFalsa(false)).Generar(new FiltroReporteMorosidad_83KI()));
        Igual(0, dal.Lecturas, "sin permiso no se lee la base");

        var reporte = new KI68ReporteMorosidadBLL(dal, new SesionFalsa(true)).Generar(null);
        Igual(1, dal.Lecturas, "con permiso se lee una vez");
        Igual(true, reporte.All(f => f.CantidadCuotasVencidas > 0), "devuelve solo morosos");
    }

    private static void GenerarFiltroInvalido()
    {
        var dal = new DalFalsa { Filas = Escenario() };
        Lanza("Errores.ReporteMorosidadDniInvalido", () => new KI68ReporteMorosidadBLL(dal, new SesionFalsa(true)).Generar(new FiltroReporteMorosidad_83KI { DNI = "abc" }));
        Igual(0, dal.Lecturas, "filtro inválido: no se lee la base");
    }

    private static void GenerarErrorDal()
    {
        var dal = new DalFalsa { Error = new Exception("Error de lectura en la base de datos") };
        Lanza("Errores.ReporteMorosidadNoDisponible", () => new KI68ReporteMorosidadBLL(dal, new SesionFalsa(true)).Generar(new FiltroReporteMorosidad_83KI()));
    }

    private static void ComisionesDelFiltro()
    {
        var dal = new DalFalsa
        {
            Comisiones = new List<ComisionListado_83KI>
            {
                new ComisionListado_83KI { IdComision = 1, Codigo = "A", Estado = "confirmada" },
                new ComisionListado_83KI { IdComision = 2, Codigo = "B", Estado = "preapertura" },
                new ComisionListado_83KI { IdComision = 3, Codigo = "C", Estado = "cancelada_falta_quorum" },
                new ComisionListado_83KI { IdComision = 4, Codigo = "D", Estado = "eliminada" }
            }
        };
        var lista = new KI68ReporteMorosidadBLL(dal, new SesionFalsa(true)).ListarComisionesParaFiltro();
        Igual("A,B", string.Join(",", lista.Select(c => c.Codigo)), "solo comisiones vigentes");
    }

    private static void DetalleDeuda()
    {
        var matricula = Cuota(1, 500m, -10);
        var parcial = Cuota_83KI.ReconstruirDesdePersistencia(99, 1, 2, 2000m, 750m,
            Hoy.AddDays(-1), Cuota_83KI.EstadoPendiente, "", Cuota_83KI.ConceptoCurso, new DateTime(2026, 10, 1));
        var hoy = Cuota(3, 1000m, 0);
        var futura = Cuota(4, 1200m, 30);
        var fila = Fila("DET", "activa", "confirmada", futura, hoy, parcial, matricula, Cuota(5, 0m, -20), null);
        var detalle = KI68ReporteMorosidadBLL.ObtenerDetalleDeuda(fila);
        Igual(4, detalle.Count, "excluye pagadas y null");
        Igual(true, object.ReferenceEquals(matricula, detalle[0]), "conserva cuota y matrícula real");
        Igual(true, object.ReferenceEquals(parcial, detalle[1]), "ordena por vencimiento sin reconstruir");
        Igual(2000m, detalle[1].MontoOriginal, "monto original, no saldo");
        Igual(750m, detalle[1].BalanceAdeudado, "saldo parcial");
        Igual(Cuota_83KI.ConceptoCurso, detalle[1].TipoConcepto, "concepto persistido");
        Igual((DateTime?)new DateTime(2026, 10, 1), detalle[1].Periodo, "período persistido");
        Igual(1, detalle[1].NumeroMensualidad, "mensualidad 1 corresponde al registro 2");
        Igual(false, KI68ReporteMorosidadBLL.EsCuotaVencidaImpaga(hoy, Hoy), "hoy no está vencida");
        Igual(false, KI68ReporteMorosidadBLL.EsCuotaVencidaImpaga(futura, Hoy), "futura no está vencida");
        KI68ReporteMorosidadBLL.CalcularDeuda(fila, Hoy);
        Igual(fila.DeudaTotal, detalle.Sum(c => c.BalanceAdeudado), "detalle concilia con total");
        Igual(fila.DeudaVencida, detalle.Where(c => KI68ReporteMorosidadBLL.EsCuotaVencidaImpaga(c, Hoy)).Sum(c => c.BalanceAdeudado), "detalle concilia con vencida");
        Igual(true, object.ReferenceEquals(futura, fila.CuotasConSaldo[0]), "no cambia el orden original");
        detalle.Clear();
        Igual(6, fila.CuotasConSaldo.Count, "lista de detalle independiente");
    }

    private static void DetalleVacio()
    {
        Igual(0, KI68ReporteMorosidadBLL.ObtenerDetalleDeuda(new ReporteMorosidadFila_83KI()).Count, "sin cuotas");
        Igual(0, KI68ReporteMorosidadBLL.ObtenerDetalleDeuda(new ReporteMorosidadFila_83KI { CuotasConSaldo = null }).Count, "cuotas null");
    }

    // ------------------------------------------------------------------ datos de prueba
    // Mismos casos que Scripts/Sql/reporte-morosidad/002_datos_prueba_morosidad.sql

    private static List<ReporteMorosidadFila_83KI> Escenario()
    {
        return new List<ReporteMorosidadFila_83KI>
        {
            // C02: solo deuda futura (una vence hoy, otra en 25 días) -> no aparece
            Fila("C02", "activa", "confirmada", Cuota(2, 20000m, 0), Cuota(3, 20000m, 25)),
            // C03: matrícula impaga hace 5 días + 2 cuotas futuras -> 1-30
            Fila("C03", "pendiente", "confirmada", Cuota(1, 15000m, -5), Cuota(2, 20000m, 25), Cuota(3, 20000m, 55)),
            // C04: 30 días -> 1-30 (borde)
            Fila("C04", "activa", "confirmada", Cuota(2, 20000m, -30), Cuota(3, 20000m, 1)),
            // C05: 31 días -> 31-60 (borde)
            Fila("C05", "activa", "confirmada", Cuota(2, 20000m, -31)),
            // C06: 61 días -> >60 (borde)
            Fila("C06", "activa", "confirmada", Cuota(2, 25000m, -61)),
            // C07: varias vencidas (90, 60 y 10 días) + 1 futura -> >60
            Fila("C07", "activa", "confirmada", Cuota(2, 10000m, -90), Cuota(3, 10000m, -60), Cuota(4, 10000m, -10), Cuota(5, 10000m, 20)),
            // C08: inscripción cancelada con deuda vencida -> no aparece
            Fila("C08", "cancelada", "confirmada", Cuota(2, 20000m, -40)),
            // C09: comisión cancelada por falta de quórum, matrícula impaga -> no aparece
            Fila("C09", "pendiente", "cancelada_falta_quorum", Cuota(1, 15000m, -30))
            // C01 (sin deuda) no tiene cuotas con saldo, así que la DAL ni siquiera lo devuelve.
        };
    }

    private static ReporteMorosidadFila_83KI Fila(string codigo, string estadoInscripcion, string estadoComision, params Cuota_83KI[] cuotas)
    {
        return new ReporteMorosidadFila_83KI
        {
            IdSolicitudInscripcion = 1,
            CodigoInscripcion = codigo,
            EstadoInscripcion = estadoInscripcion,
            EstadoComision = estadoComision,
            Apellido = "Prueba " + codigo,
            Nombre = "Alumno",
            CuotasConSaldo = cuotas.ToList()
        };
    }

    // numeroCuota 1 = matrícula. diasDesdeHoy negativo = ya venció.
    private static Cuota_83KI Cuota(int numeroCuota, decimal saldo, int diasDesdeHoy)
    {
        bool matricula = numeroCuota == 1;
        DateTime vencimiento = Hoy.AddDays(diasDesdeHoy);
        return Cuota_83KI.ReconstruirDesdePersistencia(_idCuota++, 1, numeroCuota, saldo > 0 ? saldo : 1000m, saldo, vencimiento,
            saldo > 0 ? Cuota_83KI.EstadoPendiente : Cuota_83KI.EstadoPagada, "",
            matricula ? Cuota_83KI.ConceptoMatricula : Cuota_83KI.ConceptoCurso,
            matricula ? (DateTime?)null : new DateTime(vencimiento.Year, vencimiento.Month, 1));
    }

    // ------------------------------------------------------------------ dobles de prueba

    private sealed class DalFalsa : IReporteMorosidadDAL_83KI
    {
        public List<ReporteMorosidadFila_83KI> Filas = new List<ReporteMorosidadFila_83KI>();
        public List<ComisionListado_83KI> Comisiones = new List<ComisionListado_83KI>();
        public Exception Error;
        public int Lecturas;

        public List<ReporteMorosidadFila_83KI> ObtenerInscripcionesConSaldo(FiltroReporteMorosidad_83KI filtro)
        {
            Lecturas++;
            if (Error != null) throw Error;
            return Filas;
        }

        public List<ComisionListado_83KI> ListarComisionesParaFiltro()
        {
            if (Error != null) throw Error;
            return Comisiones;
        }
    }

    private sealed class SesionFalsa : ISessionManager_83KI
    {
        private readonly bool _tienePermiso;
        public SesionFalsa(bool tienePermiso) { _tienePermiso = tienePermiso; }
        public Usuario_83KI UsuarioActivo { get { return null; } }
        public bool RequiereRecuperacionIntegridad { get { return false; } }
        public IReadOnlyList<string> TablasAfectadasIntegridad { get { return new string[0]; } }
        public bool TienePermiso(PermisoSistema_83KI permiso) { return _tienePermiso && permiso == PermisoSistema_83KI.VerReporteMorosidad; }
        public void IniciarSesion(Usuario_83KI usuario) { }
        public void CerrarSesion() { }
        public IEnumerable<Patente_83KI> ObtenerPermisos() { return new Patente_83KI[0]; }
        public void EstablecerRecuperacionIntegridad(IEnumerable<string> tablas) { }
        public void LimpiarRecuperacionIntegridad() { }
    }

    // ------------------------------------------------------------------ mini framework

    private static void Probar(string nombre, Action prueba)
    {
        try
        {
            prueba();
            _ok++;
            Console.WriteLine("OK     " + nombre);
        }
        catch (Exception ex)
        {
            _fallidas++;
            Console.WriteLine("FALLA  " + nombre + " -> " + ex.Message);
        }
    }

    private static void Igual<T>(T esperado, T obtenido, string detalle)
    {
        if (!EqualityComparer<T>.Default.Equals(esperado, obtenido))
            throw new Exception(string.Format("{0}: se esperaba '{1}' y se obtuvo '{2}'", detalle, esperado, obtenido));
    }

    private static void Lanza(string clave, Action accion)
    {
        try
        {
            accion();
        }
        catch (Exception ex)
        {
            string mensaje = (ex.Message ?? string.Empty).Split('\r', '\n')[0].Trim();
            if (mensaje == clave) return;
            throw new Exception(string.Format("se esperaba '{0}' y se lanzó '{1}'", clave, mensaje));
        }
        throw new Exception("se esperaba la excepción " + clave + " y no se lanzó ninguna");
    }
}
