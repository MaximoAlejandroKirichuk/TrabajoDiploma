using BE.DTOs.Reportes;
using BE.DTOs.Recorridos;
using BE.Entidades;
using System.Collections.Generic;

namespace Service.Interfaces
{
    public interface IReporteInteligenteDAL_83KI
    {
        ReporteInteligente_83KI Generar(FiltroReporteInteligente_83KI filtro);
        List<Curso_83KI> Cursos();
    }
    public interface IReporteInteligenteBLL_83KI
    {
        ReporteInteligente_83KI Generar(FiltroReporteInteligente_83KI filtro);
        List<Curso_83KI> Cursos();
    }
    public interface IRecorridoCursoDAL_83KI
    {
        List<RecorridoListado_83KI> Listar();
        List<Curso_83KI> Cursos();
        void Guardar(RecorridoCurso_83KI recorrido);
        void Eliminar(int id);
        void GuardarReferencia(int idCurso, int? minimo);
    }
    public interface IRecorridoCursoBLL_83KI
    {
        List<RecorridoListado_83KI> Listar();
        List<Curso_83KI> Cursos();
        void Guardar(int id, int previo, int recomendado, int orden);
        void Eliminar(int id);
        void GuardarReferencia(int idCurso, int? minimo);
    }
}
