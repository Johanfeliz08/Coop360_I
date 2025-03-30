using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Puesto {

  [Key]
  public required int ID_PUESTO {get; set;}
  public required string NOMBRE {get; set;}
  public required decimal SALARIO {get; set;}
  public int? ID_DEPARTAMENTO {get; set;}
  public string? NOMBRE_DEPARTAMENTO {get; set;}
  public DateTime? FECHA_CREACION {get; set;} 

}