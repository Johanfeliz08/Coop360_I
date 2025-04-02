using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class NivelAprobacion {

  [Key]
  public int? ID_NIVEL_APROBACION {get; set;}
  public string? NOMBRE {get; set;}
  public required decimal MONTO_DESDE {get; set;}
  public required decimal MONTO_HASTA {get; set;}
  public DateTime? FECHA_CREACION {get; set;}
  
}