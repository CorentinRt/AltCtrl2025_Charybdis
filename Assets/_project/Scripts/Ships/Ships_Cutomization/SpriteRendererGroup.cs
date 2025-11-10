using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AltCtrl.Charybdis
{
    public class SpriteRendererGroup : MonoBehaviour
    {
        #region Fields
        [Header("Sprite Renderers")]
        [SerializeField] private List<SpriteRenderer> _spriteRenderers = new List<SpriteRenderer>();


        #endregion

        #region Properties
        public List<SpriteRenderer> SpriteRenderers { get => _spriteRenderers; }


        #endregion

        [Button]
        private void CollectAllRenderers()
        {
            SpriteRenderer[] spritesRenderers = GetComponentsInChildren<SpriteRenderer>(true);

            foreach (SpriteRenderer sprite in spritesRenderers)
            {
                if (sprite == null)
                    continue;

                _spriteRenderers.Add(sprite);
            }
        }

        public void SetColorOnMaterial(Color color)
        {
            if (_spriteRenderers == null || _spriteRenderers.Count <= 0)
                return;

            foreach (SpriteRenderer sprite in _spriteRenderers)
            {
                if (sprite == null)
                    continue;

                sprite.material.color = color;
            }
        }

        public void SetMotifOnMaterial(Texture2D texture)
        {
            if (_spriteRenderers == null || _spriteRenderers.Count <= 0)
                return;

            foreach (SpriteRenderer sprite in _spriteRenderers)
            {
                if (sprite == null)
                    continue;

                sprite.material.SetTexture("_Objective_texture", texture);
            }
        }
    }
}
