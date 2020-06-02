using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IVR_C19SchedulesMVCWebApplication3.Startup))]
namespace IVR_C19SchedulesMVCWebApplication3
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
