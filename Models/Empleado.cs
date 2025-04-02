using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Empleado
{
    [Key]
    public int CODIGO_EMPLEADO { get; set; }
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
    public required string FRECUENCIA_COBRO { get; set; }
    public required string CUENTA_BANCO { get; set; }
    public int? ID_ENTIDAD_BANCARIA { get; set; }
    public string? NOMBRE_ENTIDAD_BANCARIA { get; set; }
    public int? ID_PUESTO { get; set; }
    public string? NOMBRE_PUESTO { get; set; }
    public int? ID_DEPARTAMENTO { get; set; }
    public string? NOMBRE_DEPARTAMENTO { get; set; }
    public required string TIPO_SANGRE { get; set; }
    public string? NOMBRE_FAMILIAR_PRIMARIO { get; set; }
    public string? TELEFONO_FAMILIAR_PRIMARIO { get; set; }
    public string? PARENTESCO_FAMILIAR_PRIMARIO { get; set; }
    public string? NOMBRE_FAMILIAR_SECUNDARIO { get; set; }
    public string? TELEFONO_FAMILIAR_SECUNDARIO { get; set; }
    public string? PARENTESCO_FAMILIAR_SECUNDARIO { get; set; }
    public int? ID_NIVEL_APROBACION {get; set;}
    public string? NOMBRE_NIVEL_APROBACION {get; set;}
    public required string ESTATUS { get; set; }
    public int? CREADO_POR { get; set; }
}
