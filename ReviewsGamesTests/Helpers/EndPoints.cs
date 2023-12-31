﻿using Microsoft.AspNetCore.Mvc;
using ReviewsDeGames.Controllers;
using ReviewsDeGames.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReviewsGamesIntegrationTests.Helpers
{
   
    public static class EndPoints
    {
       
        /// <summary>
        /// Obtém o endeço do <paramref name="action"/> resolvendo a rota do controlador <typeparamref name="T"/>
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public static string Resolve<T>(string action, params string[] placeholder) where T : ControllerBase
        {
            var controllerType = typeof(T);

            var routeAttributes = controllerType.GetCustomAttributes<RouteAttribute>();
            var template = routeAttributes.FirstOrDefault()?.Template ?? throw new ArgumentException($"{typeof(T)} não possui atributo de rota");
            
            template = template.Replace("[controller]", controllerType.Name.Replace("Controller", ""));
            if(placeholder.Length > 0)
                return template + "/" + action.Placeholder(placeholder);
            else
                return template + "/" + action;

        }
    }
}
