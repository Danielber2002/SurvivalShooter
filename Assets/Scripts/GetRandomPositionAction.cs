using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;

[Serializable, NodeDescription(
    name: "Get Random Position",
    story: "[Agent] gets random point within [Radius] into [Result]",
    category: "Action/Navigation",
    id: "get-random-position-action")]

public class GetRandomPositionAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<float> Radius;
    [SerializeReference] public BlackboardVariable<Vector3> Result;

    protected override Status OnStart()
    {
        GameObject selfObject = (Agent != null && Agent.Value != null) ? Agent.Value : GameObject;
        if (selfObject == null)
        {
            return Status.Failure;
        }

        NavMeshAgent navAgent = selfObject.GetComponent<NavMeshAgent>();
        if (navAgent != null && !navAgent.isOnNavMesh)
        {
            return Status.Failure;
        }

        float radiusValue = (Radius != null && Radius.Value > 0) ? Radius.Value : 10f;
        Vector3 origin = selfObject.transform.position;
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radiusValue + origin;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, radiusValue * 3f, NavMesh.AllAreas))
        {
            if (Result != null)
            {
                Result.Value = hit.position;
            }
            return Status.Success;
        }

        if (Result != null)
        {
            Result.Value = origin;
        }
        return Status.Success;
    }
}