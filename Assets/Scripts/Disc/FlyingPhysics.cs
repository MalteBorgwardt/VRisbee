using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
This is the simulation of disc throws.
We are relying on rigidbody and applying forces in correct directions.
*/
public class FlyingPhysics : MonoBehaviour
{
    private const double RHO = 1.23; //The density of air in kg/m^3.
    //private const float smooth_delta_loss = 1.001f;
    //private const float smooth_delta_turn_loss = 1.004f;

    // Used for calculation
    private float alpha = 0;
    //private float current_speed;
    private double roll;
    private double pitch;
    private double spin;

    //[Header("Visuals")]
    //// for UI
    ////public Text distance; // distance text
    ////public Text Throws; // throw count text
    //public float throw_arm_length = 5;
    //private float throw_acceleration = 1.05f; // 5% acceleraton

    [Header("States")]
    // States
    public bool isThrown = false; // Is the disc in the air

    public int number_throws = 0;

    private Disc Disc; // The Frisbee
    //public basket_script basket; // an obligatory basket, used to look at!
    private Rigidbody rigidBody;
    public bool throw_animation = false;
    //public float throw_delta_acceleration = 0;
    //public Vector3 discNextPosition; // Next throw position
    //public Vector3 current_throw_pos; // current throw pos
    public Vector3 discInitialPosition; // Disc initial position
    public Quaternion discInitialRotation; // Disc initial rotation
    ////public follow_camera cam; // camera
    //public List<Vector3> throwpositions = new List<Vector3>(); // all throw positions, history list
    
    public TMP_Text textVelocity;
    public TMP_Text textAngularVel;
    public TMP_Text textSpin;

    public void UpdateDiscComponent()
    {
        Disc = GetComponentInChildren<Disc>();
    }

    void Start()
    {
        //    cam = FindObjectOfType<follow_camera>();
        UpdateDiscComponent();
        //    basket = FindObjectOfType<basket_script>();
        rigidBody = GetComponent<Rigidbody>(); // Get Disc Rigidbody

        discInitialPosition = transform.position; // Get disc localposition related to Player
        discInitialRotation = transform.rotation; // Get disc localrotation

        // Inertia
        rigidBody.inertiaTensor = new Vector3(0.00122f, 0.00122f, 0.00235f);
    }

    private void Lift()
    {
        double cl = Disc.CL0 + Disc.CLA * alpha * Math.PI / 180; // lift constant

        Vector3 localAngularVelocity = transform.InverseTransformDirection(rigidBody.angularVelocity);
        double spinRatio = Math.Pow(localAngularVelocity.y, 2) / Math.Pow(localAngularVelocity.magnitude, 2);

        double lift = 0f;
        if (Math.Abs(localAngularVelocity.y) > 10 && spinRatio > 0.8)
        {
            lift = RHO * Math.Pow(rigidBody.velocity.magnitude * 2 * spinRatio, 2) * Disc.AREA * cl / 2;
        }

        rigidBody.AddForce((float)lift * 10 * Time.deltaTime * transform.up.normalized, ForceMode.Acceleration);

        if (lift > 0)
        {
            rigidBody.AddForce(1 / (float)rigidBody.velocity.magnitude * 10 * Time.deltaTime * transform.rotation.z * transform.rotation.eulerAngles.normalized, ForceMode.Acceleration);
        }
    }

    private void Gravity()
    {
        rigidBody.AddForce(0, -9.82f * 10 * Time.deltaTime, 0, ForceMode.Acceleration);
    }

    private void Drag()
    {
        double cd = Disc.CD0 + Disc.CDA * Mathf.Pow((float)((alpha - Disc.ALPHA0) * Math.PI / 180), 2);

        // Malte: to only take realistic spin into account
        double flatVelocity = Vector3.Cross(transform.up, rigidBody.velocity).normalized.magnitude;

        Vector3 localAngularVelocity = transform.InverseTransformDirection(rigidBody.angularVelocity);
        double spinRatio =  Math.Abs(localAngularVelocity.y) / localAngularVelocity.magnitude;

        double drag = rigidBody.velocity.magnitude; // this default value prevents same behaviour when throwing like a ball
        if (Math.Abs(localAngularVelocity.y) > 10 && spinRatio > 0.8)
        {
             drag = (RHO * Math.Pow(rigidBody.velocity.magnitude * 2 * (1 - flatVelocity) * (1 - spinRatio), 2) * Disc.AREA * cd) / 2;
        }
        rigidBody.AddForce((float)drag * 10 * Time.deltaTime * -rigidBody.velocity.normalized, ForceMode.Acceleration);
    }

    private void Torque()
    {
        roll = (Disc.CRR * rigidBody.angularVelocity.y + Disc.CRP * rigidBody.angularVelocity.x) * 1 / 2 * RHO * Math.Pow(rigidBody.velocity.magnitude, 2) * Disc.AREA * Disc.diameter * 0.01f * 6;

        spin = -(Disc.CNR * rigidBody.angularVelocity.y) * 1 / 2 * RHO * Math.Pow(rigidBody.velocity.magnitude, 2) * Disc.AREA * Disc.diameter * 0.01f;
        pitch = (Disc.CM0 + Disc.CMA * (Math.PI / 180 * (alpha)) + Disc.CMq * rigidBody.angularVelocity.z) * 1 / 2 * RHO * Math.Pow(rigidBody.velocity.magnitude, 2) * Disc.AREA * Disc.diameter * 0.01f * 6;

        rigidBody.AddTorque((float)roll * 10 * Time.deltaTime * Vector3.Cross(transform.up, rigidBody.velocity).normalized, ForceMode.Acceleration);

        //Debug.Log(new Vector3((float)roll,(float)spin,(float)pitch));

        rigidBody.AddTorque((float)spin * 10 * Time.deltaTime * transform.up, ForceMode.Acceleration);
        rigidBody.AddTorque((float)pitch * 4 * Time.deltaTime * rigidBody.velocity.normalized, ForceMode.Acceleration);
    }

    void FixedUpdate()
    {
        if (isThrown)
        { 
            alpha = Vector3.Angle(rigidBody.velocity, transform.forward);

            Lift();
            Drag();
            Torque();
            CheckLanding();
            CheckOutOfBounds();
        }
        Gravity();


        textVelocity.text = Math.Round(rigidBody.velocity.magnitude, 1).ToString();

        // Vector3 localAngularVelocity = transform.InverseTransformDirection(rigidBody.angularVelocity);
        textSpin.text = Math.Round(transform.rotation.x, 1).ToString();
        textAngularVel.text = Math.Round(transform.rotation.z, 1).ToString();
    }

    public void InitThrow()
    {
        rigidBody.maxAngularVelocity = 2000;

        rigidBody.drag = 0;
        rigidBody.mass = Disc.m;
        rigidBody.useGravity = false;
        rigidBody.isKinematic = false;

        isThrown = true;

        number_throws++;
    }

    public void ResetIsThrown()
    {
        isThrown = false;

        rigidBody.useGravity = false;
    }


    private void CheckOutOfBounds()
    {
        if (transform.position.y < -1000)
        {
          //  Reset();
            Debug.Log("Out of bounds. Reset disc position.");
        }
    }

    private void CheckLanding()
    {
        if (rigidBody.velocity.x <= 0 && rigidBody.velocity.y <= 0 && rigidBody.velocity.z <= 0)
        {
            //distance.text = Math.Round(Vector3.Distance(discInitialPosition, transform.position)).ToString();
            ResetIsThrown();
        }
    }

    public void Reset()
    {
        transform.SetPositionAndRotation(discInitialPosition, discInitialRotation);
    }
}
