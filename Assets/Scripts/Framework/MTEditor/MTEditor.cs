using MT.Data;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MT.Editor
{
    [ExecuteInEditMode]
    public class MTEditor : MonoBehaviour
    {
        public static MTEditor instance;

        [Header("ExportSetting")]
        public int level;
        public RectInt exportRect;

        private Tilemap bgmap;
        private Tilemap eventmap;

        [SerializeField] private TeleportData upstair;
        [SerializeField] private TeleportData downstair;

        private int m_level;

        private int Scale => Mathf.Max(exportRect.width, exportRect.height);

        private void Awake()
        {
            instance = this;
            GameObject grid = GameObject.Find("Stage/Grid");
            bgmap = grid.transform.GetChild(0).GetComponent<Tilemap>();
            eventmap = grid.transform.GetChild(1).GetComponent<Tilemap>();
        }

        private void Start()
        {
            this.m_level = level;
        }

        private void Update()
        {
            if (m_level == level) return;
            m_level = level;
            this.ImportStageData(this.level);
        }

        private void ImportStageData(int level)
        {
            Managers.DataManager.Instance.EditorLoadData();
            MT.Data.StageData stageData;
            try
            {
                stageData = Managers.DataManager.Instance.GetStageData(level);
            }
            catch(KeyNotFoundException)
            {
                stageData = new StageData()
                {
                    Bgm = "",
                    Scale = 0,
                    Level = level,
                    EventGroundLayer = { },
                    BackGroundLayer = { },
                    FrontGroundLayer = { },
                    Up = TeleportData.Empty,
                    Down = TeleportData.Empty,
                };
            }

            int scale = stageData.Scale;
            TileBase[] tileBases = new TileBase[scale * scale];
            this.upstair = stageData.Up;
            this.downstair = stageData.Down;

            if (scale <= 0)
            {
                this.exportRect = new RectInt(0, 0, 0, 0);
                bgmap.ClearAllTiles();
                eventmap.ClearAllTiles();
            }
            else
            {
                for (int i = 0; i < stageData.BackGroundLayer.Count; i++)
                {
                    int ix = i % scale + (scale - 1 - i / scale) * scale;
                    tileBases[i] = this.tileMaping(stageData.BackGroundLayer[ix]);
                }
                this.exportRect = new RectInt(0, 0, scale, scale);
                bgmap.SetTilesBlock(this.Rect2BoundInt(this.exportRect), tileBases.ToArray());

                for (int i = 0; i < stageData.EventGroundLayer.Count; i++)
                {
                    int ix = i % scale + (scale - 1 - i / scale) * scale;
                    tileBases[i] = this.tileMaping(stageData.EventGroundLayer[ix]);
                }
                this.exportRect = new RectInt(0, 0, scale, scale);
                eventmap.SetTilesBlock(this.Rect2BoundInt(this.exportRect), tileBases.ToArray());
            }
        }

        public void ExportConfig()
        {
            int scale = Scale;
            int[] bgIndex = new int[scale * scale];
            int[] eventIndex = new int[scale * scale];
            int[] foreIndex = new int[scale * scale];

            var tiles = bgmap.GetTilesBlock(Rect2BoundInt(this.exportRect));
            Managers.DataManager.Instance.EditorLoadData();

            for (int i = 0; i < tiles.Length; i++)
            {
                var ix = i % scale + (scale - 1 - i / scale) * scale;
                bgIndex[ix] = this.tileMaping(tiles[i]);
            }

            tiles = eventmap.GetTilesBlock(Rect2BoundInt(this.exportRect));
            for (int i = 0; i < tiles.Length; i++)
            {
                var ix = i % scale + (scale - 1 - i / scale) * scale;
                eventIndex[ix] = this.EventTileMaping(tiles[i]);
            }

            Managers.DataManager.Instance.SetStageData(this.level, new Data.StageData()
            {
                Bgm = "",
                Level = this.level,
                Scale = scale,
                BackGroundLayer = bgIndex.ToList(),
                EventGroundLayer = eventIndex.ToList(),
                FrontGroundLayer = foreIndex.ToList(),
                Up = this.upstair,
                Down = this.downstair,
            });

            Managers.DataManager.Instance.ExportStageData();
        }

        private int EventTileMaping(TileBase tileBase)
        {
            if (!tileBase) return 0;
            return Managers.DataManager.Instance.tilemapingDatas[tileBase.name].IntMap;
        }

        private BoundsInt Rect2BoundInt(RectInt rect) => new BoundsInt(rect.x, -rect.height, 0, rect.width, rect.height, 1);

        private int tileMaping(TileBase tile) => Managers.DataManager.Instance.tilemapingDatas[tile.name].IntMap;

        private TileBase tileMaping(int id)
        {
            foreach (var data in Managers.DataManager.Instance.tilemapingDatas)
            {
                if(data.Value.IntMap == id)
                {
                    TileBase tile = UnityEditor.AssetDatabase.LoadAssetAtPath<TileBase>("Assets/TileMap/Tiles/" + data.Key + ".asset");
                    return tile;
                }
            }
            return null;
        }

        private void OnDestroy()
        {
            Managers.DataManager.Instance.Dispose();
        }
    }
}



