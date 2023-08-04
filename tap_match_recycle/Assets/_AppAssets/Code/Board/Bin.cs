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
            //Idea: place matchable sprite. => Not adjusting to all resolutions correctly. Sticking to
            //recycling symbol for now
            // _matchableImage.sprite = recyclingData.Sprite;
        }
    }
}