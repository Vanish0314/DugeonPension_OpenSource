using UnityEngine;
using UnityEngine.UI;

namespace Dungeon.UI
{
    [RequireComponent(typeof(Image))]
    public class ImageSpriteAnimation : MonoBehaviour
    {
        private Image _image;
        private SpriteRenderer _spriteRendererProxy;
    
        void Awake()
        {
            _image = GetComponent<Image>();
            _spriteRendererProxy = GetComponent<SpriteRenderer>();
        }
    
        void Update()
        {
            // 实时同步SpriteRenderer的sprite到Image组件
            if(_spriteRendererProxy.sprite != _image.sprite)
            {
                _image.sprite = _spriteRendererProxy.sprite;
            }
        }
    
        void OnDestroy()
        {
            if(_spriteRendererProxy != null)
            {
                Destroy(_spriteRendererProxy.gameObject);
            }
        }
    }
}