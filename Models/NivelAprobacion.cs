using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class NivelAprobacion {

  [Key]
  public required string ID_NIVEL_APROBACION {get; set;}
  public required decimal MONTO_DESDE {get; set;}
  public required decimal MONTO_HASTA {get; set;}
  public required DateTime FECHA_CREACION {get; set;}
  
}