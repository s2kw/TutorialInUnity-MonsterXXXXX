using UnityEngine;


namespace  Fix
{
    [System.Serializable]
    public class Curve
    {
        [SerializeField] public float factor,addition;
        [SerializeField]
        public AnimationCurve value;

        public float Evaluatie(float progress)
        {
            // factorが0の場合、乗算をスキップ
            if (Mathf.Approximately(factor, 0f)) return addition;
            
            
            var result = value.Evaluate(progress) * factor + addition;
            
            // 入力値によってはNaNになることがある
            if (float.IsNaN(result)) return 0f;
            
            return result;
        }
    }

}