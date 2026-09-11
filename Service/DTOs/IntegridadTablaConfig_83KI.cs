using System;
using System.Collections.Generic;

namespace Service.DTOs
{
    /// <summary>
    /// metadatos canonicos para una tabla protegida: nombre y orden de columnas usado para hashing dvh.
    /// el orden de columnas definido aqui DEBE coincidir con la canonicalizacion en los scripts sql
    /// (002_backfill.sql) y el calculo dvh en c# en IntegridadDAL_83KI.
    /// las columnas se listan alfabeticamente. la columna dvh se excluye del conjunto canonico.
    /// </summary>
    public class IntegridadTablaConfig_83KI
    {
        public string NombreTabla { get; }
        public IReadOnlyList<string> ColumnasOrdenadas { get; }

        private IntegridadTablaConfig_83KI(string nombreTabla, params string[] columnas)
        {
            NombreTabla = nombreTabla;
            ColumnasOrdenadas = Array.AsReadOnly(columnas);
        }

        /// <summary>
        /// orden canonico de columnas para las 8 tablas protegidas.
        /// excluye BitacoraEventos (no esta en el alcance protegido).
        /// </summary>
        public static readonly IReadOnlyList<IntegridadTablaConfig_83KI> TablasProtegidas =
            new List<IntegridadTablaConfig_83KI>
            {
                // Usuarios — 12 columnas, alfabetico
                new IntegridadTablaConfig_83KI("Usuarios",
                    "Activo", "Apellido", "Bloqueado", "CodigoRol", "Contrasena",
                    "DNI", "Email", "FechaUltimoIntento", "IdiomaId",
                    "IntentosRealizados", "Nombre", "Username"),

                // Roles — 2 columnas, alfabetico
                new IntegridadTablaConfig_83KI("Roles",
                    "CodigoRol", "Nombre"),

                // Familias — 2 columnas, alfabetico
                new IntegridadTablaConfig_83KI("Familias",
                    "CodigoFamilia", "Nombre"),

                // Patentes — 2 columnas, alfabetico
                new IntegridadTablaConfig_83KI("Patentes",
                    "CodigoPatente", "Nombre"),

                // tablas de join — columnas fk, alfabetico
                new IntegridadTablaConfig_83KI("RolPatente",
                    "CodigoPatente", "CodigoRol"),

                new IntegridadTablaConfig_83KI("RolFamilia",
                    "CodigoFamilia", "CodigoRol"),

                new IntegridadTablaConfig_83KI("FamiliaPatente",
                    "CodigoFamilia", "CodigoPatente"),

                new IntegridadTablaConfig_83KI("FamiliaFamilia",
                    "CodigoFamiliaHija", "CodigoFamiliaPadre"),
            };

        /// <summary>
        /// busca una config de tabla protegida por nombre. devuelve null si no se encuentra.
        /// </summary>
        public static IntegridadTablaConfig_83KI ObtenerPorNombre(string nombreTabla)
        {
            foreach (var config in TablasProtegidas)
            {
                if (string.Equals(config.NombreTabla, nombreTabla, StringComparison.OrdinalIgnoreCase))
                    return config;
            }
            return null;
        }
    }
}
