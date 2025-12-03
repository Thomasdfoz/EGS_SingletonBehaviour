using UnityEngine;

namespace EGS.Utils
{
    public abstract class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static object _lock = new object();
        private static bool _appIsClosing = false; // Mudança de nome pra ficar claro

        public virtual bool IsDontDestroyOnLoad => true;

        public static T Instance
        {
            get
            {
                // Se o JOGO INTEIRO estiver fechando, retorna null para evitar erros
                if (_appIsClosing) return null;

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)FindAnyObjectByType(typeof(T));

                        if (_instance == null)
                        {
                            var singletonObject = new GameObject();
                            _instance = singletonObject.AddComponent<T>();
                            singletonObject.name = typeof(T).ToString() + " (Singleton)";
                        }
                    }

                    return _instance;
                }
            }
        }

        protected virtual void Awake()
        {
            if (_appIsClosing) return;

            if (_instance == null)
            {
                _instance = this as T;
                if (IsDontDestroyOnLoad && transform.parent == null)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        // Essa função é chamada SOMENTE quando fecha a janela do jogo/Unity
        protected virtual void OnApplicationQuit()
        {
            _appIsClosing = true;
        }

        protected virtual void OnDestroy()
        {
            // AQUI ESTAVA O ERRO: Removemos o "_appIsClosing = true" daqui.
            // Apenas limpamos a referência se nós formos a instância morrendo.
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}