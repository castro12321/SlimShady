namespace SlimShady
{
    public class Utils
    {
        /// <summary>Interpolates value</summary>
        /// <param name="from">Minimal value (when value=0)</param>
        /// <param name="to">Maximal value (when value=100)</param>
        /// <param name="value">Pervent value to interpolate between from and to</param>
        /// <returns>Interpolated number that is between 'from' and 'to' based on 'value'</returns>
        public static int Lerp(int from, int to, int value)
        {
            float fValue = (float)value / 100.0f;
            return (int)((to - from) * fValue + from);
        }
    }
}
