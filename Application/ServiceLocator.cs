using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp
{
	public class ServiceLocator
	{
		private static Dictionary<Type, Func<IService>> _services = new Dictionary<Type, Func<IService>>();
		private static readonly Dictionary<Type, IService> _singletonInstances = new Dictionary<Type, IService>();

		public static void RegisterSingleton<T>(Func<IService> resolver)
		{
			_services[typeof(T)] = resolver;
			_singletonInstances[typeof(T)] = resolver();
		}

		public static T ResolveSingleton<T>()
		{
			if (!_services.ContainsKey(typeof(T)))
			{
				throw new InvalidOperationException($"Service {typeof(T)} not registered.");
			}

			return (T)_singletonInstances[typeof(T)];
		}
	}
}
