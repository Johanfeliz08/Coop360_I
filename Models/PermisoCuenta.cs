using System.ComponentModel.DataAnnotations;

public class PermisoCuenta
{

    [Key]
    public required string PERMISO { get; set; }
    public string? MODULO { get; set; }

}


