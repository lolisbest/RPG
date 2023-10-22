using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Common
{
    public abstract class Singleton<T> : Singleton where T : MonoBehaviour
    {
        private static T _instance;
        private static readonly object Lock = new object();
        protected static int s_number = 0;
        ///// <summary>
        ///// 씬이 없어질 때 파괴할지
        ///// </summary>
        //[SerializeField]
        //private bool _persistent = true;

        public static T Instance
        {
            get
            {
                if (Quitting)
                {
                    // 게임이 종료되었지만 접근한 경우
                    Debug.LogWarning($"[Singleton<{typeof(T)}>] Instance will not be returned because the application is quitting.");
                    return null;
                }

                // 임계 영역. 여러 스레드에서 동시에 접근하지 못 하도록. thread safe
                // 서브 스레드 생성하고 접근하게 되는 경우에만 효과가 있을듯. 유니티는 기본적으로 메인 스레드에서 스크립트를 순차적으로 실행함
                lock (Lock)
                {
                    // 인스턴스를 가지고 있다면
                    if (_instance != null)
                        return _instance;

                    // 인스턴스를 가지고 있지 않다면
                    var instances = FindObjectsOfType<T>();
                    var count = instances.Length;
                    // 씬에 인스턴스가 존재하면
                    if (count > 0)
                    {
                        // 인스턴스가 1개인 경우
                        if (count == 1)
                        {
                            return _instance = instances[0];
                        }
                        // 인스턴스가 여러 개인 경우
                        Debug.LogWarning($"[Singleton<{typeof(T)}>] There should never be more than one Singleton of type {typeof(T)} in the scene, but {count} were found. The first instance found will be used, and all others will be destroyed.");
                        // 첫번째를 제외하고 나머지 전부 파괴
                        for (var i = 1; i < instances.Length; i++)
                            Destroy(instances[i]);
                        // 첫번째 인스턴스를 저장
                        return _instance = instances[0];
                    }

                    // 인스턴스를 찾을 수 없는 경우
                    Debug.Log($"[Singleton<{typeof(T)}>] An instance is needed in the scene and no existing instances were found, so a new instance will be created.");
                    // 새로운 게임 오브젝트를 생성하여 스크립트를 추가하고 저장 및 반환

                    return _instance = new GameObject($"(Singleton){typeof(T)}")
                               .AddComponent<T>();
                }
            }
        }

        protected virtual void Awake()
        {
            name = $"{name}[{s_number}]";
            s_number++;
            Debug.Log($"Singleton<{GetType()}>.Awake() s_number:{s_number}");

            if (s_number > 1)
            {
                Debug.Log($"Already Exists {GetType()}");
                Destroy(this.gameObject);
                return;
            }
        }


        // 앱이 종료될때 호출
        protected override void OnApplicationQuit()
        {
            Debug.Log($"{GetType()}.OnApplicationQuit s_number : {s_number}");
            base.OnApplicationQuit();
            s_number = 0;
        }

        // 객체가 파괴될때 호출
        protected new virtual void OnDestroy()
        {
            Debug.Log($"{GetType()}.OnDestroy before s_number : {s_number}");
            s_number--; 
            Debug.Log($"{GetType()}.OnDestroy after s_number : {s_number}");

            base.OnDestroy();
        }

        public abstract void Initialize();
        protected virtual void OnAwake() { }
    }

    // Singleton 을 Singleton<T> 안에 다 집어 넣으면 Quitting이 Singleton<T>를 상속한 모든 자식 클래스들이 개별적으로 가지게 됨
    public class Singleton : MonoBehaviour
    {
        public static bool Quitting { get; protected set; }
   
        // 앱이 종료될때 호출
        protected virtual void OnApplicationQuit()
        {
            Debug.Log($"{name} OnApplicationQuit");
            Quitting = true;
        }

        // 객체가 파괴될때 호출
        protected virtual void OnDestroy()
        {
            Debug.Log($"{name} OnDestroy");
            Quitting = true;
        }

        /// <summary>
        /// Singleton.Quitting를 false로 초기화
        /// </summary>
        protected static void InitQuitting()
        {
            Debug.Log("Singleton.InitQuitting()");
            Quitting = false;
        }
    }
}