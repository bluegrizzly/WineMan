#define MICROVIN

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Optimization;


namespace WineMan
{
    public class Global : System.Web.HttpApplication
    {
        public Global()
        {
        }

        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Create an administrator
            if (!Roles.RoleExists("Administrator"))
                Roles.CreateRole("Administrator");

            Membership.DeleteUser("admin");
#if (MICROVIN)
            MembershipUser adminUser = Membership.CreateUser("admin", "piepasri");
#else
            MembershipUser adminUser = Membership.CreateUser("admin", "wineman");
#endif
            if (!Roles.IsUserInRole("admin", "Administrator"))
                Roles.AddUserToRole("admin", "Administrator");

            // make sure all db are created
            Utils.InitialSetup();
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown

        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started

        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.

        }

    }
}
