using RedBjorn.ProtoTiles.Example;
using RedBjorn.Utils;
using UnityEngine;

public class IdleSelectedStatePlayer : BaseState
{
    private AreaOutline _area;
    private PathDrawer _path;
    private bool _needToMove;
    private PathUpdater _inputHandler;

    public IdleSelectedStatePlayer(UnitFSM context) : base(context)
    {
        _crossFadeTime = 0.2f;
        _animationLayer = 0;
        _animationName = "IdleNotSelected";
    }

    public override void CheckSwitchState()
    {        
        base.CheckSwitchState();    
        if(Selector.ActiveUnit == null && !_context.TilePath.Contains(_inputHandler.TileEntity) 
            && _inputHandler.TileEntity != null)
        {
            SwitchState(FactoryFSM.IdleNotSelectedState(_context));
        }
        if(Selector.ActiveUnit != null && Selector.ActiveUnit != _context)
        {
            SwitchState(FactoryFSM.IdleNotSelectedState(_context));
        }
        if(_needToMove && _context.TilePath != null && _context.TilePath[0] != _inputHandler.TileEntity
            && _context.TilePath.Contains(_inputHandler.TileEntity))
        {
            _needToMove = false;
            SwitchState(FactoryFSM.MovingState(_context));
        }
        if(_context.CMode != UnitFSM.ControlMode.Player)
        {            
            SwitchState(FactoryFSM.IdleSelectedStateAI(_context));
        }
        if(Selector.SelectedAsTargetUnit)
        {
            SwitchState(FactoryFSM.AttackState(_context));
        }
    }

    public override void EnterState()
    {
        _inputHandler = new PlayerPathUpdater(_context);
        _context.Animator.StopPlayback();
        _context.Animator.CrossFade(_animationName, _crossFadeTime, _animationLayer);
        _area = Spawner.Spawn(_context.PathAndArea.AreaOutline, Vector3.zero, Quaternion.identity);
        _context.PathAndArea.AreaShow(_area, _context.Map, _context.transform.position,
            _context.CurrentMoveRange);
        _context.PathAndArea.PathCreate(ref _path, _context.Map);
    }

    public override void UpdateState()
    {
        base.UpdateState();
        _context.PathAndArea.UpdatePath(_area, _path, _context.Map, _context.transform.position,
           _context.CurrentMoveRange);
        _inputHandler.Update(ref _context.TilePath, OnInput, _area, _path);       
    }

    public override void ExitState()
    {
        base.ExitState();
        Object.Destroy(_area.gameObject);
        Object.Destroy(_path.gameObject);
    }

    private void OnInput()
    {        
        _path.IsEnabled = true;
        _context.PathAndArea.AreaShow(_area, _context.Map, _context.transform.position, _context.CurrentMoveRange);
        _needToMove = true;
    }
}
