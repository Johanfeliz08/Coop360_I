using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Anexo {

  [Key]
  public required int ID_ANEXO {get; set;}
  public required string NOMBRE {get; set;}
  public required string OBLIGATORIO {get; set;}
  public int? ID_NIVEL_APROBACION {get; set;}
  public string? NOMBRE_NIVEL_APROBACION {get; set;}

}