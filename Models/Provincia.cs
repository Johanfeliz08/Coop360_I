using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Provincia {
    [Key]
    public required int ID_PROVINCIA { get; set; }
    public string? NOMBRE { get; set; }
    public required int ID_REGION { get; set; }


}