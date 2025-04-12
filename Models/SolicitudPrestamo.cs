using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class SolicitudPrestamo
{

  [Key]
  public required int ID_SOLICITUD { get; set; }

  public required int CODIGO_SOCIO { get; set; }

  public string? SOCIO { get; set; }

  public required int SOLICITUD_REALIZADA_POR { get; set; }

  public string? NOMBRE_SOLICITUD_REALIZADA_POR { get; set; }

  public int? SOLICITUD_APROBADA_RECHAZADA_POR { get; set; }

  public string? NOMBRE_SOLICITUD_APROBADA_RECHAZADA_POR { get; set; }

  public required decimal MONTO_SOLICITADO { get; set; }

  public required DateTime FECHA_SOLICITUD { get; set; }

  public DateTime? FECHA_APROBACION_RECHAZO { get; set; }

  public required int PLAZO_MESES { get; set; }

  public required int CANTIDAD_CUOTAS { get; set; }

  public required decimal MONTO_POR_CUOTA { get; set; }

  public required decimal TASA_INTERES { get; set; }

  public required int ID_CATEGORIA_PRESTAMO { get; set; }

  public string? NOMBRE_CATEGORIA_PRESTAMO { get; set; }

  public required int ID_NIVEL_APROBACION_REQUERIDO { get; set; }

  public string? NOMBRE_NIVEL_APROBACION { get; set; }

  public int? CODIGO_CODEUDOR_PRINCIPAL { get; set; }

  public string? NOMBRE_CODEUDOR_PRINCIPAL { get; set; }

  public int? CODIGO_CODEUDOR_SECUNDARIO { get; set; }

  public string? NOMBRE_CODEUDOR_SECUNDARIO { get; set; }

  public int? CODIGO_CODEUDOR_TERCIARIO { get; set; }

  public string? NOMBRE_CODEUDOR_TERCIARIO { get; set; }

  public required string ESTATUS { get; set; }


}