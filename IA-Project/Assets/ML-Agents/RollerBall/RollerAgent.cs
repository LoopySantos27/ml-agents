using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RollerAgent : Agent
{

    Rigidbody  rBody;
    public Transform tTarget;
    void Start()
    {
        //Agarramos el rigidbody con el que se va a mover el agente
        rBody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        //Si el agente se cae se regresa a donde empezo
        if(this.transform.localPosition.y < 0)
        {
            //Resetear sus valores para empezar de nuevo
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0, 0.5f, 0);
        }
            
            //Mover el target a un lugar random
            tTarget.localPosition = new Vector3(Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);
    }

    //Guardar y mandar los datos al cerebro
    public override void CollectObservations(VectorSensor sensor)
    {
        //Posiciones del target y del agente
        sensor.AddObservation(tTarget.localPosition);
        sensor.AddObservation(this.transform.localPosition);

        //Velocidad del agente
        sensor.AddObservation(rBody.velocity.z);
        sensor.AddObservation(rBody.velocity.x);
    }

    //Recibir las acciones y asignar las recompensas
    public float forceMultiplier = 10;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        //Valores que necesitara para moverse
        Vector3  controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        rBody.AddForce(controlSignal * forceMultiplier);

        //Rewards
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, tTarget.localPosition);

        //Alcanzo el objetivo
        if(distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        //Si se cae de la plataforma
        else if (this.transform.localPosition.y <0)
        {
            EndEpisode();
        }

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}
