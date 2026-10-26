# CUN09 - DSS (fiel al codigo y al DC)

Lifelines, en orden: Coordinadora Academica, FrmGenerarCertificado_83KI, KI68CertificadoBLL, BitacoraBLL_83KI, IntegridadDAL_83KI, KI68BaseDeDatos.


## CUN09 - DSS Generar certificado
Escenario principal con sus alternativas (2.1, 4.1, 6.1, 8.1, 8.2, 8.3, 11.1)


**A. Abrir la pantalla (ECU 1 y 2)**

1. Coordinadora Academica -> FrmGenerarCertificado_83KI: Seleccionar "Generar certificado de finalizacion"
2. FrmGenerarCertificado_83KI -> FrmGenerarCertificado_83KI: OnShown(e)
3. FrmGenerarCertificado_83KI -> KI68CertificadoBLL: ListarFinalizadas()
4. KI68CertificadoBLL -> KI68CertificadoBLL: Autorizar()
5. KI68CertificadoBLL -> KI68BaseDeDatos: ExecuteQuery(comisiones con fecha fin <= hoy)
6. KI68BaseDeDatos -> KI68CertificadoBLL: comisiones  (retorno)
7. KI68CertificadoBLL -> FrmGenerarCertificado_83KI: comisiones  (retorno)
8. FrmGenerarCertificado_83KI -> Coordinadora Academica: Informar que no hay comisiones finalizadas  (retorno)
9. FrmGenerarCertificado_83KI -> Coordinadora Academica: Mostrar pantalla con el listado de comisiones  (retorno)

**B. Buscar alumnos (ECU 3 y 4)**

10. Coordinadora Academica -> FrmGenerarCertificado_83KI: Seleccionar la comision y presionar Buscar
11. FrmGenerarCertificado_83KI -> FrmGenerarCertificado_83KI: Buscar()
12. FrmGenerarCertificado_83KI -> KI68CertificadoBLL: Buscar(idComision)
13. KI68CertificadoBLL -> KI68CertificadoBLL: Autorizar()
14. KI68CertificadoBLL -> KI68BaseDeDatos: ExecuteQuery(inscripciones con dos evaluaciones)
15. KI68BaseDeDatos -> KI68CertificadoBLL: alumnos  (retorno)
16. KI68CertificadoBLL -> FrmGenerarCertificado_83KI: alumnos  (retorno)
17. FrmGenerarCertificado_83KI -> Coordinadora Academica: Informar que no hay resultados  (retorno)
18. FrmGenerarCertificado_83KI -> Coordinadora Academica: Mostrar listado de alumnos  (retorno)

**C. Seleccionar alumno (ECU 5 y 6)**

19. Coordinadora Academica -> FrmGenerarCertificado_83KI: Seleccionar un alumno
20. FrmGenerarCertificado_83KI -> FrmGenerarCertificado_83KI: Seleccionar()
21. FrmGenerarCertificado_83KI -> KI68CertificadoBLL: Seleccionar(id)
22. KI68CertificadoBLL -> KI68CertificadoBLL: Autorizar()
23. KI68CertificadoBLL -> KI68BaseDeDatos: ExecuteQuery(inscripcion, notas, cuotas, observaciones y certificado)
24. KI68BaseDeDatos -> KI68CertificadoBLL: datos de la inscripcion  (retorno)
25. KI68CertificadoBLL -> KI68CertificadoBLL: ValidarEvaluaciones(a)
26. KI68CertificadoBLL -> FrmGenerarCertificado_83KI: alumno con certificado  (retorno)
27. FrmGenerarCertificado_83KI -> Coordinadora Academica: Mostrar certificado original y habilitar Guardar certificado  (retorno)
28. KI68CertificadoBLL -> FrmGenerarCertificado_83KI: alumno con motivos y cuotas pendientes  (retorno)
29. FrmGenerarCertificado_83KI -> Coordinadora Academica: Mostrar motivos y cuotas pendientes y habilitar Retener  (retorno)
30. KI68CertificadoBLL -> KI68CertificadoBLL: PrefijoFolio(a)
31. KI68CertificadoBLL -> FrmGenerarCertificado_83KI: alumno  (retorno)
32. FrmGenerarCertificado_83KI -> Coordinadora Academica: Precargar datos y prefijo del folio y habilitar los campos  (retorno)

