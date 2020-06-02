using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IVR_C19SchedulesMVCWebApplication4.Startup))]
namespace IVR_C19SchedulesMVCWebApplication4
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
