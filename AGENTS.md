# Layer ownership

- `BE` owns domain entities and business DTOs. Keep business DTO files under `BE/DTOs/<Domain>` and declare matching namespaces such as `BE.DTOs.Pagos` or `BE.DTOs.Comisiones`.
- `Service` owns reusable application contracts and service-boundary DTOs only. DTOs used exclusively to transport application/bootstrap, connection configuration, audit-view/filtering, or integrity state are stored under `Service/DTOs` in `Service.DTOs`.
- Do not place business DTOs in `Service.DTOs` merely because a Service interface consumes them. Service interfaces may reference `BE` business DTOs.
- `Service` must not reference concrete BLL or DAL implementations. Keep abstractions in Service and implementations in their owning projects.
- The concrete `ServiceFactory` belongs to the composition root (currently UI), where implementation projects can be referenced. Do not put the concrete factory in Service or route existing direct UI compositions through it as part of M-6.
- `ComisionXml` is a business serialization contract for commission data and belongs in `BE.DTOs.Comisiones`.
