using System.ComponentModel.DataAnnotations;

public class CuentaUsuario
{

  [Key]
  public required int ID_USUARIO { get; set; }
  public int? CODIGO_EMPLEADO { get; set; }
  public string? NOMBRE_EMPLEADO { get; set; }
  public string? CONTRASENA { get; set; }
  public int? ID_ROL { get; set; }
  public string? NOMBRE_ROL { get; set; }

}
