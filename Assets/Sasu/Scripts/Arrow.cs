using System.Runtime.InteropServices;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Assets.Sasu.Scripts
{
    public class Arrow : MonoBehaviour
    {
        private Rigidbody _rigidbody;

        public void Awake()
        {
            _rigidbody=GetComponent<Rigidbody>();
        }

        public void Start()
        {
            this.OnCollisionEnterAsObservable().Where(x => x.collider.CompareTag("Ground"))
               .Subscribe(_ => _rigidbody.velocity=Vector3.zero).AddTo(gameObject);
             
        }
        public static Arrow Create(Arrow prefab,Vector3 position,Vector3 shootForce,float elevation)
        {
            var instance = Instantiate(prefab);
            instance.transform.position = position;

            instance.transform.rotation = Quaternion.LookRotation(shootForce.normalized);
            shootForce.y = elevation;

            Debug.Log(shootForce);

            instance.Shoot(shootForce);
            return instance;
        }
        public void Shoot(Vector3 force)
        {
            _rigidbody.AddForce(force,ForceMode.Impulse);
        }
    }
}