using UnityEngine;

namespace ETModel
{
	public static class ModelGame
	{
		private static EventSystem eventSystem;
		public static EventSystem EventSystem
		{
			get
			{
				return eventSystem ?? (eventSystem = new EventSystem());
			}
		}
		
		private static ModelScene  scene;
		public static ModelScene  Scene
		{
			get
			{
				if (scene != null)
				{
					return scene;
				}
				scene = new ModelScene () { Name = "Client" };
				return scene;
			}
		}

		private static ObjectPool objectPool;
		public static ObjectPool ObjectPool
		{
			get
			{
				if (objectPool != null)
				{
					return objectPool;
				}
				objectPool = new ObjectPool() { Name = "ClientM" };
				return objectPool;
			}
		}

		public static void Close()
		{
            scene?.Dispose();
            scene = null;

            objectPool?.Dispose();
            objectPool = null;

			eventSystem = null;
		}
	}
}