using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Ciudad {
    [Key]
    public required int ID_CIUDAD { get; set; }
    public string? NOMBRE { get; set; }
    public required int ID_PROVINCIA { get; set; }


}