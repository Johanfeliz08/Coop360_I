using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public class CategoriaPrestamo {

  [Key]
  public required int ID_CATEGORIA_PRESTAMO { get; set; }
  public required string NOMBRE { get; set; }
  public required string DESCRIPCION { get; set; }
  public required decimal TASA_INTERES_BASE { get; set; }
  public required decimal TASA_INTERES_MINIMA { get; set; }
  public required decimal TASA_INTERES_MAXIMA { get; set; }
  public required int PLAZO_ESTANDAR { get; set; }
  public required int PLAZO_MINIMO { get; set; }
  public required int PLAZO_MAXIMO { get; set; }
  public DateTime? FECHA_CREACION { get; set; }
  public int? CREADO_POR { get; set; }

}