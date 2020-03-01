using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHelper : MonoBehaviour
{
    [SerializeField] Card card;

    public void FinishedPlayerAttack()
    {
        card.FinishedPlayerAttack();
    }

    public void FinishedMonsterAttack()
    {
        card.FinishedMonsterAttack();
    }
}
