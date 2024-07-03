namespace Onyx_POS.Helpers
{
    public class ValidationHelper
    {
        public bool IsEAN13_Standard(long number)
        {
            long temp_num = number / 10;
            char[] main_arr = temp_num.ToString().ToCharArray();
            int alt_num = 0, rem_sum = 0;
            for (int i = main_arr.Length - 1; i > 0; i -= 2)
            {
                alt_num += int.Parse(main_arr[i].ToString());
                rem_sum += int.Parse(main_arr[i - 1].ToString());
            }
            alt_num = (alt_num * 3) + rem_sum;
            string numberWithCheckDigit = temp_num.ToString() + (Math.Ceiling(((decimal)alt_num / 10)) * 10 - alt_num);
            return number.ToString().Equals(numberWithCheckDigit);
        }
    }
}
