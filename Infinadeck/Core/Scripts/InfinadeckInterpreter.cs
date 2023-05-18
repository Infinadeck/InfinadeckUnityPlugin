using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Infinadeck;

public class InfinadeckInterpreter : MonoBehaviour
{
    public float SlowLoopWaitTime = 0.5f;
    public bool Connected { get; private set; }
    public SpeedVector2 InfIntGetFloorSpeeds { get; private set; }
    public double InfIntGetFloorSpeedMagnitude { get; private set; }
    public double InfIntGetFloorSpeedAngle { get; private set; }
    public bool InfIntGetTreadmillRunState { get; private set; }
    public Ring InfIntGetRingValues { get; private set; }
    public bool InfIntGetTreadmillPauseState { get; private set; }
    public bool InfIntGetVirtualRingEnabled { get; private set; }
    public QuaternionVector4 InfIntGetReferenceDeviceAngleDifference { get; private set; }

    public string InfIntGetTreadmillInfoID { get; private set; }
    public string InfIntGetTreadmillInfoModel_Number { get; private set; }
    public string InfIntGetTreadmillInfoDLL_Version { get; private set; }

    private bool running;
    

    private int loopdelay = 0;

    private Coroutine slowCo = null;

    public string errorInfo;

    private void Awake()
    {
        this.enabled = false;
    }

    void OnEnable()
    {
        slowCo = StartCoroutine(SlowLoop());
    }

    void OnDisable()
    {
        if (running)
        {
            StopCoroutine(slowCo);
            running = false;
        }
    }

    private void OnApplicationQuit()
    {
        if (Connected) { Infinadeck.Infinadeck.StopTreadmill(); }
    }

    // Update is called once per frame
    void Update()
    {
        if (Connected)
        {
            FastLoop();
            StandardLoop();
        }
    }

    private IEnumerator SlowLoop() //Expensive to run
    {
        running = true;
        while (true)
        {
            if (Infinadeck.Infinadeck.CheckRuntimeOpen())
            {
                if (!Infinadeck.Infinadeck.CheckConnection())
                {
                    InfinadeckInitError e = InfinadeckInitError.InfinadeckInitError_None;
                    Infinadeck.Infinadeck.InitConnection(ref e);
                    Connected = Infinadeck.Infinadeck.CheckConnection();
                    errorInfo = "";
                    if (!Connected) ///If connection attempt failed, report why
                    {
                        if (e == InfinadeckInitError.InfinadeckInitError_None) { errorInfo = "ERROR: INIT CONNECTION FAILED WITH NO ERROR"; }
                        else if (e == InfinadeckInitError.InfinadeckInitError_Unknown) { errorInfo = "ERROR: UNKNOWN ERROR"; }
                        else if (e == InfinadeckInitError.InfinadeckInitError_NoServer) { errorInfo = "ERROR: NO SERVER FOUND"; }
                        else if (e == InfinadeckInitError.InfinadeckInitError_UpdateRequired) { errorInfo = "ERROR: GAME API UPDATE REQUIRED"; }
                        else if (e == InfinadeckInitError.InfinadeckInitError_InterfaceVerificationFailed) { errorInfo = "ERROR: FAILED TO VERIFY INTERFACE"; }
                        else if (e == InfinadeckInitError.InfinadeckInitError_ControllerVerificationFailed) { errorInfo = "ERROR: FAILED TO VERIFY CONTROLLER"; }
                        else if (e == InfinadeckInitError.InfinadeckInitError_FailedInitialization) { errorInfo = "ERROR: FAILED TO INITIALIZE"; }
                        else if (e == InfinadeckInitError.InfinadeckInitError_FailedHostResolution) { errorInfo = "ERROR: FAILED TO RESOLVE HOST"; }
                        else if (e == InfinadeckInitError.InfinadeckInitError_FailedServerConnection) { errorInfo = "ERROR: FAILED TO CONNECT TO SERVER"; }
                        else if (e == InfinadeckInitError.InfinadeckInitError_FailedServerSend) { errorInfo = "ERROR: FAILED TO SEND PACKET TO SERVER"; }
                        else if (e == InfinadeckInitError.InfinadeckInitError_RuntimeOutOfDate) { errorInfo = "ERROR: RUNTIME OUT OF DATE"; }
                        else { errorInfo = "ERROR: UNDOCUMENTED ERROR"; }
                    }
                }
                else { Connected = true; }
            }
            else { Connected = false; }

            yield return new WaitForSeconds(SlowLoopWaitTime);
        }
    }

    private void StandardLoop() //Low frequency information, delay allowed
    {
        switch (loopdelay)
        {
            case 0: InfIntGetFloorSpeedMagnitude = Infinadeck.Infinadeck.GetFloorSpeedMagnitude(); break;
            case 1: InfIntGetFloorSpeedAngle = Infinadeck.Infinadeck.GetFloorSpeedAngle(); break;
            case 2: InfIntGetRingValues = Infinadeck.Infinadeck.GetRingValues(); break;
            case 3: InfIntGetTreadmillPauseState = Infinadeck.Infinadeck.GetTreadmillPauseState(); break;
            case 4: InfIntGetVirtualRingEnabled = Infinadeck.Infinadeck.GetVirtualRingEnabled(); break;
            case 5: InfIntGetReferenceDeviceAngleDifference = Infinadeck.Infinadeck.GetReferenceDeviceAngleDifference(); break;
            case 6: InfIntGetTreadmillInfoID = Infinadeck.Infinadeck.GetTreadmillInfo().id; break;
            case 7: InfIntGetTreadmillInfoModel_Number = Infinadeck.Infinadeck.GetTreadmillInfo().model_number; break;
            case 8: InfIntGetTreadmillInfoDLL_Version = Infinadeck.Infinadeck.GetTreadmillInfo().dll_version; break;
            default: loopdelay = -1; break;
        }
        loopdelay++;
    }

    private void FastLoop() //High frequency information, cheap to run, no delay allowed
    {
        InfIntGetFloorSpeeds = Infinadeck.Infinadeck.GetFloorSpeeds();
        InfIntGetTreadmillRunState = Infinadeck.Infinadeck.GetTreadmillRunState();
    }

    public void InfIntSetManualSpeeds(double x, double y)
    {
        if (Connected) { Infinadeck.Infinadeck.SetManualSpeeds(x, y); }
    }
    public void InfIntRequestTreadmillRunState(bool run)
    {
        if (Connected) { Infinadeck.Infinadeck.RequestTreadmillRunState(run); }
    }
    public void InfIntSetTreadmillPause(bool pause)
    {
        if (Connected) { Infinadeck.Infinadeck.SetTreadmillPause(pause); }
    }
    public void InfIntSetVirtualRing(bool pause)
    {
        if (Connected) { Infinadeck.Infinadeck.SetTreadmillPause(pause); }
    }
    public void InfIntStopTreadmill()
    {
        if (Connected) { Infinadeck.Infinadeck.StopTreadmill(); }
    }
    public void InfIntStartTreadmillManualControl()
    {
        if (Connected) { Infinadeck.Infinadeck.StartTreadmillManualControl(); }
    }
    public void InfIntStartTreadmillUserControl()
    {
        if (Connected) { Infinadeck.Infinadeck.StartTreadmillUserControl(); }
    }
}
