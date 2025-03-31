using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Departamento {

  [Key]
  public required int ID_DEPARTAMENTO {get; set;}
  public required string NOMBRE {get; set;}
  public required string ENCARGADO {get; set;}
  public DateTime? FECHA_CREACION {get; set;}

}