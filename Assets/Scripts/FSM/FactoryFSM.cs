
public static class FactoryFSM
{
    public static BaseState IdleSelectedStatePlayer(UnitFSM controller)
    {
        return new IdleSelectedStatePlayer(controller);
    }
    public static BaseState IdleSelectedStateAI(UnitFSM controller)
    {
        return new IdleSelectedStateAI(controller);
    }
    public static BaseState IdleNotSelectedState(UnitFSM controller)
    {
        return new IdleNotSelectedState(controller);
    }
    public static BaseState MovingState(UnitFSM controller)
    {
        return new MovingState(controller);
    }    
    public static BaseState RotationSubState(UnitFSM controller)
    {
        return new RotationSubState(controller);
    }
    public static BaseState AttackState(UnitFSM controller)
    {
        return new AttackState(controller);
    }
    public static BaseState WalkState(UnitFSM controller)
    {
        //return new StateWalk(controller);
        return null;
    }
    public static BaseState RunState(UnitFSM controller)
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