**D. Generar certificado (ECU 7, 8 y 9)**

33. Coordinadora Academica -> FrmGenerarCertificado_83KI: Ingresar folio, modalidad, email, mencion y observaciones y presionar Generar
34. FrmGenerarCertificado_83KI -> FrmGenerarCertificado_83KI: Generar()
35. FrmGenerarCertificado_83KI -> KI68CertificadoBLL: Generar(r)
36. KI68CertificadoBLL -> KI68CertificadoBLL: Autorizar()
37. KI68CertificadoBLL -> KI68CertificadoBLL: Validar(r)
38. KI68CertificadoBLL -> FrmGenerarCertificado_83KI: error de datos  (retorno)
39. FrmGenerarCertificado_83KI -> Coordinadora Academica: Informar el error y volver a ingresar los datos  (retorno)
40. KI68CertificadoBLL -> KI68BaseDeDatos: BeginTransaction()
41. KI68CertificadoBLL -> KI68BaseDeDatos: ExecuteQuery(inscripcion, notas, cuotas y certificado)
42. KI68BaseDeDatos -> KI68CertificadoBLL: datos de la inscripcion  (retorno)
43. KI68CertificadoBLL -> KI68CertificadoBLL: ValidarEvaluaciones(a)
44. KI68CertificadoBLL -> KI68BaseDeDatos: RollbackTransaction()
45. KI68CertificadoBLL -> FrmGenerarCertificado_83KI: certificado existente  (retorno)
46. FrmGenerarCertificado_83KI -> Coordinadora Academica: Informar y mostrar el certificado original  (retorno)
47. KI68CertificadoBLL -> KI68BaseDeDatos: RollbackTransaction()
48. KI68CertificadoBLL -> FrmGenerarCertificado_83KI: inscripcion no habilitada  (retorno)
49. FrmGenerarCertificado_83KI -> Coordinadora Academica: Informar el cambio y mostrar las cuotas pendientes  (retorno)
50. KI68CertificadoBLL -> KI68CertificadoBLL: ComponerFolio(a, numero)
51. KI68CertificadoBLL -> KI68BaseDeDatos: ExecuteNonQuery(INSERT CertificadoFinalizacion)
52. KI68BaseDeDatos -> KI68CertificadoBLL: folio duplicado  (retorno)
53. KI68CertificadoBLL -> KI68BaseDeDatos: RollbackTransaction()
54. KI68CertificadoBLL -> FrmGenerarCertificado_83KI: folio duplicado  (retorno)
55. FrmGenerarCertificado_83KI -> Coordinadora Academica: Informar que el folio ya fue usado  (retorno)
56. KI68CertificadoBLL -> IntegridadDAL_83KI: ActualizarCertificado(id, conn, tran)
57. IntegridadDAL_83KI -> KI68BaseDeDatos: ExecuteNonQuery(DVH y DVV de CertificadoFinalizacion)
58. KI68CertificadoBLL -> KI68BaseDeDatos: CommitTransaction()
59. KI68CertificadoBLL -> KI68CertificadoBLL: Registrar(accion, a, detalle)
60. KI68CertificadoBLL -> BitacoraBLL_83KI: RegistrarEvento(evento)
61. BitacoraBLL_83KI -> KI68BaseDeDatos: ExecuteNonQuery(INSERT Bitacora)
62. KI68CertificadoBLL -> FrmGenerarCertificado_83KI: confirmacion  (retorno)
63. FrmGenerarCertificado_83KI -> Coordinadora Academica: Informar que el certificado fue generado y habilitar Guardar certificado  (retorno)

**E. Guardar certificado (ECU 10 y 11)**

64. Coordinadora Academica -> FrmGenerarCertificado_83KI: Presionar Guardar certificado
65. FrmGenerarCertificado_83KI -> FrmGenerarCertificado_83KI: Guardar()
66. FrmGenerarCertificado_83KI -> FrmGenerarCertificado_83KI: NombreArchivo(c)
67. FrmGenerarCertificado_83KI -> Coordinadora Academica: Abrir el dialogo de guardado  (retorno)
68. Coordinadora Academica -> FrmGenerarCertificado_83KI: Confirmar la ubicacion
69. FrmGenerarCertificado_83KI -> FrmGenerarCertificado_83KI: Exportar(c, path)
70. FrmGenerarCertificado_83KI -> Coordinadora Academica: Informar la cancelacion o el error (el certificado no cambia)  (retorno)
71. FrmGenerarCertificado_83KI -> Coordinadora Academica: Mostrar confirmacion de guardado  (retorno)

