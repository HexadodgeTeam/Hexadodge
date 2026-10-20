using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class Pool<TPoolObject> where TPoolObject : MonoBehaviour
{
    private readonly IObjectPool<TPoolObject> _pool;
    private readonly Func<TPoolObject> _instantiateObjectFunc;
    private readonly Action<TPoolObject> _destroyObjectFunc;
    private readonly int _maxSize;
    private readonly List<TPoolObject> _poolObjects = new();
    
    public int Count { get; private set; }

    public Pool(Func<TPoolObject> instantiateObjectFunc, Action<TPoolObject> destroyObjectFunc, int maxSize)
    {
        _instantiateObjectFunc = instantiateObjectFunc;
        _destroyObjectFunc = destroyObjectFunc;
        _pool = new ObjectPool<TPoolObject>(
            createFunc: CreatePoolObject,
            actionOnGet: OnGet,
            actionOnRelease: OnRelease,
            actionOnDestroy: OnDestroy,
            maxSize: maxSize,
            defaultCapacity: maxSize
        );
        _maxSize = maxSize;
    }

    public TPoolObject Spawn(Vector3 position, float rotation)
    {
        if (Count + 1 > _maxSize) return null;
        
        TPoolObject po = _pool.Get();
        po.transform.position = position;
        po.transform.eulerAngles = new Vector3(0f, 0f, rotation);
        Count++;
        _poolObjects.Add(po);
        
        return po;
    }

    public void Release(TPoolObject po)
    {
        _pool.Release(po);
        _poolObjects.Remove(po);
        Count--;
    }

    private TPoolObject CreatePoolObject()
    {
        TPoolObject po = _instantiateObjectFunc.Invoke();
        po.gameObject.SetActive(false);
        return po;
    }

    public void Clear()
    {
        foreach (TPoolObject poolObject in _poolObjects.ToList())
        {
            Release(poolObject);
        }
        _poolObjects.Clear();
        Count = 0;
    }

    private void OnGet(TPoolObject po) => po.gameObject.SetActive(true);
    private void OnRelease(TPoolObject po) => po.gameObject.SetActive(false);

    private void OnDestroy(TPoolObject po) { if (po) { _destroyObjectFunc.Invoke(po); } }
}