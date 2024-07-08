using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RedBjorn.ProtoTiles
{
    [Serializable]
    public partial class TileEntity : INode
    {
        int CachedMovabeArea;
        int ObstacleCount;
        private UnitFSM _obtacledBy;
        public TileData Data { get; private set; }
        public TilePreset Preset { get; private set; }
        public UnitFSM ObtacledBy => _obtacledBy;
        MapRules Rules;

        public int MovableArea { get { return CachedMovabeArea; } set { CachedMovabeArea = value; } }
        public bool Vacant
        {
            get
            {
                if (Data == null || Rules == null || Rules.IsMovable == null)
                {
                    return true;
                }
                return Rules.IsMovable.IsMet(this) && ObstacleCount == 0;
            }
        }

        public bool Visited { get; set; }
        public bool Considered { get; set; }
        public float Depth { get; set; }
        public float[] NeighbourMovable { get { return Data == null ? null : Data.SideHeight; } }
        public Vector3Int Position { get { return Data == null ? Vector3Int.zero : Data.TilePos; } }

        TileEntity() { }

        public TileEntity(TileData preset, TilePreset type, MapRules rules)
        {
            Data = preset;
            Rules = rules;
            Preset = type;
            MovableArea = Data.MovableArea;
        }

        public void SetObtacle(bool value, UnitFSM unit)
        {
            _obtacledBy = unit;
            SetObtacle(value);
        }

        public void SetObtacle(bool value)
        {
            if(value)
            {
                ObstacleCount = 1;
            }
            else
            {
                ObstacleCount = 0;    
            }
        }

        public override string ToString()
        {
            return string.Format("Position: {0}. Vacant = {1}", Position, Vacant);
        }

        public void ChangeMovableAreaPreset(int area)
        {
            Data.MovableArea = area;
        }
    }
}

