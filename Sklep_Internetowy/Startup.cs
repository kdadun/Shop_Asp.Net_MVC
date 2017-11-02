using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Sklep_Internetowy.Startup))]
namespace Sklep_Internetowy
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
