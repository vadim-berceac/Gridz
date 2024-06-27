
public static class FactoryFSM
{
    public static BaseState IdleSelected(UnitFSM controller)
    {
        return null;
    }
    public static BaseState IdleNotSelectedState(UnitFSM controller)
    {
        return new IdleNotSelectedState(controller);
    }
    public static BaseState Moving(UnitFSM controller)
    {
        //return new StateMoving(controller);
        return null;
    }    
    public static BaseState Walk(UnitFSM controller)
    {
        //return new StateWalk(controller);
        return null;
    }
    public static BaseState Run(UnitFSM controller)
    {
        //return new StateRun(controller);
        return null;
    }
    public static BaseState JumpStart(UnitFSM controller)
    {
        //return new StateJumpStart(controller);
        return null;
    }
    public static BaseState JumpOnAir(UnitFSM controller)
    {
        //return new StateJumpOnAir(controller);
        return null;
    }
    public static BaseState JumpLand(UnitFSM controller)
    {
        //return new StateJumpLand(controller);
        return null;
    }
    public static BaseState FallGravity(UnitFSM controller)
    {
        //return new StateFallGravity(controller);
        return null;
    }
}

