using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace ExpressionTreeExample.Extensions
{
    public static class ControllerExtension
    {
        public static IActionResult RedirectTo<TDestination>(this ControllerBase controller,
            Expression<Action<TDestination>> destinationAction) where TDestination : ControllerBase
        {
            if (destinationAction.Body.NodeType != ExpressionType.Call)
            {
                throw new InvalidOperationException($"Expression is not a valid method call: {destinationAction.Body}");
            }
            var methodCallExpression = (MethodCallExpression) destinationAction.Body;
            var actionName = GetActionName(methodCallExpression);
            var controllerName = typeof(TDestination).Name.ToUpper().Replace(nameof(Controller).ToUpper(), string.Empty);
            controllerName = controllerName.ToLower();
            var parameters = ExtractRouteValue(methodCallExpression);
            return controller.RedirectToAction(actionName, controllerName, parameters);
        }
        private static RouteValueDictionary ExtractRouteValue(MethodCallExpression methodCallExpression)
        {
            var parameters = methodCallExpression.Method.GetParameters().Select(e => e.Name).ToArray();
            var values = methodCallExpression.Arguments.Select(arg =>
            {
                if (arg.NodeType == ExpressionType.Constant)
                {
                    var constExt = (ConstantExpression) arg;
                    return constExt.Value;
                }

                var convertedValue = Expression.Convert(arg, typeof(object));
                var funExpression = Expression.Lambda<Func<object>>(convertedValue);
                return funExpression.Compile().Invoke();
            }).ToArray();

            RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] is null || values[i] is null)
                {
                    continue;
                }
                routeValueDictionary.TryAdd(parameters[i], values[i]);

            }

            return routeValueDictionary;
        }

        private static string GetActionName(MethodCallExpression expression)
        {
            var methodName = expression.Method.Name;

            var actionName = 
                expression
                    .Method
                    .GetCustomAttributes(true)
                    .OfType<ActionNameAttribute>()
                    .FirstOrDefault()
                    ?.Name;

            return actionName ?? methodName;

        }
    }
}
