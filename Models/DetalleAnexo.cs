using System.ComponentModel.DataAnnotations;

public class DetalleAnexo
{

    [Key]
    public required int ID_DETALLE_ANEXO { get; set; }
    public required int ID_ANEXO { get; set; }

    public string? NOMBRE_ANEXO { get; set; }

    public required int ID_SOLICITUD_PRESTAMO { get; set; }
    public string? VALOR { get; set; }

    public string? TIPO { get; set; }
}

