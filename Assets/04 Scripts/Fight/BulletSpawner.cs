using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private MoveBox moveBox;

    private Pool<Bullet> _pool;

    private void Awake()
    {
        _pool = new Pool<Bullet>(bulletPrefab, 10);
    }
}
