using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Sector {
    [Key]
    public required int ID_SECTOR { get; set; }
    public string? NOMBRE { get; set; }
    public required int ID_CIUDAD { get; set; }


}