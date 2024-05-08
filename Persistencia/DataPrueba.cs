using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Dominio;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Persistencia
{
    public class DataPrueba
    {
        public static async Task InsertarData(CursosOnlineContext context, UserManager<Usuario> usuarioManager)
        {
            if (!usuarioManager.Users.Any())
            {
                var usuario = new Usuario
                {
                    NombreCompleto = "LuisLachapel",
                    UserName = "luislachapel",
                    Email = "angellachapel@hotmail.com",

                };
                await usuarioManager.CreateAsync(usuario,"Contraseña123$");
            }
        }
    }
}
