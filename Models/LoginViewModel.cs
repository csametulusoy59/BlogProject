﻿using System.ComponentModel.DataAnnotations;

namespace BlogProject.Models.ViewModels // Projenizin ViewModels klasörüne göre ayarlayın
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı gerekli.")]
        [Display(Name = "Kullanıcı Adı")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Şifre gerekli.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; }

        [Display(Name = "Beni Hatırla")]
        public bool RememberMe { get; set; }
    }
}
