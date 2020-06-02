using Spine;

public class BirdSpine : EnemySpine
{
    public override void Dead()
    {
        base.Dead();
        PlayAnim("Gethit1");
    }

    public override void GetHit()
    {
        if (CurrentTrackEntry.ToString() != "Gethit1")
            PlayAnim("Gethit1");
    }

    public override void StandBy()
    {
        StandBy(false);
    }

    private void StandBy(bool loop)
    {
        PlayAnim("Standby", loop, 0);
    }

    public override void Attack()
    {
        PlayAnim("attack");
    }

    public override void CompleteHandleEvent(TrackEntry trackEntry)
    {
        switch (trackEntry.ToString())
        {
            case "attack":
                CreateArrow(0, TargetType.Hero);
                StandBy(false);
                break;
            case "Gethit1":
                WaitAttack();
                break;
            case "Standby":
                WaitAttack();
                break;
        }
    }

    private void WaitAttack()
    {
        Attack();
    }

}
