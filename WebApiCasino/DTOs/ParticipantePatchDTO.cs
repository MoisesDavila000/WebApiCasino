﻿using System.ComponentModel.DataAnnotations;
using WebApiCasino.Validaciones;

namespace WebApiCasino.DTOs
{
    public class ParticipantePatchDTO
    {
        [Required(ErrorMessage = "El campo de nombre es requerido")]
        [StringLength(maximumLength: 50, ErrorMessage = "El campo de nombre no puede excede los 50 caracteres")]
        [NombresConMayuscula]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El campo de telefono es requerido")]
        [StringLength(maximumLength: 10, ErrorMessage = "El campo de nombre no puede excede los 10 caracteres")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "El campo de correo electronico es requerido")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
