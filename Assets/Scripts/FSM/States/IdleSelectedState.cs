using RedBjorn.ProtoTiles.Example;
using RedBjorn.Utils;
using UnityEngine;

public class IdleSelectedState : BaseState
{
    private AreaOutline _area;
    private PathDrawer _path;
    private bool _needToMove;

    public IdleSelectedState(UnitFSM context) : base(context)
    {
        _crossFadeTime = 0.2f;
        _animationLayer = 0;
        _animationName = "Idle";
    }

    public override void CheckSwitchState()
    {
        base.CheckSwitchState();    
        if(Selector.SelectedUnit == null && !_context.TilePath.Contains(_context.InputHandler.TileEntity))
        {
            SwitchState(FactoryFSM.IdleNotSelectedState(_context));
        }
        if(Selector.SelectedUnit != null && Selector.SelectedUnit != _context)
        {
            SwitchState(FactoryFSM.IdleNotSelectedState(_context));
        }
        if(_needToMove && _context.TilePath != null && _context.TilePath[0] != _context.InputHandler.TileEntity
            && _context.TilePath.Contains(_context.InputHandler.TileEntity))
        {           
            _needToMove = false;
            SwitchState(FactoryFSM.MovingState(_context));
        }
    }

    public override void EnterState()
    {
        _context.Animator.StopPlayback();
        _context.Animator.CrossFade(_animationName, _crossFadeTime, _animationLayer);
        _area = Spawner.Spawn(_context.PathAndArea.AreaOutline, Vector3.zero, Quaternion.identity);
        _context.PathAndArea.AreaShow(_area, _context.Map, _context.transform.position,
            _context.UnitPattern);
        _context.PathAndArea.PathCreate(ref _path, _context.Map);
    }

    public override void UpdateState()
    {
        base.UpdateState();
        _context.PathAndArea.UpdatePath(_area, _path, _context.Map, _context.transform.position,
           _context.UnitPattern);
        _context.InputHandler.Update(ref _context.TilePath, OnInput, _area, _path);       
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
        _context.PathAndArea.AreaShow(_area, _context.Map, _context.transform.position, _context.UnitPattern);
        _needToMove = true;
    }
}
