using System;
using System.Collections;
using System.Numerics;
using Default.Scripts.Util;
using UniRx;
using UniRx.Triggers;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

namespace Assets.Sasu.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("이동")]
        public float maxSpeed = 20;
       
        public float acceleration = 1;

        [Header("공격")]
        public Aim aim;
        public Arrow arrowPrefab;
        public float minDrawTime = 0.1f;
        public float maxDrawTime = 1f;
        public float minPower = 0.1f;
        public float maxPower = 1f;
        public float elevation;
        public float minRange;
        public float maxRange;

        public Vector3 aimPoint;

        private Rigidbody _rigidbody;
        public float _drawTime;

        public void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void Start()
        {
            CreateMovementStream();
            CreateAimingStream();
            CreateShootStream();
        }

        public void OnDrawGizmos()
        {
            RaycastHit hit;
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);

            Gizmos.DrawWireSphere(hit.point, minRange/2);
            
            Gizmos.DrawWireSphere(hit.point, maxRange/2);
        }
        public void CreateMovementStream()
        {
            GlobalInputBinder.CreateGetAxisStreamOptimize("Horizontal").Subscribe(MoveX).AddTo(gameObject);
            GlobalInputBinder.CreateGetAxisStreamOptimize("Vertical").Subscribe(MoveZ).AddTo(gameObject);
        }

        public void MoveX(float direction)
        {
            if (Mathf.Abs(_rigidbody.velocity.x) < maxSpeed)
            {
                _rigidbody.AddForce(Vector3.right * acceleration * direction, ForceMode.VelocityChange);
            }
        }

        public void MoveZ(float direction)
        {
            if (Mathf.Abs(_rigidbody.velocity.z) < maxSpeed)
            {
                _rigidbody.AddForce(Vector3.forward * acceleration * direction, ForceMode.VelocityChange);
            }
        }

        public void CreateShootStream()
        {
            GlobalInputBinder.CreateGetMouseButtonDownStream(0).Subscribe(_ => DrawAndShoot()).AddTo(gameObject);
        }

        public void DrawAndShoot()
        {
            Observable.Interval(TimeSpan.FromSeconds(0.1f))
                .TakeUntil(GlobalInputBinder.CreateGetMouseButtonUpStream(0))
                .Subscribe(_ => IncreaseDrawTime(0.1f),null,Shoot).AddTo(gameObject);
        }

        public void Shoot()
        {
            var a = aimPoint;
            var b = transform.position;
            a.y = b.y = 0;
            Vector3 direction;
          

            Vector3 force;
            if (_drawTime < minDrawTime)
            {
                direction = Vector3.Normalize(RandomPoint(a,maxRange) - b);
                force = direction * minPower;
            }
            else if (_drawTime >= minDrawTime && _drawTime< maxDrawTime)
            {
                direction = Vector3.Normalize(RandomPoint(a, _drawTime / (maxDrawTime - minDrawTime) / (maxRange - minRange)) -b);
                force = direction * (minPower+(maxPower-minPower)*_drawTime/(maxDrawTime-minDrawTime));
            }
            else
            {
                direction = Vector3.Normalize(RandomPoint(a, minRange) - b);
                force = direction * maxPower;
            }

            var instance = Arrow.Create(arrowPrefab, transform.position, force, elevation);
            _drawTime = 0;
        }

        public Vector3 RandomPoint(Vector3 point,float range)
        {
            var error = Random.Range(-range/2, range/2);
            var perpendicularVector = Vector3.Cross(Vector3.Normalize(point - transform.position), Vector3.up);
            return point + perpendicularVector * error;
        }
        public void IncreaseDrawTime(float time)
        {
            _drawTime += time;
        }
        public void CreateAimingStream()
        {
            GlobalInputBinder.CreateGetMousePositionStream().Subscribe(p => aim.rectTransform.anchoredPosition = p);
            var groundMousePointStream = GlobalInputBinder.CreateGetMousePositionStream()
                .Select(p =>
                {
                    RaycastHit hit;
                    Physics.Raycast(Camera.main.ScreenPointToRay(p), out hit);
                   return hit.point;
                });
            
            groundMousePointStream.Subscribe(point => aimPoint = point);

#if UNITY_EDITOR
            groundMousePointStream.Subscribe(point => Debug.DrawLine(transform.position, point, Color.blue));
#endif
         
        }
    }
}
