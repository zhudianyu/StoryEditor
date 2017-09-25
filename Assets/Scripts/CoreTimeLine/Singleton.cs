using UnityEngine;
using System.Collections;
namespace TimeLineCore
{
	/// <summary>
	/// Be aware this will not prevent a non singleton constructor
	///   such as `T myT = new T();`
	/// To prevent that, add `protected T () {}` to your singleton class.
	/// 
	/// As a note, this is made as MonoBehaviour because we need Coroutines.
	/// </summary>
	/// <example><code>
	/// public class MyClass : MonoBehaviour {
	/// 	void Awake () {
	/// 		Debug.Log(Manager.Instance.myGlobalVar);
	/// 	}
	/// }
	/// 
	/// public class Manager : Singleton<Manager> {
	/// 	protected Manager () {} // guarantee this will be always a singleton only - can't use the constructor!
	///  
	/// 	public string myGlobalVar = "whatever";
	/// }
	/// </code></example>
	/// <remarks>ref: http://wiki.unity3d.com/index.php?title=Singleton </remarks>
	public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static T _instance;

		private static object _lock = new object();

		public static T Instance
		{
			get
			{
				if (applicationIsQuitting)
				{
					Debug.LogWarning(string.Format("[Singleton] Instance '{0}' already destroyed on application quit. Won't create again - returning null.", typeof(T)));
					return null;
				}

				lock (_lock)
				{
					if (_instance == null)
					{
						_instance = (T)FindObjectOfType(typeof(T));

						if (FindObjectsOfType(typeof(T)).Length > 1)
						{
							Debug.LogError("[Singleton] Something went really wrong  - there should never be more than 1 singleton! Reopenning the scene might fix it.");
							return _instance;
						}

						if (_instance == null)
						{
							GameObject singleton = new GameObject("(singleton) " + typeof(T));
							_instance = singleton.AddComponent<T>();
							DontDestroyOnLoad(singleton);

							Debug.Log(string.Format("[Singleton] An instance of {0} is needed in the scene, so '{1}' was created with DontDestroyOnLoad.", typeof(T), singleton));
						}
						else
						{
							Debug.Log(string.Format("[Singleton] Using instance already created: {0}", _instance.gameObject.name));
						}
					}

					return _instance;
				}
			}
		}

		private static bool applicationIsQuitting = false;
		/// <summary>
		/// When Unity quits, it destroys objects in a random order.
		/// In principle, a Singleton is only destroyed when application quits.
		/// If any script calls Instance after it have been destroyed, 
		///   it will create a buggy ghost object that will stay on the Editor scene
		///   even after stopping playing the Application. Really bad!
		/// So, this was made to be sure we're not creating that buggy ghost object.
		/// </summary>
		protected virtual void OnDestroy()
		{
			applicationIsQuitting = true;
		}
	}

	public class Singleton : Singleton<Singleton>
	{
	}
}

