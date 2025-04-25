using System.ComponentModel.DataAnnotations;

public class PermisoRol
{
  [Key]
  public required string PERMISO { get; set; }
  public string? MODULO { get; set; }


}