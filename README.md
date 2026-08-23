# Sistema de Control de Calidad — Oreplast

Aplicación web para el control de calidad de empaques flexibles en **Oreplast S.A.** (planta Mixco z.2). Proyecto de graduación (UMG).

Digitaliza la inspección en proceso, detecta variaciones de los parámetros contra la ficha técnica del producto en tiempo real y busca reducir el rechazo de lotes completos.

Este repositorio contiene la **aplicación web** (front-end + lógica). Los procedimientos almacenados se desarrollan e integran a la base de datos por separado.

## Stack técnico

- Blazor Web App, modo de render **Servidor** (Blazor Server con SignalR)
- .NET 10 (LTS) / ASP.NET Core
- C#
- Entity Framework Core (acceso a datos), apoyado en procedimientos almacenados
- SQL Server — base de datos: `OreplastCalidad`
- Autenticación: Windows Authentication (dominio OREPLAST) en producción
- Despliegue: IIS en la red interna de la planta
- UI: componentes propios con estilo tipo Infor IQM (azul `#1f6fb2`)

## Roles del sistema

- **Operador de máquina**: registra inspección, setup, producción por bobina, despeje
- **Ingeniería de Calidad**: único que libera producto y cierra orden; gestiona no conformidades, fichas técnicas y certificados
- **Jefe de Producción**: seguimiento por línea/turno, asignación de operadores
- **Gerente de Producción**: indicadores (KPI) y tendencias, consulta
- **Administrador**: usuarios, roles, catálogos, bitácora

## Autenticación y autorización

- En **producción**: Windows Authentication. La identidad llega en `HttpContext.User.Identity.Name` como `OREPLAST\usuario`. La app busca ese usuario de dominio en `seg.Usuario` y resuelve su rol y permisos (`seg.Rol`, `seg.Permiso`, `seg.RolPermiso`). Si el usuario no existe o está inactivo, se muestra "Acceso No Autorizado".
- En **desarrollo** (fuera del dominio): un modo simula un usuario de dominio y permite cambiar de rol, para probar la autorización y la demostración ante el jurado. Este modo no existe en producción.

## Convenciones de código

- C# con nombres en inglés (clases, métodos); textos visibles al usuario en español (UI en español).
- Nombres de entidades EF alineados con las tablas reales (`esquema.Tabla`).
- Separación de responsabilidades: Componentes (UI) / Servicios (lógica) / Acceso a datos (repositorios o DbContext que invocan los SP).
- Procedimientos almacenados para operaciones de escritura y consultas complejas; EF Core los invoca.
- Comentarios y mensajes de usuario en español.
- Lógica de autorización por rol centralizada, no repetida en cada componente.
