using UnityEngine;
using UnityEngine.UI;

namespace _AppAssets.Code
{
    public class Bin : MonoBehaviour
    {
        [SerializeField] private Image _binImage;
        [SerializeField] private Image _matchableImage;

        public void Initialize(RecyclingData recyclingData)
        {            
            _binImage.color *= recyclingData.RecyclingTypeColor;
            _matchableImage.sprite = recyclingData.Sprite;
        }
    }
}