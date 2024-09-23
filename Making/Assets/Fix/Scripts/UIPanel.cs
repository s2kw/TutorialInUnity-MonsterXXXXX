using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Fix
{
    public class UIPanel : MonoBehaviour, IUIPanel
    {
        [SerializeField] private TextMeshProUGUI _textMeshProUGUI;
        [SerializeField] private Image _bg;
        [SerializeField] private Curve _bgAlpha;
        [SerializeField] private Curve _textInOutX;
        [SerializeField] private float _duration;


        private static UIPanel instance;
        public static IUIPanel CreatePanel(RectTransform parent)
        {
            if (instance != null) return instance;

            var p = Resources.Load("Prefabs/UIPanel");
            var go = Instantiate(p,parent:parent);
            var r = go.GetComponent<RectTransform>();
            r.localPosition = new Vector3(0f,0f,0f);

            return go.GetComponent<IUIPanel>();
        }
        
        public void StartAnimation(Action _callback = null)
        {
            StartCoroutine(__StartAnimation(_callback));
        }

        public void Reset( string text = "")
        {
            // 初期位置へ
            var defaultColor = _bg.color;
            defaultColor.a = _bgAlpha.Evaluatie(0f);
            _bg.color = defaultColor;

            var defaultPos = _textMeshProUGUI.rectTransform.localPosition;
            defaultPos.x = _textInOutX.Evaluatie(0f);
            _textMeshProUGUI.rectTransform.localPosition = defaultPos;
         
            // 指定されていればテキストを代入
            if(string.IsNullOrEmpty(text) is not true)
                _textMeshProUGUI.SetText(text);
        }
        
        IEnumerator __StartAnimation( Action _callback = null)
        {
            var t = 0f;
            var tmpRectTransform = _textMeshProUGUI.rectTransform;

            while (t <= _duration)
            {
                var p = t / _duration;
                
                // TMPの位置
                var pos = tmpRectTransform.localPosition;
                pos.x = _textInOutX.Evaluatie(p);
                tmpRectTransform.localPosition = pos; 
                
                // BGのColor
                var currentColor = _bg.color;
                currentColor.a = _bgAlpha.Evaluatie(p);
                _bg.color = currentColor;

                t += Time.deltaTime;
                yield return null;
            }
            
            // 終了したらコールバック
            _callback?.Invoke();
        }
    }
}
