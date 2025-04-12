public class SolicitudPrestamoViewModel
{

  public SolicitudPrestamo? SolicitudPrestamo { get; set; }
  public required List<Socio> Socios { get; set; }
  public required List<CategoriaPrestamo> CategoriasPrestamos { get; set; }
  public required List<Codeudor> Codeudores { get; set; }

}
