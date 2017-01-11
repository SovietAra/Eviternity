using UnityEngine;

namespace Assets
{
    public class PlayerController : MonoBehaviour
    {
        public Vector2 VerticalBounds;
        public Vector2 HorizontalBounds;

        public int PlayerId;
        public float Speed;

        private Rigidbody _physics;
        private Transform _camera;
        private float _xMin, _xMax, _zMin, _zMax;

        void Start () {
            _physics = GetComponent<Rigidbody>();
            _camera = GameObject.Find("Main Camera").transform;
        }

        void FixedUpdate()
        {
            float moveHorizontal = Input.GetAxis("Horizontal" + PlayerId);
            float moveVertical = Input.GetAxis("Vertical" + PlayerId);
            Vector3 inputVector = new Vector3(moveHorizontal, 0.0f, moveVertical);
            Vector3 offset = inputVector * Speed
                             + transform.position;

            _xMax = _camera.position.x + HorizontalBounds.y;
            _zMax = _camera.position.z + HorizontalBounds.y;
            _xMin = _camera.position.x - HorizontalBounds.x;
            _zMin = _camera.position.z - HorizontalBounds.x;

            float clampedX = Mathf.Clamp(offset.x,_xMin,_xMax);
            float clampedZ = Mathf.Clamp(offset.z,_zMin,_zMax);

            _physics.MovePosition(new Vector3(clampedX,0,clampedZ));
        }


    }

}