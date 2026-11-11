using BE.Entidades;
using System;
using System.Collections.Generic;

namespace BLL
{
    internal static class ReglasDisponibilidadProfesor_83KI
    {
        internal static void ValidarActivaNoDuplicada(DisponibilidadProfesor_83KI disponibilidad,
            IEnumerable<DisponibilidadProfesor_83KI> existentes)
        {
            if (!disponibilidad.EstadoActivo) return;
            foreach (var existente in existentes)
            {
                if (existente.EstadoActivo
                    && existente.IdDisponibilidadProfesor != disponibilidad.IdDisponibilidadProfesor
                    && existente.DiaSemana == disponibilidad.DiaSemana
                    && existente.HoraInicio == disponibilidad.HoraInicio
                    && existente.HoraFin == disponibilidad.HoraFin)
                    throw new InvalidOperationException("Errores.DisponibilidadProfesorDuplicada");
            }
        }
    }
}
