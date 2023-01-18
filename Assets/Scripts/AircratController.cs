using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AircratController : MonoBehaviour
{

    public Transform CM;
    public Transform LiftPoint;
    public Transform EnginePoint;

    public Rigidbody main;
    public float AirPressure = 14.7f;
    public float WingAera = 10f;
    public float VerticalStablizerAera = 1f;
    public float FrontAera = 1.5f;
    public float SideAera = 10f;

    public float aileronAera;
    public float elevatorAera;
    public float rudderAera;

    public float AC = 1;
    public float EC = 1;
    public float RC = 1;

    public AnimationCurve Cl;
    public AnimationCurve RudderAOA;
    public AnimationCurve SideDrag;
    public AnimationCurve FrontDrag;

    public float AngleOfAttack;
    public float AngleOfAttackYaw;
    public float maxAngleOfAttack = 4f;
    public float maxAngleOfAttackYaw = 2f;

    public Vector3 Velocity;
    public Vector3 LocalVelocity;
    public Vector3 LocalAngularVelocity;

    public float EnginePower;

    [Header("Input")]
    public float inputPitch;
    public float inputRoll;
    public float inputYaw;


    // Start is called before the first frame update
    void Start()
    {
        main.centerOfMass = CM.localPosition;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        inputPitch = Input.GetAxis("Vertical");
        inputRoll = Input.GetAxis("Horizontal");
        inputYaw = Input.GetAxis("Yaw");

        CalculateState();
        main.AddForceAtPosition(-EnginePoint.forward * EnginePower, EnginePoint.position);
        float lift = Cl.Evaluate(AngleOfAttack) * ((AirPressure * (LocalVelocity.z * LocalVelocity.z)) / 2) * WingAera;
        main.AddForceAtPosition(LiftPoint.up * lift, LiftPoint.position);
        //drag.
        float _frontDrag = FrontDrag.Evaluate(LocalVelocity.z) * ((AirPressure*(LocalVelocity.z * LocalVelocity.z))/2) * FrontAera;
        main.AddForce(transform.forward*-_frontDrag);
        float _sideDrag = SideDrag.Evaluate(LocalVelocity.x) * ((AirPressure * (LocalVelocity.x * LocalVelocity.x)) / 2) * SideAera;
        main.AddForce(transform.right * -_sideDrag);


        //CalculateForce
        float _aileron = AC * ((AirPressure * (LocalVelocity.z * LocalVelocity.z)) / 2) * aileronAera;
        float _elevator = EC * ((AirPressure * (LocalVelocity.z * LocalVelocity.z)) / 2) * elevatorAera;
        float _rudder = RC * ((AirPressure * (LocalVelocity.z * LocalVelocity.z)) / 2) * rudderAera;

        //Torque
        main.AddTorque(transform.up * RudderAOA.Evaluate(AngleOfAttackYaw) * (0.5f * AirPressure * (LocalVelocity.z * LocalVelocity.z)) * VerticalStablizerAera);
        if(Mathf.Sqrt(AngleOfAttackYaw * AngleOfAttackYaw) < maxAngleOfAttackYaw)
            main.AddTorque(transform.up * _rudder * inputYaw);
        main.AddTorque(transform.forward * _aileron * inputRoll);
        if(Mathf.Sqrt(AngleOfAttack * AngleOfAttack) < maxAngleOfAttack)
            main.AddTorque(transform.right * _elevator * inputPitch);
    }
    void CalculateState()
    {
        var invRotation = Quaternion.Inverse(main.rotation);
        Velocity = main.velocity;
        LocalVelocity = invRotation * Velocity;  //transform world velocity into local space
        LocalAngularVelocity = invRotation * main.angularVelocity;  //transform into local space
        CalculateAngleOfAttack();
    }
    void CalculateAngleOfAttack()
    {
        AngleOfAttack = Mathf.Atan2(-LocalVelocity.y, LocalVelocity.z) * 57.2957795f;
        AngleOfAttackYaw = Mathf.Atan2(LocalVelocity.x, LocalVelocity.z) * 57.2957795f;
    }
}
