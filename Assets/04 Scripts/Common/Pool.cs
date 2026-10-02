using System;
using UnityEngine;
using UnityEngine.Pool;

public class Pool<TPoolObject> where TPoolObject : MonoBehaviour
{
    private readonly IObjectPool<TPoolObject> _pool;
    private readonly Func<TPoolObject> _instantiateObjectFunc;
    private readonly Action<TPoolObject> _destroyObjectFunc;
    private int _count = 0;
    private int _maxSize;

    public Pool(Func<TPoolObject> instantiateObjectFunc, Action<TPoolObject> destroyObjectFunc, int maxSize)
    {
        _instantiateObjectFunc = instantiateObjectFunc;
        _destroyObjectFunc = destroyObjectFunc;
        _pool = new ObjectPool<TPoolObject>(
            createFunc: CreatePoolObject,
            actionOnGet: OnGet,
            actionOnRelease: OnRelease,
            actionOnDestroy: OnDestroyParticle,
            maxSize: maxSize
        );
        _maxSize = maxSize;
    }

    public TPoolObject Spawn(Vector3 position, float rotation)
    {
        if (_count > _maxSize) return null;
        
        TPoolObject po = _pool.Get();
        po.transform.position = position;
        po.transform.eulerAngles = new Vector3(0f, 0f, rotation);
        _count++;
        
        return po;
    }

    public void Release(TPoolObject po)
    {
        _pool.Release(po);
        _count--;
    }

    private TPoolObject CreatePoolObject()
    {
        TPoolObject po = _instantiateObjectFunc.Invoke();
        po.gameObject.SetActive(false);
        return po;
    }

    private void OnGet(TPoolObject po) => po.gameObject.SetActive(true);
    private void OnRelease(TPoolObject po) => po.gameObject.SetActive(false);
    private void OnDestroyParticle(TPoolObject po) { if (po) { _destroyObjectFunc.Invoke(po); } }
}