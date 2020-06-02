using Spine;
using System.Collections;
using UnityEngine;

public class KnightSpine : EnemySpine
{
    public override void GetHit()
    {
        if (CurrentTrackEntry.ToString() != "attack1")
            PlayAnim("Gethit2");
    }

    public override void Dead()
    {
        PlayAnim("die1");
        base.Dead();
    }

    public override void Attack()
    {
        PlayAnim("attack1");
    }

    public override void CompleteHandleEvent(TrackEntry trackEntry)
    {
        switch (trackEntry.ToString())
        {
            case "attack1":
                CreateArrow(0, TargetType.Hero);
                StartCoroutine(WaitAttack());
                break;
            case "Gethit2":
                StartCoroutine(WaitAttack());
                break;
        }
    }

    IEnumerator WaitAttack()
    {
        yield return new WaitForSeconds(1.5f);
        Attack();
    }

}
