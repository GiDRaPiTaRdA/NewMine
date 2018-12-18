/*Original Code: http://wiki.unity3d.com/index.php?title=MouseOrbitZoom*/
/*Modified by Penny de Byl on 8 Aug 2017. 
  to Zoom with scroll, orbit with ALT and Pan with Q
*/

using UnityEngine;
using System.Collections;
 
public class CameraController : MonoBehaviour
{
    public Transform target;
    public Vector3 targetOffset;
    public float distance = 5.0f;
    public float maxDistance = 100;
    public float minDistance = .6f;
    public float xSpeed = 200.0f;
    public float ySpeed = 200.0f;
    public int yMinLimit = -80;
    public int yMaxLimit = 80;
    public int zoomRate = 40;
    public float panSpeed = 0.3f;
    public float zoomDampening = 5.0f;
 
    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    private float currentDistance;
    private float desiredDistance;
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    private Quaternion rotation;
    private Vector3 position;
 
    void Start() {
        this.Init(); }
 
    public void Init()
    {
        GameObject go = new GameObject("Fake Cam Target");
        go.transform.position = this.transform.position + (this.transform.forward * this.distance);
        this.target = go.transform;

        this.distance = Vector3.Distance(this.transform.position, this.target.position);
        this.currentDistance = this.distance;
        this.desiredDistance = this.distance;
 
        //be sure to grab the current rotations as starting points.
        this.position = this.transform.position;
        this.rotation = this.transform.rotation;
        this.currentRotation = this.transform.rotation;
        this.desiredRotation = this.transform.rotation;

        this.xDeg = Vector3.Angle(Vector3.right, this.transform.right );
        this.yDeg = Vector3.Angle(Vector3.up, this.transform.up );
    }
 
    /*
     * Camera logic on LateUpdate to only update after all character movement logic has been handled. 
     */
    void LateUpdate()
    {
        // If Control and Alt and Middle button? ZOOM!
        if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.LeftControl))
        {
            this.desiredDistance -= Input.GetAxis("Mouse Y") * Time.deltaTime * this.zoomRate*0.125f * Mathf.Abs(this.desiredDistance);
        }
        // If middle mouse and left alt are selected? ORBIT
        else if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftAlt))
        {
            this.xDeg += Input.GetAxis("Mouse X") * this.xSpeed * 0.02f;
            this.yDeg -= Input.GetAxis("Mouse Y") * this.ySpeed * 0.02f;
 
            ////////OrbitAngle
 
            //Clamp the vertical axis for the orbit
            this.yDeg = ClampAngle(this.yDeg, this.yMinLimit, this.yMaxLimit);
            // set camera rotation 
            this.desiredRotation = Quaternion.Euler(this.yDeg, this.xDeg, 0);
            this.currentRotation = this.transform.rotation;

            this.rotation = Quaternion.Lerp(this.currentRotation, this.desiredRotation, Time.deltaTime * this.zoomDampening);
            this.transform.rotation = this.rotation;
        }
        // left mouse button and Q key, we pan by way of transforming the target in screenspace
        else if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.Q))
        {
            //grab the rotation of the camera so we can move in a psuedo local XY space
            this.target.rotation = this.transform.rotation;
            this.target.Translate(Vector3.right * -Input.GetAxis("Mouse X") * this.panSpeed);
            this.target.Translate(this.transform.up * -Input.GetAxis("Mouse Y") * this.panSpeed, Space.World);
        }
 
        ////////Orbit Position
 
        // affect the desired Zoom distance if we roll the scrollwheel
        this.desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * this.zoomRate * Mathf.Abs(this.desiredDistance);
        //clamp the zoom min/max
        this.desiredDistance = Mathf.Clamp(this.desiredDistance, this.minDistance, this.maxDistance);
        // For smoothing of the zoom, lerp distance
        this.currentDistance = Mathf.Lerp(this.currentDistance, this.desiredDistance, Time.deltaTime * this.zoomDampening);
 
        // calculate position based on the new currentDistance 
        this.position = this.target.position - (this.rotation * Vector3.forward * this.currentDistance + this.targetOffset);
        this.transform.position = this.position;
    }
 
    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}