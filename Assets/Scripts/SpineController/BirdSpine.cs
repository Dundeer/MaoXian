using Spine;

public enum BirdLevel
{

}

public class BirdSpine : EnemySpine
{
    public override void Create()
    {
        StandBy();
        currentBloodLine.IniteBlood(CurrentBlood);
    }

    public override void Dead()
    {
        base.Dead();
        PlayAnim("Gethit1");
    }

    public override void GetHit()
    {
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
        DelayCreate(0.5f, 0);
    }

    public override void StartHandleEvent(TrackEntry trackEntry)
    {
        base.StartHandleEvent(trackEntry);
        switch (trackEntry.ToString())
        {
            case "Gethit1":
                StopCreate();
                break;
        }
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
