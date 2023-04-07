using System;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace EVEye.Extensions
{
    public static class ResourceHostExtensions
    {
        // https://www.reddit.com/r/AvaloniaUI/comments/ssplp9/comment/hx0e3zi/
        public static IServiceProvider GetServiceProvider(this IResourceHost control)
        {
            return (IServiceProvider?)control.FindResource(typeof(IServiceProvider)) ??
                   throw new Exception("Expected service provider missing");
        }

        public static T CreateInstance<T>(this IResourceHost control)
        {
            return ActivatorUtilities.CreateInstance<T>(control.GetServiceProvider());
        }
    }
}