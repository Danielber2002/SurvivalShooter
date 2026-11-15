using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, NodeDescription(
    name: "Deal Damage",
    story: "[Agent] deals [Damage] damage to [Target]",
    category: "Action/Combat",
    id: "deal-damage-action")]

public class DealDamageAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> Damage;

    protected override Status OnStart()
    {
        if (Target == null || Target.Value == null)
        {
            return Status.Failure;
        }

        float amount = (Damage != null && Damage.Value > 0) ? Damage.Value : 20f;

        if (Target.Value.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(amount);
            return Status.Success;
        }
        return Status.Failure;
    }
}