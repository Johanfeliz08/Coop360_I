using System.ComponentModel.DataAnnotations;

public class Permiso
{

  [Key]
  public required int ID_PERMISO { get; set; }

  public required string NOMBRE { get; set; }

  public required string DESCRIPCION { get; set; }

  public required string MODULO { get; set; }

}
