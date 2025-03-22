using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class EntidadBancaria {

  [Key]
  public required int ID {get; set;}
  public required string NOMBRE {get; set;}

}