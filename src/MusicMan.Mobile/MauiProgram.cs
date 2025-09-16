using Microsoft.Extensions.DependencyInjection;
using System;

namespace MusicMan.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>();

        builder.Services.AddSingleton(sp =>
        {
            var baseUri = new Uri("http://localhost:5031");
#if ANDROID
            baseUri = new Uri("http://10.0.2.2:5031");
#endif
            var http = new HttpClient { BaseAddress = baseUri };
            return new Services.CollectionApiClient(http);
        });
        builder.Services.AddSingleton<MainPage>();

        return builder.Build();
    }
}
