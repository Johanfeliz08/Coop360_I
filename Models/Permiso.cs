using System.ComponentModel.DataAnnotations;

public class Permiso {

    [Key]
    public required string PERMISO { get; set; }
    public string? MODULO { get; set; }

}


