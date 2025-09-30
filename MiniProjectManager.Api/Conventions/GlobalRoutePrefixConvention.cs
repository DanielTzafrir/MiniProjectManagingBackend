using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;

public class GlobalRoutePrefixConvention : IApplicationModelConvention
{
    private readonly AttributeRouteModel _prefix;

    public GlobalRoutePrefixConvention(IRouteTemplateProvider routeTemplateProvider)
    {
        _prefix = new AttributeRouteModel(routeTemplateProvider);
    }

    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            foreach (var selector in controller.Selectors.Where(s => s.AttributeRouteModel != null))
            {
                selector.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(_prefix, selector.AttributeRouteModel);
            }

            foreach (var selector in controller.Selectors.Where(s => s.AttributeRouteModel == null))
            {
                selector.AttributeRouteModel = _prefix;
            }
        }
    }
}