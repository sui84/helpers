using Microsoft.Practices.Unity;

namespace Common.Utils
{
    public static class UnityHelper
    {

        //using Microsoft.Practices.Unity;
        //1.IGetPayslipBLL getPayslip = new GetWCFPayslipBLL();
        //2.IUnityContainer container = new UnityContainer();
        //  container.RegisterType<IGetPayslipBLL, GetWCFPayslipBLL>();
        //3.UnityConfigurationSection section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
        //IUnityContainer container = new UnityContainer();
        //container.LoadConfiguration();
        //IGetPayslipBLL getPayslip = container.Resolve<IGetPayslipBLL>();
        //4.UnityConfigurationSection section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
        //IUnityContainer container = new UnityContainer();
        //section.Containers.Default.Configure(container);
        //IGetPayslipBLL getPayslip = container.Resolve<IGetPayslipBLL>();

        private static IUnityContainer unityContainer = new UnityContainer();

        static UnityHelper()
        {

        }

        public static IUnityContainer UnityContainer
        {
            get
            {
                return unityContainer;
            }
            set
            {
                unityContainer = value;
            }
        }

        public static T Resolve<T>(params ResolverOverride[] overrides)
        {
            return unityContainer.Resolve<T>(overrides);
        }

        public static T Resolve<T>(string name, params ResolverOverride[] overrides)
        {
            return unityContainer.Resolve<T>(name, overrides);
        }

    }
}
