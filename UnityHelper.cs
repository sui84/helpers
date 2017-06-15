   public static class UnityHelper    {        private static IUnityContainer unityContainer = new UnityContainer();
        static UnityHelper()        {
        }
        public static IUnityContainer UnityContainer        {            get            {                return unityContainer;            }            set            {                unityContainer = value;            }        }
        public static T Resolve<T>(params ResolverOverride[] overrides)        {            return unityContainer.Resolve<T>(overrides);        }
        public static T Resolve<T>(string name, params ResolverOverride[] overrides)        {            return unityContainer.Resolve<T>(name, overrides);        }    }
