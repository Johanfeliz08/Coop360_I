using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public class Socio {

  [Key]
    public int CODIGO_SOCIO { get; set; }
    public required string CEDULA { get; set; }
    public required string P_NOMBRE { get; set; }
    public string? S_NOMBRE { get; set; }
    public required string P_APELLIDO { get; set; }
    public string? S_APELLIDO { get; set; }
    public required string DIRECCION { get; set; }
    public int? ID_SECTOR { get; set; }
    public string? NOMBRE_SECTOR { get; set; }
    public int? ID_CIUDAD { get; set; }
    public string? NOMBRE_CIUDAD { get; set; }
    public int? ID_PROVINCIA { get; set; }
    public string? NOMBRE_PROVINCIA { get; set; }
    public required string PAIS_NACIMIENTO { get; set; }
    public required string TELEFONO_PRINCIPAL { get; set; }
    public string? TELEFONO_SECUNDARIO { get; set; }
    public required DateTime FECHA_NACIMIENTO { get; set; }
    public string? EMAIL { get; set; }
    public required string SEXO { get; set; }
    public required string ESTADO_CIVIL { get; set; }
    public DateTime FECHA_CREACION { get; set; }
    public required string PROFESION { get; set; }
    public required string CARGO { get; set; }
    public required string LUGAR_TRABAJO { get; set; }
    public required string DIRECCION_TRABAJO { get; set; }
    public string? TELEFONO_TRABAJO { get; set; }
    public required string TIPO_CONTRATO { get; set; }
    public required decimal INGRESOS_MENSUALES { get; set; }
    public DateTime FECHA_INGRESO_TRABAJO { get; set; }
    public required string FRECUENCIA_COBRO { get; set; }
    public required string ESTATUS { get; set; }
    public int? CREADO_POR { get; set; }


}