using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Anexo {

  [Key]
  public required int ID_ANEXO {get; set;}
  public required string NOMBRE {get; set;}
  public required string OBLIGATORIO {get; set;}
  public required string ID_NIVEL_APROBACION {get; set;}
  
}