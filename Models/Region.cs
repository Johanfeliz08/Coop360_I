using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Region {
    [Key]
    public required int ID_REGION { get; set; }
    public required string REGION { get; set; }


}