using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CodeCampster.Web.Areas.AzureADB2C.Controllers;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace CodeCampster.Web.Auth
{
    internal class AzureADB2CAccountControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>, IApplicationFeatureProvider
    {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            if (!feature.Controllers.Contains(typeof(AccountController).GetTypeInfo()))
            {
                feature.Controllers.Add(typeof(AccountController).GetTypeInfo());
            }
        }
    }
}
