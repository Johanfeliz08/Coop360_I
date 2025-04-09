public class CuentaUsuarioViewModel
{

  public required CuentaUsuario CuentaUsuario { get; set; }
  public required List<Rol> Roles { get; set; }
  public required List<Permiso> Permisos { get; set; }
  public required List<PermisoCuenta> PermisosRol { get; set; }
  public required List<PermisoCuenta> PermisosUsuario { get; set; }

}
