using System.ComponentModel.DataAnnotations;

public class Rol
{

  [Key]
  public required int ID_ROL { get; set; }
  public required string NOMBRE { get; set; }
  public required string DESCRIPCION { get; set; }

}