### Fragmentos alt

- **A1** (nivel 1, mensajes 8 a 9)
  - desde 8: [no hay comisiones finalizadas]  (ECU 2.1)
  - desde 9: [hay comisiones]
- **B1** (nivel 1, mensajes 17 a 18)
  - desde 17: [sin resultados]  (ECU 4.1)
  - desde 18: [con resultados]
- **C1** (nivel 1, mensajes 26 a 32)
  - desde 26: [ya existe certificado]  (ECU 6.1)
  - desde 28: [desaprobado o saldo pendiente > 0]  (ECU 5.1 / 6.2)
  - desde 30: [aprobado sin saldo y sin certificado]  (ECU principal)
- **D1** (nivel 1, mensajes 38 a 63)
  - desde 38: [datos invalidos]  (ECU 8.1)
  - desde 40: [datos validos]
- **D2** (nivel 2, mensajes 44 a 63)
  - desde 44: [ya existe certificado]  (ECU 8.3)
  - desde 47: [con saldo pendiente o no aprobado]  (ECU 8.2)
  - desde 50: [aprobado, sin saldo y sin certificado]  (ECU principal)
- **D3** (nivel 3, mensajes 52 a 63)
  - desde 52: [folio ya usado]  (ECU 8.1)
  - desde 56: [folio disponible]  (ECU principal)
- **E1** (nivel 1, mensajes 70 a 71)
  - desde 70: [cancelado o error al escribir el archivo]  (ECU 11.1)
  - desde 71: [archivo guardado]  (ECU principal)

## CUN09 - DSS Retener certificado
Alternativas 5.1 y 6.2 (retencion)


**F. Retener certificado (ECU 5.1 y 6.2)**

1. Coordinadora Academica -> FrmGenerarCertificado_83KI: Ingresar observaciones y presionar Retener
2. FrmGenerarCertificado_83KI -> FrmGenerarCertificado_83KI: Retener()
3. FrmGenerarCertificado_83KI -> KI68CertificadoBLL: Retener(id, observaciones)
4. KI68CertificadoBLL -> KI68CertificadoBLL: Autorizar()
5. KI68CertificadoBLL -> FrmGenerarCertificado_83KI: error de datos  (retorno)
6. FrmGenerarCertificado_83KI -> Coordinadora Academica: Informar el error y volver a ingresar las observaciones  (retorno)
7. KI68CertificadoBLL -> KI68BaseDeDatos: ExecuteQuery(inscripcion, notas, cuotas y certificado)
8. KI68BaseDeDatos -> KI68CertificadoBLL: datos de la inscripcion  (retorno)
9. KI68CertificadoBLL -> KI68CertificadoBLL: ValidarEvaluaciones(a)
10. KI68CertificadoBLL -> FrmGenerarCertificado_83KI: reconfirmar motivos  (retorno)
11. FrmGenerarCertificado_83KI -> Coordinadora Academica: Informar el cambio y mostrar los motivos actualizados  (retorno)
12. KI68CertificadoBLL -> KI68CertificadoBLL: Registrar(accion, a, detalle)
13. KI68CertificadoBLL -> BitacoraBLL_83KI: RegistrarEvento(evento)
14. BitacoraBLL_83KI -> KI68BaseDeDatos: ExecuteNonQuery(INSERT Bitacora)
15. KI68CertificadoBLL -> FrmGenerarCertificado_83KI: confirmacion  (retorno)
16. FrmGenerarCertificado_83KI -> Coordinadora Academica: Informar que la retencion fue asentada  (retorno)

### Fragmentos alt

- **F1** (nivel 1, mensajes 5 a 16)
  - desde 5: [observaciones > 500 caracteres]  (ECU 5.1.4.2 / 6.2.4.2)
  - desde 7: [observaciones validas]
- **F2** (nivel 2, mensajes 10 a 16)
  - desde 10: [cambiaron los motivos]  (ECU 5.1.4.1 / 6.2.4.1)
  - desde 12: [los motivos continuan]  (ECU principal)
