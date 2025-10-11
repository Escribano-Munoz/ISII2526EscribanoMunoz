using Microsoft.AspNetCore.Identity;

namespace AppForSEII2526.API.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser {
    public ApplicationUser()
    {
    }
    public ApplicationUser(int id, string nombreCliente, string apellidoCliente, string numTelefono, tiposMetodosPago metodoPago)
    {
        Id = id;
        NombreCliente = nombreCliente;
        ApellidoCliente = apellidoCliente;
        NumTelefono = numTelefono;
        MetodoPago = metodoPago;
    }

    public int Id { get; set; }

    [Display(Name = "Nombre del Cliente")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, ingrese el nombre del cliente")]
    public string NombreCliente { get; set; }

    [Display(Name = "Apellido del Cliente")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, ingrese el apellido del cliente")]
    public string ApellidoCliente { get; set; }

    [Display(Name = "Número de Teléfono")]
    public string? NumTelefono { get; set; }

    [Display(Name = "Metodo de pago")]
    [Required]
    public tiposMetodosPago MetodoPago { get; set; }

}
