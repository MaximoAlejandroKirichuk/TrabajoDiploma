using System;

namespace DAL
{
    /// <summary>Mapea valores anulables de instantáneas heredadas de financiación sin reemplazar los datos faltantes por valores predeterminados.</summary>
    internal static class SnapshotMapper_83KI
    {
        internal static decimal? Decimal(object value) { return value == DBNull.Value ? (decimal?)null : Convert.ToDecimal(value); }
        internal static int? Integer(object value) { return value == DBNull.Value ? (int?)null : Convert.ToInt32(value); }
        internal static DateTime? Date(object value) { return value == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(value); }
        internal static string Text(object value) { return value == DBNull.Value ? null : Convert.ToString(value); }
    }
}
