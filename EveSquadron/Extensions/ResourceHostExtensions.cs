using System;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace EveSquadron.Extensions;

public static class ResourceHostExtensions
{
    // https://www.reddit.com/r/AvaloniaUI/comments/ssplp9/comment/hx0e3zi/
    public static IServiceProvider GetServiceProvider(this IResourceHost control) => (IServiceProvider?)control.FindResource(typeof(IServiceProvider)) ??
                                                                                     throw new Exception("Expected service provider missing");

    public static T CreateInstance<T>(this IResourceHost control) => ActivatorUtilities.CreateInstance<T>(control.GetServiceProvider());
}