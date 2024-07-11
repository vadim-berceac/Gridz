using UnityEngine;

namespace RedBjorn.ProtoTiles
{
    public class MapView : MonoBehaviour
    {
        [SerializeField] private bool _createBackGround;
        [SerializeField] private Material _backGroundMaterial;
        [SerializeField] private Vector3 _backGroundScale = new(300, 1, 300);
        private GameObject _grid;
        private GameObject _backGround;

        public void Init(MapEntity map)
        {
            _grid = new GameObject("Grid");
            _grid.transform.SetParent(transform);
            _grid.transform.localPosition = Vector3.zero;
            map.CreateGrid(_grid.transform);
            if(_createBackGround)
            {
                _backGround = GameObject.CreatePrimitive(PrimitiveType.Cube);
                _backGround.transform.position = new(1, -1, 1);
                _backGround.transform.localScale = _backGroundScale;
                _backGround.transform.SetParent(transform);
                _backGround.name = "BackGround";
                _backGround.isStatic = true;
                var renderer = _backGround.GetComponent<MeshRenderer>();
                renderer.sharedMaterial = _backGroundMaterial;
            }
        }

        public void GridEnable(bool enable)
        {
            if (_grid)
            {
                _grid.SetActive(enable);
            }
            else
            {
                Log.E($"Can't enable Grid state: {enable}. It wasn't created");
            }
        }

        public void GridToggle()
        {
            if (_grid)
            {
                _grid.SetActive(!_grid.activeSelf);
            }
            else
            {
                Log.E("Can't toggle Grid state. It wasn't created");
            }
        }
    }
}