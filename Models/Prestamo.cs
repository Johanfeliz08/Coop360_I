using System.ComponentModel.DataAnnotations;

public class Prestamo
{
  [Key]
  public required int ID_PRESTAMO { get; set; }

  public int? ID_SOLICITUD { get; set; }

  public int? CODIGO_SOCIO { get; set; }

  public string? NOMBRE_SOCIO { get; set; }

  public int? SOLICITUD_REALIZADA_POR { get; set; }

  public string? NOMBRE_SOLICITUD_REALIZADA_POR { get; set; }

  public int? SOLICITUD_APROBADA_RECHAZADA_POR { get; set; }

  public string? NOMBRE_SOLICITUD_APROBADA_RECHAZADA_POR { get; set; }

  public DateTime? FECHA_APROBACION_RECHAZO { get; set; }

  public required decimal MONTO_APROBADO { get; set; }

  public required decimal BALANCE_RESTANTE { get; set; }

  public required int PLAZO_MESES { get; set; }

  public required int CANTIDAD_CUOTAS { get; set; }

  public decimal? MONTO_POR_CUOTA { get; set; }

  public required decimal TASA_INTERES { get; set; }

  public int? ID_CATEGORIA_PRESTAMO { get; set; }

  public string? NOMBRE_CATEGORIA_PRESTAMO { get; set; }

  public DateTime? FECHA_CREACION { get; set; }

  public DateTime? FECHA_INICIO { get; set; }

  public DateTime? FECHA_FINAL { get; set; }

  public int? CODIGO_CODEUDOR_PRINCIPAL { get; set; }

  public string? NOMBRE_CODEUDOR_PRINCIPAL { get; set; }

  public int? CODIGO_CODEUDOR_SECUNDARIO { get; set; }

  public string? NOMBRE_CODEUDOR_SECUNDARIO { get; set; }

  public int? CODIGO_CODEUDOR_TERCIARIO { get; set; }

  public string? NOMBRE_CODEUDOR_TERCIARIO { get; set; }

  public string? ESTATUS_DESEMBOLSO { get; set; }

  public string? ESTATUS { get; set; }

}
