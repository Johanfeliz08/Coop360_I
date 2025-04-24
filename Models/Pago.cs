using System.ComponentModel.DataAnnotations;

public class Pago
{

  [Key]
  public int? ID_PAGO { get; set; }
  public required int ID_PRESTAMO { get; set; }
  public required int NUMERO_CUOTA { get; set; }
  public required int CODIGO_EMPLEADO { get; set; }
  public required decimal MONTO_A_PAGAR { get; set; }
  public required decimal MONTO_RECIBIDO { get; set; }
  public required decimal MONTO_DEVUELTO { get; set; }
  public required decimal MONTO_INTERES { get; set; }
  public required decimal MONTO_CAPITAL { get; set; }
  public required decimal MONTO_RESTANTE { get; set; }
  public required decimal IMPUESTO { get; set; }
  public required DateTime FECHA { get; set; }
  public string? ESTATUS { get; set; }

}