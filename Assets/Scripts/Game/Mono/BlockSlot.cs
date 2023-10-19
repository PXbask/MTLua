using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MT.Mono
{
    public class BlockSlot : MonoBehaviour
    {
        private readonly int FIRST = 0;
        public Vector2Int blockPos;

        public SpriteRenderer bg_render;
        public Animator bg_animator;
        
        public SpriteRenderer event_render;
        public Animator event_animator;
        public EventObjectLogic eventlogic;
        
        public SpriteRenderer fore_render;
        public Animator fore_animator;

        public Transform eventChildRoot;

        public BlockSlot SetPos(Vector2Int pos)
        {
            this.blockPos = pos;
            this.transform.position = MT.Util.UnityUtil.GetTPosition(pos);
            return this;
        }

        public void ResetEventObject(GameObject @object)
        {
            for (int i = 0; i < eventChildRoot.childCount; i++)
            {
                Manager.Pool.UnSpawn("EventObject", this.eventlogic.assetName, eventChildRoot.GetChild(i));
            }
            @object.transform.SetParent(eventChildRoot, false);
            this.Refresh();
        }

        private void Refresh()
        {
            Transform obj = eventChildRoot.GetChild(FIRST);
            this.event_render = obj.GetComponent<SpriteRenderer>();
            this.event_animator = obj.GetComponent<Animator>();
            this.eventlogic = obj.GetComponent<EventObjectLogic>();
        }

        internal void ResetBackGround(Sprite v) => this.bg_render.sprite = v;
    }
}
