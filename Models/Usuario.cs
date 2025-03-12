
using System.ComponentModel.DataAnnotations;

public class Usuario {

    [Key]
    public int ID_USUARIO { get; set; }
    public required int CODIGO_EMPLEADO { get; set; }
    public required string CONTRASENA { get; set; }

    public DateTime FECHA_CREACION { get; set; }
}