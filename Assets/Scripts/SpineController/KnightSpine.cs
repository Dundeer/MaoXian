using Spine;
using System.Collections;
using UnityEngine;

public class KnightSpine : EnemySpine
{
    public override void GetHit()
    {
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
        DelayCreate(0.5f, 0);
    }

    public override void StartHandleEvent(TrackEntry trackEntry)
    {
        base.StartHandleEvent(trackEntry);
        switch (trackEntry.ToString())
        {
            case "Gethit2":
                StopCreate();
                break;
        }
    }

    public override void CompleteHandleEvent(TrackEntry trackEntry)
    {
        switch (trackEntry.ToString())
        {
            case "attack1":
                StartCoroutine(WaitAttack());
                break;
            case "Gethit2":
                Attack();
                break;
        }
    }

    IEnumerator WaitAttack()
    {
        yield return new WaitForSeconds(1.5f);
        Attack();
    }

}
