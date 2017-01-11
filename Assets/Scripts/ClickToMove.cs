using UnityEngine;

public class ClickToMove : MonoBehaviour
{
    public float speed;
    public CharacterController controller;
    private Vector3 _position;

    // Use this for initialization
    private void Start()
    {
        _position = transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            //locate the destination point
            LocatePosition();
        }
        //move the player to destination point
        MoveToPosition();
    }

    private void LocatePosition()
    {
        RaycastHit hit;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 1000))
        {
            _position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            Debug.Log(_position);
        }
    }

    private void MoveToPosition()
    {
        if (Vector3.Distance(transform.position, _position) > 10) //10 is cube size
        {
            var newRotation = Quaternion.LookRotation(_position - transform.position);

            newRotation.x = 0f;
            newRotation.z = 0f;

            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.fixedDeltaTime * 10);
            controller.SimpleMove(transform.forward * speed);
        }
    }
}