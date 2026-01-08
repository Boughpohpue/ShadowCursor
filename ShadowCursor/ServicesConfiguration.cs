using Microsoft.Extensions.DependencyInjection;

namespace ShadowCursor
{
    public static class ServicesConfiguration
    {
        public static void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<TransparentWindow, TransparentWindow>();
            services.AddSingleton<MainWindow, MainWindow>();
        }
    }
}
