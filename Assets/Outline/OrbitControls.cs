using UnityEngine;

namespace Outline
{
    public class OrbitControls : MonoBehaviour
    {
        [SerializeField] [Range(1, 10)] private float rotateSensitivity = 10f;
        [SerializeField] [Range(0, 1)] private float panSensitivity = 1f;
        [SerializeField] [Range(0, 1)] private float zoomSensitivity = 1f;
        [SerializeField] private bool invertPan = true;
        [SerializeField] private bool invertZoom = true;

        private bool orbiting;
        private bool panning;

        private void Update()
        {
            if (panning)
                Pan();
            if (orbiting)
                Orbit();
            Zoom();

            if (Input.GetKeyDown(KeyCode.Mouse1))
                orbiting = true;
            else if (Input.GetKeyUp(KeyCode.Mouse1)) orbiting = false;

            if (Input.GetKeyDown(KeyCode.Mouse2))
                panning = true;
            else if (Input.GetKeyUp(KeyCode.Mouse2)) panning = false;

            if (panning || orbiting)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }

        private void Pan()
        {
            var newPositionX = Input.GetAxis("Mouse X");
            newPositionX *= invertPan ? -1 : 1;
            var newPositionY = Input.GetAxis("Mouse Y");
            transform.Translate(new Vector3(newPositionX, 0, 0) * panSensitivity);
            transform.Translate(new Vector3(0, newPositionY, 0) * panSensitivity, Space.World);
        }

        private void Orbit()
        {
            var newRotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * rotateSensitivity;
            var newRotationY = transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * rotateSensitivity;
            transform.localEulerAngles = new Vector3(newRotationY, newRotationX, 0f);
        }

        private void Zoom()
        {
            var zoomAxis = Input.GetAxis("Mouse ScrollWheel");
            zoomAxis *= invertZoom ? -1 : 1;
            if (zoomAxis != 0) Camera.main.orthographicSize += zoomSensitivity * zoomAxis;
        }
    }
}