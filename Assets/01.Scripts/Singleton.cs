using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // 싱글톤 인스턴스를 저장할 정적 변수
    private static T _instance;
    
    // 인스턴스에 접근하기 위한 프로퍼티
    public static T Instance
    {
        get
        {
            // 인스턴스가 없는 경우 검색
            if (_instance == null)
            {
                // 씬에서 오브젝트 검색
                _instance = FindObjectOfType<T>();
                
                // 씬에 오브젝트가 없는 경우
                if (_instance == null)
                {
                    // 새 게임오브젝트 생성
                    GameObject singletonObject = new GameObject($"{typeof(T).Name} (Singleton)");
                    _instance = singletonObject.AddComponent<T>();
                }
            }
            
            return _instance;
        }
    }
    
    // 중복 생성 방지 및 씬 전환 시 파괴 방지
    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            // 이미 인스턴스가 존재하는 경우 현재 오브젝트 파괴
            Destroy(gameObject);
        }
    }
}