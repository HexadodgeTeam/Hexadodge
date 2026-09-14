using UnityEngine;
using UnityEngine.Pool;

public class Pool<TPoolObject> where TPoolObject : MonoBehaviour
{
    private IObjectPool<TPoolObject> _pool;
    private TPoolObject _poolObjectPrefab;
    private Transform _parent;

    public Pool(TPoolObject poolObjectPrefab, int maxSize)
    {
        _pool = new ObjectPool<TPoolObject>(
            createFunc: CreatePoolObject,
            actionOnGet: OnGet,
            actionOnRelease: OnRelease,
            actionOnDestroy: OnDestroyParticle,
            maxSize: maxSize
        );
    }

    public void Spawn(Vector2 position, float rotation)
    {
        TPoolObject po = _pool.Get();
        po.transform.position = position;
        po.transform.eulerAngles = new Vector3(0f, 0f, rotation);
    }

    private TPoolObject CreatePoolObject()
    {
        TPoolObject po = GameObject.Instantiate(_poolObjectPrefab, _parent);
        po.gameObject.SetActive(false);
        return po;
    }

    private void OnGet(TPoolObject po) => po.gameObject.SetActive(true);
    private void OnRelease(TPoolObject po) => po.gameObject.SetActive(false);
    private void OnDestroyParticle(TPoolObject po) { if (po) { GameObject.Destroy(po.gameObject); } }
}