using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Puesto {

  [Key]
  public required int ID_PUESTO {get; set;}
  public required string NOMBRE {get; set;}
  public required decimal SALARIO {get; set;}
  public required int ID_DEPARTAMENTO {get; set;}
  public required DateTime FECHA_CREACION {get; set;}

}