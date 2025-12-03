using System;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace EGS.Utils 
{
   public abstract class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static object _lock = new object();
        private static bool _isQuitting = false;

        // Propriedade para configurar se deve persistir entre cenas
        public bool IsDontDestroyOnLoad = true; 

        public static T Instance
        {
            get
            {
                if (_isQuitting)
                {
                    // Debug.LogWarning($"[Singleton] Tentou acessar '{typeof(T)}' após o encerramento do jogo. Retornando null.");
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        // Tenta achar na cena
                        _instance = (T)FindAnyObjectByType(typeof(T));

                        // Se ainda não existe e não estamos saindo...
                        if (_instance == null)
                        {
                            // Cria um novo objeto
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
                // Se já existe outro, destrói este impostor
                Destroy(gameObject); 
            }
        }

        // A Unity chama isso automaticamente quando você aperta Stop ou fecha o jogo.
        protected virtual void OnApplicationQuit()
        {
            _isQuitting = true;
        }

        // Garante que se o objeto for destruído manualmente, a referência estática limpa
        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _isQuitting = true; // Previne recriação se destruído manualmente na troca de cena
            }
        }
    }
}
